CREATE TABLE dbo.StudentAttendance
(
    AttendanceId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,
    SubjectCode NVARCHAR(50) NULL,
    PeriodNo INT NULL,
    Status NVARCHAR(20) NOT NULL, -- Present/Absent
    Remarks NVARCHAR(500) NULL,
    MarkedByUserId INT NULL,
    MarkedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    SubjectKey AS ISNULL(SubjectCode, '') PERSISTED,
    PeriodKey AS ISNULL(PeriodNo, 0) PERSISTED
);
CREATE UNIQUE INDEX UX_StudentAttendance_Unique
    ON dbo.StudentAttendance(StudentId, AttendanceDate, SubjectKey, PeriodKey);
CREATE INDEX IX_StudentAttendance_DateClass
    ON dbo.StudentAttendance(AttendanceDate, ClassName, Section);

GO


CREATE OR ALTER PROCEDURE UpsertStudentAttendance
    @StudentId INT,
    @AttendanceDate DATE,
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @SubjectCode NVARCHAR(50) = NULL,
    @PeriodNo INT = NULL,
    @Status NVARCHAR(20),
    @Remarks NVARCHAR(500) = NULL,
    @MarkedByUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AttendanceId INT;

    -- lock row to avoid race on upsert
    SELECT @AttendanceId = AttendanceId
    FROM dbo.StudentAttendance WITH (UPDLOCK, HOLDLOCK)
    WHERE StudentId = @StudentId
      AND AttendanceDate = @AttendanceDate
      AND ISNULL(SubjectCode,'') = ISNULL(@SubjectCode,'')
      AND ISNULL(PeriodNo,0) = ISNULL(@PeriodNo,0);

    IF @AttendanceId IS NULL
    BEGIN
        INSERT INTO dbo.StudentAttendance
        (StudentId, AttendanceDate, ClassName, Section, SubjectCode, PeriodNo, Status, Remarks, MarkedByUserId)
        VALUES
        (@StudentId, @AttendanceDate, @ClassName, @Section, @SubjectCode, @PeriodNo, @Status, @Remarks, @MarkedByUserId);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.StudentAttendance
        SET ClassName = @ClassName,
            Section = @Section,
            SubjectCode = @SubjectCode,
            PeriodNo = @PeriodNo,
            Status = @Status,
            Remarks = @Remarks,
            MarkedByUserId = @MarkedByUserId,
            MarkedAtUtc = SYSUTCDATETIME()
        WHERE AttendanceId = @AttendanceId;

        RETURN @AttendanceId;
    END
END
GO

CREATE OR ALTER PROCEDURE UpdateStudentAttendanceStatus
    @AttendanceId INT,
    @Status NVARCHAR(20),
    @Remarks NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.StudentAttendance
    SET Status = @Status,
        Remarks = @Remarks,
        MarkedAtUtc = SYSUTCDATETIME()
    WHERE AttendanceId = @AttendanceId;

    RETURN @AttendanceId;
END
GO

CREATE OR ALTER PROCEDURE DeleteStudentAttendance
    @AttendanceId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.StudentAttendance WHERE AttendanceId = @AttendanceId;
    RETURN @AttendanceId;
END
GO

CREATE OR ALTER PROCEDURE GetClassAttendanceByDate
    @AttendanceDate DATE,
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @SubjectCode NVARCHAR(50) = NULL,
    @PeriodNo INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AttendanceId, StudentId, AttendanceDate, ClassName, Section, SubjectCode, PeriodNo, Status, Remarks, MarkedByUserId, MarkedAtUtc
    FROM dbo.StudentAttendance
    WHERE AttendanceDate = @AttendanceDate
      AND ClassName = @ClassName
      AND (@Section IS NULL OR Section = @Section)
      AND (ISNULL(SubjectCode,'') = ISNULL(@SubjectCode,''))
      AND (ISNULL(PeriodNo,0) = ISNULL(@PeriodNo,0))
    ORDER BY StudentId;
END
GO

CREATE OR ALTER PROCEDURE GetStudentAttendanceRange
    @StudentId INT,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
                                
    SELECT AttendanceId, StudentId, AttendanceDate, ClassName, Section, SubjectCode, PeriodNo, Status, Remarks, MarkedByUserId, MarkedAtUtc
    FROM dbo.StudentAttendance
    WHERE StudentId = @StudentId
      AND AttendanceDate >= @FromDate
      AND AttendanceDate <= @ToDate
    ORDER BY AttendanceDate DESC, AttendanceId DESC;
