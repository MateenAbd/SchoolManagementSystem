CREATE TABLE dbo.FeeHeads
(
    HeadId INT IDENTITY(1,1) PRIMARY KEY,
    HeadCode NVARCHAR(50) NOT NULL,
    HeadName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    SortOrder INT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
CREATE UNIQUE INDEX UX_FeeHeads_Code ON dbo.FeeHeads(HeadCode);
GO

CREATE TABLE dbo.FeeTerms
(
    TermId INT IDENTITY(1,1) PRIMARY KEY,
    AcademicYear NVARCHAR(15) NOT NULL,
    TermCode NVARCHAR(50) NOT NULL,
    TermName NVARCHAR(200) NOT NULL,
    SequenceNo INT NOT NULL,
    DueDate DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
CREATE UNIQUE INDEX UX_FeeTerms_Year_Code ON dbo.FeeTerms(AcademicYear, TermCode);
GO

 CREATE TABLE dbo.FeeStructures
(
    StructureId INT IDENTITY(1,1) PRIMARY KEY,
    AcademicYear NVARCHAR(15) NOT NULL,
    ClassName NVARCHAR(50) NOT NULL,
    Section NVARCHAR(10) NULL,
    TermId INT NOT NULL,
    EffectiveFrom DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    CONSTRAINT FK_FeeStructures_Terms FOREIGN KEY (TermId) REFERENCES dbo.FeeTerms(TermId)
);
CREATE UNIQUE INDEX UX_FeeStructures_AY_Class_Term ON dbo.FeeStructures(AcademicYear, ClassName, Section, TermId);
GO

CREATE TABLE dbo.FeeStructureDetails
(
    DetailId INT IDENTITY(1,1) PRIMARY KEY,
    StructureId INT NOT NULL,
    HeadId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    IsOptional BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_FeeStructureDetails_Structures FOREIGN KEY (StructureId) REFERENCES dbo.FeeStructures(StructureId) ON DELETE CASCADE,
    CONSTRAINT FK_FeeStructureDetails_Heads FOREIGN KEY (HeadId) REFERENCES dbo.FeeHeads(HeadId)
);
CREATE UNIQUE INDEX UX_FeeStructureDetails_U ON dbo.FeeStructureDetails(StructureId, HeadId);
GO

CREATE OR ALTER PROCEDURE CreateFeeHead
    @HeadCode NVARCHAR(50),
    @HeadName NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @SortOrder INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.FeeHeads WHERE HeadCode = @HeadCode)
        THROW 60001, 'Fee Head code already exists', 1;

    INSERT INTO dbo.FeeHeads (HeadCode, HeadName, Description, SortOrder, IsActive)
    VALUES (@HeadCode, @HeadName, @Description, @SortOrder, @IsActive);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateFeeHead
    @HeadId INT,
    @HeadCode NVARCHAR(50),
    @HeadName NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @SortOrder INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.FeeHeads WHERE HeadCode = @HeadCode AND HeadId <> @HeadId)
        THROW 60002, 'Fee Head code used by another head', 1;

    UPDATE dbo.FeeHeads
    SET HeadCode = @HeadCode, HeadName = @HeadName, Description = @Description, SortOrder = @SortOrder, IsActive = @IsActive
    WHERE HeadId = @HeadId;

    RETURN @HeadId;
END
GO

CREATE OR ALTER PROCEDURE DeleteFeeHead
    @HeadId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.FeeHeads WHERE HeadId = @HeadId;
    RETURN @HeadId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeHeadById
    @HeadId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.FeeHeads WHERE HeadId = @HeadId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeHeadList
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.FeeHeads
    WHERE (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY COALESCE(SortOrder, 9999), HeadName;
END
GO

CREATE OR ALTER PROCEDURE CreateFeeTerm
    @AcademicYear NVARCHAR(15),
    @TermCode NVARCHAR(50),
    @TermName NVARCHAR(200),
    @SequenceNo INT,
    @DueDate DATETIME2 = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.FeeTerms WHERE AcademicYear = @AcademicYear AND TermCode = @TermCode)
        THROW 60101, 'Term code already exists for this academic year', 1;

    INSERT INTO dbo.FeeTerms (AcademicYear, TermCode, TermName, SequenceNo, DueDate, IsActive)
    VALUES (@AcademicYear, @TermCode, @TermName, @SequenceNo, @DueDate, @IsActive);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE UpdateFeeTerm
    @TermId INT,
    @AcademicYear NVARCHAR(15),
    @TermCode NVARCHAR(50),
    @TermName NVARCHAR(200),
    @SequenceNo INT,
    @DueDate DATETIME2 = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.FeeTerms WHERE AcademicYear = @AcademicYear AND TermCode = @TermCode AND TermId <> @TermId)
        THROW 60102, 'Term code already used by another term in the same academic year', 1;

    UPDATE dbo.FeeTerms
    SET AcademicYear = @AcademicYear, TermCode = @TermCode, TermName = @TermName, SequenceNo = @SequenceNo, DueDate = @DueDate, IsActive = @IsActive
    WHERE TermId = @TermId;

    RETURN @TermId;
END
GO

CREATE OR ALTER PROCEDURE DeleteFeeTerm
    @TermId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.FeeTerms WHERE TermId = @TermId;
    RETURN @TermId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeTermById
    @TermId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.FeeTerms WHERE TermId = @TermId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeTermList
    @AcademicYear NVARCHAR(15) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.FeeTerms
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY AcademicYear DESC, SequenceNo ASC, TermId DESC;
END
GO


