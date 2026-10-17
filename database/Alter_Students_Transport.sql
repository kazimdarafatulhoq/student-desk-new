/*
================================================================================
 Alter_Students_Transport.sql
 Adds school transport (Gari) waitlist + activation columns to existing DBs.

 Run against StudentManagementSDB, then re-run StoredProcedures_StudentSave.sql.
================================================================================
*/

USE StudentManagementSDB;
GO

IF COL_LENGTH(N'dbo.Students', N'WantTransport') IS NULL
    ALTER TABLE dbo.Students ADD WantTransport BIT NOT NULL
        CONSTRAINT DF_Students_WantTransport DEFAULT (0);
GO

IF COL_LENGTH(N'dbo.Students', N'TransportActive') IS NULL
    ALTER TABLE dbo.Students ADD TransportActive BIT NOT NULL
        CONSTRAINT DF_Students_TransportActive DEFAULT (0);
GO

IF COL_LENGTH(N'dbo.Students', N'MonthlyTransportFee') IS NULL
    ALTER TABLE dbo.Students ADD MonthlyTransportFee DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_Students_TransportFee DEFAULT (0);
GO

IF COL_LENGTH(N'dbo.Students', N'TransportActivatedOn') IS NULL
    ALTER TABLE dbo.Students ADD TransportActivatedOn DATE NULL;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = N'CK_Students_TransportFee' AND parent_object_id = OBJECT_ID(N'dbo.Students'))
    ALTER TABLE dbo.Students ADD CONSTRAINT CK_Students_TransportFee CHECK (MonthlyTransportFee >= 0);
GO

PRINT N'Students transport columns ready.';
GO
