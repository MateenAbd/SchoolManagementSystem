CREATE TABLE dbo.Subjects
(
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    SubjectCode NVARCHAR(50) NOT NULL,
    SubjectName NVARCHAR(200) NOT NULL,
    ShortName NVARCHAR(50) NULL,
    [Description] NVARCHAR(1000) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
CREATE UNIQUE INDEX UX_Subjects_Code ON dbo.Subjects(SubjectCode);

GO

CREATE TABLE dbo.Courses
(
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    SubjectId INT NOT NULL,
    ClassName NVARCHAR(50) NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Courses_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId)
);
CREATE INDEX IX_Courses_Year_Class_Subject ON dbo.Courses(AcademicYear, ClassName, SubjectId);

GO

CREATE TABLE dbo.CourseSyllabus
(
    SyllabusId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    UnitNo INT NULL,
    Topic NVARCHAR(500) NOT NULL,
    SubTopic NVARCHAR(500) NULL,
    Objectives NVARCHAR(2000) NULL,
    ReferenceMaterials NVARCHAR(1000) NULL,
    EstimatedHours DECIMAL(10,2) NULL,
    OrderIndex INT NULL,
    CONSTRAINT FK_CourseSyllabus_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses(CourseId) ON DELETE CASCADE
);
CREATE INDEX IX_Syllabus_Course ON dbo.CourseSyllabus(CourseId, OrderIndex);

GO

CREATE OR ALTER PROCEDURE CreateSubject
    @SubjectCode NVARCHAR(50),
    @SubjectName NVARCHAR(200),
    @ShortName NVARCHAR(50) = NULL,
    @Description NVARCHAR(1000) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Subjects WHERE SubjectCode = @SubjectCode)
        THROW 56001, 'Subject code already exists', 1;

    INSERT INTO dbo.Subjects (SubjectCode, SubjectName, ShortName, [Description], IsActive)
    VALUES (@SubjectCode, @SubjectName, @ShortName, @Description, @IsActive);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateSubject
    @SubjectId INT,
    @SubjectCode NVARCHAR(50),
    @SubjectName NVARCHAR(200),
    @ShortName NVARCHAR(50) = NULL,
    @Description NVARCHAR(1000) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Subjects WHERE SubjectCode = @SubjectCode AND SubjectId <> @SubjectId)
        THROW 56002, 'Subject code already exists for another subject', 1;

    UPDATE dbo.Subjects
    SET SubjectCode = @SubjectCode,
        SubjectName = @SubjectName,
        ShortName = @ShortName,
        [Description] = @Description,
        IsActive = @IsActive
    WHERE SubjectId = @SubjectId;

    RETURN @SubjectId;
END
GO

CREATE OR ALTER PROCEDURE DeleteSubject
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Subjects WHERE SubjectId = @SubjectId;
    RETURN @SubjectId;
END
GO

CREATE OR ALTER PROCEDURE GetSubjectById
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM dbo.Subjects WHERE SubjectId = @SubjectId;
END
GO

CREATE OR ALTER PROCEDURE GetSubjectList
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM dbo.Subjects
    WHERE (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY SubjectName;
END
GO



CREATE OR ALTER PROCEDURE CreateCourse
    @SubjectId INT,
    @ClassName NVARCHAR(50),
    @AcademicYear NVARCHAR(15),
    @Description NVARCHAR(1000) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Subjects WHERE SubjectId = @SubjectId)
        THROW 56101, 'Subject not found', 1;

    INSERT INTO dbo.Courses (SubjectId, ClassName, AcademicYear, [Description], IsActive)
    VALUES (@SubjectId, @ClassName, @AcademicYear, @Description, @IsActive);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateCourse
    @CourseId INT,
    @SubjectId INT,
    @ClassName NVARCHAR(50),
    @AcademicYear NVARCHAR(15),
    @Description NVARCHAR(1000) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Subjects WHERE SubjectId = @SubjectId)
        THROW 56102, 'Subject not found', 1;

    UPDATE dbo.Courses
    SET SubjectId = @SubjectId,
        ClassName = @ClassName,
        AcademicYear = @AcademicYear,
        [Description] = @Description,
        IsActive = @IsActive
    WHERE CourseId = @CourseId;

    RETURN @CourseId;
END
GO

CREATE OR ALTER PROCEDURE DeleteCourse
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Courses WHERE CourseId = @CourseId;
    RETURN @CourseId;
END
GO

CREATE OR ALTER PROCEDURE GetCourseById
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM dbo.Courses WHERE CourseId = @CourseId;
END
GO

CREATE OR ALTER PROCEDURE GetCourseList
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @SubjectId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Courses
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@ClassName IS NULL OR ClassName = @ClassName)
      AND (@SubjectId IS NULL OR SubjectId = @SubjectId)
      AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY AcademicYear DESC, ClassName, CourseId DESC;
