CREATE TABLE dbo.AdmissionInquiries
(
    InquiryId INT IDENTITY(1,1) PRIMARY KEY,
    Source NVARCHAR(50) NOT NULL, -- Online, Walk-in, Phone, Referral
    LeadStatus NVARCHAR(50) NOT NULL DEFAULT 'New', -- New, Contacted, Applied, Dropped
    ApplicantName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NULL,
    Phone NVARCHAR(50) NULL,
    InterestedClass NVARCHAR(50) NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    Notes NVARCHAR(1000) NULL,
    FollowUpDate DATETIME2 NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL
);
CREATE INDEX IX_Inquiries_Year_Class_Status ON dbo.AdmissionInquiries(AcademicYear, InterestedClass, LeadStatus);

GO

CREATE TABLE dbo.AdmissionApplications
(
    ApplicationId INT IDENTITY(1,1) PRIMARY KEY,
    InquiryId INT NULL,
    ApplicationNo NVARCHAR(50) NULL, -- generated if null/empty
    ApplicantName NVARCHAR(200) NOT NULL,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(20) NULL,
    Email NVARCHAR(256) NULL,
    Phone NVARCHAR(50) NULL,
    Address NVARCHAR(500) NULL,
    ParentName NVARCHAR(200) NULL,
    ParentPhone NVARCHAR(50) NULL,
    ParentEmail NVARCHAR(256) NULL,
    PreviousSchool NVARCHAR(200) NULL,
    ClassAppliedFor NVARCHAR(50) NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Submitted',
    TotalMarks DECIMAL(18,2) NULL,
    EntranceScore DECIMAL(18,2) NULL,
    Category NVARCHAR(50) NULL,
    DocumentsVerified BIT NOT NULL DEFAULT 0,
    ApplicationDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    StudentId INT NULL,
    CONSTRAINT FK_AdmissionApplications_Inquiries FOREIGN KEY (InquiryId) REFERENCES dbo.AdmissionInquiries(InquiryId),
    CONSTRAINT FK_AdmissionApplications_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId)
);
CREATE INDEX IX_Applications_Year_Class_Status ON dbo.AdmissionApplications(AcademicYear, ClassAppliedFor, Status);

GO




CREATE OR ALTER PROCEDURE CreateAdmissionInquiry
    @Source NVARCHAR(50),
    @LeadStatus NVARCHAR(50),
    @ApplicantName NVARCHAR(200),
    @Email NVARCHAR(256) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @InterestedClass NVARCHAR(50),
    @AcademicYear NVARCHAR(15),
    @Notes NVARCHAR(1000) = NULL,
    @FollowUpDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.AdmissionInquiries
    (Source, LeadStatus, ApplicantName, Email, Phone, InterestedClass, AcademicYear, Notes, FollowUpDate)
    VALUES (@Source, @LeadStatus, @ApplicantName, @Email, @Phone, @InterestedClass, @AcademicYear, @Notes, @FollowUpDate);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateAdmissionInquiry
    @InquiryId INT,
    @Source NVARCHAR(50),
    @LeadStatus NVARCHAR(50),
    @ApplicantName NVARCHAR(200),
    @Email NVARCHAR(256) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @InterestedClass NVARCHAR(50),
    @AcademicYear NVARCHAR(15),
    @Notes NVARCHAR(1000) = NULL,
    @FollowUpDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.AdmissionInquiries
    SET Source = @Source,
        LeadStatus = @LeadStatus,
        ApplicantName = @ApplicantName,
        Email = @Email,
        Phone = @Phone,
        InterestedClass = @InterestedClass,
        AcademicYear = @AcademicYear,
        Notes = @Notes,
        FollowUpDate = @FollowUpDate,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE InquiryId = @InquiryId;

    RETURN @InquiryId;
END
GO



CREATE OR ALTER PROCEDURE UpdateAdmissionInquiryStatus
    @InquiryId INT,
    @LeadStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.AdmissionInquiries
    SET LeadStatus = @LeadStatus,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE InquiryId = @InquiryId;

    RETURN @InquiryId;