END
GO

CREATE TABLE dbo.StudentLeaveRequests
(
    LeaveId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    FromDate DATE NOT NULL,
    ToDate DATE NOT NULL,
    Reason NVARCHAR(500) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending/Approved/Rejected
    AppliedByUserId INT NULL,
    AppliedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ApprovedByUserId INT NULL,
    ApprovedAtUtc DATETIME2 NULL,
    RejectionReason NVARCHAR(500) NULL,
    AttachmentUrl NVARCHAR(1000) NULL,
    CONSTRAINT FK_StudentLeaveRequests_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId) ON DELETE CASCADE
);
CREATE INDEX IX_StudentLeave_StudentDate ON dbo.StudentLeaveRequests(StudentId, FromDate, ToDate);
CREATE INDEX IX_StudentLeave_Status ON dbo.StudentLeaveRequests(Status, FromDate, ToDate);

GO



CREATE OR ALTER PROCEDURE ApplyStudentLeaveRequest
    @StudentId INT,
    @LeaveType NVARCHAR(50),
    @FromDate DATE,
    @ToDate DATE,
    @Reason NVARCHAR(500) = NULL,
    @AppliedByUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ToDate < @FromDate
        THROW 53001, 'Invalid date range', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentId = @StudentId)
        THROW 53002, 'Student not found', 1;

    INSERT INTO dbo.StudentLeaveRequests (StudentId, LeaveType, FromDate, ToDate, Reason, AppliedByUserId)
    VALUES (@StudentId, @LeaveType, @FromDate, @ToDate, @Reason, @AppliedByUserId);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE ApproveStudentLeaveRequest
    @LeaveId INT,
    @ApprovedByUserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.StudentLeaveRequests
    SET Status = 'Approved',
        ApprovedByUserId = @ApprovedByUserId,
        ApprovedAtUtc = SYSUTCDATETIME(),
        RejectionReason = NULL
    WHERE LeaveId = @LeaveId;

    RETURN @LeaveId;
END
GO

CREATE OR ALTER PROCEDURE RejectStudentLeaveRequest
    @LeaveId INT,
    @ApprovedByUserId INT,
    @RejectionReason NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.StudentLeaveRequests
    SET Status = 'Rejected',
        ApprovedByUserId = @ApprovedByUserId,
        ApprovedAtUtc = SYSUTCDATETIME(),
        RejectionReason = @RejectionReason
    WHERE LeaveId = @LeaveId;

    RETURN @LeaveId;
END
GO

CREATE OR ALTER PROCEDURE CancelStudentLeaveRequest
    @LeaveId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.StudentLeaveRequests
    SET Status = 'Cancelled'
    WHERE LeaveId = @LeaveId;

    RETURN @LeaveId;
END
GO

CREATE OR ALTER PROCEDURE GetStudentLeavesRange
    @StudentId INT,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.StudentLeaveRequests
    WHERE StudentId = @StudentId
      AND FromDate <= @ToDate
      AND ToDate >= @FromDate
    ORDER BY FromDate DESC, LeaveId DESC;
END
GO

-- Optional filters (date window and/or class/section and/or status)
CREATE OR ALTER PROCEDURE GetPendingStudentLeaves
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @Status NVARCHAR(20) = NULL -- default 'Pending'
AS
BEGIN
    SET NOCOUNT ON;

    SELECT l.*
    FROM dbo.StudentLeaveRequests l
    INNER JOIN dbo.Students s ON s.StudentId = l.StudentId
    WHERE (@FromDate IS NULL OR l.FromDate <= @ToDate OR @ToDate IS NULL)
      AND (@ToDate IS NULL OR l.ToDate >= @FromDate OR @FromDate IS NULL)
      AND (@ClassName IS NULL OR s.ClassName = @ClassName)
      AND (@Section IS NULL OR s.Section = @Section)
      AND (@Status IS NULL OR l.Status = @Status)
    ORDER BY l.AppliedAtUtc DESC, l.LeaveId DESC;
END
GO

CREATE TABLE dbo.StaffAttendance
(
    AttendanceId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL,        -- Present/Absent/Late/Excused
    InTime DATETIME2 NULL,
    OutTime DATETIME2 NULL,
    Remarks NVARCHAR(500) NULL,
    MarkedByUserId INT NULL,
    MarkedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Source NVARCHAR(20) NULL,            -- Manual/Biometric/RFID
    CONSTRAINT FK_StaffAttendance_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
);
CREATE UNIQUE INDEX UX_StaffAttendance_Unique
    ON dbo.StaffAttendance(UserId, AttendanceDate);
