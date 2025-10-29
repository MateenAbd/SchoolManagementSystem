CREATE TABLE dbo.Subjects
(
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    SubjectCode NVARCHAR(50) NOT NULL,
    SubjectName NVARCHAR(200) NOT NULL,
    ShortName NVARCHAR(50) NULL,
    Description NVARCHAR(1000) NULL,
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
    Description NVARCHAR(1000) NULL,
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

    INSERT INTO dbo.Subjects (SubjectCode, SubjectName, ShortName, Description, IsActive)
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
        Description = @Description,
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

    INSERT INTO dbo.Courses (SubjectId, ClassName, AcademicYear, Description, IsActive)
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
        Description = @Description,
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
    Name NVARCHAR(100) NULL,
    Capacity INT NULL,
    Location NVARCHAR(100) NULL,
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

    INSERT INTO dbo.Classrooms(RoomCode, Name, Capacity, Location, IsActive)
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
    SET RoomCode = @RoomCode, Name = @Name, Capacity = @Capacity, Location = @Location, IsActive = @IsActive
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

-- return codes: >0 OK (TimetabelId),  -1 class conflict, -2 teacher conflict,-3 room conflict
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
             CASE WHEN PeriodNo IS NOT NULL THEN PeriodNo ELSE 1000 END,--end mai null period wale
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

CREATE TABLE dbo.LessonPlans
(
    PlanId INT IDENTITY(1,1) PRIMARY KEY,
    AcademicYear NVARCHAR(15) NOT NULL,
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,

    CourseId INT NULL,
    SubjectId INT NULL,

    TeacherUserId INT NOT NULL,
    PlanDate DATE NOT NULL,
    PeriodNo INT NULL,
    StartTime TIME NULL,
    EndTime TIME NULL,

    Topic NVARCHAR(500) NOT NULL,
    SubTopic NVARCHAR(500) NULL,
    Objectives NVARCHAR(2000) NULL,
    Activities NVARCHAR(2000) NULL,
    Resources NVARCHAR(1000) NULL,
    Homework NVARCHAR(1000) NULL,
    AssessmentMethods NVARCHAR(1000) NULL, --quiz/worksheet/viva/etc

    Status NVARCHAR(20) NOT NULL DEFAULT 'Draft', --Draft/Planned/Delivered
    Notes NVARCHAR(1000) NULL,

    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    DeliveredAtUtc DATETIME2 NULL,

    CONSTRAINT FK_LP_Course FOREIGN KEY (CourseId) REFERENCES dbo.Courses(CourseId),
    CONSTRAINT FK_LP_Subject FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId),
    CONSTRAINT FK_LP_Teacher FOREIGN KEY (TeacherUserId) REFERENCES dbo.Users(UserId)
);

CREATE INDEX IX_LP_Date ON dbo.LessonPlans(PlanDate);
CREATE INDEX IX_LP_Class ON dbo.LessonPlans(AcademicYear, ClassName, Section);
CREATE INDEX IX_LP_Teacher ON dbo.LessonPlans(TeacherUserId, PlanDate);
GO

