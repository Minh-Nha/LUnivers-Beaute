using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class DatabaseHelpers
    {
        private static readonly string connectionString =
            @"Data Source=PC-QUYS\SQLEXPRESS;Initial Catalog=LUnivers_Beaute;Integrated Security=True;Trust Server Certificate=True";

        public static DataTable GetData(string storeProcedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(storeProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
            return dt;
        }
    }
}