END
GO

CREATE OR ALTER PROCEDURE GetAdmissionInquiryById
    @InquiryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.AdmissionInquiries WHERE InquiryId = @InquiryId;
END
GO

CREATE OR ALTER PROCEDURE GetAdmissionInquiryList
    @AcademicYear NVARCHAR(15) = NULL,
    @InterestedClass NVARCHAR(50) = NULL,
    @LeadStatus NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.AdmissionInquiries
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@InterestedClass IS NULL OR InterestedClass = @InterestedClass)
      AND (@LeadStatus IS NULL OR LeadStatus = @LeadStatus)
    ORDER BY CreatedAtUtc DESC, InquiryId DESC;
END
GO

--application sp
CREATE OR ALTER PROCEDURE CreateAdmissionApplication
    @InquiryId INT = NULL,
    @ApplicationNo NVARCHAR(50) = NULL,
    @ApplicantName NVARCHAR(200),
    @DateOfBirth DATE = NULL,
    @Gender NVARCHAR(20) = NULL,
    @Email NVARCHAR(256) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Address NVARCHAR(500) = NULL,
    @ParentName NVARCHAR(200) = NULL,
    @ParentPhone NVARCHAR(50) = NULL,
    @ParentEmail NVARCHAR(256) = NULL,
    @PreviousSchool NVARCHAR(200) = NULL,
    @ClassAppliedFor NVARCHAR(50),
    @AcademicYear NVARCHAR(15),
    @Status NVARCHAR(50),
    @TotalMarks DECIMAL(18,2) = NULL,
    @EntranceScore DECIMAL(18,2) = NULL,
    @Category NVARCHAR(50) = NULL,
    @DocumentsVerified BIT = 0,
    @ApplicationDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.AdmissionApplications
    (InquiryId, ApplicationNo, ApplicantName, DateOfBirth, Gender, Email, Phone, Address, ParentName, ParentPhone, ParentEmail, PreviousSchool,
     ClassAppliedFor, AcademicYear, Status, TotalMarks, EntranceScore, Category, DocumentsVerified, ApplicationDate)
    VALUES
    (@InquiryId, @ApplicationNo, @ApplicantName, @DateOfBirth, @Gender, @Email, @Phone, @Address, @ParentName, @ParentPhone, @ParentEmail, @PreviousSchool,
     @ClassAppliedFor, @AcademicYear, @Status, @TotalMarks, @EntranceScore, @Category, @DocumentsVerified, @ApplicationDate);

    DECLARE @NewId INT = CONVERT(INT, SCOPE_IDENTITY());

    -- Auto-generate ApplicationNo if empty
    IF (@ApplicationNo IS NULL OR LTRIM(RTRIM(ISNULL(@ApplicationNo, ''))) = '')
    BEGIN
        DECLARE @Gen NVARCHAR(50) = 'APP-' + @AcademicYear + '-' + RIGHT('000000' + CAST(@NewId AS VARCHAR(6)), 6);
        UPDATE dbo.AdmissionApplications SET ApplicationNo = @Gen WHERE ApplicationId = @NewId;
    END

    -- If linked inquiry, mark as Applied
    IF @InquiryId IS NOT NULL
        UPDATE dbo.AdmissionInquiries SET LeadStatus = 'Applied', UpdatedAtUtc = SYSUTCDATETIME() WHERE InquiryId = @InquiryId;

    RETURN @NewId;
END
GO