CREATE OR ALTER PROCEDURE CreateLessonPlan
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @CourseId INT = NULL,
    @SubjectId INT = NULL,
    @TeacherUserId INT,
    @PlanDate DATE,
    @PeriodNo INT = NULL,
    @StartTime TIME = NULL,
    @EndTime TIME = NULL,
    @Topic NVARCHAR(500),
    @SubTopic NVARCHAR(500) = NULL,
    @Objectives NVARCHAR(2000) = NULL,
    @Activities NVARCHAR(2000) = NULL,
    @Resources NVARCHAR(1000) = NULL,
    @Homework NVARCHAR(1000) = NULL,
    @AssessmentMethods NVARCHAR(1000) = NULL,
    @Status NVARCHAR(20) = 'Draft',
    @Notes NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF (@CourseId IS NULL AND @SubjectId IS NULL)
        THROW 57201, 'Either CourseId or SubjectId is required', 1;
    IF (@PeriodNo IS NULL AND @StartTime IS NULL AND @EndTime IS NULL)
        THROW 57202, 'Provide PeriodNo or both StartTime and EndTime', 1;
    IF (@PeriodNo IS NULL AND ((@StartTime IS NOT NULL AND @EndTime IS NULL) OR (@StartTime IS NULL AND @EndTime IS NOT NULL)))
        THROW 57203, 'Provide both StartTime and EndTime', 1;


    INSERT INTO dbo.LessonPlans
    (AcademicYear, ClassName, Section, CourseId, SubjectId, TeacherUserId, PlanDate, PeriodNo, StartTime, EndTime,
     Topic, SubTopic, Objectives, Activities, Resources, Homework, AssessmentMethods, Status, Notes)
    VALUES
    (@AcademicYear, @ClassName, @Section, @CourseId, @SubjectId, @TeacherUserId, @PlanDate, @PeriodNo, @StartTime, @EndTime,
     @Topic, @SubTopic, @Objectives, @Activities, @Resources, @Homework, @AssessmentMethods, @Status, @Notes);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateLessonPlan
    @PlanId INT,
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @CourseId INT = NULL,
    @SubjectId INT = NULL,
    @TeacherUserId INT,
    @PlanDate DATE,
    @PeriodNo INT = NULL,
    @StartTime TIME = NULL,
    @EndTime TIME = NULL,
    @Topic NVARCHAR(500),
    @SubTopic NVARCHAR(500) = NULL,
    @Objectives NVARCHAR(2000) = NULL,
    @Activities NVARCHAR(2000) = NULL,
    @Resources NVARCHAR(1000) = NULL,
    @Homework NVARCHAR(1000) = NULL,
    @AssessmentMethods NVARCHAR(1000) = NULL,
    @Status NVARCHAR(20) = 'Draft',
    @Notes NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF (@CourseId IS NULL AND @SubjectId IS NULL)
        THROW 57211, 'Either CourseId or SubjectId is required', 1;
    IF (@PeriodNo IS NULL AND @StartTime IS NULL AND @EndTime IS NULL)
        THROW 57212, 'Provide PeriodNo or both StartTime and EndTime', 1;
    IF (@PeriodNo IS NULL AND ((@StartTime IS NOT NULL AND @EndTime IS NULL) OR (@StartTime IS NULL AND @EndTime IS NOT NULL)))
        THROW 57213, 'Provide both StartTime and EndTime', 1;

    UPDATE dbo.LessonPlans
    SET AcademicYear = @AcademicYear,
        ClassName = @ClassName,
        Section = @Section,
        CourseId = @CourseId,
        SubjectId = @SubjectId,
        TeacherUserId = @TeacherUserId,
        PlanDate = @PlanDate,
        PeriodNo = @PeriodNo,
        StartTime = @StartTime,
        EndTime = @EndTime,
        Topic = @Topic,
        SubTopic = @SubTopic,
        Objectives = @Objectives,
        Activities = @Activities,
        Resources = @Resources,
        Homework = @Homework,
        AssessmentMethods = @AssessmentMethods,
        Status = @Status,
        Notes = @Notes,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE PlanId = @PlanId;

    RETURN @PlanId;
END
GO

CREATE OR ALTER PROCEDURE DeleteLessonPlan
    @PlanId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.LessonPlans WHERE PlanId = @PlanId;
    RETURN @PlanId;
END
GO

CREATE OR ALTER PROCEDURE UpdateLessonPlanStatus
    @PlanId INT,
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.LessonPlans
    SET Status = @Status,
        DeliveredAtUtc = CASE WHEN @Status = 'Delivered' THEN SYSUTCDATETIME() ELSE DeliveredAtUtc END,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE PlanId = @PlanId;

    RETURN @PlanId;
END
GO

CREATE OR ALTER PROCEDURE GetLessonPlanById
    @PlanId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.LessonPlans WHERE PlanId = @PlanId;
END
GO

CREATE OR ALTER PROCEDURE GetLessonPlanList
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @SubjectId INT = NULL,
    @TeacherUserId INT = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.LessonPlans
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@ClassName IS NULL OR ClassName = @ClassName)
      AND (@Section IS NULL OR Section = @Section)
      AND (@SubjectId IS NULL OR SubjectId = @SubjectId)
      AND (@TeacherUserId IS NULL OR TeacherUserId = @TeacherUserId)
      AND (@FromDate IS NULL OR PlanDate >= @FromDate)
      AND (@ToDate IS NULL OR PlanDate <= @ToDate)
      AND (@Status IS NULL OR Status = @Status)
    ORDER BY PlanDate DESC, ClassName, Section, PlanId DESC;
END
GO


 CREATE TABLE dbo.CalendarEvents
