using System;
using System.Data;
using Microsoft.Data.SqlClient;
using StudentManagement.UserObjects.Objects;

namespace StudentManagement.DataAccessLayer.Data_Access_Methods
{
    public class tblAcademic
    {
        public DataTable GetClassSectionLookup()
        {
            var objExecute = new Execute();
            return (DataTable)objExecute.Executes(
                "usp_GetClassSectionLookup", ReturnType.DataTable, CommandType.StoredProcedure)!;
        }

        public DataTable GetClasses()
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT ClassId, ClassName, ClassCode, SortOrder, IsActive
FROM dbo.Classes
WHERE IsDeleted = 0 AND IsActive = 1
ORDER BY SortOrder, ClassName", con);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetSectionsByClass(int classId)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(@"
SELECT SectionId, ClassId, SectionName, Capacity, IsActive
FROM dbo.Sections
WHERE ClassId = @ClassId AND IsDeleted = 0 AND IsActive = 1
ORDER BY SectionName", con);
            cmd.Parameters.AddWithValue("@ClassId", classId);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable GetBloodGroups()
        {
            var objExecute = new Execute();
            return (DataTable)objExecute.Executes(
                "usp_GetBloodGroups", ReturnType.DataTable, CommandType.StoredProcedure)!;
        }

        public DataTable GetPaymentMethods()
        {
            var objExecute = new Execute();
            return (DataTable)objExecute.Executes(
                "usp_GetPaymentMethods", ReturnType.DataTable, CommandType.StoredProcedure)!;
        }

        public DataTable GetClassesAdmin()
        {
            var objExecute = new Execute();
            return (DataTable)objExecute.Executes(
                "usp_GetClassesAdmin", ReturnType.DataTable, CommandType.StoredProcedure)!;
        }

        public int SaveClass(ClassDto dto, string? updatedBy)
        {
            var pOut = Execute.AddOutputParameter("@OutClassId", SqlDbType.Int);
            var objExecute = new Execute();
            objExecute.Executes(
                "usp_SaveClass",
                ReturnType.NonQuery,
                CommandType.StoredProcedure,
                Execute.AddParameter("@ClassId", dto.ClassId),
                Execute.AddParameter("@ClassName", dto.ClassName.Trim()),
                Execute.AddParameter("@ClassCode", string.IsNullOrWhiteSpace(dto.ClassCode) ? null : dto.ClassCode.Trim()),
                Execute.AddParameter("@SortOrder", dto.SortOrder),
                Execute.AddParameter("@IsActive", dto.IsActive),
                Execute.AddParameter("@UpdatedBy", updatedBy),
                pOut);
            return Convert.ToInt32(pOut.Value);
        }

        public DataTable GetSectionsAdmin(int? classId = null)
        {
            var objExecute = new Execute();
            return (DataTable)objExecute.Executes(
                "usp_GetSectionsAdmin",
                ReturnType.DataTable,
                CommandType.StoredProcedure,
                Execute.AddParameter("@ClassId", classId.HasValue && classId.Value > 0 ? classId.Value : (object)DBNull.Value))!;
        }

        public int SaveSection(SectionDto dto, string? updatedBy)
        {
            var pOut = Execute.AddOutputParameter("@OutSectionId", SqlDbType.Int);
            var objExecute = new Execute();
            objExecute.Executes(
                "usp_SaveSection",
                ReturnType.NonQuery,
                CommandType.StoredProcedure,
                Execute.AddParameter("@SectionId", dto.SectionId),
                Execute.AddParameter("@ClassId", dto.ClassId),
                Execute.AddParameter("@SectionName", dto.SectionName.Trim()),
                Execute.AddParameter("@Capacity", dto.Capacity),
                Execute.AddParameter("@IsActive", dto.IsActive),
                Execute.AddParameter("@UpdatedBy", updatedBy),
                pOut);
            return Convert.ToInt32(pOut.Value);
        }
    }
}