CREATE INDEX IX_StaffAttendance_Date ON dbo.StaffAttendance(AttendanceDate, Status);

GO

-- Upsert staff attendance (per day)
CREATE OR ALTER PROCEDURE UpsertStaffAttendance
    @UserId INT,
    @AttendanceDate DATE,
    @Status NVARCHAR(20),
    @InTime DATETIME2 = NULL,
    @OutTime DATETIME2 = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @MarkedByUserId INT = NULL,
    @Source NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AttendanceId INT;

    SELECT @AttendanceId = AttendanceId
    FROM dbo.StaffAttendance WITH (UPDLOCK, HOLDLOCK)
    WHERE UserId = @UserId AND AttendanceDate = @AttendanceDate;

    IF @AttendanceId IS NULL
    BEGIN
        INSERT INTO dbo.StaffAttendance
        (UserId, AttendanceDate, Status, InTime, OutTime, Remarks, MarkedByUserId, Source)
        VALUES
        (@UserId, @AttendanceDate, @Status, @InTime, @OutTime, @Remarks, @MarkedByUserId, @Source);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.StaffAttendance
        SET Status = @Status,
            InTime = @InTime,
            OutTime = @OutTime,
            Remarks = @Remarks,
            MarkedByUserId = @MarkedByUserId,
            Source = @Source,
            MarkedAtUtc = SYSUTCDATETIME()
        WHERE AttendanceId = @AttendanceId;

        RETURN @AttendanceId;
    END
END
GO

CREATE OR ALTER PROCEDURE UpdateStaffAttendanceStatus
    @AttendanceId INT,
    @Status NVARCHAR(20),
    @Remarks NVARCHAR(500) = NULL,
    @InTime DATETIME2 = NULL,
    @OutTime DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.StaffAttendance
    SET Status = @Status,
        Remarks = @Remarks,
        InTime = @InTime,
        OutTime = @OutTime,
        MarkedAtUtc = SYSUTCDATETIME()
    WHERE AttendanceId = @AttendanceId;

    RETURN @AttendanceId;
END
GO

CREATE OR ALTER PROCEDURE DeleteStaffAttendance
    @AttendanceId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.StaffAttendance WHERE AttendanceId = @AttendanceId;
    RETURN @AttendanceId;
END
GO

CREATE OR ALTER PROCEDURE GetStaffAttendanceRange
    @UserId INT,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sa.AttendanceId, sa.UserId, sa.AttendanceDate, sa.Status, sa.InTime, sa.OutTime, sa.Remarks,
           sa.MarkedByUserId, sa.MarkedAtUtc, sa.Source
    FROM dbo.StaffAttendance sa
    WHERE sa.UserId = @UserId
      AND sa.AttendanceDate >= @FromDate
      AND sa.AttendanceDate <= @ToDate
    ORDER BY sa.AttendanceDate DESC, sa.AttendanceId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetStaffDailyAttendance
    @AttendanceDate DATE,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sa.AttendanceId, sa.UserId, sa.AttendanceDate, sa.Status, sa.InTime, sa.OutTime, sa.Remarks,
           sa.MarkedByUserId, sa.MarkedAtUtc, sa.Source
    FROM dbo.StaffAttendance sa
    WHERE sa.AttendanceDate = @AttendanceDate
      AND (@Status IS NULL OR sa.Status = @Status)
    ORDER BY sa.UserId;
END
GO


CREATE TABLE dbo.StaffLeaveRequests
(
    LeaveId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    FromDate DATE NOT NULL,
    ToDate DATE NOT NULL,
    Reason NVARCHAR(500) NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending/Approved/Rejected/Cancelled
    AppliedByUserId INT NULL,
    AppliedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ApprovedByUserId INT NULL,
    ApprovedAtUtc DATETIME2 NULL,
    RejectionReason NVARCHAR(500) NULL,
    AttachmentUrl NVARCHAR(1000) NULL,
    CONSTRAINT FK_StaffLeaveRequests_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
);
CREATE INDEX IX_StaffLeave_UserDate ON dbo.StaffLeaveRequests(UserId, FromDate, ToDate);
CREATE INDEX IX_StaffLeave_Status ON dbo.StaffLeaveRequests(Status, FromDate, ToDate);