CREATE OR ALTER PROCEDURE UpdateAdmissionApplication
    @ApplicationId INT,
    @InquiryId INT = NULL,
    @ApplicationNo NVARCHAR(50),
    @ApplicantName NVARCHAR(200),
    @DateOfBirth DATE = NULL,
    @Gender NVARCHAR(20) = NULL,
    @Email NVARCHAR(256) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Address NVARCHAR(500) = NULL,
    @ParentName NVARCHAR(200) = NULL,
    @ParentPhone NVARCHAR(50) = NULL,
    @ParentEmail NVARCHAR(256) = NULL,
    @PreviousSchool NVARCHAR(200) = NULL,
    @ClassAppliedFor NVARCHAR(50),
    @AcademicYear NVARCHAR(15),
    @Status NVARCHAR(50),
    @TotalMarks DECIMAL(18,2) = NULL,
    @EntranceScore DECIMAL(18,2) = NULL,
    @Category NVARCHAR(50) = NULL,
    @DocumentsVerified BIT = 0,
    @ApplicationDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.AdmissionApplications
    SET InquiryId = @InquiryId,
        ApplicationNo = @ApplicationNo,
        ApplicantName = @ApplicantName,
        DateOfBirth = @DateOfBirth,
        Gender = @Gender,
        Email = @Email,
        Phone = @Phone,
        Address = @Address,
        ParentName = @ParentName,
        ParentPhone = @ParentPhone,
        ParentEmail = @ParentEmail,
        PreviousSchool = @PreviousSchool,
        ClassAppliedFor = @ClassAppliedFor,
        AcademicYear = @AcademicYear,
        Status = @Status,
        TotalMarks = @TotalMarks,
        EntranceScore = @EntranceScore,
        Category = @Category,
        DocumentsVerified = @DocumentsVerified,
        ApplicationDate = @ApplicationDate,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE ApplicationId = @ApplicationId;

    RETURN @ApplicationId;
END
GO

CREATE OR ALTER PROCEDURE UpdateAdmissionApplicationStatus
    @ApplicationId INT,
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.AdmissionApplications
    SET Status = @Status,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE ApplicationId = @ApplicationId;

    RETURN @ApplicationId;
END
GO

CREATE OR ALTER PROCEDURE GetAdmissionApplicationById
    @ApplicationId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.AdmissionApplications WHERE ApplicationId = @ApplicationId;
END
GO

CREATE OR ALTER PROCEDURE GetAdmissionApplicationList
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassAppliedFor NVARCHAR(50) = NULL,
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.AdmissionApplications
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@ClassAppliedFor IS NULL OR ClassAppliedFor = @ClassAppliedFor)
      AND (@Status IS NULL OR Status = @Status)
    ORDER BY CreatedAtUtc DESC, ApplicationId DESC;
END
GO

--Application gee payments
CREATE TABLE dbo.AdmissionFeePayments
(
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationId INT NOT NULL,
    ReceiptNo NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL DEFAULT 'INR',
    PaymentMode NVARCHAR(30) NOT NULL,
    ReferenceNo NVARCHAR(100) NULL,
    Remarks NVARCHAR(500) NULL,
    CollectedByUserId INT NULL,
    PaymentDate DATETIME2 NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_AdmissionFeePayments_AdmissionApplications FOREIGN KEY (ApplicationId) REFERENCES dbo.AdmissionApplications(ApplicationId) ON DELETE CASCADE
);
CREATE INDEX IX_AdmissionFeePayments_App ON dbo.AdmissionFeePayments(ApplicationId);

GO

CREATE TABLE dbo.AdmissionShortlist
(
    ShortlistId INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    ClassAppliedFor NVARCHAR(50) NOT NULL,
    Score DECIMAL(18,4) NOT NULL,
    [Rank] INT NOT NULL,
    GeneratedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Notes NVARCHAR(500) NULL,
    CONSTRAINT FK_AdmissionShortlist_Applications FOREIGN KEY (ApplicationId) REFERENCES dbo.AdmissionApplications(ApplicationId) ON DELETE CASCADE
);
CREATE INDEX IX_Shortlist_YearClass ON dbo.AdmissionShortlist(AcademicYear, ClassAppliedFor);

GO