-- Upsert header: returns StructureId (new or existing updated)
CREATE OR ALTER PROCEDURE UpsertFeeStructureHeader
    @StructureId INT,
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @TermId INT,
    @EffectiveFrom DATETIME2 = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF (@StructureId = 0)
    BEGIN
        -- prevent duplicates for same AY/Class/Section/Term
        IF EXISTS (SELECT 1 FROM dbo.FeeStructures WHERE AcademicYear = @AcademicYear AND ClassName = @ClassName AND ISNULL(Section,'') = ISNULL(@Section,'') AND TermId = @TermId)
            THROW 60201, 'Fee structure already exists for the given class/section and term', 1;

        INSERT INTO dbo.FeeStructures (AcademicYear, ClassName, Section, TermId, EffectiveFrom, IsActive)
        VALUES (@AcademicYear, @ClassName, @Section, @TermId, @EffectiveFrom, @IsActive);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        -- prevent duplicates on update
        IF EXISTS (SELECT 1 FROM dbo.FeeStructures WHERE AcademicYear = @AcademicYear AND ClassName = @ClassName AND ISNULL(Section,'') = ISNULL(@Section,'') AND TermId = @TermId AND StructureId <> @StructureId)
            THROW 60202, 'Another fee structure exists for the same class/section and term', 1;

        UPDATE dbo.FeeStructures
        SET AcademicYear = @AcademicYear,
            ClassName = @ClassName,
            Section = @Section,
            TermId = @TermId,
            EffectiveFrom = @EffectiveFrom,
            IsActive = @IsActive,
            UpdatedAtUtc = SYSUTCDATETIME()
        WHERE StructureId = @StructureId;

        RETURN @StructureId;
    END
END
GO

CREATE OR ALTER PROCEDURE DeleteFeeStructureDetails
    @StructureId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.FeeStructureDetails WHERE StructureId = @StructureId;
    RETURN @StructureId;
END
GO

CREATE OR ALTER PROCEDURE AddFeeStructureDetail
    @StructureId INT,
    @HeadId INT,
    @Amount DECIMAL(18,2),
    @IsOptional BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.FeeStructureDetails WHERE StructureId = @StructureId AND HeadId = @HeadId)
    BEGIN
        UPDATE dbo.FeeStructureDetails
        SET Amount = @Amount, IsOptional = @IsOptional
        WHERE StructureId = @StructureId AND HeadId = @HeadId;

        RETURN (SELECT DetailId FROM dbo.FeeStructureDetails WHERE StructureId = @StructureId AND HeadId = @HeadId);
    END
    ELSE
    BEGIN
        INSERT INTO dbo.FeeStructureDetails (StructureId, HeadId, Amount, IsOptional)
        VALUES (@StructureId, @HeadId, @Amount, @IsOptional);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
END
GO

CREATE OR ALTER PROCEDURE DeleteFeeStructure
    @StructureId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.FeeStructures WHERE StructureId = @StructureId;
    RETURN @StructureId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeStructureHeaderById
    @StructureId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.FeeStructures WHERE StructureId = @StructureId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeStructureDetailsByStructureId
    @StructureId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.FeeStructureDetails WHERE StructureId = @StructureId ORDER BY DetailId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeStructureHeaderByClassTerm
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50),
    @Section NVARCHAR(10) = NULL,
    @TermId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.FeeStructures
    WHERE AcademicYear = @AcademicYear
      AND ClassName = @ClassName
      AND ISNULL(Section,'') = ISNULL(@Section,'')
      AND TermId = @TermId
    ORDER BY StructureId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetFeeStructureHeaders
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @TermId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.FeeStructures
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@ClassName IS NULL OR ClassName = @ClassName)
      AND (@Section IS NULL OR Section = @Section)
      AND (@TermId IS NULL OR TermId = @TermId)
      AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY AcademicYear DESC, ClassName, Section, TermId, StructureId DESC;
END
GO




CREATE TABLE dbo.FeeReceipts
(
    ReceiptId INT IDENTITY(1,1) PRIMARY KEY,
    ReceiptNo NVARCHAR(50) NOT NULL,
    StudentId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    TermId INT NOT NULL,
    PaymentMode NVARCHAR(30) NOT NULL,
    ReferenceNo NVARCHAR(100) NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    ReceiptDate DATETIME2 NOT NULL,
    ReceivedByUserId INT NULL,
    IsCancelled BIT NOT NULL DEFAULT 0,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_FeeReceipts_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_FeeReceipts_Terms FOREIGN KEY (TermId) REFERENCES dbo.FeeTerms(TermId)
);
CREATE UNIQUE INDEX UX_FeeReceipts_ReceiptNo ON dbo.FeeReceipts(ReceiptNo);
CREATE INDEX IX_FeeReceipts_Student ON dbo.FeeReceipts(StudentId, AcademicYear, TermId, ReceiptDate);
GO

CREATE TABLE dbo.FeeReceiptItems
(
    ReceiptItemId INT IDENTITY(1,1) PRIMARY KEY,
    ReceiptId INT NOT NULL,
    HeadId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_FeeReceiptItems_Receipts FOREIGN KEY (ReceiptId) REFERENCES dbo.FeeReceipts(ReceiptId) ON DELETE CASCADE,
    CONSTRAINT FK_FeeReceiptItems_Heads FOREIGN KEY (HeadId) REFERENCES dbo.FeeHeads(HeadId)
);
CREATE INDEX IX_FeeReceiptItems_Receipt ON dbo.FeeReceiptItems(ReceiptId);
GO