GO

-- Optional helpers to seed/operate staff leave
CREATE OR ALTER PROCEDURE ApplyStaffLeaveRequest
    @UserId INT,
    @LeaveType NVARCHAR(50),
    @FromDate DATE,
    @ToDate DATE,
    @Reason NVARCHAR(500) = NULL,
    @AppliedByUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @ToDate < @FromDate THROW 54001, 'Invalid date range', 1;

    INSERT INTO dbo.StaffLeaveRequests (UserId, LeaveType, FromDate, ToDate, Reason, AppliedByUserId)
    VALUES (@UserId, @LeaveType, @FromDate, @ToDate, @Reason, @AppliedByUserId);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE ApproveStaffLeaveRequest
    @LeaveId INT,
    @ApprovedByUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.StaffLeaveRequests
    SET Status = 'Approved', ApprovedByUserId = @ApprovedByUserId, ApprovedAtUtc = SYSUTCDATETIME(), RejectionReason = NULL
    WHERE LeaveId = @LeaveId;
    RETURN @LeaveId;
END
GO

CREATE OR ALTER PROCEDURE RejectStaffLeaveRequest
    @LeaveId INT,
    @ApprovedByUserId INT,
    @RejectionReason NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.StaffLeaveRequests
    SET Status = 'Rejected', ApprovedByUserId = @ApprovedByUserId, ApprovedAtUtc = SYSUTCDATETIME(), RejectionReason = @RejectionReason
    WHERE LeaveId = @LeaveId;
    RETURN @LeaveId;
END
GO

CREATE OR ALTER PROCEDURE CancelStaffLeaveRequest
    @LeaveId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.StaffLeaveRequests SET Status = 'Cancelled' WHERE LeaveId = @LeaveId;
    RETURN @LeaveId;
END
GO

-- Student: auto-mark approved leaves across a date range
CREATE OR ALTER PROCEDURE AutoMarkStudentApprovedLeavesRange
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    IF @ToDate < @FromDate THROW 54101, 'Invalid date range', 1;

    DECLARE @Inserted INT = 0, @Updated INT = 0;

    ;WITH Dates AS
    (
        SELECT @FromDate AS D
        UNION ALL
        SELECT DATEADD(DAY, 1, D) FROM Dates WHERE D < @ToDate
    )
    -- Insert missing attendance rows as Excused
    INSERT INTO dbo.StudentAttendance (StudentId, AttendanceDate, ClassName, Section, SubjectCode, PeriodNo, Status, Remarks, MarkedByUserId)
    SELECT s.StudentId, d.D, s.ClassName, s.Section, NULL, NULL, 'Excused', 'Auto: Approved Leave', NULL
    FROM Dates d
    INNER JOIN dbo.StudentLeaveRequests l ON l.Status = 'Approved' AND d.D BETWEEN l.FromDate AND l.ToDate
    INNER JOIN dbo.Students s ON s.StudentId = l.StudentId
    LEFT JOIN dbo.StudentAttendance a ON a.StudentId = s.StudentId AND a.AttendanceDate = d.D
                                      AND ISNULL(a.SubjectCode,'') = '' AND ISNULL(a.PeriodNo,0) = 0
    WHERE a.AttendanceId IS NULL
    OPTION (MAXRECURSION 32767);

    SET @Inserted = @@ROWCOUNT;

    ;WITH Dates AS
    (
        SELECT @FromDate AS D
        UNION ALL
        SELECT DATEADD(DAY, 1, D) FROM Dates WHERE D < @ToDate
    )
    UPDATE a
    SET a.Status = 'Excused',
        a.Remarks = CASE WHEN a.Remarks IS NULL OR LTRIM(RTRIM(a.Remarks)) = '' THEN 'Auto: Approved Leave'
                         ELSE a.Remarks + '; Auto: Approved Leave' END,
        a.MarkedAtUtc = SYSUTCDATETIME()
    FROM dbo.StudentAttendance a
    INNER JOIN dbo.StudentLeaveRequests l ON l.Status = 'Approved' AND a.StudentId = l.StudentId
    INNER JOIN Dates d ON d.D = a.AttendanceDate
    WHERE a.AttendanceDate BETWEEN @FromDate AND @ToDate
      AND a.Status = 'Absent'
      AND ISNULL(a.SubjectCode,'') = '' AND ISNULL(a.PeriodNo,0) = 0
      AND a.AttendanceDate BETWEEN l.FromDate AND l.ToDate
    OPTION (MAXRECURSION 32767);

    SET @Updated = @Updated + @@ROWCOUNT;

    RETURN (@Inserted + @Updated);
