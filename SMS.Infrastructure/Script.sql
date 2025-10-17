CREATE TABLE Students
(
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    AdmissionNo NVARCHAR(50) NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NULL,
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,
    Gender NVARCHAR(20) NULL,
    Email NVARCHAR(200) NULL,
    Phone NVARCHAR(50) NULL,
    Address NVARCHAR(500) NULL,
    GuardianName NVARCHAR(200) NULL,
    HealthInfo NVARCHAR(1000) NULL,
    PhotoUrl NVARCHAR(500) NULL
);


CREATE OR ALTER PROCEDURE AddStudent
    @AdmissionNo NVARCHAR(50) = NULL,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10),
    @Gender NVARCHAR(20),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(50),
    @Address NVARCHAR(500),
    @GuardianName NVARCHAR(200),
    @HealthInfo NVARCHAR(1000),
    @PhotoUrl NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Students
    (AdmissionNo, FirstName, LastName, ClassName, Section, Gender, Email, Phone, Address, GuardianName, HealthInfo, PhotoUrl)
    VALUES
    (@AdmissionNo, @FirstName, @LastName, @ClassName, @Section, @Gender, @Email, @Phone, @Address, @GuardianName, @HealthInfo, @PhotoUrl);

    DECLARE @NewId INT = CONVERT(INT, SCOPE_IDENTITY()); --COPE_IDENTITY() returns the last identity value inserted into an IDENTITY, so StudentId inserted is returned

    IF (@AdmissionNo IS NULL)
    BEGIN
        DECLARE @Gen NVARCHAR(50) = 'AS-' + FORMAT(GETDATE(), 'yyyy') + '-' + RIGHT('000000' + CAST(@NewId AS VARCHAR(6)), 6);--right 6 takes rightmost 6 digits from the concatenated string
        UPDATE dbo.Students SET AdmissionNo = @Gen WHERE StudentId = @NewId;
    END

    RETURN @NewId;
END
GO

CREATE OR ALTER PROCEDURE UpdateStudent
    @StudentId INT,
    @AdmissionNo NVARCHAR(50),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10),
    @Gender NVARCHAR(20),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(50),
    @Address NVARCHAR(500),
    @GuardianName NVARCHAR(200),
    @HealthInfo NVARCHAR(1000),
    @PhotoUrl NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Students
    SET AdmissionNo = @AdmissionNo,
        FirstName = @FirstName,
        LastName = @LastName,
        ClassName = @ClassName,
        Section = @Section,
        Gender = @Gender,
        Email = @Email,
        Phone = @Phone,
        Address = @Address,
        GuardianName = @GuardianName,
        HealthInfo = @HealthInfo,
        PhotoUrl = @PhotoUrl
    WHERE StudentId = @StudentId;

    RETURN @StudentId;
END
GO

CREATE OR ALTER PROCEDURE DeleteStudentById
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Students WHERE StudentId = @StudentId;
    RETURN @StudentId;
END
GO

CREATE OR ALTER PROCEDURE GetStudentById
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
    FROM dbo.Students
    WHERE StudentId = @StudentId;
    RETURN @StudentId;
END
GO

CREATE OR ALTER PROCEDURE GetStudentList
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Students ORDER BY StudentId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetStudentById
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *
    FROM dbo.Students
    WHERE StudentId = @StudentId;
END
GO

CREATE TABLE dbo.StudentDocuments
(
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    FileName NVARCHAR(260) NOT NULL,
    FilePath NVARCHAR(1000) NOT NULL,
    ContentType NVARCHAR(150) NULL,
    Description NVARCHAR(500) NULL,
    UploadedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_StudentDocuments_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId) ON DELETE CASCADE
);

CREATE TABLE dbo.StudentEnrollments
(
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,
    EnrollmentDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_StudentEnrollments_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId) ON DELETE CASCADE
);
CREATE UNIQUE INDEX UX_Enrollments_Student_Year ON dbo.StudentEnrollments(StudentId, AcademicYear);


CREATE OR ALTER PROCEDURE SetStudentPhoto
    @StudentId INT,
    @PhotoUrl NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Students SET PhotoUrl = @PhotoUrl WHERE StudentId = @StudentId;
    RETURN @StudentId;
END
GO


CREATE OR ALTER PROCEDURE AddStudentDocument
    @StudentId INT,
    @FileName NVARCHAR(260),
    @FilePath NVARCHAR(1000),
    @ContentType NVARCHAR(150) = NULL,
    @Description NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.StudentDocuments (StudentId, FileName, FilePath, ContentType, Description)
    VALUES (@StudentId, @FileName, @FilePath, @ContentType, @Description);
    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE GetStudentDocuments
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.StudentDocuments WHERE StudentId = @StudentId ORDER BY UploadedOn DESC, DocumentId DESC;
END
GO

CREATE OR ALTER PROCEDURE DeleteStudentDocumentById
    @DocumentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.StudentDocuments WHERE DocumentId = @DocumentId;
    RETURN @DocumentId;
END
GO


CREATE OR ALTER PROCEDURE AddOrUpdateEnrollment
    @EnrollmentId INT,
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @EnrollmentDate DATETIME2,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@EnrollmentId IS NOT NULL AND @EnrollmentId > 0)
    BEGIN
        UPDATE dbo.StudentEnrollments
        SET StudentId = @StudentId,
            AcademicYear = @AcademicYear,
            ClassName = @ClassName,
            Section = @Section,
            EnrollmentDate = @EnrollmentDate,
            IsActive = @IsActive
        WHERE EnrollmentId = @EnrollmentId;

        RETURN @EnrollmentId;
    END
    ELSE
    BEGIN
        -- Deactivate other active enrollments for the same year
        UPDATE dbo.StudentEnrollments SET IsActive = 0 WHERE StudentId = @StudentId AND AcademicYear = @AcademicYear;

        INSERT INTO dbo.StudentEnrollments (StudentId, AcademicYear, ClassName, Section, EnrollmentDate, IsActive)
        VALUES (@StudentId, @AcademicYear, @ClassName, @Section, @EnrollmentDate, @IsActive);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
END
GO

CREATE OR ALTER PROCEDURE GetEnrollmentByStudent
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * 
    FROM dbo.StudentEnrollments
    WHERE StudentId = @StudentId AND IsActive = 1
    ORDER BY EnrollmentDate DESC, EnrollmentId DESC;
END
GO