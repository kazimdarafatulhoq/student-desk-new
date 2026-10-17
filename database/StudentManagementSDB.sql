/*
================================================================================
 StudentManagementSDB.sql
 Fouzdarhat Safiria Dakhil Madrasah — Student Management & Financial Accounting
 SQL Server 2022 schema aligned with the WinForms desktop design:
   Admission · Fee Collection Counter · Ledger · Exam Clearance · Admit Card
   Dashboard · User Management

 Database name MUST match App.config / appsettings.json: StudentManagementSDB

 Run order (SSMS as sa / Administrator):
   1. CreateDatabaseAndGrantAccess.sql
   2. This script (StudentManagementSDB.sql)

 Re-run safe: drops/recreates application objects (dev/demo). Do not run on
 production with live data without a backup.
================================================================================
*/

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF DB_ID(N'StudentManagementSDB') IS NULL
BEGIN
    CREATE DATABASE StudentManagementSDB;
END
GO

USE StudentManagementSDB;
GO

/* ---------- Drop stored procedures ---------- */
IF OBJECT_ID(N'dbo.usp_GetNextRegistrationNo', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetNextRegistrationNo;
IF OBJECT_ID(N'dbo.usp_GetNextReceiptNo', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetNextReceiptNo;
IF OBJECT_ID(N'dbo.usp_GetNextInvoiceNo', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetNextInvoiceNo;
IF OBJECT_ID(N'dbo.usp_GetNextAdmitCardNo', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetNextAdmitCardNo;
IF OBJECT_ID(N'dbo.usp_GetStudentBalance', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetStudentBalance;
IF OBJECT_ID(N'dbo.usp_GetStudentLedger', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetStudentLedger;
IF OBJECT_ID(N'dbo.usp_GetDashboardStats', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetDashboardStats;
IF OBJECT_ID(N'dbo.usp_SearchStudents', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_SearchStudents;
IF OBJECT_ID(N'dbo.usp_GetOutstandingDues', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetOutstandingDues;
IF OBJECT_ID(N'dbo.usp_GetClassSectionLookup', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetClassSectionLookup;
IF OBJECT_ID(N'dbo.usp_PostTuitionInvoice', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_PostTuitionInvoice;
IF OBJECT_ID(N'dbo.usp_RecordFeePayment', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_RecordFeePayment;
GO

/* ---------- Drop views ---------- */
IF OBJECT_ID(N'dbo.vw_StudentLedgerDetailed', N'V') IS NOT NULL DROP VIEW dbo.vw_StudentLedgerDetailed;
IF OBJECT_ID(N'dbo.vw_StudentBalanceSummary', N'V') IS NOT NULL DROP VIEW dbo.vw_StudentBalanceSummary;
IF OBJECT_ID(N'dbo.vw_ActiveStudents', N'V') IS NOT NULL DROP VIEW dbo.vw_ActiveStudents;
GO

/* ---------- Drop tables (FK order) ---------- */
IF OBJECT_ID(N'dbo.AdmitCards', N'U') IS NOT NULL DROP TABLE dbo.AdmitCards;
IF OBJECT_ID(N'dbo.ExamClearances', N'U') IS NOT NULL DROP TABLE dbo.ExamClearances;
IF OBJECT_ID(N'dbo.ExamTerms', N'U') IS NOT NULL DROP TABLE dbo.ExamTerms;
IF OBJECT_ID(N'dbo.FeePayments', N'U') IS NOT NULL DROP TABLE dbo.FeePayments;
IF OBJECT_ID(N'dbo.FeeInvoices', N'U') IS NOT NULL DROP TABLE dbo.FeeInvoices;
IF OBJECT_ID(N'dbo.StudentLedger', N'U') IS NOT NULL DROP TABLE dbo.StudentLedger;
IF OBJECT_ID(N'dbo.StudentFeeStructures', N'U') IS NOT NULL DROP TABLE dbo.StudentFeeStructures;
IF OBJECT_ID(N'dbo.FeeCategories', N'U') IS NOT NULL DROP TABLE dbo.FeeCategories;
IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID(N'dbo.Sections', N'U') IS NOT NULL DROP TABLE dbo.Sections;
IF OBJECT_ID(N'dbo.Classes', N'U') IS NOT NULL DROP TABLE dbo.Classes;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID(N'dbo.InstitutionSettings', N'U') IS NOT NULL DROP TABLE dbo.InstitutionSettings;
IF OBJECT_ID(N'dbo.NumberSequences', N'U') IS NOT NULL DROP TABLE dbo.NumberSequences;
GO

/* Institution branding — matches AppSession / App.config design */
CREATE TABLE dbo.InstitutionSettings
(
    SettingId           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InstitutionSettings PRIMARY KEY,
    InstitutionName     NVARCHAR(200) NOT NULL,
    CampusName          NVARCHAR(200) NULL,
    AcademicSession     NVARCHAR(20) NOT NULL,
    AddressLine         NVARCHAR(300) NULL,
    Phone               NVARCHAR(30) NULL,
    Email               NVARCHAR(150) NULL,
    CurrencySymbol      NVARCHAR(10) NOT NULL CONSTRAINT DF_Institution_Currency DEFAULT (N'৳'),
    IsActive            BIT NOT NULL CONSTRAINT DF_Institution_IsActive DEFAULT (1),
    CreatedAt           DATETIME2 NOT NULL CONSTRAINT DF_Institution_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy           NVARCHAR(100) NULL,
    UpdatedAt           DATETIME2 NULL,
    UpdatedBy           NVARCHAR(100) NULL,
    IsDeleted           BIT NOT NULL CONSTRAINT DF_Institution_IsDeleted DEFAULT (0)
);
GO

/* ========================================================================
   TABLES
   Enum ints match Domain:
     UserRole: 1 SuperAdmin, 2 AccountsManager, 3 AdmissionOfficer, 4 Teacher
     Gender: 1 Male, 2 Female, 3 Other
     BloodGroup: 1 A+, 2 A-, 3 B+, 4 B-, 5 AB+, 6 AB-, 7 O+, 8 O-
     StudentStatus: 1 Active, 2 Inactive, 3 Graduated, 4 Transferred, 5 Suspended
     LedgerEntryType: 1 Invoice, 2 Payment, 3 Fine, 4 Waiver, 5 Adjustment, 6 Advance
     LedgerStatus: 1 Open, 2 Paid, 3 Partial, 4 Waived, 5 Cancelled
     PaymentMethod: 1 Cash, 2 Bkash, 3 Nagad, 4 BankTransfer
   ======================================================================== */

CREATE TABLE dbo.UserRoles
(
    UserRoleId  INT NOT NULL CONSTRAINT PK_UserRoles PRIMARY KEY,
    Name        NVARCHAR(50) NOT NULL,
    Code        NVARCHAR(30) NOT NULL,
    SortOrder   INT NOT NULL CONSTRAINT DF_UserRoles_SortOrder DEFAULT (0),
    IsActive    BIT NOT NULL CONSTRAINT DF_UserRoles_IsActive DEFAULT (1),
    CreatedAt   DATETIME2 NOT NULL CONSTRAINT DF_UserRoles_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy   NVARCHAR(100) NULL,
    UpdatedAt   DATETIME2 NULL,
    UpdatedBy   NVARCHAR(100) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_UserRoles_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_UserRoles_Code UNIQUE (Code)
);
GO

CREATE TABLE dbo.Users
(
    UserId          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Username        NVARCHAR(50)  NOT NULL,
    FullName        NVARCHAR(150) NOT NULL,
    PasswordHash    NVARCHAR(500) NOT NULL,
    Email           NVARCHAR(150) NULL,
    Phone           NVARCHAR(20)  NULL,
    Role            INT NOT NULL CONSTRAINT DF_Users_Role DEFAULT (4),
    IsActive        BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    LastLoginAt     DATETIME2 NULL,
    CreatedAt       DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       NVARCHAR(100) NULL,
    UpdatedAt       DATETIME2 NULL,
    UpdatedBy       NVARCHAR(100) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_Users_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT CK_Users_Role CHECK (Role BETWEEN 1 AND 4),
    CONSTRAINT FK_Users_UserRoles FOREIGN KEY (Role) REFERENCES dbo.UserRoles(UserRoleId)
);
GO

CREATE TABLE dbo.BloodGroups
(
    BloodGroupId INT NOT NULL CONSTRAINT PK_BloodGroups PRIMARY KEY,
    Name         NVARCHAR(50)  NOT NULL,
    Code         NVARCHAR(10)  NOT NULL,
    SortOrder    INT NOT NULL CONSTRAINT DF_BloodGroups_SortOrder DEFAULT (0),
    IsActive     BIT NOT NULL CONSTRAINT DF_BloodGroups_IsActive DEFAULT (1),
    CreatedAt    DATETIME2 NOT NULL CONSTRAINT DF_BloodGroups_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy    NVARCHAR(100) NULL,
    UpdatedAt    DATETIME2 NULL,
    UpdatedBy    NVARCHAR(100) NULL,
    IsDeleted    BIT NOT NULL CONSTRAINT DF_BloodGroups_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_BloodGroups_Code UNIQUE (Code)
);
GO

CREATE TABLE dbo.PaymentMethods
(
    PaymentMethodId INT NOT NULL CONSTRAINT PK_PaymentMethods PRIMARY KEY,
    Name            NVARCHAR(50) NOT NULL,
    SortOrder       INT NOT NULL CONSTRAINT DF_PaymentMethods_SortOrder DEFAULT (0),
    IsActive        BIT NOT NULL CONSTRAINT DF_PaymentMethods_IsActive DEFAULT (1),
    CreatedAt       DATETIME2 NOT NULL CONSTRAINT DF_PaymentMethods_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       NVARCHAR(100) NULL,
    UpdatedAt       DATETIME2 NULL,
    UpdatedBy       NVARCHAR(100) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_PaymentMethods_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_PaymentMethods_Name UNIQUE (Name)
);
GO

CREATE TABLE dbo.Classes
(
    ClassId     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Classes PRIMARY KEY,
    ClassName   NVARCHAR(100) NOT NULL,
    ClassCode   NVARCHAR(20) NULL,
    SortOrder   INT NOT NULL CONSTRAINT DF_Classes_SortOrder DEFAULT (0),
    IsActive    BIT NOT NULL CONSTRAINT DF_Classes_IsActive DEFAULT (1),
    CreatedAt   DATETIME2 NOT NULL CONSTRAINT DF_Classes_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy   NVARCHAR(100) NULL,
    UpdatedAt   DATETIME2 NULL,
    UpdatedBy   NVARCHAR(100) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_Classes_IsDeleted DEFAULT (0)
);
GO

CREATE TABLE dbo.Sections
(
    SectionId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Sections PRIMARY KEY,
    ClassId     INT NOT NULL,
    SectionName NVARCHAR(50) NOT NULL,
    Capacity    INT NOT NULL CONSTRAINT DF_Sections_Capacity DEFAULT (40),
    IsActive    BIT NOT NULL CONSTRAINT DF_Sections_IsActive DEFAULT (1),
    CreatedAt   DATETIME2 NOT NULL CONSTRAINT DF_Sections_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy   NVARCHAR(100) NULL,
    UpdatedAt   DATETIME2 NULL,
    UpdatedBy   NVARCHAR(100) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_Sections_IsDeleted DEFAULT (0),
    CONSTRAINT FK_Sections_Classes FOREIGN KEY (ClassId) REFERENCES dbo.Classes(ClassId),
    CONSTRAINT UQ_Sections_ClassName UNIQUE (ClassId, SectionName)
);
GO

CREATE TABLE dbo.Students
(
    StudentId           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Students PRIMARY KEY,
    RegistrationNo      NVARCHAR(30)  NOT NULL,
    FullName            NVARCHAR(150) NOT NULL,
    FatherName          NVARCHAR(150) NOT NULL,
    MotherName          NVARCHAR(150) NULL,
    BloodGroup          INT NOT NULL,
    Gender              INT NOT NULL,
    DateOfBirth         DATE NOT NULL,
    GuardianPhone       NVARCHAR(20) NOT NULL,
    PresentAddress      NVARCHAR(500) NULL,
    PermanentAddress    NVARCHAR(500) NULL,
    ClassId             INT NOT NULL,
    SectionId           INT NOT NULL,
    RollNumber          NVARCHAR(20) NOT NULL,
    MonthlyTuitionFee   DECIMAL(18,2) NOT NULL CONSTRAINT DF_Students_Fee DEFAULT (0),
    AdmissionDate       DATE NOT NULL CONSTRAINT DF_Students_AdmissionDate DEFAULT (CAST(GETDATE() AS DATE)),
    Status              INT NOT NULL CONSTRAINT DF_Students_Status DEFAULT (1),
    PhotoPath           NVARCHAR(260) NULL,
    AcademicSession     NVARCHAR(20) NOT NULL CONSTRAINT DF_Students_Session DEFAULT (N'2025-2026'),
    WantTransport       BIT NOT NULL CONSTRAINT DF_Students_WantTransport DEFAULT (0),
    TransportActive     BIT NOT NULL CONSTRAINT DF_Students_TransportActive DEFAULT (0),
    MonthlyTransportFee DECIMAL(18,2) NOT NULL CONSTRAINT DF_Students_TransportFee DEFAULT (0),
    TransportActivatedOn DATE NULL,
    CreatedAt           DATETIME2 NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy           NVARCHAR(100) NULL,
    UpdatedAt           DATETIME2 NULL,
    UpdatedBy           NVARCHAR(100) NULL,
    IsDeleted           BIT NOT NULL CONSTRAINT DF_Students_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_Students_RegistrationNo UNIQUE (RegistrationNo),
    CONSTRAINT FK_Students_Classes FOREIGN KEY (ClassId) REFERENCES dbo.Classes(ClassId),
    CONSTRAINT FK_Students_Sections FOREIGN KEY (SectionId) REFERENCES dbo.Sections(SectionId),
    CONSTRAINT CK_Students_Gender CHECK (Gender BETWEEN 1 AND 3),
    CONSTRAINT CK_Students_BloodGroup CHECK (BloodGroup BETWEEN 1 AND 8),
    CONSTRAINT CK_Students_Status CHECK (Status BETWEEN 1 AND 5),
    CONSTRAINT CK_Students_Fee CHECK (MonthlyTuitionFee >= 0),
    CONSTRAINT CK_Students_TransportFee CHECK (MonthlyTransportFee >= 0),
    CONSTRAINT FK_Students_BloodGroups FOREIGN KEY (BloodGroup) REFERENCES dbo.BloodGroups(BloodGroupId)
);
GO

CREATE INDEX IX_Students_Mobile ON dbo.Students(GuardianPhone) WHERE IsDeleted = 0;
CREATE INDEX IX_Students_ClassSection ON dbo.Students(ClassId, SectionId, RollNumber) WHERE IsDeleted = 0;
CREATE INDEX IX_Students_Status ON dbo.Students(Status) WHERE IsDeleted = 0;
CREATE INDEX IX_Students_Name ON dbo.Students(FullName) WHERE IsDeleted = 0;
GO

CREATE TABLE dbo.FeeCategories
(
    FeeCategoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FeeCategories PRIMARY KEY,
    CategoryName  NVARCHAR(100) NOT NULL,
    Description   NVARCHAR(250) NULL,
    IsRecurring   BIT NOT NULL CONSTRAINT DF_FeeCategories_IsRecurring DEFAULT (1),
    IsActive      BIT NOT NULL CONSTRAINT DF_FeeCategories_IsActive DEFAULT (1),
    CreatedAt     DATETIME2 NOT NULL CONSTRAINT DF_FeeCategories_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy     NVARCHAR(100) NULL,
    UpdatedAt     DATETIME2 NULL,
    UpdatedBy     NVARCHAR(100) NULL,
    IsDeleted     BIT NOT NULL CONSTRAINT DF_FeeCategories_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_FeeCategories_Name UNIQUE (CategoryName)
);
GO

CREATE TABLE dbo.StudentFeeStructures
(
    StructureId     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_StudentFeeStructures PRIMARY KEY,
    StudentId       INT NOT NULL,
    FeeCategoryId   INT NOT NULL,
    Amount          DECIMAL(18,2) NOT NULL,
    AcademicSession NVARCHAR(20) NOT NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_SFS_IsActive DEFAULT (1),
    CreatedAt       DATETIME2 NOT NULL CONSTRAINT DF_SFS_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       NVARCHAR(100) NULL,
    UpdatedAt       DATETIME2 NULL,
    UpdatedBy       NVARCHAR(100) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_SFS_IsDeleted DEFAULT (0),
    CONSTRAINT FK_SFS_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_SFS_FeeCategories FOREIGN KEY (FeeCategoryId) REFERENCES dbo.FeeCategories(FeeCategoryId),
    CONSTRAINT CK_SFS_Amount CHECK (Amount >= 0)
);
GO

CREATE INDEX IX_SFS_StudentSession ON dbo.StudentFeeStructures(StudentId, AcademicSession) WHERE IsDeleted = 0;
GO

CREATE TABLE dbo.StudentLedger
(
    LedgerId        BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_StudentLedger PRIMARY KEY,
    StudentId       INT NOT NULL,
    TransactionDate DATETIME2 NOT NULL,
    VoucherNo       NVARCHAR(50) NOT NULL,
    Particulars     NVARCHAR(500) NOT NULL,
    FeePeriod       NVARCHAR(100) NULL,
    DebitAmount     DECIMAL(18,2) NOT NULL CONSTRAINT DF_Ledger_Debit DEFAULT (0),
    CreditAmount    DECIMAL(18,2) NOT NULL CONSTRAINT DF_Ledger_Credit DEFAULT (0),
    EntryType       INT NOT NULL,
    Status          INT NOT NULL CONSTRAINT DF_Ledger_Status DEFAULT (1),
    ReferenceNo     NVARCHAR(100) NULL,
    CreatedByUserId INT NULL,
    CreatedAt       DATETIME2 NOT NULL CONSTRAINT DF_Ledger_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       NVARCHAR(100) NULL,
    UpdatedAt       DATETIME2 NULL,
    UpdatedBy       NVARCHAR(100) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_Ledger_IsDeleted DEFAULT (0),
    CONSTRAINT FK_Ledger_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT CK_Ledger_Amounts CHECK (DebitAmount >= 0 AND CreditAmount >= 0),
    CONSTRAINT CK_Ledger_EntryType CHECK (EntryType BETWEEN 1 AND 6),
    CONSTRAINT CK_Ledger_Status CHECK (Status BETWEEN 1 AND 5)
);
GO

CREATE INDEX IX_StudentLedger_StudentDate ON dbo.StudentLedger(StudentId, TransactionDate, LedgerId) WHERE IsDeleted = 0;
CREATE INDEX IX_StudentLedger_VoucherNo ON dbo.StudentLedger(VoucherNo) WHERE IsDeleted = 0;
GO

CREATE TABLE dbo.FeeInvoices
(
    InvoiceId     BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FeeInvoices PRIMARY KEY,
    StudentId     INT NOT NULL,
    InvoiceNo     NVARCHAR(50) NOT NULL,
    FeePeriod     NVARCHAR(100) NOT NULL,
    CategoryName  NVARCHAR(100) NOT NULL,
    Amount        DECIMAL(18,2) NOT NULL,
    FineAmount    DECIMAL(18,2) NOT NULL CONSTRAINT DF_FeeInvoices_Fine DEFAULT (0),
    WaiverAmount  DECIMAL(18,2) NOT NULL CONSTRAINT DF_FeeInvoices_Waiver DEFAULT (0),
    InvoiceDate   DATE NOT NULL,
    DueDate       DATE NULL,
    Status        INT NOT NULL,
    LedgerId      BIGINT NULL,
    CreatedAt     DATETIME2 NOT NULL CONSTRAINT DF_FeeInvoices_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy     NVARCHAR(100) NULL,
    UpdatedAt     DATETIME2 NULL,
    UpdatedBy     NVARCHAR(100) NULL,
    IsDeleted     BIT NOT NULL CONSTRAINT DF_FeeInvoices_IsDeleted DEFAULT (0),
    CONSTRAINT FK_FeeInvoices_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT UQ_FeeInvoices_InvoiceNo UNIQUE (InvoiceNo),
    CONSTRAINT CK_FeeInvoices_Amounts CHECK (Amount >= 0 AND FineAmount >= 0 AND WaiverAmount >= 0)
);
GO

CREATE INDEX IX_FeeInvoices_Student ON dbo.FeeInvoices(StudentId, InvoiceDate) WHERE IsDeleted = 0;
GO

CREATE TABLE dbo.FeePayments
(
    CollectionId      BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FeePayments PRIMARY KEY,
    StudentId         INT NOT NULL,
    ReceiptNo         NVARCHAR(50) NOT NULL,
    PaymentDate       DATETIME2 NOT NULL,
    AmountPaid        DECIMAL(18,2) NOT NULL,
    FineCollected     DECIMAL(18,2) NOT NULL CONSTRAINT DF_FeePayments_Fine DEFAULT (0),
    WaiverApplied     DECIMAL(18,2) NOT NULL CONSTRAINT DF_FeePayments_Waiver DEFAULT (0),
    PaymentMethod     INT NOT NULL,
    TransactionRef    NVARCHAR(100) NULL,
    FeePeriods        NVARCHAR(250) NULL,
    Remarks           NVARCHAR(500) NULL,
    LedgerId          BIGINT NULL,
    CollectedByUserId INT NOT NULL,
    IsPrinted         BIT NOT NULL CONSTRAINT DF_FeePayments_IsPrinted DEFAULT (0),
    CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_FeePayments_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy         NVARCHAR(100) NULL,
    UpdatedAt         DATETIME2 NULL,
    UpdatedBy         NVARCHAR(100) NULL,
    IsDeleted         BIT NOT NULL CONSTRAINT DF_FeePayments_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_FeePayments_ReceiptNo UNIQUE (ReceiptNo),
    CONSTRAINT FK_FeePayments_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT CK_FeePayments_Amounts CHECK (AmountPaid >= 0 AND FineCollected >= 0 AND WaiverApplied >= 0),
    CONSTRAINT CK_FeePayments_Method CHECK (PaymentMethod BETWEEN 1 AND 4)
);
GO

CREATE INDEX IX_FeePayments_StudentDate ON dbo.FeePayments(StudentId, PaymentDate) WHERE IsDeleted = 0;
CREATE INDEX IX_FeePayments_PaymentDate ON dbo.FeePayments(PaymentDate) WHERE IsDeleted = 0;
GO

CREATE TABLE dbo.ExamTerms
(
    ExamTermId      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExamTerms PRIMARY KEY,
    TermName        NVARCHAR(100) NOT NULL,
    AcademicSession NVARCHAR(20) NOT NULL,
    StartDate       DATE NOT NULL,
    EndDate         DATE NOT NULL,
    TimetableJson   NVARCHAR(MAX) NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_ExamTerms_IsActive DEFAULT (1),
    CreatedAt       DATETIME2 NOT NULL CONSTRAINT DF_ExamTerms_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       NVARCHAR(100) NULL,
    UpdatedAt       DATETIME2 NULL,
    UpdatedBy       NVARCHAR(100) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_ExamTerms_IsDeleted DEFAULT (0),
    CONSTRAINT CK_ExamTerms_Dates CHECK (EndDate >= StartDate)
);
GO

CREATE TABLE dbo.ExamClearances
(
    ClearanceId     BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExamClearances PRIMARY KEY,
    StudentId       INT NOT NULL,
    ExamTermId      INT NOT NULL,
    DuesAtClearance DECIMAL(18,2) NOT NULL CONSTRAINT DF_ExamClearances_Dues DEFAULT (0),
    IsCleared       BIT NOT NULL CONSTRAINT DF_ExamClearances_IsCleared DEFAULT (0),
    AdminOverride   BIT NOT NULL CONSTRAINT DF_ExamClearances_Override DEFAULT (0),
    OverrideReason  NVARCHAR(500) NULL,
    ClearedByUserId INT NULL,
    ClearedAt       DATETIME2 NULL,
    Remarks         NVARCHAR(500) NULL,
    CreatedAt       DATETIME2 NOT NULL CONSTRAINT DF_ExamClearances_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       NVARCHAR(100) NULL,
    UpdatedAt       DATETIME2 NULL,
    UpdatedBy       NVARCHAR(100) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_ExamClearances_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_ExamClearances_StudentTerm UNIQUE (StudentId, ExamTermId),
    CONSTRAINT FK_ExamClearances_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_ExamClearances_ExamTerms FOREIGN KEY (ExamTermId) REFERENCES dbo.ExamTerms(ExamTermId)
);
GO

CREATE TABLE dbo.AdmitCards
(
    AdmitCardId     BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AdmitCards PRIMARY KEY,
    StudentId       INT NOT NULL,
    ExamTermId      INT NOT NULL,
    AdmitCardNo     NVARCHAR(50) NOT NULL,
    BarcodeValue    NVARCHAR(100) NOT NULL,
    IssuedAt        DATETIME2 NOT NULL,
    IssuedByUserId  INT NOT NULL,
    IsPrinted       BIT NOT NULL CONSTRAINT DF_AdmitCards_IsPrinted DEFAULT (0),
    ClearanceId     BIGINT NULL,
    CreatedAt       DATETIME2 NOT NULL CONSTRAINT DF_AdmitCards_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       NVARCHAR(100) NULL,
    UpdatedAt       DATETIME2 NULL,
    UpdatedBy       NVARCHAR(100) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_AdmitCards_IsDeleted DEFAULT (0),
    CONSTRAINT UQ_AdmitCards_No UNIQUE (AdmitCardNo),
    CONSTRAINT FK_AdmitCards_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_AdmitCards_ExamTerms FOREIGN KEY (ExamTermId) REFERENCES dbo.ExamTerms(ExamTermId),
    CONSTRAINT FK_AdmitCards_Clearances FOREIGN KEY (ClearanceId) REFERENCES dbo.ExamClearances(ClearanceId)
);
GO

CREATE INDEX IX_AdmitCards_StudentTerm ON dbo.AdmitCards(StudentId, ExamTermId) WHERE IsDeleted = 0;
GO

/* Atomic counters used by number-generation stored procedures */
CREATE TABLE dbo.NumberSequences
(
    SequenceKey   NVARCHAR(40) NOT NULL CONSTRAINT PK_NumberSequences PRIMARY KEY,
    LastValue     INT NOT NULL CONSTRAINT DF_NumberSequences_LastValue DEFAULT (0),
    UpdatedAt     DATETIME2 NOT NULL CONSTRAINT DF_NumberSequences_UpdatedAt DEFAULT (SYSUTCDATETIME())
);
GO

/* ========================================================================
   VIEWS
   ======================================================================== */

CREATE VIEW dbo.vw_StudentLedgerDetailed
AS
SELECT
    l.LedgerId,
    l.StudentId,
    s.RegistrationNo,
    s.FullName,
    l.TransactionDate,
    l.VoucherNo,
    l.Particulars,
    l.FeePeriod,
    l.DebitAmount,
    l.CreditAmount,
    SUM(l.DebitAmount - l.CreditAmount) OVER (
        PARTITION BY l.StudentId
        ORDER BY l.TransactionDate, l.LedgerId
        ROWS UNBOUNDED PRECEDING
    ) AS RunningBalance,
    l.EntryType,
    l.Status,
    l.ReferenceNo,
    l.CreatedByUserId,
    l.CreatedAt
FROM dbo.StudentLedger l
INNER JOIN dbo.Students s ON s.StudentId = l.StudentId
WHERE l.IsDeleted = 0 AND s.IsDeleted = 0;
GO

CREATE VIEW dbo.vw_StudentBalanceSummary
AS
SELECT
    s.StudentId,
    s.RegistrationNo,
    s.FullName,
    s.GuardianPhone,
    c.ClassName,
    sec.SectionName,
    s.RollNumber,
    s.MonthlyTuitionFee,
    s.Status,
    s.AcademicSession,
    ISNULL(SUM(l.DebitAmount), 0) AS TotalDebit,
    ISNULL(SUM(l.CreditAmount), 0) AS TotalCredit,
    ISNULL(SUM(l.DebitAmount - l.CreditAmount), 0) AS NetDue
FROM dbo.Students s
INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
INNER JOIN dbo.Sections sec ON sec.SectionId = s.SectionId
LEFT JOIN dbo.StudentLedger l ON l.StudentId = s.StudentId AND l.IsDeleted = 0
WHERE s.IsDeleted = 0
GROUP BY
    s.StudentId, s.RegistrationNo, s.FullName, s.GuardianPhone,
    c.ClassName, sec.SectionName, s.RollNumber, s.MonthlyTuitionFee,
    s.Status, s.AcademicSession;
GO

CREATE VIEW dbo.vw_ActiveStudents
AS
SELECT
    s.StudentId,
    s.RegistrationNo,
    s.FullName,
    s.FatherName,
    s.MotherName,
    s.GuardianPhone,
    s.DateOfBirth,
    s.Gender,
    s.BloodGroup,
    c.ClassId,
    c.ClassName,
    sec.SectionId,
    sec.SectionName,
    s.RollNumber,
    s.MonthlyTuitionFee,
    s.AdmissionDate,
    s.AcademicSession,
    s.PhotoPath
FROM dbo.Students s
INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
INNER JOIN dbo.Sections sec ON sec.SectionId = s.SectionId
WHERE s.IsDeleted = 0 AND s.Status = 1;
GO

/* ========================================================================
   STORED PROCEDURES
   ======================================================================== */

CREATE PROCEDURE dbo.usp_GetNextRegistrationNo
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Year IS NULL
        SET @Year = YEAR(GETDATE());

    DECLARE @Key NVARCHAR(40) = N'REG-' + CAST(@Year AS NVARCHAR(4));
    DECLARE @Next INT;

    BEGIN TRAN;

    IF NOT EXISTS (SELECT 1 FROM dbo.NumberSequences WITH (UPDLOCK, HOLDLOCK) WHERE SequenceKey = @Key)
    BEGIN
        /* Seed from existing students so re-run stays continuous */
        DECLARE @MaxExisting INT = 0;
        SELECT @MaxExisting = ISNULL(MAX(
            TRY_CAST(RIGHT(RegistrationNo, 4) AS INT)), 0)
        FROM dbo.Students
        WHERE RegistrationNo LIKE @Key + N'-%'
          AND LEN(RegistrationNo) >= LEN(@Key) + 5;

        INSERT INTO dbo.NumberSequences (SequenceKey, LastValue)
        VALUES (@Key, @MaxExisting);
    END

    UPDATE dbo.NumberSequences
    SET LastValue = LastValue + 1,
        UpdatedAt = SYSUTCDATETIME()
    WHERE SequenceKey = @Key;

    SELECT @Next = LastValue FROM dbo.NumberSequences WHERE SequenceKey = @Key;

    COMMIT TRAN;

    SELECT
        @Key + N'-' + RIGHT(N'0000' + CAST(@Next AS NVARCHAR(10)), 4) AS RegistrationNo,
        @Next AS SequenceValue;
END
GO

CREATE PROCEDURE dbo.usp_GetNextReceiptNo
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Ym NVARCHAR(6) = CONVERT(CHAR(6), GETDATE(), 112); -- yyyyMM
    DECLARE @Key NVARCHAR(40) = N'RCP-' + @Ym;
    DECLARE @Next INT;

    BEGIN TRAN;

    IF NOT EXISTS (SELECT 1 FROM dbo.NumberSequences WITH (UPDLOCK, HOLDLOCK) WHERE SequenceKey = @Key)
    BEGIN
        DECLARE @MaxExisting INT = 0;
        SELECT @MaxExisting = ISNULL(MAX(
            TRY_CAST(RIGHT(ReceiptNo, 5) AS INT)), 0)
        FROM dbo.FeePayments
        WHERE ReceiptNo LIKE @Key + N'-%';

        INSERT INTO dbo.NumberSequences (SequenceKey, LastValue)
        VALUES (@Key, @MaxExisting);
    END

    UPDATE dbo.NumberSequences
    SET LastValue = LastValue + 1,
        UpdatedAt = SYSUTCDATETIME()
    WHERE SequenceKey = @Key;

    SELECT @Next = LastValue FROM dbo.NumberSequences WHERE SequenceKey = @Key;

    COMMIT TRAN;

    SELECT
        @Key + N'-' + RIGHT(N'00000' + CAST(@Next AS NVARCHAR(10)), 5) AS ReceiptNo,
        @Next AS SequenceValue;
END
GO

CREATE PROCEDURE dbo.usp_GetNextInvoiceNo
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Ym NVARCHAR(6) = CONVERT(CHAR(6), GETDATE(), 112);
    DECLARE @Key NVARCHAR(40) = N'INV-' + @Ym;
    DECLARE @Next INT;

    BEGIN TRAN;

    IF NOT EXISTS (SELECT 1 FROM dbo.NumberSequences WITH (UPDLOCK, HOLDLOCK) WHERE SequenceKey = @Key)
    BEGIN
        DECLARE @MaxExisting INT = 0;
        SELECT @MaxExisting = ISNULL(MAX(
            TRY_CAST(RIGHT(InvoiceNo, 5) AS INT)), 0)
        FROM dbo.FeeInvoices
        WHERE InvoiceNo LIKE @Key + N'-%';

        INSERT INTO dbo.NumberSequences (SequenceKey, LastValue)
        VALUES (@Key, @MaxExisting);
    END

    UPDATE dbo.NumberSequences
    SET LastValue = LastValue + 1,
        UpdatedAt = SYSUTCDATETIME()
    WHERE SequenceKey = @Key;

    SELECT @Next = LastValue FROM dbo.NumberSequences WHERE SequenceKey = @Key;

    COMMIT TRAN;

    SELECT
        @Key + N'-' + RIGHT(N'00000' + CAST(@Next AS NVARCHAR(10)), 5) AS InvoiceNo,
        @Next AS SequenceValue;
END
GO

CREATE PROCEDURE dbo.usp_GetNextAdmitCardNo
    @ExamTermId INT,
    @RegistrationNo NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ExamTermId IS NULL OR @RegistrationNo IS NULL OR LTRIM(RTRIM(@RegistrationNo)) = N''
    BEGIN
        RAISERROR(N'ExamTermId and RegistrationNo are required.', 16, 1);
        RETURN;
    END

    SELECT
        N'AC-' + RIGHT(N'00' + CAST(@ExamTermId AS NVARCHAR(10)), 2) + N'-' + @RegistrationNo AS AdmitCardNo,
        @RegistrationNo + N'|' + CAST(@ExamTermId AS NVARCHAR(10)) + N'|' + CONVERT(CHAR(8), GETDATE(), 112) AS BarcodeValue;
END
GO

CREATE PROCEDURE dbo.usp_GetStudentBalance
    @StudentId INT = NULL,
    @RegistrationNo NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @StudentId IS NULL AND @RegistrationNo IS NOT NULL
        SELECT @StudentId = StudentId FROM dbo.Students WHERE RegistrationNo = @RegistrationNo AND IsDeleted = 0;

    IF @StudentId IS NULL
    BEGIN
        RAISERROR(N'Student not found. Pass @StudentId or @RegistrationNo.', 16, 1);
        RETURN;
    END

    SELECT *
    FROM dbo.vw_StudentBalanceSummary
    WHERE StudentId = @StudentId;
END
GO

CREATE PROCEDURE dbo.usp_GetStudentLedger
    @StudentId INT = NULL,
    @RegistrationNo NVARCHAR(30) = NULL,
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @StudentId IS NULL AND @RegistrationNo IS NOT NULL
        SELECT @StudentId = StudentId FROM dbo.Students WHERE RegistrationNo = @RegistrationNo AND IsDeleted = 0;

    IF @StudentId IS NULL
    BEGIN
        RAISERROR(N'Student not found. Pass @StudentId or @RegistrationNo.', 16, 1);
        RETURN;
    END

    /* Header / balance */
    SELECT *
    FROM dbo.vw_StudentBalanceSummary
    WHERE StudentId = @StudentId;

    /* Detail lines with running balance */
    SELECT *
    FROM dbo.vw_StudentLedgerDetailed
    WHERE StudentId = @StudentId
      AND (@FromDate IS NULL OR CAST(TransactionDate AS DATE) >= @FromDate)
      AND (@ToDate IS NULL OR CAST(TransactionDate AS DATE) <= @ToDate)
    ORDER BY TransactionDate, LedgerId;
END
GO

CREATE PROCEDURE dbo.usp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    /* KPIs mirror frmDashboard cards: enrolled, clear, dues, collection, exam-eligible */

    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @MonthStart DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    DECLARE @Active INT = (SELECT COUNT(*) FROM dbo.Students WHERE IsDeleted = 0 AND Status = 1);
    DECLARE @Clear INT = (SELECT COUNT(*) FROM dbo.vw_StudentBalanceSummary WHERE Status = 1 AND NetDue <= 0);
    DECLARE @WithDues INT = (SELECT COUNT(*) FROM dbo.vw_StudentBalanceSummary WHERE Status = 1 AND NetDue > 0);
    DECLARE @Receivables DECIMAL(18,2) = (SELECT ISNULL(SUM(NetDue), 0) FROM dbo.vw_StudentBalanceSummary WHERE Status = 1 AND NetDue > 0);
    DECLARE @MonthlyTuition DECIMAL(18,2) = (SELECT ISNULL(SUM(MonthlyTuitionFee), 0) FROM dbo.Students WHERE IsDeleted = 0 AND Status = 1);
    DECLARE @Rate INT = CASE WHEN @Active > 0 THEN CAST(ROUND(@Clear * 100.0 / @Active, 0) AS INT) ELSE 0 END;

    SELECT
        @Active AS ActiveStudents,
        @Clear AS ClearStudents,
        @WithDues AS StudentsWithDues,
        @Receivables AS TotalReceivables,
        @Rate AS FeeCollectionRatePct,
        @MonthlyTuition AS MonthlyTuitionBook,
        (SELECT ISNULL(SUM(AmountPaid), 0)
         FROM dbo.FeePayments
         WHERE IsDeleted = 0 AND CAST(PaymentDate AS DATE) = @Today) AS CollectionsToday,
        (SELECT COUNT(*) FROM dbo.FeePayments
         WHERE IsDeleted = 0 AND CAST(PaymentDate AS DATE) = @Today) AS ReceiptsToday,
        (SELECT ISNULL(SUM(AmountPaid), 0)
         FROM dbo.FeePayments
         WHERE IsDeleted = 0 AND CAST(PaymentDate AS DATE) >= @MonthStart) AS CollectionsThisMonth,
        (SELECT COUNT(*) FROM dbo.ExamClearances WHERE IsDeleted = 0 AND IsCleared = 1) AS ClearedForExam,
        (SELECT COUNT(*) FROM dbo.AdmitCards WHERE IsDeleted = 0) AS AdmitCardsIssued,
        /* Category share bars on dashboard (invoice totals by design categories) */
        (SELECT ISNULL(SUM(Amount), 0) FROM dbo.FeeInvoices WHERE IsDeleted = 0 AND CategoryName = N'Tuition Fee') AS TuitionInvoiced,
        (SELECT ISNULL(SUM(Amount), 0) FROM dbo.FeeInvoices WHERE IsDeleted = 0 AND CategoryName = N'ICT & Computer Lab Fees') AS IctInvoiced,
        (SELECT ISNULL(SUM(Amount), 0) FROM dbo.FeeInvoices WHERE IsDeleted = 0 AND CategoryName LIKE N'Examination Fee%') AS ExamInvoiced;
END
GO

CREATE PROCEDURE dbo.usp_SearchStudents
    @Term NVARCHAR(100) = NULL,
    @ClassId INT = NULL,
    @SectionId INT = NULL,
    @Status INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Term = NULLIF(LTRIM(RTRIM(@Term)), N'');

    SELECT
        s.StudentId,
        s.RegistrationNo,
        s.FullName,
        s.FatherName,
        s.GuardianPhone,
        c.ClassName,
        sec.SectionName,
        s.RollNumber,
        s.MonthlyTuitionFee,
        s.Status,
        s.AcademicSession,
        ISNULL(bal.NetDue, 0) AS NetDue
    FROM dbo.Students s
    INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
    INNER JOIN dbo.Sections sec ON sec.SectionId = s.SectionId
    LEFT JOIN dbo.vw_StudentBalanceSummary bal ON bal.StudentId = s.StudentId
    WHERE s.IsDeleted = 0
      AND (@ClassId IS NULL OR s.ClassId = @ClassId)
      AND (@SectionId IS NULL OR s.SectionId = @SectionId)
      AND (@Status IS NULL OR s.Status = @Status)
      AND (
            @Term IS NULL
            OR s.RegistrationNo LIKE N'%' + @Term + N'%'
            OR s.FullName LIKE N'%' + @Term + N'%'
            OR s.GuardianPhone LIKE N'%' + @Term + N'%'
            OR s.FatherName LIKE N'%' + @Term + N'%'
            OR s.RollNumber LIKE N'%' + @Term + N'%'
          )
    ORDER BY c.SortOrder, sec.SectionName, s.RollNumber, s.FullName;
END
GO

CREATE PROCEDURE dbo.usp_GetOutstandingDues
    @MinDue DECIMAL(18,2) = 0.01,
    @ClassId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.vw_StudentBalanceSummary
    WHERE NetDue >= @MinDue
      AND Status = 1
      AND (@ClassId IS NULL OR ClassName IN (
            SELECT ClassName FROM dbo.Classes WHERE ClassId = @ClassId))
    ORDER BY NetDue DESC, FullName;
END
GO

CREATE PROCEDURE dbo.usp_GetClassSectionLookup
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.ClassId,
        c.ClassName,
        c.ClassCode,
        c.SortOrder,
        sec.SectionId,
        sec.SectionName,
        sec.Capacity,
        (SELECT COUNT(*) FROM dbo.Students st
         WHERE st.SectionId = sec.SectionId AND st.IsDeleted = 0 AND st.Status = 1) AS EnrolledCount
    FROM dbo.Classes c
    INNER JOIN dbo.Sections sec ON sec.ClassId = c.ClassId AND sec.IsDeleted = 0 AND sec.IsActive = 1
    WHERE c.IsDeleted = 0 AND c.IsActive = 1
    ORDER BY c.SortOrder, sec.SectionName;
END
GO

CREATE PROCEDURE dbo.usp_PostTuitionInvoice
    @StudentId INT,
    @FeePeriod NVARCHAR(100),
    @Amount DECIMAL(18,2),
    @CategoryName NVARCHAR(100) = N'Tuition Fee',
    @FineAmount DECIMAL(18,2) = 0,
    @UserId INT = NULL,
    @InvoiceNo NVARCHAR(50) OUTPUT,
    @LedgerId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Amount IS NULL OR @Amount < 0
    BEGIN
        RAISERROR(N'Amount must be >= 0.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentId = @StudentId AND IsDeleted = 0)
    BEGIN
        RAISERROR(N'Student not found.', 16, 1);
        RETURN;
    END

    BEGIN TRAN;

    /* Allocate invoice number inside the same transaction */
    DECLARE @Ym NVARCHAR(6) = CONVERT(CHAR(6), GETDATE(), 112);
    DECLARE @Key NVARCHAR(40) = N'INV-' + @Ym;
    DECLARE @Next INT;

    IF NOT EXISTS (SELECT 1 FROM dbo.NumberSequences WITH (UPDLOCK, HOLDLOCK) WHERE SequenceKey = @Key)
    BEGIN
        DECLARE @MaxExisting INT = 0;
        SELECT @MaxExisting = ISNULL(MAX(TRY_CAST(RIGHT(InvoiceNo, 5) AS INT)), 0)
        FROM dbo.FeeInvoices WHERE InvoiceNo LIKE @Key + N'-%';
        INSERT INTO dbo.NumberSequences (SequenceKey, LastValue) VALUES (@Key, @MaxExisting);
    END

    UPDATE dbo.NumberSequences SET LastValue = LastValue + 1, UpdatedAt = SYSUTCDATETIME()
    WHERE SequenceKey = @Key;
    SELECT @Next = LastValue FROM dbo.NumberSequences WHERE SequenceKey = @Key;
    SET @InvoiceNo = @Key + N'-' + RIGHT(N'00000' + CAST(@Next AS NVARCHAR(10)), 5);

    DECLARE @Voucher NVARCHAR(50) = N'INV-' + CONVERT(CHAR(8), GETDATE(), 112) + N'-' + RIGHT(N'0000' + CAST(@StudentId AS NVARCHAR(10)), 4);
    DECLARE @Particulars NVARCHAR(500) = @CategoryName + N' - ' + @FeePeriod;

    INSERT INTO dbo.StudentLedger
        (StudentId, TransactionDate, VoucherNo, Particulars, FeePeriod, DebitAmount, CreditAmount, EntryType, Status, CreatedByUserId, CreatedBy)
    VALUES
        (@StudentId, SYSUTCDATETIME(), @Voucher, @Particulars, @FeePeriod, @Amount + ISNULL(@FineAmount, 0), 0, 1, 1, @UserId, N'sp');

    SET @LedgerId = SCOPE_IDENTITY();

    INSERT INTO dbo.FeeInvoices
        (StudentId, InvoiceNo, FeePeriod, CategoryName, Amount, FineAmount, WaiverAmount, InvoiceDate, Status, LedgerId, CreatedBy)
    VALUES
        (@StudentId, @InvoiceNo, @FeePeriod, @CategoryName, @Amount, ISNULL(@FineAmount, 0), 0, CAST(GETDATE() AS DATE), 1, @LedgerId, N'sp');

    COMMIT TRAN;

    SELECT @InvoiceNo AS InvoiceNo, @LedgerId AS LedgerId;
END
GO

CREATE PROCEDURE dbo.usp_RecordFeePayment
    @StudentId INT,
    @AmountPaid DECIMAL(18,2),
    @PaymentMethod INT = 1,
    @FeePeriods NVARCHAR(250) = NULL,
    @FineCollected DECIMAL(18,2) = 0,
    @WaiverApplied DECIMAL(18,2) = 0,
    @TransactionRef NVARCHAR(100) = NULL,
    @Remarks NVARCHAR(500) = NULL,
    @CollectedByUserId INT,
    @ReceiptNo NVARCHAR(50) OUTPUT,
    @LedgerId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @AmountPaid IS NULL OR @AmountPaid <= 0
    BEGIN
        RAISERROR(N'AmountPaid must be > 0.', 16, 1);
        RETURN;
    END

    IF @PaymentMethod NOT BETWEEN 1 AND 4
    BEGIN
        RAISERROR(N'PaymentMethod must be 1-4 (Cash/Bkash/Nagad/BankTransfer).', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentId = @StudentId AND IsDeleted = 0)
    BEGIN
        RAISERROR(N'Student not found.', 16, 1);
        RETURN;
    END

    BEGIN TRAN;

    DECLARE @Ym NVARCHAR(6) = CONVERT(CHAR(6), GETDATE(), 112);
    DECLARE @Key NVARCHAR(40) = N'RCP-' + @Ym;
    DECLARE @Next INT;

    IF NOT EXISTS (SELECT 1 FROM dbo.NumberSequences WITH (UPDLOCK, HOLDLOCK) WHERE SequenceKey = @Key)
    BEGIN
        DECLARE @MaxExisting INT = 0;
        SELECT @MaxExisting = ISNULL(MAX(TRY_CAST(RIGHT(ReceiptNo, 5) AS INT)), 0)
        FROM dbo.FeePayments WHERE ReceiptNo LIKE @Key + N'-%';
        INSERT INTO dbo.NumberSequences (SequenceKey, LastValue) VALUES (@Key, @MaxExisting);
    END

    UPDATE dbo.NumberSequences SET LastValue = LastValue + 1, UpdatedAt = SYSUTCDATETIME()
    WHERE SequenceKey = @Key;
    SELECT @Next = LastValue FROM dbo.NumberSequences WHERE SequenceKey = @Key;
    SET @ReceiptNo = @Key + N'-' + RIGHT(N'00000' + CAST(@Next AS NVARCHAR(10)), 5);

    DECLARE @MethodName NVARCHAR(30) =
        CASE @PaymentMethod
            WHEN 1 THEN N'Cash'
            WHEN 2 THEN N'bKash'
            WHEN 3 THEN N'Nagad'
            WHEN 4 THEN N'Bank Transfer'
            ELSE N'Other'
        END;

    DECLARE @Particulars NVARCHAR(500) =
        N'Fee Collection via ' + @MethodName +
        CASE WHEN @FeePeriods IS NULL OR @FeePeriods = N'' THEN N'' ELSE N' (' + @FeePeriods + N')' END;

    INSERT INTO dbo.StudentLedger
        (StudentId, TransactionDate, VoucherNo, Particulars, FeePeriod, DebitAmount, CreditAmount, EntryType, Status, ReferenceNo, CreatedByUserId, CreatedBy)
    VALUES
        (@StudentId, SYSUTCDATETIME(), @ReceiptNo, @Particulars, @FeePeriods, 0, @AmountPaid, 2, 2, @TransactionRef, @CollectedByUserId, N'sp');

    SET @LedgerId = SCOPE_IDENTITY();

    IF ISNULL(@WaiverApplied, 0) > 0
    BEGIN
        INSERT INTO dbo.StudentLedger
            (StudentId, TransactionDate, VoucherNo, Particulars, FeePeriod, DebitAmount, CreditAmount, EntryType, Status, CreatedByUserId, CreatedBy)
        VALUES
            (@StudentId, SYSUTCDATETIME(), N'WVR-' + @ReceiptNo, N'Waiver / Discount', @FeePeriods, 0, @WaiverApplied, 4, 4, @CollectedByUserId, N'sp');
    END

    INSERT INTO dbo.FeePayments
        (StudentId, ReceiptNo, PaymentDate, AmountPaid, FineCollected, WaiverApplied, PaymentMethod,
         TransactionRef, FeePeriods, Remarks, LedgerId, CollectedByUserId, CreatedBy)
    VALUES
        (@StudentId, @ReceiptNo, SYSUTCDATETIME(), @AmountPaid, ISNULL(@FineCollected, 0), ISNULL(@WaiverApplied, 0),
         @PaymentMethod, @TransactionRef, @FeePeriods, @Remarks, @LedgerId, @CollectedByUserId, N'sp');

    COMMIT TRAN;

    SELECT @ReceiptNo AS ReceiptNo, @LedgerId AS LedgerId;
END
GO

/* ========================================================================
   SEED DATA
   Password hashes are placeholders. DatabaseBootstrapper re-hashes on app
   start so Admin@123 / Accounts@123 / Admission@123 / Teacher@123 work.
   ======================================================================== */

INSERT INTO dbo.UserRoles (UserRoleId, Name, Code, SortOrder, CreatedBy) VALUES
(1, N'Super Admin',        N'SuperAdmin',        1, N'seed'),
(2, N'Accounts Manager',   N'AccountsManager',   2, N'seed'),
(3, N'Admission Officer',  N'AdmissionOfficer',  3, N'seed'),
(4, N'Teacher',            N'Teacher',           4, N'seed');
GO

INSERT INTO dbo.Users (Username, FullName, PasswordHash, Email, Role, IsActive, CreatedBy)
VALUES
(N'admin', N'System Super Admin',
 N'SHA256$PLACEHOLDER$REPLACE_ON_BOOTSTRAP',
 N'admin@school.edu', 1, 1, N'seed'),
(N'accounts', N'Accounts Manager',
 N'SHA256$PLACEHOLDER$REPLACE_ON_BOOTSTRAP',
 N'accounts@school.edu', 2, 1, N'seed'),
(N'admission', N'Admission Officer',
 N'SHA256$PLACEHOLDER$REPLACE_ON_BOOTSTRAP',
 N'admission@school.edu', 3, 1, N'seed'),
(N'teacher', N'Class Teacher',
 N'SHA256$PLACEHOLDER$REPLACE_ON_BOOTSTRAP',
 N'teacher@school.edu', 4, 1, N'seed');
GO

INSERT INTO dbo.BloodGroups (BloodGroupId, Name, Code, SortOrder, CreatedBy) VALUES
(1, N'A Positive',  N'A+',  1, N'seed'),
(2, N'A Negative',  N'A-',  2, N'seed'),
(3, N'B Positive',  N'B+',  3, N'seed'),
(4, N'B Negative',  N'B-',  4, N'seed'),
(5, N'AB Positive', N'AB+', 5, N'seed'),
(6, N'AB Negative', N'AB-', 6, N'seed'),
(7, N'O Positive',  N'O+',  7, N'seed'),
(8, N'O Negative',  N'O-',  8, N'seed');
GO

INSERT INTO dbo.PaymentMethods (PaymentMethodId, Name, SortOrder, CreatedBy) VALUES
(1, N'Cash', 1, N'seed'),
(2, N'bKash', 2, N'seed'),
(3, N'Nagad', 3, N'seed'),
(4, N'Bank Transfer', 4, N'seed');
GO

INSERT INTO dbo.Classes (ClassName, ClassCode, SortOrder, CreatedBy) VALUES
(N'Class 6', N'C6', 6, N'seed'),
(N'Class 7', N'C7', 7, N'seed'),
(N'Class 8', N'C8', 8, N'seed'),
(N'Class 9', N'C9', 9, N'seed'),
(N'Class 10', N'C10', 10, N'seed');
GO

INSERT INTO dbo.Sections (ClassId, SectionName, Capacity, CreatedBy)
SELECT ClassId, N'A', 40, N'seed' FROM dbo.Classes
UNION ALL
SELECT ClassId, N'B', 40, N'seed' FROM dbo.Classes;
GO

/* Fee categories — names match Fee Collection Counter designer (+ dashboard ICT bar) */
INSERT INTO dbo.FeeCategories (CategoryName, Description, IsRecurring, CreatedBy)
VALUES
(N'Tuition Fee', N'Monthly tuition (Tuition Fee Months matrix)', 1, N'seed'),
(N'Registration Fee', N'Optional fee on Fee Collection Counter', 0, N'seed'),
(N'New Admission / Re-admission', N'Optional fee on Fee Collection Counter', 0, N'seed'),
(N'Monthly Transport Fee', N'Optional fee on Fee Collection Counter', 1, N'seed'),
(N'Examination Fee (Term / Annual)', N'Optional fee / dashboard term exam bar', 0, N'seed'),
(N'ICT & Computer Lab Fees', N'Dashboard revenue bar category', 1, N'seed'),
(N'Transcript / Testimonial / Certificate', N'Optional fee on Fee Collection Counter', 0, N'seed'),
(N'Transfer Certificate / Letter', N'Optional fee on Fee Collection Counter', 0, N'seed'),
(N'Hostel Food Charges', N'Optional fee on Fee Collection Counter', 1, N'seed'),
(N'Miscellaneous', N'Optional fee on Fee Collection Counter', 0, N'seed');
GO

INSERT INTO dbo.InstitutionSettings
    (InstitutionName, CampusName, AcademicSession, AddressLine, Phone, Email, CurrencySymbol, CreatedBy)
VALUES
(N'Fouzdarhat Safiria Dakhil Madrasah',
 N'Fouzdarhat, Chittagong, Bangladesh, 4317',
 N'2025-2026',
 N'Fouzdarhat, Chittagong, Bangladesh, 4317',
 N'',
 N'',
 N'৳',
 N'seed');
GO

INSERT INTO dbo.ExamTerms (TermName, AcademicSession, StartDate, EndDate, TimetableJson, IsActive, CreatedBy)
VALUES
(N'Mid-Term 2026', N'2025-2026', '2026-03-01', '2026-03-15',
 N'[{"subject":"Bangla","date":"2026-03-01","time":"10:00 AM"},{"subject":"English","date":"2026-03-03","time":"10:00 AM"},{"subject":"Mathematics","date":"2026-03-05","time":"10:00 AM"},{"subject":"Science","date":"2026-03-07","time":"10:00 AM"}]', 1, N'seed'),
(N'Final Exam 2026', N'2025-2026', '2026-06-01', '2026-06-20',
 N'[{"subject":"Bangla","date":"2026-06-01","time":"09:30 AM"},{"subject":"English","date":"2026-06-03","time":"09:30 AM"},{"subject":"Mathematics","date":"2026-06-05","time":"09:30 AM"}]', 1, N'seed');
GO

DECLARE @C6 INT = (SELECT ClassId FROM dbo.Classes WHERE ClassCode = N'C6');
DECLARE @C7 INT = (SELECT ClassId FROM dbo.Classes WHERE ClassCode = N'C7');
DECLARE @C8 INT = (SELECT ClassId FROM dbo.Classes WHERE ClassCode = N'C8');
DECLARE @S6A INT = (SELECT SectionId FROM dbo.Sections WHERE ClassId = @C6 AND SectionName = N'A');
DECLARE @S7A INT = (SELECT SectionId FROM dbo.Sections WHERE ClassId = @C7 AND SectionName = N'A');
DECLARE @S8A INT = (SELECT SectionId FROM dbo.Sections WHERE ClassId = @C8 AND SectionName = N'A');
DECLARE @S6B INT = (SELECT SectionId FROM dbo.Sections WHERE ClassId = @C6 AND SectionName = N'B');
DECLARE @S7B INT = (SELECT SectionId FROM dbo.Sections WHERE ClassId = @C7 AND SectionName = N'B');

INSERT INTO dbo.Students
(RegistrationNo, FullName, FatherName, MotherName, BloodGroup, Gender, DateOfBirth, GuardianPhone,
 PresentAddress, PermanentAddress, ClassId, SectionId, RollNumber, MonthlyTuitionFee, AdmissionDate, Status, AcademicSession, CreatedBy)
VALUES
(N'REG-2026-0001', N'Ayesha Rahman', N'Md. Karim Rahman', N'Nasrin Akter', 7, 2, '2014-05-12', N'01711000001',
 N'12 Green Road, Dhaka', N'12 Green Road, Dhaka', @C6, @S6A, N'01', 2500.00, '2026-01-05', 1, N'2025-2026', N'seed'),
(N'REG-2026-0002', N'Rahim Uddin', N'Abdul Jalil', N'Salma Begum', 1, 1, '2013-08-21', N'01711000002',
 N'45 Mirpur-10, Dhaka', N'Village: Savar, Dhaka', @C7, @S7A, N'05', 2800.00, '2026-01-06', 1, N'2025-2026', N'seed'),
(N'REG-2026-0003', N'Nusrat Jahan', N'Syed Hasan', N'Farhana Syed', 3, 2, '2012-11-03', N'01711000003',
 N'88 Dhanmondi, Dhaka', N'88 Dhanmondi, Dhaka', @C8, @S8A, N'12', 3200.00, '2026-01-07', 1, N'2025-2026', N'seed'),
(N'REG-2026-0004', N'Tanvir Ahmed', N'Imran Ahmed', N'Laila Ahmed', 5, 1, '2014-02-18', N'01711000004',
 N'3 Uttara Sector 7', N'3 Uttara Sector 7', @C6, @S6B, N'08', 2500.00, '2026-01-08', 1, N'2025-2026', N'seed'),
(N'REG-2026-0005', N'Meherin Islam', N'Nazrul Islam', N'Shahana Islam', 7, 2, '2013-01-30', N'01711000005',
 N'22 Banani, Dhaka', N'Comilla Sadar', @C7, @S7B, N'03', 2800.00, '2026-01-09', 1, N'2025-2026', N'seed');
GO

DECLARE @S1 INT = (SELECT StudentId FROM dbo.Students WHERE RegistrationNo = N'REG-2026-0001');
DECLARE @S2 INT = (SELECT StudentId FROM dbo.Students WHERE RegistrationNo = N'REG-2026-0002');
DECLARE @S3 INT = (SELECT StudentId FROM dbo.Students WHERE RegistrationNo = N'REG-2026-0003');
DECLARE @S4 INT = (SELECT StudentId FROM dbo.Students WHERE RegistrationNo = N'REG-2026-0004');
DECLARE @S5 INT = (SELECT StudentId FROM dbo.Students WHERE RegistrationNo = N'REG-2026-0005');
DECLARE @Admin INT = (SELECT TOP 1 UserId FROM dbo.Users WHERE Username = N'admin');
DECLARE @TuitionId INT = (SELECT FeeCategoryId FROM dbo.FeeCategories WHERE CategoryName = N'Tuition Fee');

INSERT INTO dbo.StudentFeeStructures (StudentId, FeeCategoryId, Amount, AcademicSession, CreatedBy)
SELECT StudentId, @TuitionId, MonthlyTuitionFee, AcademicSession, N'seed'
FROM dbo.Students;

INSERT INTO dbo.StudentLedger (StudentId, TransactionDate, VoucherNo, Particulars, FeePeriod, DebitAmount, CreditAmount, EntryType, Status, CreatedByUserId, CreatedBy)
VALUES
(@S1, '2026-01-05', N'INV-20260105-0001', N'Tuition Fee - Jan-2026', N'Jan-2026', 2500, 0, 1, 1, @Admin, N'seed'),
(@S1, '2026-01-10', N'RCPT-20260110-0001', N'Fee Collection via Cash (Jan-2026)', N'Jan-2026', 0, 2500, 2, 2, @Admin, N'seed'),
(@S1, '2026-02-01', N'INV-20260201-0001', N'Tuition Fee - Feb-2026', N'Feb-2026', 2500, 0, 1, 1, @Admin, N'seed'),
(@S2, '2026-01-06', N'INV-20260106-0002', N'Tuition Fee - Jan-2026', N'Jan-2026', 2800, 0, 1, 1, @Admin, N'seed'),
(@S2, '2026-01-15', N'RCPT-20260115-0002', N'Fee Collection via bKash (Jan-2026)', N'Jan-2026', 0, 2000, 2, 3, @Admin, N'seed'),
(@S3, '2026-01-07', N'INV-20260107-0003', N'Tuition Fee - Jan-2026', N'Jan-2026', 3200, 0, 1, 1, @Admin, N'seed'),
(@S3, '2026-01-20', N'RCPT-20260120-0003', N'Fee Collection via Nagad (Jan-2026)', N'Jan-2026', 0, 3200, 2, 2, @Admin, N'seed'),
(@S3, '2026-02-01', N'INV-20260201-0003', N'Tuition Fee - Feb-2026', N'Feb-2026', 3200, 0, 1, 1, @Admin, N'seed'),
(@S4, '2026-01-08', N'INV-20260108-0004', N'Tuition Fee - Jan-2026', N'Jan-2026', 2500, 0, 1, 1, @Admin, N'seed'),
(@S5, '2026-01-09', N'INV-20260109-0005', N'Tuition Fee - Jan-2026', N'Jan-2026', 2800, 0, 1, 1, @Admin, N'seed'),
(@S5, '2026-01-12', N'RCPT-20260112-0005', N'Fee Collection via Bank Transfer (Jan-2026)', N'Jan-2026', 0, 2800, 2, 2, @Admin, N'seed'),
(@S5, '2026-02-01', N'INV-20260201-0005', N'Tuition Fee - Feb-2026', N'Feb-2026', 2800, 0, 1, 1, @Admin, N'seed'),
(@S5, '2026-02-05', N'WVR-20260205-0005', N'Waiver / Discount (Feb-2026)', N'Feb-2026', 0, 300, 4, 4, @Admin, N'seed');

INSERT INTO dbo.FeeInvoices (StudentId, InvoiceNo, FeePeriod, CategoryName, Amount, InvoiceDate, Status, CreatedBy)
VALUES
(@S1, N'INV-202601-00001', N'Jan-2026', N'Tuition Fee', 2500, '2026-01-05', 2, N'seed'),
(@S1, N'INV-202602-00001', N'Feb-2026', N'Tuition Fee', 2500, '2026-02-01', 1, N'seed'),
(@S2, N'INV-202601-00002', N'Jan-2026', N'Tuition Fee', 2800, '2026-01-06', 3, N'seed'),
(@S3, N'INV-202601-00003', N'Jan-2026', N'Tuition Fee', 3200, '2026-01-07', 2, N'seed'),
(@S3, N'INV-202602-00003', N'Feb-2026', N'Tuition Fee', 3200, '2026-02-01', 1, N'seed'),
(@S4, N'INV-202601-00004', N'Jan-2026', N'Tuition Fee', 2500, '2026-01-08', 1, N'seed'),
(@S5, N'INV-202601-00005', N'Jan-2026', N'Tuition Fee', 2800, '2026-01-09', 2, N'seed'),
(@S5, N'INV-202602-00005', N'Feb-2026', N'Tuition Fee', 2800, '2026-02-01', 3, N'seed');

INSERT INTO dbo.FeePayments (StudentId, ReceiptNo, PaymentDate, AmountPaid, PaymentMethod, FeePeriods, CollectedByUserId, CreatedBy)
VALUES
(@S1, N'RCP-202601-00001', '2026-01-10', 2500, 1, N'Jan-2026', @Admin, N'seed'),
(@S2, N'RCP-202601-00002', '2026-01-15', 2000, 2, N'Jan-2026', @Admin, N'seed'),
(@S3, N'RCP-202601-00003', '2026-01-20', 3200, 3, N'Jan-2026', @Admin, N'seed'),
(@S5, N'RCP-202601-00004', '2026-01-12', 2800, 4, N'Jan-2026', @Admin, N'seed');

/* Align number sequences with seeded documents */
MERGE dbo.NumberSequences AS t
USING (VALUES
    (N'REG-2026', 5),
    (N'RCP-202601', 4),
    (N'INV-202601', 5),
    (N'INV-202602', 5)
) AS s (SequenceKey, LastValue)
ON t.SequenceKey = s.SequenceKey
WHEN MATCHED THEN UPDATE SET LastValue = s.LastValue, UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT (SequenceKey, LastValue) VALUES (s.SequenceKey, s.LastValue);
GO

PRINT N'';
PRINT N'================================================================';
PRINT N' StudentManagementSDB ready.';
PRINT N' Tables, views, stored procedures, and demo seed applied.';
PRINT N' Login after first app start: admin / Admin@123';
PRINT N'================================================================';
GO
