using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class LogDAL
    {
        public int InsertAccessLog(string userName, string ipAddress, string deviceName, string location)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertLichSuTruyCap",
                new SqlParameter("@UserName", userName ?? (object)DBNull.Value),
                new SqlParameter("@IpAddress", ipAddress ?? (object)DBNull.Value),
                new SqlParameter("@DeviceName", deviceName ?? (object)DBNull.Value),
                new SqlParameter("@Location", location ?? (object)DBNull.Value));
        }

        public DataTable GetAllAccessLogs()
        {
            return DatabaseHelpers.GetData("sp_GetAllLichSuTruyCap");
        }

        public int InsertEditLog(string userName, string action, string detail, string icon)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertLichSuChinhSua",
                new SqlParameter("@UserName", userName ?? (object)DBNull.Value),
                new SqlParameter("@Action", action ?? (object)DBNull.Value),
                new SqlParameter("@Detail", detail ?? (object)DBNull.Value),
                new SqlParameter("@Icon", icon ?? (object)DBNull.Value));
        }

        public DataTable GetAllEditLogs()
        {
            return DatabaseHelpers.GetData("sp_GetAllLichSuChinhSua");
        }
    }
}