END
GO

-- Staff: auto-mark approved leaves across a date range
CREATE OR ALTER PROCEDURE AutoMarkStaffApprovedLeavesRange
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    IF @ToDate < @FromDate THROW 54102, 'Invalid date range', 1;

    DECLARE @Inserted INT = 0, @Updated INT = 0;

    ;WITH Dates AS
    (
        SELECT @FromDate AS D
        UNION ALL
        SELECT DATEADD(DAY, 1, D) FROM Dates WHERE D < @ToDate
    )
    INSERT INTO dbo.StaffAttendance (UserId, AttendanceDate, Status, InTime, OutTime, Remarks, MarkedByUserId, Source)
    SELECT l.UserId, d.D, 'Excused', NULL, NULL, 'Auto: Approved Leave', NULL, 'System'
    FROM Dates d
    INNER JOIN dbo.StaffLeaveRequests l ON l.Status = 'Approved' AND d.D BETWEEN l.FromDate AND l.ToDate
    LEFT JOIN dbo.StaffAttendance a ON a.UserId = l.UserId AND a.AttendanceDate = d.D
    WHERE a.AttendanceId IS NULL
    OPTION (MAXRECURSION 32767);

    SET @Inserted = @@ROWCOUNT;

    ;WITH Dates AS
    (
        SELECT @FromDate AS D
        UNION ALL
        SELECT DATEADD(DAY, 1, D) FROM Dates WHERE D < @ToDate
    )
    UPDATE a
    SET a.Status = 'Excused',
        a.Remarks = CASE WHEN a.Remarks IS NULL OR LTRIM(RTRIM(a.Remarks)) = '' THEN 'Auto: Approved Leave'
                         ELSE a.Remarks + '; Auto: Approved Leave' END,
        a.MarkedAtUtc = SYSUTCDATETIME()
    FROM dbo.StaffAttendance a
    INNER JOIN dbo.StaffLeaveRequests l ON l.Status = 'Approved' AND a.UserId = l.UserId
    INNER JOIN Dates d ON d.D = a.AttendanceDate
    WHERE a.AttendanceDate BETWEEN @FromDate AND @ToDate
      AND a.Status = 'Absent'
      AND a.AttendanceDate BETWEEN l.FromDate AND l.ToDate
    OPTION (MAXRECURSION 32767);

    SET @Updated = @Updated + @@ROWCOUNT;

    RETURN (@Inserted + @Updated);
END

GO

-- Student daily summary (group by Class/Section)
CREATE OR ALTER PROCEDURE GetDailyStudentAttendanceSummary
    @AttendanceDate DATE,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        @AttendanceDate AS AttendanceDate,
        ClassName,
        Section,
        SUM(CASE WHEN Status = 'Present' THEN 1 ELSE 0 END) AS PresentCount,
        SUM(CASE WHEN Status = 'Absent' THEN 1 ELSE 0 END) AS AbsentCount,
        SUM(CASE WHEN Status = 'Late' THEN 1 ELSE 0 END) AS LateCount,
        SUM(CASE WHEN Status = 'Excused' THEN 1 ELSE 0 END) AS ExcusedCount,
        COUNT(*) AS Total
    FROM dbo.StudentAttendance
    WHERE AttendanceDate = @AttendanceDate
      AND (@ClassName IS NULL OR ClassName = @ClassName)
      AND (@Section IS NULL OR Section = @Section)
    GROUP BY ClassName, Section
    ORDER BY ClassName, Section;
END
GO

-- Staff daily summary
CREATE OR ALTER PROCEDURE GetDailyStaffAttendanceSummary
    @AttendanceDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        @AttendanceDate AS AttendanceDate,
        SUM(CASE WHEN Status = 'Present' THEN 1 ELSE 0 END) AS PresentCount,
        SUM(CASE WHEN Status = 'Absent' THEN 1 ELSE 0 END) AS AbsentCount,
        SUM(CASE WHEN Status = 'Late' THEN 1 ELSE 0 END) AS LateCount,
        SUM(CASE WHEN Status = 'Excused' THEN 1 ELSE 0 END) AS ExcusedCount,
        COUNT(*) AS Total
    FROM dbo.StaffAttendance
    WHERE AttendanceDate = @AttendanceDate;