CREATE TABLE dbo.AdmissionMeritList
(
    MeritId INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    ClassAppliedFor NVARCHAR(50) NOT NULL,
    Score DECIMAL(18,4) NOT NULL,
    [Rank] INT NOT NULL,
    GeneratedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Notes NVARCHAR(500) NULL,
    CONSTRAINT FK_AdmissionMeritList_Applications FOREIGN KEY (ApplicationId) REFERENCES dbo.AdmissionApplications(ApplicationId) ON DELETE CASCADE
);
CREATE INDEX IX_Merit_YearClass ON dbo.AdmissionMeritList(AcademicYear, ClassAppliedFor);

GO

-- Collect Application Fee (auto ReceiptNo)
CREATE OR ALTER PROCEDURE CollectAdmissionApplicationFee
    @ApplicationId INT,
    @Amount DECIMAL(18,2),
    @Currency NVARCHAR(3),
    @PaymentMode NVARCHAR(30),
    @ReferenceNo NVARCHAR(100) = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @CollectedByUserId INT = NULL,
    @PaymentDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.AdmissionApplications WHERE ApplicationId = @ApplicationId)
        THROW 51001, 'Application not found', 1;

    DECLARE @Year NVARCHAR(15) = (SELECT TOP 1 AcademicYear FROM dbo.AdmissionApplications WHERE ApplicationId = @ApplicationId);

    INSERT INTO dbo.AdmissionFeePayments
    (ApplicationId, ReceiptNo, Amount, Currency, PaymentMode, ReferenceNo, Remarks, CollectedByUserId, PaymentDate)
    VALUES
    (@ApplicationId, '', @Amount, @Currency, @PaymentMode, @ReferenceNo, @Remarks, @CollectedByUserId, @PaymentDate);

    DECLARE @PaymentId INT = CONVERT(INT, SCOPE_IDENTITY());
    DECLARE @Receipt NVARCHAR(50) = 'AF-' + @Year + '-' + RIGHT('000000' + CAST(@PaymentId AS VARCHAR(6)), 6);

    UPDATE dbo.AdmissionFeePayments SET ReceiptNo = @Receipt WHERE PaymentId = @PaymentId;

    -- Update application status to UnderReview if still Submitted
    UPDATE dbo.AdmissionApplications
    SET Status = CASE WHEN Status = 'Submitted' THEN 'UnderReview' ELSE Status END,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE ApplicationId = @ApplicationId;

    RETURN @PaymentId;
END
GO

CREATE OR ALTER PROCEDURE GetApplicationFees
    @ApplicationId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PaymentId, ApplicationId, ReceiptNo, Amount, Currency, PaymentMode, ReferenceNo, Remarks, CollectedByUserId, PaymentDate, CreatedAtUtc
    FROM dbo.AdmissionFeePayments
    WHERE ApplicationId = @ApplicationId
    ORDER BY PaymentDate DESC, PaymentId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetApplicationFeeSummary
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassAppliedFor NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COALESCE(@AcademicYear, '') AS AcademicYear,
        COALESCE(@ClassAppliedFor, '') AS ClassAppliedFor,
        COUNT(DISTINCT a.ApplicationId) AS ApplicationsCount,
        COUNT(p.PaymentId) AS PaymentsCount,
        COALESCE(SUM(p.Amount), 0) AS TotalAmount
    FROM dbo.AdmissionApplications a
    LEFT JOIN dbo.AdmissionFeePayments p ON p.ApplicationId = a.ApplicationId
    WHERE (@AcademicYear IS NULL OR a.AcademicYear = @AcademicYear)
      AND (@ClassAppliedFor IS NULL OR a.ClassAppliedFor = @ClassAppliedFor);
END
GO

