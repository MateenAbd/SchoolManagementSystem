CREATE TABLE dbo.StudentAttendance
(
    AttendanceId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,
    SubjectCode NVARCHAR(50) NULL,
    PeriodNo INT NULL,
    Status NVARCHAR(20) NOT NULL, -- Present/Absent/Late/Excused
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

-- Upsert attendance (handles class-wise or subject-wise)
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
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending/Approved/Rejected/Cancelled
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