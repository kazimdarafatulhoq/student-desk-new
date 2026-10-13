using System.Data;
using Microsoft.Data.SqlClient;

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
SELECT ClassId, ClassName, ClassCode
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
SELECT SectionId, ClassId, SectionName
FROM dbo.Sections
WHERE ClassId = @ClassId AND IsDeleted = 0 AND IsActive = 1
ORDER BY SectionName", con);
            cmd.Parameters.AddWithValue("@ClassId", classId);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}
