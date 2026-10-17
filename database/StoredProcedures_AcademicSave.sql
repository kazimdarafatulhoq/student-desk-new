/*
================================================================================
 StoredProcedures_AcademicSave.sql
 Lookups + Class / Section CRUD (SYSTEM menu)
================================================================================
*/

USE StudentManagementSDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.usp_GetBloodGroups', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetBloodGroups;
IF OBJECT_ID(N'dbo.usp_GetPaymentMethods', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetPaymentMethods;
IF OBJECT_ID(N'dbo.usp_GetUserRoles', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetUserRoles;
IF OBJECT_ID(N'dbo.usp_GetClassesAdmin', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetClassesAdmin;
IF OBJECT_ID(N'dbo.usp_SaveClass', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_SaveClass;
IF OBJECT_ID(N'dbo.usp_GetSectionsAdmin', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetSectionsAdmin;
IF OBJECT_ID(N'dbo.usp_SaveSection', N'P') IS NOT NULL DROP PROCEDURE dbo.usp_SaveSection;
GO

CREATE PROCEDURE dbo.usp_GetBloodGroups
AS
BEGIN
    SET NOCOUNT ON;
    SELECT BloodGroupId, Name, Code, SortOrder, IsActive
    FROM dbo.BloodGroups
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY SortOrder, BloodGroupId;
END
GO

CREATE PROCEDURE dbo.usp_GetPaymentMethods
AS
BEGIN
    SET NOCOUNT ON;
    SELECT PaymentMethodId, Name, SortOrder, IsActive
    FROM dbo.PaymentMethods
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY SortOrder, PaymentMethodId;
END
GO

CREATE PROCEDURE dbo.usp_GetUserRoles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserRoleId, Name, Code, SortOrder, IsActive
    FROM dbo.UserRoles
    WHERE IsDeleted = 0 AND IsActive = 1
    ORDER BY SortOrder, UserRoleId;
END
GO

CREATE PROCEDURE dbo.usp_GetClassesAdmin
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ClassId, ClassName, ClassCode, SortOrder, IsActive
    FROM dbo.Classes
    WHERE IsDeleted = 0
    ORDER BY SortOrder, ClassName;
END
GO

CREATE PROCEDURE dbo.usp_SaveClass
    @ClassId    INT = 0,
    @ClassName  NVARCHAR(100),
    @ClassCode  NVARCHAR(20) = NULL,
    @SortOrder  INT = 0,
    @IsActive   BIT = 1,
    @UpdatedBy  NVARCHAR(100) = NULL,
    @OutClassId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ClassName IS NULL OR LTRIM(RTRIM(@ClassName)) = N''
    BEGIN RAISERROR(N'Class name is required.', 16, 1); RETURN; END

    SET @ClassName = LTRIM(RTRIM(@ClassName));
    SET @ClassCode = NULLIF(LTRIM(RTRIM(@ClassCode)), N'');

    IF ISNULL(@ClassId, 0) <= 0
    BEGIN
        INSERT INTO dbo.Classes (ClassName, ClassCode, SortOrder, IsActive, CreatedBy)
        VALUES (@ClassName, @ClassCode, ISNULL(@SortOrder, 0), ISNULL(@IsActive, 1), @UpdatedBy);
        SET @OutClassId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.Classes WHERE ClassId = @ClassId AND IsDeleted = 0)
        BEGIN RAISERROR(N'Class not found.', 16, 1); RETURN; END

        UPDATE dbo.Classes
        SET ClassName = @ClassName,
            ClassCode = @ClassCode,
            SortOrder = ISNULL(@SortOrder, 0),
            IsActive = ISNULL(@IsActive, 1),
            UpdatedAt = SYSUTCDATETIME(),
            UpdatedBy = @UpdatedBy
        WHERE ClassId = @ClassId AND IsDeleted = 0;

        SET @OutClassId = @ClassId;
    END

    SELECT ClassId, ClassName, ClassCode, SortOrder, IsActive
    FROM dbo.Classes WHERE ClassId = @OutClassId;
END
GO

CREATE PROCEDURE dbo.usp_GetSectionsAdmin
    @ClassId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.SectionId, s.ClassId, c.ClassName, s.SectionName, s.Capacity, s.IsActive
    FROM dbo.Sections s
    INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
    WHERE s.IsDeleted = 0
      AND (@ClassId IS NULL OR @ClassId <= 0 OR s.ClassId = @ClassId)
    ORDER BY c.SortOrder, c.ClassName, s.SectionName;
END
GO

CREATE PROCEDURE dbo.usp_SaveSection
    @SectionId   INT = 0,
    @ClassId     INT,
    @SectionName NVARCHAR(50),
    @Capacity    INT = 40,
    @IsActive    BIT = 1,
    @UpdatedBy   NVARCHAR(100) = NULL,
    @OutSectionId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ClassId IS NULL OR @ClassId <= 0
    BEGIN RAISERROR(N'Class is required.', 16, 1); RETURN; END
    IF @SectionName IS NULL OR LTRIM(RTRIM(@SectionName)) = N''
    BEGIN RAISERROR(N'Section name is required.', 16, 1); RETURN; END
    IF NOT EXISTS (SELECT 1 FROM dbo.Classes WHERE ClassId = @ClassId AND IsDeleted = 0)
    BEGIN RAISERROR(N'Class not found.', 16, 1); RETURN; END

    SET @SectionName = LTRIM(RTRIM(@SectionName));
    IF @Capacity IS NULL OR @Capacity < 0 SET @Capacity = 40;

    IF ISNULL(@SectionId, 0) <= 0
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Sections WHERE ClassId = @ClassId AND SectionName = @SectionName AND IsDeleted = 0)
        BEGIN RAISERROR(N'Section already exists for this class.', 16, 1); RETURN; END

        INSERT INTO dbo.Sections (ClassId, SectionName, Capacity, IsActive, CreatedBy)
        VALUES (@ClassId, @SectionName, @Capacity, ISNULL(@IsActive, 1), @UpdatedBy);
        SET @OutSectionId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.Sections WHERE SectionId = @SectionId AND IsDeleted = 0)
        BEGIN RAISERROR(N'Section not found.', 16, 1); RETURN; END

        IF EXISTS (
            SELECT 1 FROM dbo.Sections
            WHERE ClassId = @ClassId AND SectionName = @SectionName AND IsDeleted = 0 AND SectionId <> @SectionId)
        BEGIN RAISERROR(N'Section already exists for this class.', 16, 1); RETURN; END

        UPDATE dbo.Sections
        SET ClassId = @ClassId,
            SectionName = @SectionName,
            Capacity = @Capacity,
            IsActive = ISNULL(@IsActive, 1),
            UpdatedAt = SYSUTCDATETIME(),
            UpdatedBy = @UpdatedBy
        WHERE SectionId = @SectionId AND IsDeleted = 0;

        SET @OutSectionId = @SectionId;
    END

    SELECT s.SectionId, s.ClassId, c.ClassName, s.SectionName, s.Capacity, s.IsActive
    FROM dbo.Sections s
    INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
    WHERE s.SectionId = @OutSectionId;
END
GO

PRINT N'Academic / lookup stored procedures ready.';
GO