-- Generate shortlist (clears previous shortlist for same year/class)
CREATE OR ALTER PROCEDURE GenerateAdmissionShortlist
    @AcademicYear NVARCHAR(15),
    @ClassAppliedFor NVARCHAR(50),
    @MinEntranceScore DECIMAL(18,2) = NULL,
    @TopN INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.AdmissionShortlist WHERE AcademicYear = @AcademicYear AND ClassAppliedFor = @ClassAppliedFor;

    ;WITH C AS
    (
        SELECT
            a.ApplicationId,
            a.AcademicYear,
            a.ClassAppliedFor,
            CAST(COALESCE(a.EntranceScore, 0) * 1000 + COALESCE(a.TotalMarks, 0) AS DECIMAL(18,4)) AS Score
        FROM dbo.AdmissionApplications a
        WHERE a.AcademicYear = @AcademicYear
          AND a.ClassAppliedFor = @ClassAppliedFor
          AND (@MinEntranceScore IS NULL OR COALESCE(a.EntranceScore, 0) >= @MinEntranceScore)
    ),
    R AS
    (
        SELECT ApplicationId, AcademicYear, ClassAppliedFor, Score,
               ROW_NUMBER() OVER (ORDER BY Score DESC, ApplicationId ASC) AS RN
        FROM C
    )
    INSERT INTO dbo.AdmissionShortlist (ApplicationId, AcademicYear, ClassAppliedFor, Score, [Rank], GeneratedOn)
    SELECT ApplicationId, AcademicYear, ClassAppliedFor, Score, RN, SYSUTCDATETIME()
    FROM R
    WHERE (@TopN IS NULL OR RN <= @TopN)
    ORDER BY RN;

    -- Update application status to Shortlisted for those in shortlist
    UPDATE a
    SET a.Status = 'Shortlisted',
        a.UpdatedAtUtc = SYSUTCDATETIME()
    FROM dbo.AdmissionApplications a
    INNER JOIN dbo.AdmissionShortlist s ON s.ApplicationId = a.ApplicationId
    WHERE s.AcademicYear = @AcademicYear AND s.ClassAppliedFor = @ClassAppliedFor;

    -- Return affected count
    DECLARE @Count INT = (SELECT COUNT(*) FROM dbo.AdmissionShortlist WHERE AcademicYear = @AcademicYear AND ClassAppliedFor = @ClassAppliedFor);
    RETURN @Count;
END
GO

CREATE OR ALTER PROCEDURE GetAdmissionShortlist
    @AcademicYear NVARCHAR(15),
    @ClassAppliedFor NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.ShortlistId, s.ApplicationId, a.ApplicationNo, a.ApplicantName, s.AcademicYear, s.ClassAppliedFor, s.Score, s.[Rank], s.GeneratedOn, s.Notes
    FROM dbo.AdmissionShortlist s
    INNER JOIN dbo.AdmissionApplications a ON a.ApplicationId = s.ApplicationId
    WHERE s.AcademicYear = @AcademicYear AND s.ClassAppliedFor = @ClassAppliedFor
    ORDER BY s.[Rank] ASC;
END
GO

-- Generate merit list (clears previous merit for same year/class)
CREATE OR ALTER PROCEDURE GenerateAdmissionMeritList
    @AcademicYear NVARCHAR(15),
    @ClassAppliedFor NVARCHAR(50),
    @TopN INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.AdmissionMeritList WHERE AcademicYear = @AcademicYear AND ClassAppliedFor = @ClassAppliedFor;

    ;WITH C AS
    (
        SELECT
            a.ApplicationId,
            a.AcademicYear,
            a.ClassAppliedFor,
            CAST(COALESCE(a.EntranceScore, 0) * 1000 + COALESCE(a.TotalMarks, 0) AS DECIMAL(18,4)) AS Score
        FROM dbo.AdmissionApplications a
        WHERE a.AcademicYear = @AcademicYear
          AND a.ClassAppliedFor = @ClassAppliedFor
          AND a.Status IN ('UnderReview','Shortlisted','Submitted') -- be flexible
    ),
    R AS
    (
        SELECT ApplicationId, AcademicYear, ClassAppliedFor, Score,
               ROW_NUMBER() OVER (ORDER BY Score DESC, ApplicationId ASC) AS RN
        FROM C
    )
    INSERT INTO dbo.AdmissionMeritList (ApplicationId, AcademicYear, ClassAppliedFor, Score, [Rank], GeneratedOn)
    SELECT ApplicationId, AcademicYear, ClassAppliedFor, Score, RN, SYSUTCDATETIME()
    FROM R
    WHERE (@TopN IS NULL OR RN <= @TopN)
    ORDER BY RN;

    -- Do not change status here beyond Shortlisted — confirmation happens in Phase 2 Part 3
    UPDATE a
    SET a.Status = 'Shortlisted',
        a.UpdatedAtUtc = SYSUTCDATETIME()
    FROM dbo.AdmissionApplications a
    INNER JOIN dbo.AdmissionMeritList m ON m.ApplicationId = a.ApplicationId
    WHERE m.AcademicYear = @AcademicYear AND m.ClassAppliedFor = @ClassAppliedFor;

    DECLARE @Count INT = (SELECT COUNT(*) FROM dbo.AdmissionMeritList WHERE AcademicYear = @AcademicYear AND ClassAppliedFor = @ClassAppliedFor);
    RETURN @Count;