CREATE TABLE dbo.StudentFeeLedger
(
    LedgerId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    TermId INT NOT NULL,
    HeadId INT NULL,
    EntryType NVARCHAR(10) NOT NULL, -- Debit/Credit
    Amount DECIMAL(18,2) NOT NULL,
    Narration NVARCHAR(500) NULL,
    Balance DECIMAL(18,2) NULL,
    ReceiptId INT NULL,
    EntryDate DATE NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_SFL_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_SFL_Terms FOREIGN KEY (TermId) REFERENCES dbo.FeeTerms(TermId),
    CONSTRAINT FK_SFL_Heads FOREIGN KEY (HeadId) REFERENCES dbo.FeeHeads(HeadId),
    CONSTRAINT FK_SFL_Receipts FOREIGN KEY (ReceiptId) REFERENCES dbo.FeeReceipts(ReceiptId)
);
CREATE INDEX IX_SFL_StudentTerm ON dbo.StudentFeeLedger(StudentId, AcademicYear, TermId, EntryDate);
GO

Create or ALTER PROCEDURE dbo.GenerateStudentTermFee
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @TermId INT
AS
BEGIN
    SET NOCOUNT, XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRAN;

        -- 1. Get student class/section
        DECLARE @ClassName NVARCHAR(50), @Section NVARCHAR(10);
        SELECT @ClassName = ClassName, @Section = Section
        FROM dbo.Students WHERE StudentId = @StudentId;

        IF @ClassName IS NULL
            THROW 61001, 'Student not found', 1;

        -- 2. Find fee structure
        DECLARE @StructureId INT;
        SELECT TOP 1 @StructureId = StructureId
        FROM dbo.FeeStructures
        WHERE AcademicYear = @AcademicYear
          AND ClassName = @ClassName
          AND ISNULL(Section,'') = ISNULL(@Section,'')
          AND TermId = @TermId
          AND IsActive = 1
        ORDER BY StructureId DESC;

        IF @StructureId IS NULL
            THROW 61002, 'Fee structure not found', 1;

        -- 3. Insert missing debits
        INSERT INTO dbo.StudentFeeLedger (
            StudentId, AcademicYear, TermId, HeadId, EntryType,
            Amount, Narration, ReceiptId, EntryDate, CreatedAtUtc, Balance
        )
        SELECT 
            @StudentId, @AcademicYear, @TermId,
            d.HeadId, 'Debit', d.Amount, 'Term Fee',
            NULL, CAST(SYSUTCDATETIME() AS DATE), SYSUTCDATETIME(),
            NULL  -- temp NULL
        FROM dbo.FeeStructureDetails d
        WHERE d.StructureId = @StructureId
          AND NOT EXISTS (
              SELECT 1 FROM dbo.StudentFeeLedger l
              WHERE l.StudentId = @StudentId
                AND l.AcademicYear = @AcademicYear
                AND l.TermId = @TermId
                AND l.HeadId = d.HeadId
                AND l.EntryType = 'Debit'
          );

        DECLARE @Inserted INT = @@ROWCOUNT;
        IF @Inserted = 0
        BEGIN
            COMMIT TRAN;
            RETURN 0;
        END

        -- 4. REBUILD RUNNING BALANCE FOR ALL ENTRIES (in correct order)
        ;WITH LedgerOrdered AS (
            SELECT 
                LedgerId,
                Amount,
                CASE WHEN EntryType = 'Debit' THEN -Amount ELSE Amount END AS SignedAmount
            FROM dbo.StudentFeeLedger
            WHERE StudentId = @StudentId
             -- AND AcademicYear = @AcademicYear
              --AND TermId = @TermId
        ),
        Running AS (
            SELECT 
                LedgerId,
                SUM(SignedAmount) OVER (ORDER BY LedgerId ROWS UNBOUNDED PRECEDING) AS RunningBalance
            FROM LedgerOrdered
        )
        UPDATE l
        SET Balance = r.RunningBalance
        FROM dbo.StudentFeeLedger l
        INNER JOIN Running r ON l.LedgerId = r.LedgerId
        WHERE l.StudentId = @StudentId
          AND l.AcademicYear = @AcademicYear
          AND l.TermId = @TermId;

        COMMIT TRAN;
        RETURN @Inserted;

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE CreateFeeReceipt
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @TermId INT,
    @PaymentMode NVARCHAR(30),
    @ReferenceNo NVARCHAR(100) = NULL,
    @TotalAmount DECIMAL(18,2),
    @ReceiptDate DATETIME2,
    @ReceivedByUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.FeeReceipts (ReceiptNo, StudentId, AcademicYear, TermId, PaymentMode, ReferenceNo, TotalAmount, ReceiptDate, ReceivedByUserId)
    VALUES ('', @StudentId, @AcademicYear, @TermId, @PaymentMode, @ReferenceNo, @TotalAmount, @ReceiptDate, @ReceivedByUserId);

    DECLARE @ReceiptId INT = CONVERT(INT, SCOPE_IDENTITY());
    DECLARE @AYPart NVARCHAR(9) = @AcademicYear; -- e.g., "2024-2025"
    DECLARE @RecNo NVARCHAR(50) = 'FR-' + @AYPart + '-' + RIGHT('000000' + CAST(@ReceiptId AS VARCHAR(6)), 6);

    UPDATE dbo.FeeReceipts SET ReceiptNo = @RecNo WHERE ReceiptId = @ReceiptId;

    RETURN @ReceiptId;
END
GO

CREATE OR ALTER PROCEDURE AddFeeReceiptItem
    @ReceiptId INT,
    @HeadId INT,
    @Amount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.FeeReceiptItems (ReceiptId, HeadId, Amount)
    VALUES (@ReceiptId, @HeadId, @Amount);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO

