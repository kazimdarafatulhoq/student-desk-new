/*
================================================================================
 Alter_Fix_FeePayment_LocalDate.sql
 Fix: FeePayments.PaymentDate was SYSUTCDATETIME() (UTC) while dashboard
 "today" uses CAST(GETDATE() AS DATE) (local) — receipts could miss Today's
 Collection. Also backfill today's UTC-stamped rows to local calendar date.

 Run against StudentManagementSDB.
================================================================================
*/

USE StudentManagementSDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* Backfill: any payment whose UTC calendar day differs from local "today"
   but falls within the last 36 hours → treat as collected today (local). */
UPDATE dbo.FeePayments
SET PaymentDate = GETDATE()
WHERE IsDeleted = 0
  AND CAST(PaymentDate AS DATE) <> CAST(GETDATE() AS DATE)
  AND PaymentDate >= DATEADD(HOUR, -36, SYSUTCDATETIME());
GO

IF OBJECT_ID(N'dbo.usp_RecordFeePayment', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_RecordFeePayment;
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
        (@StudentId, GETDATE(), @ReceiptNo, @Particulars, @FeePeriods, 0, @AmountPaid, 2, 2, @TransactionRef, @CollectedByUserId, N'sp');

    SET @LedgerId = SCOPE_IDENTITY();

    IF ISNULL(@WaiverApplied, 0) > 0
    BEGIN
        INSERT INTO dbo.StudentLedger
            (StudentId, TransactionDate, VoucherNo, Particulars, FeePeriod, DebitAmount, CreditAmount, EntryType, Status, CreatedByUserId, CreatedBy)
        VALUES
            (@StudentId, GETDATE(), N'WVR-' + @ReceiptNo, N'Waiver / Discount', @FeePeriods, 0, @WaiverApplied, 4, 4, @CollectedByUserId, N'sp');
    END

    INSERT INTO dbo.FeePayments
        (StudentId, ReceiptNo, PaymentDate, AmountPaid, FineCollected, WaiverApplied, PaymentMethod,
         TransactionRef, FeePeriods, Remarks, LedgerId, CollectedByUserId, CreatedBy)
    VALUES
        (@StudentId, @ReceiptNo, GETDATE(), @AmountPaid, ISNULL(@FineCollected, 0), ISNULL(@WaiverApplied, 0),
         @PaymentMethod, @TransactionRef, @FeePeriods, @Remarks, @LedgerId, @CollectedByUserId, N'sp');

    COMMIT TRAN;

    SELECT @ReceiptNo AS ReceiptNo, @LedgerId AS LedgerId;
END
GO

PRINT N'usp_RecordFeePayment now stores local PaymentDate; recent UTC rows backfilled.';
GO