END
GO

CREATE OR ALTER PROCEDURE GetAdmissionMeritList
    @AcademicYear NVARCHAR(15),
    @ClassAppliedFor NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT m.MeritId, m.ApplicationId, a.ApplicationNo, a.ApplicantName, m.AcademicYear, m.ClassAppliedFor, m.Score, m.[Rank], m.GeneratedOn, m.Notes
    FROM dbo.AdmissionMeritList m
    INNER JOIN dbo.AdmissionApplications a ON a.ApplicationId = m.ApplicationId
    WHERE m.AcademicYear = @AcademicYear AND m.ClassAppliedFor = @ClassAppliedFor
    ORDER BY m.[Rank] ASC;
END
GO

CREATE TABLE dbo.AdmissionApplicationDocuments
(
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationId INT NOT NULL,
    FileName NVARCHAR(260) NOT NULL,
    FilePath NVARCHAR(1000) NOT NULL,
    ContentType NVARCHAR(150) NULL,
    Description NVARCHAR(500) NULL,
    UploadedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Verified BIT NOT NULL DEFAULT 0,
    VerifiedByUserId INT NULL,
    VerifiedAtUtc DATETIME2 NULL,
    CONSTRAINT FK_AdmissionApplicationDocuments_AdmissionApplications FOREIGN KEY (ApplicationId) REFERENCES dbo.AdmissionApplications(ApplicationId) ON DELETE CASCADE
);
CREATE INDEX IX_AppDocs_Application ON dbo.AdmissionApplicationDocuments(ApplicationId);

GO

-- Add application document
CREATE OR ALTER PROCEDURE AddAdmissionApplicationDocument
    @ApplicationId INT,
    @FileName NVARCHAR(260),
    @FilePath NVARCHAR(1000),
    @ContentType NVARCHAR(150) = NULL,
    @Description NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.AdmissionApplications WHERE ApplicationId = @ApplicationId)
        THROW 52001, 'Application not found', 1;

    INSERT INTO dbo.AdmissionApplicationDocuments (ApplicationId, FileName, FilePath, ContentType, Description)
    VALUES (@ApplicationId, @FileName, @FilePath, @ContentType, @Description);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE GetAdmissionApplicationDocuments
    @ApplicationId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DocumentId, ApplicationId, FileName, FilePath, ContentType, Description, UploadedOn, Verified, VerifiedByUserId, VerifiedAtUtc
    FROM dbo.AdmissionApplicationDocuments
    WHERE ApplicationId = @ApplicationId
    ORDER BY UploadedOn DESC, DocumentId DESC;
END
GO

CREATE OR ALTER PROCEDURE DeleteAdmissionApplicationDocumentById
    @DocumentId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.AdmissionApplicationDocuments WHERE DocumentId = @DocumentId;
    RETURN @DocumentId;
END
GO