Create or ALTER PROCEDURE dbo.InsertStudentFeeLedgerEntry
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @TermId INT,
    @HeadId INT = NULL,
    @EntryType NVARCHAR(10), -- 'Debit' or 'Credit'
    @Amount DECIMAL(18,2),
    @Narration NVARCHAR(500) = NULL,
    @ReceiptId INT = NULL,
    @EntryDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- 1?? Insert the ledger entry (Balance will be filled after calculation)
        INSERT INTO dbo.StudentFeeLedger (
            StudentId,
            AcademicYear,
            TermId,
            HeadId,
            EntryType,
            Amount,
            Narration,
            ReceiptId,
            EntryDate,
            CreatedAtUtc,
            Balance
        )
        VALUES (
            @StudentId,
            @AcademicYear,
            @TermId,
            @HeadId,
            @EntryType,
            @Amount,
            @Narration,
            @ReceiptId,
            @EntryDate,
            SYSUTCDATETIME(),
            NULL
        );

        DECLARE @LedgerId INT = CONVERT(INT, SCOPE_IDENTITY());
        DECLARE @TotalDebit DECIMAL(18,2) = 0;
        DECLARE @TotalCredit DECIMAL(18,2) = 0;

        -- 2?? Use a table variable to capture GetStudentFeeBalance result
        DECLARE @BalanceResult TABLE (
            StudentId INT,
            AcademicYear NVARCHAR(15),
            TermId INT,
            TotalDebit DECIMAL(18,2),
            TotalCredit DECIMAL(18,2)
        );

        INSERT INTO @BalanceResult
        EXEC dbo.GetStudentFeeBalance
            @StudentId = @StudentId,
            @AcademicYear = @AcademicYear,
            @TermId = @TermId;

        SELECT TOP (1)
            @TotalDebit = ISNULL(TotalDebit, 0),
            @TotalCredit = ISNULL(TotalCredit, 0)
        FROM @BalanceResult;

        DECLARE @Balance DECIMAL(18,2) = @TotalCredit - @TotalDebit;

        -- 3?? Update the same ledger row with the computed balance
        UPDATE dbo.StudentFeeLedger
        SET Balance = @Balance
        WHERE LedgerId = @LedgerId;

        COMMIT TRAN;

        -- Optional: also show info if you run it manually
        SELECT @LedgerId AS InsertedLedgerId, @Balance AS CurrentBalance;

        RETURN CONVERT(INT, @LedgerId);
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
        RETURN -1; -- optional error indicator
    END CATCH
END;
GO


CREATE OR ALTER PROCEDURE GetFeeReceiptById
    @ReceiptId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.FeeReceipts WHERE ReceiptId = @ReceiptId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeReceiptItemsByReceiptId
    @ReceiptId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.FeeReceiptItems WHERE ReceiptId = @ReceiptId ORDER BY ReceiptItemId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeReceiptList
    @AcademicYear NVARCHAR(15) = NULL,
    @StudentId INT = NULL,
    @TermId INT = NULL,
    @FromDate DATETIME2 = NULL,
    @ToDate DATETIME2 = NULL,
    @PaymentMode NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.FeeReceipts
    WHERE (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@StudentId IS NULL OR StudentId = @StudentId)
      AND (@TermId IS NULL OR TermId = @TermId)
      AND (@PaymentMode IS NULL OR PaymentMode = @PaymentMode)
      AND (@FromDate IS NULL OR ReceiptDate >= @FromDate)
      AND (@ToDate IS NULL OR ReceiptDate <= @ToDate)
    ORDER BY ReceiptDate DESC, ReceiptId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetStudentFeeLedger
    @StudentId INT,
    @AcademicYear NVARCHAR(15) = NULL,
    @TermId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.StudentFeeLedger
    WHERE StudentId = @StudentId
      AND (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@TermId IS NULL OR TermId = @TermId)
    ORDER BY EntryDate DESC, LedgerId DESC;
END
GO

CREATE OR ALTER PROCEDURE GetStudentFeeBalance
    @StudentId INT,
    @AcademicYear NVARCHAR(15) = NULL,
    @TermId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        @StudentId AS StudentId,
        COALESCE(@AcademicYear, '') AS AcademicYear,
        @TermId AS TermId,
        SUM(CASE WHEN EntryType = 'Debit' THEN Amount ELSE 0 END) AS TotalDebit,
        SUM(CASE WHEN EntryType = 'Credit' THEN Amount ELSE 0 END) AS TotalCredit
    FROM dbo.StudentFeeLedger
    WHERE StudentId = @StudentId
      AND (@AcademicYear IS NULL OR AcademicYear = @AcademicYear)
      AND (@TermId IS NULL OR TermId = @TermId);
END
GO


CREATE TABLE dbo.FeeFineRules
(
    RuleId INT IDENTITY(1,1) PRIMARY KEY,
    AcademicYear NVARCHAR(15) NOT NULL,
    ClassName NVARCHAR(50) NULL,
    Section NVARCHAR(10) NULL,
    TermId INT NOT NULL,
    GraceDays INT NOT NULL,
    Mode NVARCHAR(20) NOT NULL, -- PerDayFixed/PerDayPercent/FixedOnce/PercentOnce
    Rate DECIMAL(18,2) NOT NULL,
    MaxAmount DECIMAL(18,2) NULL,
    FineHeadId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_FFR_Term FOREIGN KEY (TermId) REFERENCES dbo.FeeTerms(TermId),
    CONSTRAINT FK_FFR_Head FOREIGN KEY (FineHeadId) REFERENCES dbo.FeeHeads(HeadId)
);
CREATE INDEX IX_FeeFineRules ON dbo.FeeFineRules(AcademicYear, ClassName, Section, TermId, IsActive);
GO