END
GO


CREATE OR ALTER PROCEDURE AddSyllabusItem
    @CourseId INT,
    @UnitNo INT = NULL,
    @Topic NVARCHAR(500),
    @SubTopic NVARCHAR(500) = NULL,
    @Objectives NVARCHAR(2000) = NULL,
    @ReferenceMaterials NVARCHAR(1000) = NULL,
    @EstimatedHours DECIMAL(10,2) = NULL,
    @OrderIndex INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Courses WHERE CourseId = @CourseId)
        THROW 56201, 'Course not found', 1;

    INSERT INTO dbo.CourseSyllabus (CourseId, UnitNo, Topic, SubTopic, Objectives, ReferenceMaterials, EstimatedHours, OrderIndex)
    VALUES (@CourseId, @UnitNo, @Topic, @SubTopic, @Objectives, @ReferenceMaterials, @EstimatedHours, @OrderIndex);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateSyllabusItem
    @SyllabusId INT,
    @CourseId INT,
    @UnitNo INT = NULL,
    @Topic NVARCHAR(500),
    @SubTopic NVARCHAR(500) = NULL,
    @Objectives NVARCHAR(2000) = NULL,
    @ReferenceMaterials NVARCHAR(1000) = NULL,
    @EstimatedHours DECIMAL(10,2) = NULL,
    @OrderIndex INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CourseSyllabus
    SET CourseId = @CourseId,
        UnitNo = @UnitNo,
        Topic = @Topic,
        SubTopic = @SubTopic,
        Objectives = @Objectives,
        ReferenceMaterials = @ReferenceMaterials,
        EstimatedHours = @EstimatedHours,
        OrderIndex = @OrderIndex
    WHERE SyllabusId = @SyllabusId;

    RETURN @SyllabusId;
END
GO

CREATE OR ALTER PROCEDURE DeleteSyllabusItem
    @SyllabusId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.CourseSyllabus WHERE SyllabusId = @SyllabusId;
    RETURN @SyllabusId;
END
GO

CREATE OR ALTER PROCEDURE GetSyllabusByCourse
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM dbo.CourseSyllabus
    WHERE CourseId = @CourseId
    ORDER BY COALESCE(OrderIndex, 9999), SyllabusId;
END
GO







CREATE TABLE dbo.Classrooms
(
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    RoomCode NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(100) NULL,
    Capacity INT NULL,
    [Location] NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
CREATE UNIQUE INDEX UX_Classrooms_RoomCode ON dbo.Classrooms(RoomCode);
GO

CREATE TABLE dbo.TimetableEntries
(
    TimetableId INT IDENTITY(1,1) PRIMARY KEY,
    AcademicYear NVARCHAR(15) NOT NULL,
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,
    DayOfWeek TINYINT NOT NULL, -- 1..7
    PeriodNo INT NULL,--either PeriodNo or start/end time
    StartTime TIME NULL,
    EndTime TIME NULL,
    SubjectId INT NOT NULL,
    CourseId INT NULL,
    TeacherUserId INT NULL,
    RoomId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    CONSTRAINT FK_Timetable_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId),
    CONSTRAINT FK_Timetable_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses(CourseId),
    CONSTRAINT FK_Timetable_Teachers FOREIGN KEY (TeacherUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Timetable_Rooms FOREIGN KEY (RoomId) REFERENCES dbo.Classrooms(RoomId)
);
CREATE INDEX IX_Timetable_Class ON dbo.TimetableEntries(AcademicYear, ClassName, Section, DayOfWeek);
CREATE INDEX IX_Timetable_Teacher ON dbo.TimetableEntries(AcademicYear, TeacherUserId, DayOfWeek);
CREATE INDEX IX_Timetable_Room ON dbo.TimetableEntries(AcademicYear, RoomId, DayOfWeek);
GO

CREATE OR ALTER PROCEDURE CreateClassroom
    @RoomCode NVARCHAR(50),
    @Name NVARCHAR(100) = NULL,
    @Capacity INT = NULL,
    @Location NVARCHAR(100) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Classrooms WHERE RoomCode = @RoomCode)
        THROW 57001, 'RoomCode already exists', 1;

    INSERT INTO dbo.Classrooms(RoomCode, [Name], Capacity, [Location], IsActive)
    VALUES (@RoomCode, @Name, @Capacity, @Location, @IsActive);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateClassroom
    @RoomId INT,
    @RoomCode NVARCHAR(50),
    @Name NVARCHAR(100) = NULL,
    @Capacity INT = NULL,
    @Location NVARCHAR(100) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Classrooms WHERE RoomCode = @RoomCode AND RoomId <> @RoomId)
        THROW 57002, 'RoomCode already used by another room', 1;

    UPDATE dbo.Classrooms
    SET RoomCode = @RoomCode, [Name] = @Name, Capacity = @Capacity, [Location] = @Location, IsActive = @IsActive
    WHERE RoomId = @RoomId;

    RETURN @RoomId;