(
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    AcademicYear NVARCHAR(15) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    EventType NVARCHAR(50) NOT NULL,--Holiday/Exam/Activity/PTM/General
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    IsAllDay BIT NOT NULL DEFAULT 1,
    ClassName NVARCHAR(50) NULL,
    Section NVARCHAR(10) NULL,
    Location NVARCHAR(200) NULL,
    Description NVARCHAR(2000) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL
);
CREATE INDEX IX_CalendarEvents_Range ON dbo.CalendarEvents(AcademicYear, StartDate, EndDate);
CREATE INDEX IX_CalendarEvents_Type ON dbo.CalendarEvents(EventType, IsActive);
GO

CREATE OR ALTER PROCEDURE CreateCalendarEvent
    @AcademicYear NVARCHAR(15),
    @Title NVARCHAR(200),
    @EventType NVARCHAR(50),
    @StartDate DATETIME2,
    @EndDate DATETIME2 = NULL,
    @IsAllDay BIT = 1,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @Location NVARCHAR(200) = NULL,
    @Description NVARCHAR(2000) = NULL,
    @IsActive BIT = 1,
    @CreatedByUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.CalendarEvents
    (AcademicYear, Title, EventType, StartDate, EndDate, IsAllDay, ClassName, Section, Location, Description, IsActive, CreatedByUserId)
    VALUES
    (@AcademicYear, @Title, @EventType, @StartDate, @EndDate, @IsAllDay, @ClassName, @Section, @Location, @Description, @IsActive, @CreatedByUserId);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateCalendarEvent
    @EventId INT,
    @AcademicYear NVARCHAR(15),
    @Title NVARCHAR(200),
    @EventType NVARCHAR(50),
    @StartDate DATETIME2,
    @EndDate DATETIME2 = NULL,
    @IsAllDay BIT = 1,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @Location NVARCHAR(200) = NULL,
    @Description NVARCHAR(2000) = NULL,
    @IsActive BIT = 1,
    @UpdatedByUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CalendarEvents
    SET AcademicYear = @AcademicYear,
        Title = @Title,
        EventType = @EventType,
        StartDate = @StartDate,
        EndDate = @EndDate,
        IsAllDay = @IsAllDay,
        ClassName = @ClassName,
        Section = @Section,
        Location = @Location,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAtUtc = SYSUTCDATETIME(),
        UpdatedByUserId = @UpdatedByUserId
    WHERE EventId = @EventId;

    RETURN @EventId;
END
GO

CREATE OR ALTER PROCEDURE DeleteCalendarEvent
    @EventId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.CalendarEvents WHERE EventId = @EventId;
    RETURN @EventId;
END
GO

CREATE OR ALTER PROCEDURE GetCalendarEventById
    @EventId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM dbo.CalendarEvents WHERE EventId = @EventId;
END
GO

CREATE OR ALTER PROCEDURE GetCalendarEvents
    @AcademicYear NVARCHAR(15) = NULL,
    @FromDate DATETIME2 = NULL,
    @ToDate DATETIME2 = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @EventType NVARCHAR(50) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.CalendarEvents
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@IsActive IS NULL OR IsActive = @IsActive)
      AND (@EventType IS NULL OR EventType = @EventType)
      AND (@ClassName IS NULL OR ClassName = @ClassName)
      AND (@Section IS NULL OR Section = @Section)
      AND (
            @FromDate IS NULL OR @ToDate IS NULL
            OR (StartDate <= @ToDate AND ISNULL(EndDate, StartDate) >= @FromDate)
          )
    ORDER BY StartDate ASC, EventId ASC;
END
GO


CREATE TABLE dbo.Exams
(
    ExamId INT IDENTITY(1,1) PRIMARY KEY,
    AcademicYear NVARCHAR(15) NOT NULL,
    ExamName NVARCHAR(200) NOT NULL,
    ExamType NVARCHAR(50) NOT NULL, --UnitTest/Term/Final/Other
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,
    StartDate DATE NULL,
    EndDate DATE NULL,
    IsPublished BIT NOT NULL DEFAULT 0,
    Description NVARCHAR(1000) NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL
);
CREATE INDEX IX_Exams_Year_Class ON dbo.Exams(AcademicYear, ClassName, Section);
GO

