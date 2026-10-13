using System;
using System.Data;
using Microsoft.Data.SqlClient;
using StudentManagement.UserObjects.Enums;

namespace StudentManagement.DataAccessLayer.Data_Access_Methods
{
    public class tblFee
    {
        public DataSet GetStudentLedger(int studentId)
        {
            var objExecute = new Execute();
            return objExecute.ExecuteDataSet(
                "usp_GetStudentLedger",
                Execute.AddParameter("@StudentId", studentId));
        }

        public (string InvoiceNo, long LedgerId) PostTuitionInvoice(
            int studentId, string feePeriod, decimal amount, string categoryName,
            decimal fineAmount, int? userId)
        {
            var pInvoiceNo = Execute.AddOutputParameter("@InvoiceNo", SqlDbType.NVarChar, 50);
            var pLedgerId = Execute.AddOutputParameter("@LedgerId", SqlDbType.BigInt);
            var objExecute = new Execute();
            objExecute.Executes(
                "usp_PostTuitionInvoice",
                ReturnType.NonQuery,
                CommandType.StoredProcedure,
                Execute.AddParameter("@StudentId", studentId),
                Execute.AddParameter("@FeePeriod", feePeriod),
                Execute.AddParameter("@Amount", amount),
                Execute.AddParameter("@CategoryName", categoryName),
                Execute.AddParameter("@FineAmount", fineAmount),
                Execute.AddParameter("@UserId", userId),
                pInvoiceNo,
                pLedgerId);

            return (Convert.ToString(pInvoiceNo.Value) ?? string.Empty, Convert.ToInt64(pLedgerId.Value));
        }

        public (string ReceiptNo, long LedgerId, long CollectionId) RecordFeePayment(
            int studentId, decimal amountPaid, PaymentMethod paymentMethod, string? feePeriods,
            decimal fineCollected, decimal waiverApplied, string? transactionRef, string? remarks,
            int collectedByUserId)
        {
            var pReceiptNo = Execute.AddOutputParameter("@ReceiptNo", SqlDbType.NVarChar, 50);
            var pLedgerId = Execute.AddOutputParameter("@LedgerId", SqlDbType.BigInt);
            var objExecute = new Execute();
            objExecute.Executes(
                "usp_RecordFeePayment",
                ReturnType.NonQuery,
                CommandType.StoredProcedure,
                Execute.AddParameter("@StudentId", studentId),
                Execute.AddParameter("@AmountPaid", amountPaid),
                Execute.AddParameter("@PaymentMethod", (int)paymentMethod),
                Execute.AddParameter("@FeePeriods", feePeriods),
                Execute.AddParameter("@FineCollected", fineCollected),
                Execute.AddParameter("@WaiverApplied", waiverApplied),
                Execute.AddParameter("@TransactionRef", transactionRef),
                Execute.AddParameter("@Remarks", remarks),
                Execute.AddParameter("@CollectedByUserId", collectedByUserId),
                pReceiptNo,
                pLedgerId);

            var receiptNo = Convert.ToString(pReceiptNo.Value) ?? string.Empty;
            var ledgerId = Convert.ToInt64(pLedgerId.Value);

            long collectionId = 0;
            using (var con = new SqlConnection(DbConfig.ConnectionString))
            using (var cmd = new SqlCommand(
                       "SELECT TOP 1 CollectionId FROM dbo.FeePayments WHERE ReceiptNo = @R ORDER BY CollectionId DESC", con))
            {
                cmd.Parameters.AddWithValue("@R", receiptNo);
                con.Open();
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    collectionId = Convert.ToInt64(result);
            }

            return (receiptNo, ledgerId, collectionId);
        }

        public DataTable GetDashboardStats()
        {
            var objExecute = new Execute();
            return (DataTable)objExecute.Executes(
                "usp_GetDashboardStats", ReturnType.DataTable, CommandType.StoredProcedure)!;
        }
    }
}