CREATE TABLE dbo.FeeDiscountSchemes
(
    SchemeId INT IDENTITY(1,1) PRIMARY KEY,
    SchemeCode NVARCHAR(50) NOT NULL,
    SchemeName NVARCHAR(200) NOT NULL,
    AcademicYear NVARCHAR(15) NULL,
    ClassName NVARCHAR(50) NULL,
    Section NVARCHAR(10) NULL,
    TermId INT NULL,
    Mode NVARCHAR(10) NOT NULL, -- Percent/Amount
    Value DECIMAL(18,2) NOT NULL,
    CapAmount DECIMAL(18,2) NULL,
    DiscountHeadId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT UX_FDS_Code UNIQUE (SchemeCode),
    CONSTRAINT FK_FDS_Term FOREIGN KEY (TermId) REFERENCES dbo.FeeTerms(TermId),
    CONSTRAINT FK_FDS_Head FOREIGN KEY (DiscountHeadId) REFERENCES dbo.FeeHeads(HeadId)
);
CREATE INDEX IX_FeeDiscountSchemes ON dbo.FeeDiscountSchemes(AcademicYear, ClassName, Section, TermId, IsActive);
GO

CREATE TABLE dbo.StudentScholarships
(
    ScholarshipId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    TermId INT NULL,
    SchemeId INT NULL,
    Mode NVARCHAR(10) NOT NULL, -- Percent/Amount
    Value DECIMAL(18,2) NOT NULL,
    CapAmount DECIMAL(18,2) NULL,
    ScholarshipHeadId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_SS_Student FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_SS_Scheme FOREIGN KEY (SchemeId) REFERENCES dbo.FeeDiscountSchemes(SchemeId),
    CONSTRAINT FK_SS_Head FOREIGN KEY (ScholarshipHeadId) REFERENCES dbo.FeeHeads(HeadId)
);
CREATE INDEX IX_StudentScholarships ON dbo.StudentScholarships(StudentId, AcademicYear, TermId, IsActive);
GO

CREATE TABLE dbo.StudentFeeAdjustments
(
    AdjustmentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    TermId INT NULL,
    HeadId INT NULL,
    Type NVARCHAR(20) NOT NULL, -- Fine/Discount/Scholarship/WriteOff
    Amount DECIMAL(18,2) NOT NULL,
    Narration NVARCHAR(1000) NULL,
    EntryDate DATE NOT NULL,
    CreatedByUserId INT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_SFA_Student FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_SFA_Term FOREIGN KEY (TermId) REFERENCES dbo.FeeTerms(TermId),
    CONSTRAINT FK_SFA_Head FOREIGN KEY (HeadId) REFERENCES dbo.FeeHeads(HeadId)
);
CREATE INDEX IX_SFA ON dbo.StudentFeeAdjustments(StudentId, AcademicYear, TermId, Type, EntryDate);
GO

CREATE OR ALTER PROCEDURE UpsertFeeFineRule
    @RuleId INT,
    @AcademicYear NVARCHAR(15),
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @TermId INT,
    @GraceDays INT,
    @Mode NVARCHAR(20),
    @Rate DECIMAL(18,2),
    @MaxAmount DECIMAL(18,2) = NULL,
    @FineHeadId INT,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @RuleId = 0
    BEGIN
        INSERT INTO dbo.FeeFineRules (AcademicYear, ClassName, Section, TermId, GraceDays, Mode, Rate, MaxAmount, FineHeadId, IsActive)
        VALUES (@AcademicYear, @ClassName, @Section, @TermId, @GraceDays, @Mode, @Rate, @MaxAmount, @FineHeadId, @IsActive);
        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.FeeFineRules
        SET AcademicYear=@AcademicYear, ClassName=@ClassName, Section=@Section, TermId=@TermId,
            GraceDays=@GraceDays, Mode=@Mode, Rate=@Rate, MaxAmount=@MaxAmount, FineHeadId=@FineHeadId, IsActive=@IsActive
        WHERE RuleId=@RuleId;
        RETURN @RuleId;
    END
END
GO

CREATE OR ALTER PROCEDURE DeleteFeeFineRule
    @RuleId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.FeeFineRules WHERE RuleId=@RuleId; RETURN @RuleId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeFineRules
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @TermId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.FeeFineRules
    WHERE (@AcademicYear IS NULL OR AcademicYear=@AcademicYear)
      AND (@ClassName IS NULL OR ClassName=@ClassName)
      AND (@Section IS NULL OR Section=@Section)
      AND (@TermId IS NULL OR TermId=@TermId)
      AND (@IsActive IS NULL OR IsActive=@IsActive)
    ORDER BY AcademicYear DESC, ClassName, Section, TermId, RuleId DESC;
END
GO

CREATE OR ALTER PROCEDURE UpsertFeeDiscountScheme
    @SchemeId INT,
    @SchemeCode NVARCHAR(50),
    @SchemeName NVARCHAR(200),
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @TermId INT = NULL,
    @Mode NVARCHAR(10),
    @Value DECIMAL(18,2),
    @CapAmount DECIMAL(18,2) = NULL,
    @DiscountHeadId INT,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF (@SchemeId = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.FeeDiscountSchemes WHERE SchemeCode=@SchemeCode)
            THROW 62001, 'Scheme code already exists', 1;

        INSERT INTO dbo.FeeDiscountSchemes (SchemeCode, SchemeName, AcademicYear, ClassName, Section, TermId, Mode, Value, CapAmount, DiscountHeadId, IsActive)
        VALUES (@SchemeCode, @SchemeName, @AcademicYear, @ClassName, @Section, @TermId, @Mode, @Value, @CapAmount, @DiscountHeadId, @IsActive);

        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.FeeDiscountSchemes WHERE SchemeCode=@SchemeCode AND SchemeId<>@SchemeId)
            THROW 62002, 'Scheme code used by another scheme', 1;

        UPDATE dbo.FeeDiscountSchemes
        SET SchemeCode=@SchemeCode, SchemeName=@SchemeName, AcademicYear=@AcademicYear, ClassName=@ClassName, Section=@Section, TermId=@TermId,
            Mode=@Mode, Value=@Value, CapAmount=@CapAmount, DiscountHeadId=@DiscountHeadId, IsActive=@IsActive
        WHERE SchemeId=@SchemeId;

        RETURN @SchemeId;
    END