CREATE OR ALTER PROCEDURE VerifyAdmissionApplicationDocument
    @DocumentId INT,
    @VerifiedByUserId INT,
    @Verified BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.AdmissionApplicationDocuments
    SET Verified = @Verified,
        VerifiedByUserId = @VerifiedByUserId,
        VerifiedAtUtc = CASE WHEN @Verified = 1 THEN SYSUTCDATETIME() ELSE NULL END
    WHERE DocumentId = @DocumentId;

    RETURN @DocumentId;
END
GO

CREATE OR ALTER PROCEDURE SetApplicationDocumentsVerified
    @ApplicationId INT,
    @DocumentsVerified BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.AdmissionApplications
    SET DocumentsVerified = @DocumentsVerified,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE ApplicationId = @ApplicationId;

    RETURN @ApplicationId;
END
GO

-- Confirm Admission: creates Student and Enrollment, links back to application
CREATE OR ALTER PROCEDURE ConfirmAdmission
    @ApplicationId INT,
    @Section NVARCHAR(10) = NULL,
    @EnrollmentDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.AdmissionApplications WHERE ApplicationId = @ApplicationId)
        THROW 52011, 'Application not found', 1;

    DECLARE @ApplicantName NVARCHAR(200),
            @Email NVARCHAR(256),
            @Phone NVARCHAR(50),
            @Address NVARCHAR(500),
            @ParentName NVARCHAR(200),
            @ClassAppliedFor NVARCHAR(50),
            @AcademicYear NVARCHAR(15),
            @Gender NVARCHAR(20),
            @DocumentsVerified BIT,
            @Status NVARCHAR(50);

    SELECT
        @ApplicantName = ApplicantName,
        @Email = Email,
        @Phone = Phone,
        @Address = Address,
        @ParentName = ParentName,
        @ClassAppliedFor = ClassAppliedFor,
        @AcademicYear = AcademicYear,
        @Gender = Gender,
        @DocumentsVerified = DocumentsVerified,
        @Status = Status
    FROM dbo.AdmissionApplications
    WHERE ApplicationId = @ApplicationId;

    -- Optional safety checks
    IF (@DocumentsVerified = 0)
        THROW 52012, 'Documents not verified', 1;

    IF (@Status IS NULL)
        SET @Status = 'UnderReview';

    BEGIN TRAN;

    -- Create student
    INSERT INTO dbo.Students
    (AdmissionNo, FirstName, LastName, ClassName, Section, Gender, Email, Phone, Address, GuardianName, HealthInfo, PhotoUrl)
    VALUES
    (NULL, @ApplicantName, NULL, @ClassAppliedFor, @Section, @Gender, @Email, @Phone, @Address, @ParentName, NULL, NULL);

    DECLARE @NewStudentId INT = CONVERT(INT, SCOPE_IDENTITY());

    -- Generate AdmissionNo if null
    DECLARE @GenAdm NVARCHAR(50) = 'STD-' + FORMAT(GETDATE(), 'yyyy') + '-' + RIGHT('000000' + CAST(@NewStudentId AS VARCHAR(6)), 6);
    UPDATE dbo.Students SET AdmissionNo = @GenAdm WHERE StudentId = @NewStudentId;

    -- Enrollment
    -- Deactivate existing enrollments for same year (safety)
    UPDATE dbo.StudentEnrollments SET IsActive = 0 WHERE StudentId = @NewStudentId AND AcademicYear = @AcademicYear;

    INSERT INTO dbo.StudentEnrollments (StudentId, AcademicYear, ClassName, Section, EnrollmentDate, IsActive)
    VALUES (@NewStudentId, @AcademicYear, @ClassAppliedFor, @Section, @EnrollmentDate, 1);

    -- Update application
    UPDATE dbo.AdmissionApplications
    SET Status = 'Confirmed',
        StudentId = @NewStudentId,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE ApplicationId = @ApplicationId;

    COMMIT;

    RETURN @NewStudentId;
END
GO