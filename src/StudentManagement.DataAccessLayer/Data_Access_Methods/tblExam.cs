using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentManagement.DataAccessLayer.Data_Access_Methods
{
    public class tblExam
    {
        public DataTable GetActiveTerms()
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT ExamTermId, TermName, AcademicSession, StartDate, EndDate, TimetableJson
FROM dbo.ExamTerms
WHERE IsDeleted = 0 AND IsActive = 1
ORDER BY StartDate DESC", con);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public long UpsertClearance(int studentId, int examTermId, decimal dues,
            bool isCleared, bool adminOverride, string? overrideReason, int? clearedByUserId, string? remarks)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            con.Open();

            using (var find = new SqlCommand(@"
SELECT TOP 1 ClearanceId FROM dbo.ExamClearances
WHERE StudentId = @StudentId AND ExamTermId = @ExamTermId AND IsDeleted = 0", con))
            {
                find.Parameters.AddWithValue("@StudentId", studentId);
                find.Parameters.AddWithValue("@ExamTermId", examTermId);
                var existing = find.ExecuteScalar();
                if (existing != null && existing != DBNull.Value)
                {
                    using var upd = new SqlCommand(@"
UPDATE dbo.ExamClearances SET
  DuesAtClearance = @Dues, IsCleared = @IsCleared, AdminOverride = @AdminOverride,
  OverrideReason = @OverrideReason, ClearedByUserId = @ClearedByUserId,
  ClearedAt = SYSUTCDATETIME(), Remarks = @Remarks, UpdatedAt = SYSUTCDATETIME()
WHERE ClearanceId = @ClearanceId;
SELECT @ClearanceId;", con);
                    upd.Parameters.AddWithValue("@ClearanceId", Convert.ToInt64(existing));
                    upd.Parameters.AddWithValue("@Dues", dues);
                    upd.Parameters.AddWithValue("@IsCleared", isCleared);
                    upd.Parameters.AddWithValue("@AdminOverride", adminOverride);
                    upd.Parameters.AddWithValue("@OverrideReason", (object?)overrideReason ?? DBNull.Value);
                    upd.Parameters.AddWithValue("@ClearedByUserId", (object?)clearedByUserId ?? DBNull.Value);
                    upd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);
                    return Convert.ToInt64(upd.ExecuteScalar());
                }
            }

            using var ins = new SqlCommand(@"
INSERT INTO dbo.ExamClearances
 (StudentId, ExamTermId, DuesAtClearance, IsCleared, AdminOverride, OverrideReason, ClearedByUserId, ClearedAt, Remarks)
VALUES
 (@StudentId, @ExamTermId, @Dues, @IsCleared, @AdminOverride, @OverrideReason, @ClearedByUserId, SYSUTCDATETIME(), @Remarks);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", con);
            ins.Parameters.AddWithValue("@StudentId", studentId);
            ins.Parameters.AddWithValue("@ExamTermId", examTermId);
            ins.Parameters.AddWithValue("@Dues", dues);
            ins.Parameters.AddWithValue("@IsCleared", isCleared);
            ins.Parameters.AddWithValue("@AdminOverride", adminOverride);
            ins.Parameters.AddWithValue("@OverrideReason", (object?)overrideReason ?? DBNull.Value);
            ins.Parameters.AddWithValue("@ClearedByUserId", (object?)clearedByUserId ?? DBNull.Value);
            ins.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);
            return Convert.ToInt64(ins.ExecuteScalar());
        }

        public long IssueAdmitCard(int studentId, int examTermId, string admitCardNo,
            string barcodeValue, int issuedByUserId, long? clearanceId)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.AdmitCards
 (StudentId, ExamTermId, AdmitCardNo, BarcodeValue, IssuedAt, IssuedByUserId, ClearanceId)
VALUES
 (@StudentId, @ExamTermId, @AdmitCardNo, @BarcodeValue, SYSUTCDATETIME(), @IssuedByUserId, @ClearanceId);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@ExamTermId", examTermId);
            cmd.Parameters.AddWithValue("@AdmitCardNo", admitCardNo);
            cmd.Parameters.AddWithValue("@BarcodeValue", barcodeValue);
            cmd.Parameters.AddWithValue("@IssuedByUserId", issuedByUserId);
            cmd.Parameters.AddWithValue("@ClearanceId", (object?)clearanceId ?? DBNull.Value);
            con.Open();
            return Convert.ToInt64(cmd.ExecuteScalar());
        }
    }
}