END
GO

CREATE OR ALTER PROCEDURE DeleteFeeDiscountScheme
    @SchemeId INT
AS
BEGIN
    SET NOCOUNT ON; DELETE FROM dbo.FeeDiscountSchemes WHERE SchemeId=@SchemeId; RETURN @SchemeId;
END
GO

CREATE OR ALTER PROCEDURE GetFeeDiscountSchemes
    @AcademicYear NVARCHAR(15) = NULL,
    @ClassName NVARCHAR(50) = NULL,
    @Section NVARCHAR(10) = NULL,
    @TermId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.FeeDiscountSchemes
    WHERE (@AcademicYear IS NULL OR AcademicYear=@AcademicYear)
      AND (@ClassName IS NULL OR ClassName=@ClassName)
      AND (@Section IS NULL OR Section=@Section)
      AND (@TermId IS NULL OR TermId=@TermId)
      AND (@IsActive IS NULL OR IsActive=@IsActive)
    ORDER BY SchemeName;
END
GO

CREATE OR ALTER PROCEDURE UpsertStudentScholarship
    @ScholarshipId INT,
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @TermId INT = NULL,
    @SchemeId INT = NULL,
    @Mode NVARCHAR(10),
    @Value DECIMAL(18,2),
    @CapAmount DECIMAL(18,2) = NULL,
    @ScholarshipHeadId INT,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @ScholarshipId = 0
    BEGIN
        INSERT INTO dbo.StudentScholarships (StudentId, AcademicYear, TermId, SchemeId, Mode, Value, CapAmount, ScholarshipHeadId, IsActive)
        VALUES (@StudentId, @AcademicYear, @TermId, @SchemeId, @Mode, @Value, @CapAmount, @ScholarshipHeadId, @IsActive);
        RETURN CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE dbo.StudentScholarships
        SET StudentId=@StudentId, AcademicYear=@AcademicYear, TermId=@TermId, SchemeId=@SchemeId,
            Mode=@Mode, Value=@Value, CapAmount=@CapAmount, ScholarshipHeadId=@ScholarshipHeadId, IsActive=@IsActive
        WHERE ScholarshipId=@ScholarshipId;
        RETURN @ScholarshipId;
    END
END
GO

CREATE OR ALTER PROCEDURE DeleteStudentScholarship
    @ScholarshipId INT
AS
BEGIN
    SET NOCOUNT ON; DELETE FROM dbo.StudentScholarships WHERE ScholarshipId=@ScholarshipId; RETURN @ScholarshipId;
END
GO

CREATE OR ALTER PROCEDURE GetStudentScholarships
    @StudentId INT = NULL,
    @AcademicYear NVARCHAR(15) = NULL,
    @TermId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.StudentScholarships
    WHERE (@StudentId IS NULL OR StudentId=@StudentId)
      AND (@AcademicYear IS NULL OR AcademicYear=@AcademicYear)
      AND (@TermId IS NULL OR TermId=@TermId)
      AND (@IsActive IS NULL OR IsActive=@IsActive)
    ORDER BY ScholarshipId DESC;
END
GO

-- Uses FeeTerms.DueDate, fine rules, and current outstanding
CREATE OR ALTER PROCEDURE ApplyLateFeeForTerm
    @AcademicYear NVARCHAR(15),
    @TermId INT,
    @AsOfDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DueDate DATETIME2 = (SELECT TOP 1 DueDate FROM dbo.FeeTerms WHERE AcademicYear=@AcademicYear AND TermId=@TermId);
    IF @DueDate IS NULL THROW 63001, 'Term due date not found', 1;
    IF @AsOfDate <= @DueDate RETURN 0;

    ;WITH BAL AS
    (
        SELECT s.StudentId,
               SUM(CASE WHEN l.EntryType='Debit' THEN l.Amount ELSE 0 END) AS DebitAmt,
               SUM(CASE WHEN l.EntryType='Credit' THEN l.Amount ELSE 0 END) AS CreditAmt
        FROM dbo.Students s
        LEFT JOIN dbo.StudentFeeLedger l ON l.StudentId=s.StudentId AND l.AcademicYear=@AcademicYear AND l.TermId=@TermId
        GROUP BY s.StudentId
    ),
    OVR AS --find Students Who Owe Money
    (
        SELECT b.StudentId, (b.DebitAmt - b.CreditAmt) AS Outstanding
        FROM BAL b
        WHERE (b.DebitAmt - b.CreditAmt) > 0
    ),
    RULES AS --get the Fine Rules for the Term
    (
        SELECT r.*
        FROM dbo.FeeFineRules r
        WHERE r.AcademicYear=@AcademicYear AND r.TermId=@TermId AND r.IsActive=1
    )

    --determine who gets fined and how much:
    SELECT o.StudentId, r.RuleId, r.Mode, r.Rate, r.MaxAmount, r.FineHeadId,
           DATEDIFF(DAY, @DueDate, @AsOfDate) - r.GraceDays AS ChargeDays,
           o.Outstanding
    INTO #Fines
    FROM OVR o
    CROSS APPLY (
        SELECT TOP 1 r.* FROM RULES r
        LEFT JOIN dbo.Students s ON s.StudentId=o.StudentId
        WHERE (r.ClassName IS NULL OR r.ClassName = s.ClassName)
          AND (r.Section IS NULL OR r.Section = s.Section)
        ORDER BY r.ClassName DESC, r.Section DESC, r.RuleId DESC
    ) AS r
    WHERE (DATEDIFF(DAY, @DueDate, @AsOfDate) - r.GraceDays) > 0;

    DECLARE @Posted INT = 0;

    INSERT INTO dbo.StudentFeeLedger (StudentId, AcademicYear, TermId, HeadId, EntryType, Amount, Narration, ReceiptId, EntryDate)
    SELECT f.StudentId, @AcademicYear, @TermId, f.FineHeadId, 'Debit',
           CASE f.Mode
                WHEN 'PerDayFixed'   THEN CONVERT(DECIMAL(18,2), f.Rate * f.ChargeDays)
                WHEN 'PerDayPercent' THEN CONVERT(DECIMAL(18,2), (f.Outstanding * f.Rate/100.0) * f.ChargeDays)
                WHEN 'FixedOnce'     THEN f.Rate
                WHEN 'PercentOnce'   THEN CONVERT(DECIMAL(18,2), f.Outstanding * f.Rate/100.0)
            END AS Amount,
           CONCAT('Late fee as of ', CONVERT(date,@AsOfDate)),
           NULL,
           CAST(@AsOfDate AS DATE)
    FROM #Fines f
    WHERE
        CASE f.Mode
            WHEN 'PerDayFixed'   THEN f.Rate * f.ChargeDays
            WHEN 'PerDayPercent' THEN (f.Outstanding * f.Rate/100.0) * f.ChargeDays
            WHEN 'FixedOnce'     THEN f.Rate
            WHEN 'PercentOnce'   THEN (f.Outstanding * f.Rate/100.0)
        END > 0
      AND (f.MaxAmount IS NULL OR
           CASE f.Mode
               WHEN 'PerDayFixed'   THEN f.Rate * f.ChargeDays
               WHEN 'PerDayPercent' THEN (f.Outstanding * f.Rate/100.0) * f.ChargeDays
               WHEN 'FixedOnce'     THEN f.Rate
               WHEN 'PercentOnce'   THEN (f.Outstanding * f.Rate/100.0)
           END <= f.MaxAmount);

    SET @Posted = @@ROWCOUNT;
    DROP TABLE IF EXISTS #Fines;

    RETURN @Posted;