END
GO

CREATE OR ALTER PROCEDURE DeleteClassroom
    @RoomId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Classrooms WHERE RoomId = @RoomId;
    RETURN @RoomId;
END
GO

CREATE OR ALTER PROCEDURE GetClassroomById
    @RoomId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.Classrooms WHERE RoomId = @RoomId;
END
GO

CREATE OR ALTER PROCEDURE GetClassroomList
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Classrooms
    WHERE (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY RoomCode;
END
GO


CREATE OR ALTER PROCEDURE GetAvailableRoomsBySlot
    @AcademicYear NVARCHAR(15),
    @DayOfWeek TINYINT,
    @PeriodNo INT = NULL,
    @StartTime TIME = NULL,
    @EndTime TIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    --rooms occupied in that slot
    WITH OCC AS
    (
        SELECT DISTINCT RoomId
        FROM dbo.TimetableEntries
        WHERE AcademicYear = @AcademicYear
          AND DayOfWeek = @DayOfWeek
          AND RoomId IS NOT NULL
          AND IsActive = 1
          AND (
                (@PeriodNo IS NOT NULL AND PeriodNo IS NOT NULL AND PeriodNo = @PeriodNo)
                OR
                (@PeriodNo IS NULL AND PeriodNo IS NULL
                 AND @StartTime IS NOT NULL AND @EndTime IS NOT NULL
                 AND StartTime IS NOT NULL AND EndTime IS NOT NULL
                 AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime))
              )
    )
    SELECT c.*
    FROM dbo.Classrooms c
    WHERE c.IsActive = 1
      AND c.RoomId NOT IN (SELECT RoomId FROM OCC)
    ORDER BY c.RoomCode;
END
GO

-- return codes: >0 OK (id),  -1 class conflict, -2 teacher conflict,-3 room conflict
CREATE OR ALTER PROCEDURE UpsertTimetableEntry
    @TimetableId INT,
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @DayOfWeek TINYINT,
    @PeriodNo INT = NULL,
    @StartTime TIME = NULL,
    @EndTime TIME = NULL,
    @SubjectId INT,
    @CourseId INT = NULL,
    @TeacherUserId INT = NULL,
    @RoomId INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF (@PeriodNo IS NULL AND (@StartTime IS NULL OR @EndTime IS NULL))
        THROW 57101, 'Provide PeriodNo or Start/End time', 1;

    --check class conflict
    IF EXISTS (
        SELECT 1 FROM dbo.TimetableEntries
        WHERE AcademicYear = @AcademicYear AND ClassName = @ClassName AND ISNULL(Section,'') = ISNULL(@Section,'')
          AND DayOfWeek = @DayOfWeek AND IsActive = 1
          AND (@TimetableId = 0 OR TimetableId <> @TimetableId)
          AND (
               (@PeriodNo IS NOT NULL AND PeriodNo IS NOT NULL AND PeriodNo = @PeriodNo)
               OR
               (@PeriodNo IS NULL AND PeriodNo IS NULL
                AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime))
          )
    )
    BEGIN
        RETURN -1;
    END

    --check teacher conflict
    IF (@TeacherUserId IS NOT NULL)
    BEGIN
        IF EXISTS (
            SELECT 1 FROM dbo.TimetableEntries
            WHERE AcademicYear = @AcademicYear AND DayOfWeek = @DayOfWeek
              AND TeacherUserId = @TeacherUserId AND IsActive = 1
              AND (@TimetableId = 0 OR TimetableId <> @TimetableId)
              AND (
                   (@PeriodNo IS NOT NULL AND PeriodNo IS NOT NULL AND PeriodNo = @PeriodNo)
                   OR
                   (@PeriodNo IS NULL AND PeriodNo IS NULL
                    AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime))
              )
        )
        BEGIN
            RETURN -2;
        END
    END

    --check room conflict
    IF (@RoomId IS NOT NULL)
    BEGIN
        IF EXISTS (
            SELECT 1 FROM dbo.TimetableEntries
            WHERE AcademicYear = @AcademicYear AND DayOfWeek = @DayOfWeek
              AND RoomId = @RoomId AND IsActive = 1
              AND (@TimetableId = 0 OR TimetableId <> @TimetableId)
              AND (
                   (@PeriodNo IS NOT NULL AND PeriodNo IS NOT NULL AND PeriodNo = @PeriodNo)
                   OR
                   (@PeriodNo IS NULL AND PeriodNo IS NULL
                    AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime))
              )
        )
        BEGIN
            RETURN -3;
        END
    END

    IF @TimetableId = 0
    BEGIN
        INSERT INTO dbo.TimetableEntries
        (AcademicYear, ClassName, Section, DayOfWeek, PeriodNo, StartTime, EndTime, SubjectId, CourseId, TeacherUserId, RoomId, IsActive)
        VALUES
        (@AcademicYear, @ClassName, @Section, @DayOfWeek, @PeriodNo, @StartTime, @EndTime, @SubjectId, @CourseId, @TeacherUserId, @RoomId, @IsActive);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.TimetableEntries
        SET AcademicYear = @AcademicYear,
            ClassName = @ClassName,
            Section = @Section,
            DayOfWeek = @DayOfWeek,
            PeriodNo = @PeriodNo,
            StartTime = @StartTime,
            EndTime = @EndTime,
            SubjectId = @SubjectId,
            CourseId = @CourseId,
            TeacherUserId = @TeacherUserId,
            RoomId = @RoomId,
            IsActive = @IsActive,
            UpdatedAtUtc = SYSUTCDATETIME()
        WHERE TimetableId = @TimetableId;

        RETURN @TimetableId;
    END