END
GO



CREATE TABLE dbo.BiometricDevices
(
    DeviceId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SerialNo NVARCHAR(100) NOT NULL,
    Vendor NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    RegisteredAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE UNIQUE INDEX UX_BioDevices_SerialNo ON dbo.BiometricDevices(SerialNo);

GO

CREATE TABLE dbo.BiometricUserMaps
(
    MapId INT IDENTITY(1,1) PRIMARY KEY,
    DeviceId INT NOT NULL,
    ExternalUserId NVARCHAR(100) NOT NULL,
    PersonType NVARCHAR(20) NOT NULL, -- Student/Staff
    StudentId INT NULL,
    UserId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_BioMap_Device FOREIGN KEY (DeviceId) REFERENCES dbo.BiometricDevices(DeviceId) ON DELETE CASCADE
);
CREATE UNIQUE INDEX UX_BioMap_DeviceUser ON dbo.BiometricUserMaps(DeviceId, ExternalUserId);

GO

CREATE TABLE dbo.BiometricRawPunches
(
    PunchId BIGINT IDENTITY(1,1) PRIMARY KEY,
    DeviceId INT NOT NULL,
    ExternalUserId NVARCHAR(100) NOT NULL,
    PunchTime DATETIME2 NOT NULL,
    Direction NVARCHAR(10) NULL,   -- In/Out
    Source NVARCHAR(20) NOT NULL DEFAULT 'Biometric',
    Processed BIT NOT NULL DEFAULT 0,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_BioRaw_Device FOREIGN KEY (DeviceId) REFERENCES dbo.BiometricDevices(DeviceId) ON DELETE CASCADE
);
CREATE UNIQUE INDEX UX_BioRaw_Unique ON dbo.BiometricRawPunches(DeviceId, ExternalUserId, PunchTime);
CREATE INDEX IX_BioRaw_Process ON dbo.BiometricRawPunches(Processed, PunchTime);

GO

CREATE OR ALTER PROCEDURE RegisterBiometricDevice
    @Name NVARCHAR(100),
    @SerialNo NVARCHAR(100),
    @Vendor NVARCHAR(100) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeviceId INT = (SELECT DeviceId FROM dbo.BiometricDevices WHERE SerialNo = @SerialNo);
    IF @DeviceId IS NULL
    BEGIN
        INSERT INTO dbo.BiometricDevices(Name, SerialNo, Vendor, IsActive)
        VALUES (@Name, @SerialNo, @Vendor, @IsActive);
        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.BiometricDevices
        SET Name = @Name, Vendor = @Vendor, IsActive = @IsActive
        WHERE DeviceId = @DeviceId;
        RETURN @DeviceId;
    END
END
GO

CREATE OR ALTER PROCEDURE UpsertBiometricUserMap
    @DeviceId INT,
    @ExternalUserId NVARCHAR(100),
    @PersonType NVARCHAR(20),
    @StudentId INT = NULL,
    @UserId INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MapId INT = (SELECT MapId FROM dbo.BiometricUserMaps WHERE DeviceId = @DeviceId AND ExternalUserId = @ExternalUserId);

    IF @PersonType NOT IN ('Student','Staff') THROW 55001, 'Invalid PersonType', 1;
    IF (@PersonType = 'Student' AND @StudentId IS NULL) THROW 55002, 'StudentId required', 1;
    IF (@PersonType = 'Staff' AND @UserId IS NULL) THROW 55003, 'UserId required', 1;

    IF @MapId IS NULL
    BEGIN
        INSERT INTO dbo.BiometricUserMaps (DeviceId, ExternalUserId, PersonType, StudentId, UserId, IsActive)
        VALUES (@DeviceId, @ExternalUserId, @PersonType, @StudentId, @UserId, @IsActive);
        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.BiometricUserMaps
        SET PersonType = @PersonType,
            StudentId = @StudentId,
            UserId = @UserId,
            IsActive = @IsActive
        WHERE MapId = @MapId;
        RETURN @MapId;
    END
END
GO

CREATE OR ALTER PROCEDURE GetBiometricDevices
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.BiometricDevices
    WHERE (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY RegisteredAtUtc DESC, DeviceId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetBiometricUserMaps
    @DeviceId INT = NULL,
    @PersonType NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.BiometricUserMaps
    WHERE (@DeviceId IS NULL OR DeviceId = @DeviceId)
      AND (@PersonType IS NULL OR PersonType = @PersonType)
    ORDER BY CreatedAtUtc DESC, MapId DESC;
END
GO

CREATE OR ALTER PROCEDURE InsertBiometricRawPunch
    @DeviceId INT,
    @ExternalUserId NVARCHAR(100),
    @PunchTime DATETIME2,
    @Direction NVARCHAR(10) = NULL,
    @Source NVARCHAR(20) = 'Biometric'
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.BiometricRawPunches WHERE DeviceId = @DeviceId AND ExternalUserId = @ExternalUserId AND PunchTime = @PunchTime)
        RETURN -1;

    INSERT INTO dbo.BiometricRawPunches (DeviceId, ExternalUserId, PunchTime, Direction, Source)
    VALUES (@DeviceId, @ExternalUserId, @PunchTime, @Direction, @Source);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO


CREATE OR ALTER PROCEDURE ProcessBiometricPunches
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    IF @ToDate < @FromDate THROW 55011, 'Invalid date range', 1;

    -- Process window
    SELECT rp.PunchId, rp.DeviceId, rp.ExternalUserId, rp.PunchTime, CAST(rp.PunchTime AS DATE) AS PDate
    INTO #P
    FROM dbo.BiometricRawPunches rp
    WHERE rp.Processed = 0
      AND CAST(rp.PunchTime AS DATE) BETWEEN @FromDate AND @ToDate;

    -- Students: mark Present if any punch on day
    SELECT DISTINCT m.StudentId, p.PDate
    INTO #PM_S
    FROM #P p
    INNER JOIN dbo.BiometricUserMaps m
        ON m.DeviceId = p.DeviceId AND m.ExternalUserId = p.ExternalUserId AND m.PersonType = 'Student' AND m.IsActive = 1
    WHERE m.StudentId IS NOT NULL;

    -- Staff: collect min/max punch (In/Out)
    SELECT m.UserId,
           p.PDate,
           MIN(p.PunchTime) AS InTime,
           MAX(p.PunchTime) AS OutTime
    INTO #PM_T
    FROM #P p
    INNER JOIN dbo.BiometricUserMaps m
        ON m.DeviceId = p.DeviceId AND m.ExternalUserId = p.ExternalUserId AND m.PersonType = 'Staff' AND m.IsActive = 1
    WHERE m.UserId IS NOT NULL
    GROUP BY m.UserId, p.PDate;

    -- Upsert StudentAttendance (class/section sourced from Students)
    MERGE dbo.StudentAttendance AS tgt
    USING (
        SELECT s.StudentId, s.ClassName, s.Section, s2.PDate
        FROM #PM_S s2
        INNER JOIN dbo.Students s ON s.StudentId = s2.StudentId
    ) AS src
    ON (tgt.StudentId = src.StudentId AND tgt.AttendanceDate = src.PDate AND ISNULL(tgt.SubjectCode,'') = '' AND ISNULL(tgt.PeriodNo,0) = 0)
    WHEN MATCHED THEN
        UPDATE SET tgt.Status = 'Present', tgt.Remarks = COALESCE(NULLIF(tgt.Remarks,''),'Biometric'), tgt.MarkedAtUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (StudentId, AttendanceDate, ClassName, Section, SubjectCode, PeriodNo, Status, Remarks, MarkedByUserId)
        VALUES (src.StudentId, src.PDate, src.ClassName, src.Section, NULL, NULL, 'Present', 'Biometric', NULL);

    -- Upsert StaffAttendance
    MERGE dbo.StaffAttendance AS tgt
    USING (SELECT UserId, PDate, InTime, OutTime FROM #PM_T) AS src
    ON (tgt.UserId = src.UserId AND tgt.AttendanceDate = src.PDate)
    WHEN MATCHED THEN
        UPDATE SET tgt.Status = 'Present', tgt.InTime = src.InTime, tgt.OutTime = src.OutTime, tgt.Remarks = COALESCE(NULLIF(tgt.Remarks,''),'Biometric'), tgt.MarkedAtUtc = SYSUTCDATETIME(), tgt.Source = 'Biometric'
    WHEN NOT MATCHED THEN
        INSERT (UserId, AttendanceDate, Status, InTime, OutTime, Remarks, MarkedByUserId, Source)
        VALUES (src.UserId, src.PDate, 'Present', src.InTime, src.OutTime, 'Biometric', NULL, 'Biometric');

    -- Mark processed
    UPDATE rp
    SET rp.Processed = 1
    FROM dbo.BiometricRawPunches rp
    INNER JOIN #P p ON p.PunchId = rp.PunchId;

    -- Return count of processed punches
    DECLARE @ProcessedCount INT = (SELECT COUNT(*) FROM #P);
    RETURN @ProcessedCount;
END
GO



CREATE TABLE dbo.NotificationLogs
(
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    Type NVARCHAR(20) NOT NULL, -- Email/SMS
    Recipient NVARCHAR(256) NOT NULL, -- email or phone
    Subject NVARCHAR(200) NULL,
    Body NVARCHAR(2000) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending/Sent/Failed
    [Error] NVARCHAR(1000) NULL,
    RelatedDate DATE NOT NULL,
    ClassName NVARCHAR(50) NULL,
    Section NVARCHAR(10) NULL,
    StudentId INT NULL,
    AttemptCount INT NOT NULL DEFAULT 0,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    SentAtUtc DATETIME2 NULL
);
CREATE INDEX IX_NotificationLogs_Date ON dbo.NotificationLogs(RelatedDate, Type, Status);
CREATE INDEX IX_NotificationLogs_Student ON dbo.NotificationLogs(StudentId);

GO

-- Fetch absentees (exclude Excused)
CREATE OR ALTER PROCEDURE GetAbsentStudentContacts
    @AttendanceDate DATE,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.StudentId,
        CONCAT(s.FirstName, CASE WHEN ISNULL(s.LastName,'') = '' THEN '' ELSE ' ' + s.LastName END) AS StudentName,
        s.ClassName,
        s.Section,
        @AttendanceDate AS AttendanceDate,
        s.GuardianName,
        s.Email,
        s.Phone
    FROM dbo.StudentAttendance a
    INNER JOIN dbo.Students s ON s.StudentId = a.StudentId
    WHERE a.AttendanceDate = @AttendanceDate
      AND a.Status = 'Absent'
      AND ISNULL(a.SubjectCode,'') = '' AND ISNULL(a.PeriodNo,0) = 0
      AND (@ClassName IS NULL OR s.ClassName = @ClassName)
      AND (@Section IS NULL OR s.Section = @Section)
    ORDER BY s.ClassName, s.Section, s.StudentId;
END
GO

CREATE OR ALTER PROCEDURE InsertNotificationLog
    @Type NVARCHAR(20),
    @Recipient NVARCHAR(256),
    @Subject NVARCHAR(200) = NULL,
    @Body NVARCHAR(2000),
    @Status NVARCHAR(20),
    @Error NVARCHAR(1000) = NULL,
    @RelatedDate DATE,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @StudentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.NotificationLogs
    (Type, Recipient, Subject, Body, Status, [Error], RelatedDate, ClassName, Section, StudentId)
    VALUES
    (@Type, @Recipient, @Subject, @Body, @Status, @Error, @RelatedDate, @ClassName, @Section, @StudentId);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateNotificationLogStatus
    @NotificationId INT,
    @Status NVARCHAR(20),
    @Error NVARCHAR(1000) = NULL,
    @SentAtUtc DATETIME2 = NULL,
    @AttemptIncrement INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.NotificationLogs
    SET Status = @Status,
        [Error] = @Error,
        SentAtUtc = CASE WHEN @SentAtUtc IS NOT NULL THEN @SentAtUtc ELSE SentAtUtc END,
        AttemptCount = AttemptCount + @AttemptIncrement
    WHERE NotificationId = @NotificationId;

    RETURN @NotificationId;
END
GO

CREATE OR ALTER PROCEDURE GetNotificationLogs
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Type NVARCHAR(20) = NULL,
    @Status NVARCHAR(20) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @StudentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.NotificationLogs
    WHERE (@FromDate IS NULL OR RelatedDate >= @FromDate)
      AND (@ToDate IS NULL OR RelatedDate <= @ToDate)
      AND (@Type IS NULL OR Type = @Type)
      AND (@Status IS NULL OR Status = @Status)
      AND (@ClassName IS NULL OR ClassName = @ClassName)
      AND (@Section IS NULL OR Section = @Section)
      AND (@StudentId IS NULL OR StudentId = @StudentId)
    ORDER BY CreatedAtUtc DESC, NotificationId DESC;
END
GO