END
GO

CREATE OR ALTER PROCEDURE ApplyDiscountForStudentTerm
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @TermId INT,
    @SchemeId INT = NULL,
    @Mode NVARCHAR(10) = NULL,     -- Percent/Amount if no scheme
    @Value DECIMAL(18,2) = NULL,
    @CapAmount DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @useMode NVARCHAR(10), @useValue DECIMAL(18,2), @useCap DECIMAL(18,2), @headId INT;

    IF @SchemeId IS NOT NULL
    BEGIN
        SELECT TOP 1 @useMode=Mode, @useValue=Value, @useCap=CapAmount, @headId=DiscountHeadId
        FROM dbo.FeeDiscountSchemes WHERE SchemeId=@SchemeId AND IsActive=1;
        IF @useMode IS NULL THROW 64001, 'Scheme not found or inactive', 1;
    END
    ELSE
    BEGIN
        SET @useMode = @Mode;
        SET @useValue = @Value;
        SET @useCap = @CapAmount;
        -- Default: require a discount head; using a standard DISCOUNT head is recommended
        SELECT TOP 1 @headId = HeadId FROM dbo.FeeHeads WHERE HeadCode='DISCOUNT';
        IF @headId IS NULL THROW 64002, 'DISCOUNT head not found; create FeeHead with HeadCode=DISCOUNT', 1;
    END

    IF @useMode NOT IN ('Percent','Amount') THROW 64003, 'Invalid mode', 1;

    DECLARE @Debit DECIMAL(18,2) = (
        SELECT COALESCE(SUM(Amount),0) FROM dbo.StudentFeeLedger
        WHERE StudentId=@StudentId AND AcademicYear=@AcademicYear AND TermId=@TermId AND EntryType='Debit'
    );
    DECLARE @Credit DECIMAL(18,2) = (
        SELECT COALESCE(SUM(Amount),0) FROM dbo.StudentFeeLedger
        WHERE StudentId=@StudentId AND AcademicYear=@AcademicYear AND TermId=@TermId AND EntryType='Credit'
    );
    DECLARE @Outstanding DECIMAL(18,2) = @Debit - @Credit;
    IF @Outstanding <= 0 RETURN 0;

    DECLARE @Discount DECIMAL(18,2) = CASE WHEN @useMode='Percent' THEN @Outstanding * @useValue/100.0 ELSE @useValue END;
    IF @useCap IS NOT NULL AND @Discount > @useCap SET @Discount = @useCap;

    IF @Discount <= 0 RETURN 0;

    INSERT INTO dbo.StudentFeeLedger (StudentId, AcademicYear, TermId, HeadId, EntryType, Amount, Narration, ReceiptId, EntryDate)
    VALUES (@StudentId, @AcademicYear, @TermId, @headId, 'Credit', @Discount, 'Discount applied', NULL, CAST(SYSUTCDATETIME() AS DATE));

    RETURN 1;
END
GO

CREATE OR ALTER PROCEDURE InsertStudentFeeAdjustment
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @TermId INT = NULL,
    @HeadId INT = NULL,
    @Type NVARCHAR(20),
    @Amount DECIMAL(18,2),
    @Narration NVARCHAR(1000) = NULL,
    @EntryDate DATE,
    @CreatedByUserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.StudentFeeAdjustments (StudentId, AcademicYear, TermId, HeadId, Type, Amount, Narration, EntryDate, CreatedByUserId)
    VALUES (@StudentId, @AcademicYear, @TermId, @HeadId, @Type, @Amount, @Narration, @EntryDate, @CreatedByUserId);

    DECLARE @AdjId INT = CONVERT(INT, SCOPE_IDENTITY());

    DECLARE @EntryType NVARCHAR(10) = CASE WHEN @Type='Fine' THEN 'Debit' ELSE 'Credit' END;

    INSERT INTO dbo.StudentFeeLedger (StudentId, AcademicYear, TermId, HeadId, EntryType, Amount, Narration, ReceiptId, EntryDate)
    VALUES (@StudentId, @AcademicYear, @TermId, @HeadId, @EntryType, @Amount, CONCAT('Adjustment: ', @Type), NULL, @EntryDate);

    RETURN @AdjId;