END
GO

CREATE OR ALTER PROCEDURE DeleteTimetableEntry
    @TimetableId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.TimetableEntries WHERE TimetableId = @TimetableId;
    RETURN @TimetableId;
END
GO

CREATE OR ALTER PROCEDURE GetClassTimetable
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.TimetableEntries
    WHERE AcademicYear = @AcademicYear
      AND ClassName = @ClassName
      AND ISNULL(Section,'') = ISNULL(@Section,'')
      AND IsActive = 1
    ORDER BY DayOfWeek,
             CASE WHEN PeriodNo IS NOT NULL THEN PeriodNo ELSE 1000 END,
             StartTime,
             TimetableId;
END
GO

CREATE OR ALTER PROCEDURE GetTeacherTimetable
    @AcademicYear NVARCHAR(15),
    @TeacherUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.TimetableEntries
    WHERE AcademicYear = @AcademicYear
      AND TeacherUserId = @TeacherUserId
      AND IsActive = 1
    ORDER BY DayOfWeek,
             CASE WHEN PeriodNo IS NOT NULL THEN PeriodNo ELSE 1000 END,
             StartTime,
             TimetableId;
END
GO

CREATE OR ALTER PROCEDURE ValidateTimetableConflict
    @TimetableId INT,
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @DayOfWeek TINYINT,
    @PeriodNo INT = NULL,
    @StartTime TIME = NULL,
    @EndTime TIME = NULL,
    @TeacherUserId INT = NULL,
    @RoomId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    --class conflict
    IF EXISTS (
        SELECT 1 FROM dbo.TimetableEntries
        WHERE AcademicYear = @AcademicYear AND ClassName = @ClassName AND ISNULL(Section,'') = ISNULL(@Section,'')
          AND DayOfWeek = @DayOfWeek AND IsActive = 1
          AND (@TimetableId = 0 OR TimetableId <> @TimetableId)
          AND (
               (@PeriodNo IS NOT NULL AND PeriodNo IS NOT NULL AND PeriodNo = @PeriodNo)
               OR
               (@PeriodNo IS NULL AND PeriodNo IS NULL
                AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime))
          )
    ) RETURN -1;

    --teacher conflict
    IF (@TeacherUserId IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.TimetableEntries
        WHERE AcademicYear = @AcademicYear AND DayOfWeek = @DayOfWeek
          AND TeacherUserId = @TeacherUserId AND IsActive = 1
          AND (@TimetableId = 0 OR TimetableId <> @TimetableId)
          AND (
               (@PeriodNo IS NOT NULL AND PeriodNo IS NOT NULL AND PeriodNo = @PeriodNo)
               OR
               (@PeriodNo IS NULL AND PeriodNo IS NULL
                AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime))
          )
    )) RETURN -2;

    --room conflict
    IF (@RoomId IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.TimetableEntries
        WHERE AcademicYear = @AcademicYear AND DayOfWeek = @DayOfWeek
          AND RoomId = @RoomId AND IsActive = 1
          AND (@TimetableId = 0 OR TimetableId <> @TimetableId)
          AND (
               (@PeriodNo IS NOT NULL AND PeriodNo IS NOT NULL AND PeriodNo = @PeriodNo)
               OR
               (@PeriodNo IS NULL AND PeriodNo IS NULL
                AND NOT (EndTime <= @StartTime OR StartTime >= @EndTime))
          )
    )) RETURN -3;

    RETURN 0;
END
GO