using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentManagement.DataAccessLayer
{
    public enum ReturnType
    {
        DataTable,
        NonQuery,
        Scalar
    }

    /// <summary>Simple ADO.NET helper — same idea as DyeERP Execute class.</summary>
    public class Execute
    {
        public static SqlParameter AddParameter(string name, object? value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static SqlParameter AddOutputParameter(string name, SqlDbType type, int size = 0)
        {
            var p = size > 0 ? new SqlParameter(name, type, size) : new SqlParameter(name, type);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        public object? Executes(
            string commandText,
            ReturnType returnType,
            CommandType commandType,
            params SqlParameter[] parameters)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(commandText, con);
            cmd.CommandType = commandType;
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            con.Open();

            switch (returnType)
            {
                case ReturnType.DataTable:
                {
                    using var adapter = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
                case ReturnType.Scalar:
                    return cmd.ExecuteScalar();
                default:
                    return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Runs a stored procedure and returns all result sets as a DataSet.</summary>
        public DataSet ExecuteDataSet(string procedureName, params SqlParameter[] parameters)
        {
            using var con = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(procedureName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
    }
}
