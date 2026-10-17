/*
================================================================================
 StoredProcedures_StudentSave.sql
 Ideal High School — student admit / update / status via stored procedures

 Run against StudentManagementSDB (after StudentManagementSDB.sql).
 The desktop app calls these for Save & Edit Profile.
================================================================================
*/

USE StudentManagementSDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.usp_AdmitStudent', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_AdmitStudent;
IF OBJECT_ID(N'dbo.usp_UpdateStudent', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_UpdateStudent;
IF OBJECT_ID(N'dbo.usp_SetStudentStatus', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_SetStudentStatus;
GO

/* ---------- Admit: student + opening tuition invoice + ledger ---------- */
CREATE PROCEDURE dbo.usp_AdmitStudent
    @FullName           NVARCHAR(150),
    @FatherName         NVARCHAR(150),
    @MotherName         NVARCHAR(150) = NULL,
    @BloodGroup         INT,
    @Gender             INT,
    @DateOfBirth        DATE,
    @GuardianPhone      NVARCHAR(20),
    @PresentAddress     NVARCHAR(500) = NULL,
    @PermanentAddress   NVARCHAR(500) = NULL,
    @ClassId            INT,
    @SectionId          INT,
    @RollNumber         NVARCHAR(20),
    @MonthlyTuitionFee  DECIMAL(18,2),
    @AcademicSession    NVARCHAR(20) = N'2025-2026',
    @PhotoPath          NVARCHAR(260) = NULL,
    @WantTransport      BIT = 0,
    @MonthlyTransportFee DECIMAL(18,2) = 0,
    @CreatedBy          NVARCHAR(100) = NULL,
    @StudentId          INT OUTPUT,
    @RegistrationNo     NVARCHAR(30) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @FullName IS NULL OR LTRIM(RTRIM(@FullName)) = N''
    BEGIN RAISERROR(N'Full name is required.', 16, 1); RETURN; END
    IF @FatherName IS NULL OR LTRIM(RTRIM(@FatherName)) = N''
    BEGIN RAISERROR(N'Father name is required.', 16, 1); RETURN; END
    IF @GuardianPhone IS NULL OR LTRIM(RTRIM(@GuardianPhone)) = N''
    BEGIN RAISERROR(N'Guardian phone is required.', 16, 1); RETURN; END
    IF @ClassId IS NULL OR @ClassId <= 0 OR @SectionId IS NULL OR @SectionId <= 0
    BEGIN RAISERROR(N'Class and section are required.', 16, 1); RETURN; END
    IF @RollNumber IS NULL OR LTRIM(RTRIM(@RollNumber)) = N''
    BEGIN RAISERROR(N'Roll number is required.', 16, 1); RETURN; END
    IF @MonthlyTuitionFee IS NULL OR @MonthlyTuitionFee < 0
    BEGIN RAISERROR(N'Tuition fee cannot be negative.', 16, 1); RETURN; END
    IF @MonthlyTransportFee IS NULL OR @MonthlyTransportFee < 0
    BEGIN RAISERROR(N'Transport fee cannot be negative.', 16, 1); RETURN; END
    IF @DateOfBirth IS NULL OR @DateOfBirth >= CAST(GETDATE() AS DATE)
    BEGIN RAISERROR(N'Date of birth must be in the past.', 16, 1); RETURN; END
    IF NOT EXISTS (SELECT 1 FROM dbo.Classes WHERE ClassId = @ClassId AND IsDeleted = 0)
    BEGIN RAISERROR(N'Class not found.', 16, 1); RETURN; END
    IF NOT EXISTS (SELECT 1 FROM dbo.Sections WHERE SectionId = @SectionId AND ClassId = @ClassId AND IsDeleted = 0)
    BEGIN RAISERROR(N'Section not found for class.', 16, 1); RETURN; END

    DECLARE @Year INT = YEAR(GETDATE());
    DECLARE @Key NVARCHAR(40) = N'REG-' + CAST(@Year AS NVARCHAR(4));
    DECLARE @Next INT;

    BEGIN TRAN;

    IF NOT EXISTS (SELECT 1 FROM dbo.NumberSequences WITH (UPDLOCK, HOLDLOCK) WHERE SequenceKey = @Key)
    BEGIN
        DECLARE @MaxExisting INT = 0;
        SELECT @MaxExisting = ISNULL(MAX(TRY_CAST(RIGHT(RegistrationNo, 4) AS INT)), 0)
        FROM dbo.Students
        WHERE RegistrationNo LIKE @Key + N'-%';
        INSERT INTO dbo.NumberSequences (SequenceKey, LastValue) VALUES (@Key, @MaxExisting);
    END

    UPDATE dbo.NumberSequences
    SET LastValue = LastValue + 1, UpdatedAt = SYSUTCDATETIME()
    WHERE SequenceKey = @Key;
    SELECT @Next = LastValue FROM dbo.NumberSequences WHERE SequenceKey = @Key;
    SET @RegistrationNo = @Key + N'-' + RIGHT(N'0000' + CAST(@Next AS NVARCHAR(10)), 4);

    INSERT INTO dbo.Students
    (
        RegistrationNo, FullName, FatherName, MotherName, BloodGroup, Gender, DateOfBirth,
        GuardianPhone, PresentAddress, PermanentAddress, ClassId, SectionId, RollNumber,
        MonthlyTuitionFee, AdmissionDate, Status, PhotoPath, AcademicSession,
        WantTransport, TransportActive, MonthlyTransportFee, TransportActivatedOn, CreatedBy
    )
    VALUES
    (
        @RegistrationNo, LTRIM(RTRIM(@FullName)), LTRIM(RTRIM(@FatherName)),
        NULLIF(LTRIM(RTRIM(@MotherName)), N''), @BloodGroup, @Gender, @DateOfBirth,
        LTRIM(RTRIM(@GuardianPhone)), @PresentAddress, @PermanentAddress, @ClassId, @SectionId,
        LTRIM(RTRIM(@RollNumber)), @MonthlyTuitionFee, CAST(GETDATE() AS DATE), 1,
        @PhotoPath, ISNULL(NULLIF(LTRIM(RTRIM(@AcademicSession)), N''), N'2025-2026'),
        ISNULL(@WantTransport, 0), 0, ISNULL(@MonthlyTransportFee, 0), NULL, @CreatedBy
    );

    SET @StudentId = SCOPE_IDENTITY();

    DECLARE @Period NVARCHAR(100) = FORMAT(GETDATE(), 'MMM-yyyy');
    DECLARE @Voucher NVARCHAR(50) = N'INV-' + CONVERT(CHAR(8), GETDATE(), 112) + N'-' + RIGHT(N'0000' + CAST(@StudentId AS NVARCHAR(10)), 4);
    DECLARE @InvoiceNo NVARCHAR(50);
    DECLARE @Ym NVARCHAR(6) = CONVERT(CHAR(6), GETDATE(), 112);
    DECLARE @InvKey NVARCHAR(40) = N'INV-' + @Ym;
    DECLARE @InvNext INT;

    IF NOT EXISTS (SELECT 1 FROM dbo.NumberSequences WITH (UPDLOCK, HOLDLOCK) WHERE SequenceKey = @InvKey)
    BEGIN
        DECLARE @MaxInv INT = 0;
        SELECT @MaxInv = ISNULL(MAX(TRY_CAST(RIGHT(InvoiceNo, 5) AS INT)), 0)
        FROM dbo.FeeInvoices WHERE InvoiceNo LIKE @InvKey + N'-%';
        INSERT INTO dbo.NumberSequences (SequenceKey, LastValue) VALUES (@InvKey, @MaxInv);
    END

    UPDATE dbo.NumberSequences SET LastValue = LastValue + 1, UpdatedAt = SYSUTCDATETIME() WHERE SequenceKey = @InvKey;
    SELECT @InvNext = LastValue FROM dbo.NumberSequences WHERE SequenceKey = @InvKey;
    SET @InvoiceNo = @InvKey + N'-' + RIGHT(N'00000' + CAST(@InvNext AS NVARCHAR(10)), 5);

    DECLARE @LedgerId BIGINT;

    INSERT INTO dbo.StudentLedger
        (StudentId, TransactionDate, VoucherNo, Particulars, FeePeriod, DebitAmount, CreditAmount, EntryType, Status, CreatedBy)
    VALUES
        (@StudentId, SYSUTCDATETIME(), @Voucher, N'Admission / Tuition Fee - ' + @Period, @Period, @MonthlyTuitionFee, 0, 1, 1, @CreatedBy);

    SET @LedgerId = SCOPE_IDENTITY();

    INSERT INTO dbo.FeeInvoices
        (StudentId, InvoiceNo, FeePeriod, CategoryName, Amount, FineAmount, WaiverAmount, InvoiceDate, DueDate, Status, LedgerId, CreatedBy)
    VALUES
        (@StudentId, @InvoiceNo, @Period, N'Tuition Fee', @MonthlyTuitionFee, 0, 0, CAST(GETDATE() AS DATE), DATEADD(DAY, 10, CAST(GETDATE() AS DATE)), 1, @LedgerId, @CreatedBy);

    COMMIT TRAN;

    SELECT @StudentId AS StudentId, @RegistrationNo AS RegistrationNo, @InvoiceNo AS InvoiceNo, @LedgerId AS LedgerId;
END
GO

/* ---------- Update profile (edit) ---------- */
CREATE PROCEDURE dbo.usp_UpdateStudent
    @StudentId          INT,
    @FullName           NVARCHAR(150),
    @FatherName         NVARCHAR(150),
    @MotherName         NVARCHAR(150) = NULL,
    @BloodGroup         INT,
    @Gender             INT,
    @DateOfBirth        DATE,
    @GuardianPhone      NVARCHAR(20),
    @PresentAddress     NVARCHAR(500) = NULL,
    @PermanentAddress   NVARCHAR(500) = NULL,
    @ClassId            INT,
    @SectionId          INT,
    @RollNumber         NVARCHAR(20),
    @MonthlyTuitionFee  DECIMAL(18,2),
    @Status             INT,
    @PhotoPath          NVARCHAR(260) = NULL,
    @AcademicSession    NVARCHAR(20) = NULL,
    @WantTransport      BIT = 0,
    @TransportActive    BIT = 0,
    @MonthlyTransportFee DECIMAL(18,2) = 0,
    @TransportActivatedOn DATE = NULL,
    @UpdatedBy          NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentId = @StudentId AND IsDeleted = 0)
    BEGIN RAISERROR(N'Student not found.', 16, 1); RETURN; END

    IF @FullName IS NULL OR LTRIM(RTRIM(@FullName)) = N''
    BEGIN RAISERROR(N'Full name is required.', 16, 1); RETURN; END
    IF @FatherName IS NULL OR LTRIM(RTRIM(@FatherName)) = N''
    BEGIN RAISERROR(N'Father name is required.', 16, 1); RETURN; END
    IF @GuardianPhone IS NULL OR LTRIM(RTRIM(@GuardianPhone)) = N''
    BEGIN RAISERROR(N'Guardian phone is required.', 16, 1); RETURN; END
    IF @ClassId IS NULL OR @ClassId <= 0 OR @SectionId IS NULL OR @SectionId <= 0
    BEGIN RAISERROR(N'Class and section are required.', 16, 1); RETURN; END
    IF @RollNumber IS NULL OR LTRIM(RTRIM(@RollNumber)) = N''
    BEGIN RAISERROR(N'Roll number is required.', 16, 1); RETURN; END
    IF @MonthlyTuitionFee IS NULL OR @MonthlyTuitionFee < 0
    BEGIN RAISERROR(N'Tuition fee cannot be negative.', 16, 1); RETURN; END
    IF @MonthlyTransportFee IS NULL OR @MonthlyTransportFee < 0
    BEGIN RAISERROR(N'Transport fee cannot be negative.', 16, 1); RETURN; END
    IF ISNULL(@TransportActive, 0) = 1 AND ISNULL(@MonthlyTransportFee, 0) <= 0
    BEGIN RAISERROR(N'Set Monthly Transport Fee before activating Gari service.', 16, 1); RETURN; END
    IF @Status NOT BETWEEN 1 AND 5
    BEGIN RAISERROR(N'Invalid student status.', 16, 1); RETURN; END
    IF NOT EXISTS (SELECT 1 FROM dbo.Sections WHERE SectionId = @SectionId AND ClassId = @ClassId AND IsDeleted = 0)
    BEGIN RAISERROR(N'Section not found for class.', 16, 1); RETURN; END

    DECLARE @PrevActivated DATE = (SELECT TransportActivatedOn FROM dbo.Students WHERE StudentId = @StudentId);
    DECLARE @NewActive BIT = ISNULL(@TransportActive, 0);
    DECLARE @NewActivated DATE = NULL;
    IF @NewActive = 1
        SET @NewActivated = COALESCE(@TransportActivatedOn, @PrevActivated, CAST(GETDATE() AS DATE));

    UPDATE dbo.Students
    SET FullName = LTRIM(RTRIM(@FullName)),
        FatherName = LTRIM(RTRIM(@FatherName)),
        MotherName = NULLIF(LTRIM(RTRIM(@MotherName)), N''),
        BloodGroup = @BloodGroup,
        Gender = @Gender,
        DateOfBirth = @DateOfBirth,
        GuardianPhone = LTRIM(RTRIM(@GuardianPhone)),
        PresentAddress = @PresentAddress,
        PermanentAddress = @PermanentAddress,
        ClassId = @ClassId,
        SectionId = @SectionId,
        RollNumber = LTRIM(RTRIM(@RollNumber)),
        MonthlyTuitionFee = @MonthlyTuitionFee,
        Status = @Status,
        PhotoPath = @PhotoPath,
        AcademicSession = CASE
            WHEN @AcademicSession IS NULL OR LTRIM(RTRIM(@AcademicSession)) = N'' THEN AcademicSession
            ELSE LTRIM(RTRIM(@AcademicSession))
        END,
        WantTransport = CASE WHEN @NewActive = 1 OR ISNULL(@WantTransport, 0) = 1 THEN 1 ELSE 0 END,
        TransportActive = @NewActive,
        MonthlyTransportFee = ISNULL(@MonthlyTransportFee, 0),
        TransportActivatedOn = @NewActivated,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedBy = @UpdatedBy
    WHERE StudentId = @StudentId AND IsDeleted = 0;

    SELECT StudentId, RegistrationNo, Status
    FROM dbo.Students
    WHERE StudentId = @StudentId;
END
GO

/* ---------- Set status only (Mark Inactive / Reactivate) ---------- */
CREATE PROCEDURE dbo.usp_SetStudentStatus
    @StudentId  INT,
    @Status     INT,
    @UpdatedBy  NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentId = @StudentId AND IsDeleted = 0)
    BEGIN RAISERROR(N'Student not found.', 16, 1); RETURN; END

    IF @Status NOT BETWEEN 1 AND 5
    BEGIN RAISERROR(N'Invalid student status (1-5).', 16, 1); RETURN; END

    UPDATE dbo.Students
    SET Status = @Status,
        UpdatedAt = SYSUTCDATETIME(),
        UpdatedBy = @UpdatedBy
    WHERE StudentId = @StudentId AND IsDeleted = 0;

    SELECT StudentId, RegistrationNo, Status
    FROM dbo.Students
    WHERE StudentId = @StudentId;
END
GO

PRINT N'Student save/edit stored procedures ready: usp_AdmitStudent, usp_UpdateStudent, usp_SetStudentStatus';
GO