END
GO

CREATE OR ALTER PROCEDURE GetStudentFeeAdjustments
    @StudentId INT = NULL,
    @AcademicYear NVARCHAR(15) = NULL,
    @TermId INT = NULL,
    @Type NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM dbo.StudentFeeAdjustments
    WHERE (@StudentId IS NULL OR StudentId=@StudentId)
      AND (@AcademicYear IS NULL OR AcademicYear=@AcademicYear)
      AND (@TermId IS NULL OR TermId=@TermId)
      AND (@Type IS NULL OR Type=@Type)
    ORDER BY EntryDate DESC, AdjustmentId DESC;
END
GO



CREATE TABLE dbo.PaymentGatewayOrders
(
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    OrderNo NVARCHAR(50) NOT NULL,
    GatewayName NVARCHAR(50) NOT NULL,
    StudentId INT NOT NULL,
    AcademicYear NVARCHAR(15) NOT NULL,
    TermId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- Initiated/Pending/Success/Failed/Cancelled
    GatewayOrderId NVARCHAR(100) NULL,
    PaymentId NVARCHAR(100) NULL,
    PaymentMode NVARCHAR(30) NULL,
    ReferenceNo NVARCHAR(100) NULL,
    ReturnUrl NVARCHAR(500) NULL,
    CallbackUrl NVARCHAR(500) NULL,
    ItemsJson NVARCHAR(MAX) NOT NULL,
    ReceiptId INT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc DATETIME2 NULL,
    CONSTRAINT UX_PGO_OrderNo UNIQUE (OrderNo),
    CONSTRAINT FK_PGO_Student FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_PGO_Term FOREIGN KEY (TermId) REFERENCES dbo.FeeTerms(TermId)
);
CREATE INDEX IX_PGO_Student ON dbo.PaymentGatewayOrders(StudentId, AcademicYear, TermId);
GO

CREATE TABLE dbo.PaymentGatewayEvents
(
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    EventType NVARCHAR(50) NOT NULL,
    Payload NVARCHAR(MAX) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_PGE_Order FOREIGN KEY (OrderId) REFERENCES dbo.PaymentGatewayOrders(OrderId) ON DELETE CASCADE
);
CREATE INDEX IX_PGE_Order ON dbo.PaymentGatewayEvents(OrderId, EventType, CreatedAtUtc DESC);
GO

CREATE OR ALTER PROCEDURE CreatePaymentOrder
    @GatewayName NVARCHAR(50),
    @StudentId INT,
    @AcademicYear NVARCHAR(15),
    @TermId INT,
    @Amount DECIMAL(18,2),
    @Currency NVARCHAR(10),
    @ReturnUrl NVARCHAR(500) = NULL,
    @CallbackUrl NVARCHAR(500) = NULL,
    @ItemsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PaymentGatewayOrders
    (OrderNo, GatewayName, StudentId, AcademicYear, TermId, Amount, Currency, Status, ReturnUrl, CallbackUrl, ItemsJson)
    VALUES ('', @GatewayName, @StudentId, @AcademicYear, @TermId, @Amount, @Currency, 'Initiated', @ReturnUrl, @CallbackUrl, @ItemsJson);

    DECLARE @OrderId INT = CONVERT(INT, SCOPE_IDENTITY());
    DECLARE @OrderNo NVARCHAR(50) = CONCAT('PG-', CONVERT(CHAR(8), GETDATE(), 112), '-', RIGHT('000000' + CAST(@OrderId AS VARCHAR(6)), 6));

    UPDATE dbo.PaymentGatewayOrders SET OrderNo = @OrderNo WHERE OrderId = @OrderId;

    RETURN @OrderId;
END
GO

CREATE OR ALTER PROCEDURE UpdatePaymentOrderStatus
    @OrderId INT,
    @Status NVARCHAR(20),
    @PaymentId NVARCHAR(100) = NULL,
    @GatewayOrderId NVARCHAR(100) = NULL,
    @ReferenceNo NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PaymentGatewayOrders
    SET Status = @Status,
        PaymentId = COALESCE(@PaymentId, PaymentId),
        GatewayOrderId = COALESCE(@GatewayOrderId, GatewayOrderId),
        ReferenceNo = COALESCE(@ReferenceNo, ReferenceNo),
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE OrderId = @OrderId;

    RETURN @OrderId;
END
GO

CREATE OR ALTER PROCEDURE GetPaymentOrderByOrderNo
    @OrderNo NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.PaymentGatewayOrders WHERE OrderNo = @OrderNo;
END
GO

CREATE OR ALTER PROCEDURE MarkPaymentOrderReceipted
    @OrderId INT,
    @ReceiptId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PaymentGatewayOrders
    SET ReceiptId = @ReceiptId,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE OrderId = @OrderId;

    RETURN @OrderId;
END
GO


CREATE OR ALTER PROCEDURE InsertPaymentGatewayEvent
    @OrderId INT,
    @EventType NVARCHAR(50),
    @Payload NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PaymentGatewayEvents (OrderId, EventType, Payload)
    VALUES (@OrderId, @EventType, @Payload);

    RETURN CONVERT(INT, SCOPE_IDENTITY());
END
GO


CREATE OR ALTER PROCEDURE GetPaymentOrderByGatewayOrderId
    @GatewayOrderId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM dbo.PaymentGatewayOrders WHERE GatewayOrderId = @GatewayOrderId;
END
GO