CREATE TABLE dbo.ExamPapers
(
    PaperId INT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL,
    SubjectId INT NOT NULL,
    ExamDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    DurationMinutes INT NOT NULL,
    RoomId INT NULL,
    InvigilatorUserId INT NULL,
    MaxMarks INT NOT NULL,
    PassingMarks INT NULL,
    Notes NVARCHAR(1000) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    CONSTRAINT FK_ExamPapers_Exams FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId) ON DELETE CASCADE,
    CONSTRAINT FK_ExamPapers_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId),
    CONSTRAINT FK_ExamPapers_Rooms FOREIGN KEY (RoomId) REFERENCES dbo.Classrooms(RoomId),
    CONSTRAINT FK_ExamPapers_Users FOREIGN KEY (InvigilatorUserId) REFERENCES dbo.Users(UserId)
);
CREATE INDEX IX_ExamPapers_Exam ON dbo.ExamPapers(ExamId, ExamDate, StartTime);
GO

CREATE OR ALTER PROCEDURE CreateExam
    @AcademicYear NVARCHAR(15),
    @ExamName NVARCHAR(200),
    @ExamType NVARCHAR(50),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL,
    @IsPublished BIT = 0,
    @Description NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Exams (AcademicYear, ExamName, ExamType, ClassName, Section, StartDate, EndDate, IsPublished, Description)
    VALUES (@AcademicYear, @ExamName, @ExamType, @ClassName, @Section, @StartDate, @EndDate, @IsPublished, @Description);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateExam
    @ExamId INT,
    @AcademicYear NVARCHAR(15),
    @ExamName NVARCHAR(200),
    @ExamType NVARCHAR(50),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL,
    @IsPublished BIT = 0,
    @Description NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Exams
    SET AcademicYear = @AcademicYear,
        ExamName = @ExamName,
        ExamType = @ExamType,
        ClassName = @ClassName,
        Section = @Section,
        StartDate = @StartDate,
        EndDate = @EndDate,
        IsPublished = @IsPublished,
        Description = @Description,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE ExamId = @ExamId;

    RETURN @ExamId;
END
GO

CREATE OR ALTER PROCEDURE DeleteExam
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Exams WHERE ExamId = @ExamId;
    RETURN @ExamId;
END
GO

CREATE OR ALTER PROCEDURE GetExamById
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.Exams WHERE ExamId = @ExamId;
END
GO

CREATE OR ALTER PROCEDURE GetExamList
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @ExamType NVARCHAR(50) = NULL,
    @IsPublished BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.Exams
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@ClassName IS NULL OR ClassName = @ClassName)
      AND (@Section IS NULL OR Section = @Section)
      AND (@ExamType IS NULL OR ExamType = @ExamType)
      AND (@IsPublished IS NULL OR IsPublished = @IsPublished)
    ORDER BY AcademicYear DESC, ClassName, Section, ExamId DESC;
END
GO


