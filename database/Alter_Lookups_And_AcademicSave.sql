/*
================================================================================
 Alter_Lookups_And_AcademicSave.sql
 - BloodGroups / PaymentMethods lookup tables (dropdown sources)
 - Class / Section save & list stored procedures

 Run against StudentManagementSDB (existing DB). Fresh installs also get these
 from StudentManagementSDB.sql + StoredProcedures_AcademicSave.sql.
================================================================================
*/

USE StudentManagementSDB;
GO

/* ---------- BloodGroups ---------- */
IF OBJECT_ID(N'dbo.BloodGroups', N'U') IS NULL
BEGIN
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
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.BloodGroups)
BEGIN
    INSERT INTO dbo.BloodGroups (BloodGroupId, Name, Code, SortOrder, CreatedBy) VALUES
    (1, N'A Positive',  N'A+',  1, N'seed'),
    (2, N'A Negative',  N'A-',  2, N'seed'),
    (3, N'B Positive',  N'B+',  3, N'seed'),
    (4, N'B Negative',  N'B-',  4, N'seed'),
    (5, N'AB Positive', N'AB+', 5, N'seed'),
    (6, N'AB Negative', N'AB-', 6, N'seed'),
    (7, N'O Positive',  N'O+',  7, N'seed'),
    (8, N'O Negative',  N'O-',  8, N'seed');
END
GO

/* ---------- PaymentMethods ---------- */
IF OBJECT_ID(N'dbo.PaymentMethods', N'U') IS NULL
BEGIN
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
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PaymentMethods)
BEGIN
    INSERT INTO dbo.PaymentMethods (PaymentMethodId, Name, SortOrder, CreatedBy) VALUES
    (1, N'Cash', 1, N'seed'),
    (2, N'bKash', 2, N'seed'),
    (3, N'Nagad', 3, N'seed'),
    (4, N'Bank Transfer', 4, N'seed');
END
GO

/* Optional FKs (ignore if existing orphan ints) */
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Students_BloodGroups')
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.Students WITH NOCHECK
        ADD CONSTRAINT FK_Students_BloodGroups
            FOREIGN KEY (BloodGroup) REFERENCES dbo.BloodGroups(BloodGroupId);
    END TRY BEGIN CATCH END CATCH
END
GO

/* ---------- UserRoles ---------- */
IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NULL
BEGIN
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
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.UserRoles)
BEGIN
    INSERT INTO dbo.UserRoles (UserRoleId, Name, Code, SortOrder, CreatedBy) VALUES
    (1, N'Super Admin',        N'SuperAdmin',        1, N'seed'),
    (2, N'Accounts Manager',   N'AccountsManager',   2, N'seed'),
    (3, N'Admission Officer',  N'AdmissionOfficer',  3, N'seed'),
    (4, N'Teacher',            N'Teacher',           4, N'seed');
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Users_UserRoles')
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.Users WITH NOCHECK
        ADD CONSTRAINT FK_Users_UserRoles
            FOREIGN KEY (Role) REFERENCES dbo.UserRoles(UserRoleId);
    END TRY BEGIN CATCH END CATCH
END
GO

PRINT N'BloodGroups / PaymentMethods / UserRoles ready.';
GO
