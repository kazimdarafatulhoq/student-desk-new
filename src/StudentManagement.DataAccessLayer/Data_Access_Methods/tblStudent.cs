using System;
using System.Data;
using Microsoft.Data.SqlClient;
using StudentManagement.UserObjects.Enums;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.DataAccessLayer.Data_Access_Methods
{
    public class tblStudent
    {
        public DataTable SearchStudents(string? term, int? classId, int? sectionId, int? status)
        {
            var objExecute = new Execute();
            var param = new[]
            {
                Execute.AddParameter("@Term", string.IsNullOrWhiteSpace(term) ? null : term.Trim()),
                Execute.AddParameter("@ClassId", classId),
                Execute.AddParameter("@SectionId", sectionId),
                Execute.AddParameter("@Status", status)
            };
            return (DataTable)objExecute.Executes(
                "usp_SearchStudents", ReturnType.DataTable, CommandType.StoredProcedure, param)!;
        }

        public DataTable GetById(int studentId)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT s.*, c.ClassName, sec.SectionName,
       ISNULL((SELECT SUM(DebitAmount)-SUM(CreditAmount) FROM dbo.StudentLedger
               WHERE StudentId = s.StudentId AND IsDeleted = 0), 0) AS NetDue
FROM dbo.Students s
INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
INNER JOIN dbo.Sections sec ON sec.SectionId = s.SectionId
WHERE s.StudentId = @StudentId AND s.IsDeleted = 0", con);
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable FindQuick(string query)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT TOP 1 s.*, c.ClassName, sec.SectionName,
       ISNULL((SELECT SUM(DebitAmount)-SUM(CreditAmount) FROM dbo.StudentLedger
               WHERE StudentId = s.StudentId AND IsDeleted = 0), 0) AS NetDue
FROM dbo.Students s
INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
INNER JOIN dbo.Sections sec ON sec.SectionId = s.SectionId
WHERE s.IsDeleted = 0
  AND (s.RegistrationNo = @Q OR s.GuardianPhone = @Q)
ORDER BY s.StudentId", con);
            cmd.Parameters.AddWithValue("@Q", query.Trim());
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetLookup()
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT s.StudentId, s.RegistrationNo, s.FullName, s.ClassId, c.ClassName,
       s.SectionId, sec.SectionName, s.RollNumber, s.GuardianPhone, s.Status
FROM dbo.Students s
INNER JOIN dbo.Classes c ON c.ClassId = s.ClassId
INNER JOIN dbo.Sections sec ON sec.SectionId = s.SectionId
WHERE s.IsDeleted = 0 AND s.Status = 1
ORDER BY s.ClassId, s.RollNumber, s.FullName", con);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public (int StudentId, string RegistrationNo) AdmitStudent(StudentAdmissionRequest request)
        {
            var pStudentId = Execute.AddOutputParameter("@StudentId", SqlDbType.Int);
            var pRegNo = Execute.AddOutputParameter("@RegistrationNo", SqlDbType.NVarChar, 30);
            var objExecute = new Execute();
            objExecute.Executes(
                "usp_AdmitStudent",
                ReturnType.NonQuery,
                CommandType.StoredProcedure,
                Execute.AddParameter("@FullName", request.FullName.Trim()),
                Execute.AddParameter("@FatherName", request.FatherName.Trim()),
                Execute.AddParameter("@MotherName", string.IsNullOrWhiteSpace(request.MotherName) ? null : request.MotherName.Trim()),
                Execute.AddParameter("@BloodGroup", (int)request.BloodGroup),
                Execute.AddParameter("@Gender", (int)request.Gender),
                Execute.AddParameter("@DateOfBirth", request.DateOfBirth.Date),
                Execute.AddParameter("@GuardianPhone", request.GuardianPhone.Trim()),
                Execute.AddParameter("@PresentAddress", request.PresentAddress),
                Execute.AddParameter("@PermanentAddress", request.PermanentAddress),
                Execute.AddParameter("@ClassId", request.ClassId),
                Execute.AddParameter("@SectionId", request.SectionId),
                Execute.AddParameter("@RollNumber", request.RollNumber.Trim()),
                Execute.AddParameter("@MonthlyTuitionFee", request.MonthlyTuitionFee),
                Execute.AddParameter("@AcademicSession",
                    string.IsNullOrWhiteSpace(request.AcademicSession) ? "2025-2026" : request.AcademicSession.Trim()),
                Execute.AddParameter("@PhotoPath", request.PhotoPath),
                Execute.AddParameter("@CreatedBy", request.CreatedBy),
                pStudentId,
                pRegNo);

            return (Convert.ToInt32(pStudentId.Value), Convert.ToString(pRegNo.Value) ?? string.Empty);
        }

        public void UpdateStudent(StudentDto student, string? updatedBy)
        {
            var objExecute = new Execute();
            objExecute.Executes(
                "usp_UpdateStudent",
                ReturnType.NonQuery,
                CommandType.StoredProcedure,
                Execute.AddParameter("@StudentId", student.StudentId),
                Execute.AddParameter("@FullName", student.FullName.Trim()),
                Execute.AddParameter("@FatherName", student.FatherName.Trim()),
                Execute.AddParameter("@MotherName", string.IsNullOrWhiteSpace(student.MotherName) ? null : student.MotherName.Trim()),
                Execute.AddParameter("@BloodGroup", (int)student.BloodGroup),
                Execute.AddParameter("@Gender", (int)student.Gender),
                Execute.AddParameter("@DateOfBirth", student.DateOfBirth.Date),
                Execute.AddParameter("@GuardianPhone", student.GuardianPhone.Trim()),
                Execute.AddParameter("@PresentAddress", student.PresentAddress),
                Execute.AddParameter("@PermanentAddress", student.PermanentAddress),
                Execute.AddParameter("@ClassId", student.ClassId),
                Execute.AddParameter("@SectionId", student.SectionId),
                Execute.AddParameter("@RollNumber", student.RollNumber.Trim()),
                Execute.AddParameter("@MonthlyTuitionFee", student.MonthlyTuitionFee),
                Execute.AddParameter("@Status", (int)student.Status),
                Execute.AddParameter("@PhotoPath", student.PhotoPath),
                Execute.AddParameter("@AcademicSession", student.AcademicSession),
                Execute.AddParameter("@UpdatedBy", updatedBy));
        }

        public void SetStatus(int studentId, StudentStatus status, string? updatedBy)
        {
            var objExecute = new Execute();
            objExecute.Executes(
                "usp_SetStudentStatus",
                ReturnType.NonQuery,
                CommandType.StoredProcedure,
                Execute.AddParameter("@StudentId", studentId),
                Execute.AddParameter("@Status", (int)status),
                Execute.AddParameter("@UpdatedBy", updatedBy));
        }
    }
}