CREATE OR ALTER PROCEDURE UpsertExamPaper
    @PaperId INT,
    @ExamId INT,
    @SubjectId INT,
    @ExamDate DATE,
    @StartTime TIME,
    @EndTime TIME,
    @DurationMinutes INT,
    @RoomId INT = NULL,
    @InvigilatorUserId INT = NULL,
    @MaxMarks INT,
    @PassingMarks INT = NULL,
    @Notes NVARCHAR(1000) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @EndTime <= @StartTime THROW 58001, 'EndTime must be greater than StartTime', 1;

    DECLARE @AcademicYear NVARCHAR(15), @ClassName NVARCHAR(50), @Section NVARCHAR(10);
    SELECT @AcademicYear = e.AcademicYear, @ClassName = e.ClassName, @Section = e.Section
    FROM dbo.Exams e WHERE e.ExamId = @ExamId;

    IF @AcademicYear IS NULL THROW 58002, 'Exam not found', 1;

    --class conflict: another paper for the same exam/class overlaps
    IF EXISTS (
        SELECT 1
        FROM dbo.ExamPapers p
        WHERE p.ExamId = @ExamId
          AND p.PaperId <> ISNULL(@PaperId, 0)
          AND p.IsActive = 1
          AND p.ExamDate = @ExamDate
          AND NOT (p.EndTime <= @StartTime OR p.StartTime >= @EndTime)
    )
    BEGIN
        RETURN -1;
    END

    --invigilator conflict (optional)
    IF (@InvigilatorUserId IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.ExamPapers p
        INNER JOIN dbo.Exams ex ON ex.ExamId = p.ExamId
        WHERE p.PaperId <> ISNULL(@PaperId, 0)
          AND p.IsActive = 1
          AND p.ExamDate = @ExamDate
          AND p.InvigilatorUserId = @InvigilatorUserId
          AND NOT (p.EndTime <= @StartTime OR p.StartTime >= @EndTime)
    ))
    BEGIN
        RETURN -2;
    END

    --room conflict (optional)
    IF (@RoomId IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.ExamPapers p
        WHERE p.PaperId <> ISNULL(@PaperId, 0)
          AND p.IsActive = 1
          AND p.ExamDate = @ExamDate
          AND p.RoomId = @RoomId
          AND NOT (p.EndTime <= @StartTime OR p.StartTime >= @EndTime)
    ))
    BEGIN
        RETURN -3;
    END

    IF @PaperId IS NULL OR @PaperId = 0
    BEGIN
        INSERT INTO dbo.ExamPapers
        (ExamId, SubjectId, ExamDate, StartTime, EndTime, DurationMinutes, RoomId, InvigilatorUserId, MaxMarks, PassingMarks, Notes, IsActive)
        VALUES
        (@ExamId, @SubjectId, @ExamDate, @StartTime, @EndTime, @DurationMinutes, @RoomId, @InvigilatorUserId, @MaxMarks, @PassingMarks, @Notes, @IsActive);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.ExamPapers
        SET SubjectId = @SubjectId,
            ExamDate = @ExamDate,
            StartTime = @StartTime,
            EndTime = @EndTime,
            DurationMinutes = @DurationMinutes,
            RoomId = @RoomId,
            InvigilatorUserId = @InvigilatorUserId,
            MaxMarks = @MaxMarks,
            PassingMarks = @PassingMarks,
            Notes = @Notes,
            IsActive = @IsActive,
            UpdatedAtUtc = SYSUTCDATETIME()
        WHERE PaperId = @PaperId;

        RETURN @PaperId;
    END
END
GO

CREATE OR ALTER PROCEDURE DeleteExamPaper
    @PaperId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ExamPapers WHERE PaperId = @PaperId;
    RETURN @PaperId;
END
GO

CREATE OR ALTER PROCEDURE GetExamTimetableByExam
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.ExamPapers
    WHERE ExamId = @ExamId AND IsActive = 1
    ORDER BY ExamDate, StartTime, PaperId;
END
GO

CREATE OR ALTER PROCEDURE GetExamTimetableByClass
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.*
    FROM dbo.ExamPapers p
    INNER JOIN dbo.Exams e ON e.ExamId = p.ExamId
    WHERE e.AcademicYear = @AcademicYear
      AND e.ClassName = @ClassName
      AND ISNULL(e.Section,'') = ISNULL(@Section,'')
      AND p.IsActive = 1
    ORDER BY p.ExamDate, p.StartTime, p.PaperId;
END
GO

CREATE OR ALTER PROCEDURE ValidateExamPaperConflict
    @PaperId INT,
    @ExamId INT,
    @ExamDate DATE,
    @StartTime TIME,
    @EndTime TIME,
    @RoomId INT = NULL,
    @InvigilatorUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    --class conflict within same exam
    IF EXISTS (
        SELECT 1 FROM dbo.ExamPapers p
        WHERE p.ExamId = @ExamId
          AND p.PaperId <> ISNULL(@PaperId,0)
          AND p.IsActive = 1
          AND p.ExamDate = @ExamDate
          AND NOT (p.EndTime <= @StartTime OR p.StartTime >= @EndTime)
    ) RETURN -1;

    --invigilator conflict
    IF (@InvigilatorUserId IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.ExamPapers p
        WHERE p.PaperId <> ISNULL(@PaperId,0)
          AND p.IsActive = 1
          AND p.ExamDate = @ExamDate
          AND p.InvigilatorUserId = @InvigilatorUserId
          AND NOT (p.EndTime <= @StartTime OR p.StartTime >= @EndTime)
    )) RETURN -2;

    --room conflict
    IF (@RoomId IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.ExamPapers p
        WHERE p.PaperId <> ISNULL(@PaperId,0)
          AND p.IsActive = 1
          AND p.ExamDate = @ExamDate
          AND p.RoomId = @RoomId
          AND NOT (p.EndTime <= @StartTime OR p.StartTime >= @EndTime)
    )) RETURN -3;

    RETURN 0;
END
GO