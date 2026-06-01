using System;
using System.Collections.Generic;
using System.Data;
using BUS;

namespace LUnivers_Beaute.Services
{
    public class AccessLog
    {
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        public string Location { get; set; }
    }

    public class EditLog
    {
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Detail { get; set; }
        public string Icon { get; set; }
    }

    public static class LogService
    {
        private static readonly LogBUS LogBus = new LogBUS();

        static LogService()
        {
            try
            {
                string databaseSqlPath = @"d:\HK4\CN.NET\LUnivers-Beaute\LUnivers Beaute\Database\Database.sql";
                string storeSqlPath = @"d:\HK4\CN.NET\LUnivers-Beaute\LUnivers Beaute\Database\Store.sql";
                string logsSqlPath = @"d:\HK4\CN.NET\LUnivers-Beaute\LUnivers Beaute\Database\Database_Logs.sql";

                // 1. Cập nhật Database.sql (Bảng)
                if (System.IO.File.Exists(databaseSqlPath))
                {
                    string databaseContent = System.IO.File.ReadAllText(databaseSqlPath, System.Text.Encoding.Unicode);
                    if (!databaseContent.Contains("LichSuTruyCap"))
                    {
                        string appendText = "\r\n\r\n-- ==========================================\r\n" +
                                            "-- BẢNG LỊCH SỬ TRUY CẬP & LỊCH SỬ CHỈNH SỬA (Thêm tự động)\r\n" +
                                            "-- ==========================================\r\n" +
                                            "CREATE TABLE LichSuTruyCap (\r\n" +
                                            "    Id INT IDENTITY(1,1) PRIMARY KEY,\r\n" +
                                            "    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),\r\n" +
                                            "    UserName NVARCHAR(100) NOT NULL,\r\n" +
                                            "    IpAddress VARCHAR(50) NOT NULL,\r\n" +
                                            "    DeviceName NVARCHAR(100) NOT NULL,\r\n" +
                                            "    Location NVARCHAR(250) NOT NULL\r\n" +
                                            ");\r\n" +
                                            "GO\r\n\r\n" +
                                            "CREATE TABLE LichSuChinhSua (\r\n" +
                                            "    Id INT IDENTITY(1,1) PRIMARY KEY,\r\n" +
                                            "    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),\r\n" +
                                            "    UserName NVARCHAR(100) NOT NULL,\r\n" +
                                            "    Action NVARCHAR(100) NOT NULL,\r\n" +
                                            "    Detail NVARCHAR(MAX) NOT NULL,\r\n" +
                                            "    Icon NVARCHAR(50) NOT NULL\r\n" +
                                            ");\r\n" +
                                            "GO\r\n";
                        System.IO.File.AppendAllText(databaseSqlPath, appendText, System.Text.Encoding.Unicode);
                    }
                }

                // 2. Cập nhật Store.sql (Stored Procedures)
                if (System.IO.File.Exists(storeSqlPath))
                {
                    string storeContent = System.IO.File.ReadAllText(storeSqlPath, System.Text.Encoding.Unicode);
                    if (!storeContent.Contains("sp_InsertLichSuTruyCap"))
                    {
                        string appendText = "\r\n\r\n-- ==========================================\r\n" +
                                            "-- STORED PROCEDURES LỊCH SỬ (Thêm tự động)\r\n" +
                                            "-- ==========================================\r\n" +
                                            "/****** Object:  StoredProcedure [dbo].[sp_InsertLichSuTruyCap] ******/\r\n" +
                                            "SET ANSI_NULLS ON\r\n" +
                                            "GO\r\n" +
                                            "SET QUOTED_IDENTIFIER ON\r\n" +
                                            "GO\r\n" +
                                            "CREATE PROCEDURE [dbo].[sp_InsertLichSuTruyCap]\r\n" +
                                            "    @UserName NVARCHAR(100),\r\n" +
                                            "    @IpAddress VARCHAR(50),\r\n" +
                                            "    @DeviceName NVARCHAR(100),\r\n" +
                                            "    @Location NVARCHAR(250)\r\n" +
                                            "AS\r\n" +
                                            "BEGIN\r\n" +
                                            "    INSERT INTO LichSuTruyCap (UserName, IpAddress, DeviceName, Location)\r\n" +
                                            "    VALUES (@UserName, @IpAddress, @DeviceName, @Location);\r\n" +
                                            "END;\r\n" +
                                            "GO\r\n\r\n" +
                                            "/****** Object:  StoredProcedure [dbo].[sp_GetAllLichSuTruyCap] ******/\r\n" +
                                            "SET ANSI_NULLS ON\r\n" +
                                            "GO\r\n" +
                                            "SET QUOTED_IDENTIFIER ON\r\n" +
                                            "GO\r\n" +
                                            "CREATE PROCEDURE [dbo].[sp_GetAllLichSuTruyCap]\r\n" +
                                            "AS\r\n" +
                                            "BEGIN\r\n" +
                                            "    SELECT Id, Timestamp, UserName, IpAddress, DeviceName, Location\r\n" +
                                            "    FROM LichSuTruyCap\r\n" +
                                            "    ORDER BY Timestamp DESC;\r\n" +
                                            "END;\r\n" +
                                            "GO\r\n\r\n" +
                                            "/****** Object:  StoredProcedure [dbo].[sp_InsertLichSuChinhSua] ******/\r\n" +
                                            "SET ANSI_NULLS ON\r\n" +
                                            "GO\r\n" +
                                            "SET QUOTED_IDENTIFIER ON\r\n" +
                                            "GO\r\n" +
                                            "CREATE PROCEDURE [dbo].[sp_InsertLichSuChinhSua]\r\n" +
                                            "    @UserName NVARCHAR(100),\r\n" +
                                            "    @Action NVARCHAR(100),\r\n" +
                                            "    @Detail NVARCHAR(MAX),\r\n" +
                                            "    @Icon NVARCHAR(50)\r\n" +
                                            "AS\r\n" +
                                            "BEGIN\r\n" +
                                            "    INSERT INTO LichSuChinhSua (UserName, Action, Detail, Icon)\r\n" +
                                            "    VALUES (@UserName, @Action, @Detail, @Icon);\r\n" +
                                            "END;\r\n" +
                                            "GO\r\n\r\n" +
                                            "/****** Object:  StoredProcedure [dbo].[sp_GetAllLichSuChinhSua] ******/\r\n" +
                                            "SET ANSI_NULLS ON\r\n" +
                                            "GO\r\n" +
                                            "SET QUOTED_IDENTIFIER ON\r\n" +
                                            "GO\r\n" +
                                            "CREATE PROCEDURE [dbo].[sp_GetAllLichSuChinhSua]\r\n" +
                                            "AS\r\n" +
                                            "BEGIN\r\n" +
                                            "    SELECT Id, Timestamp, UserName, Action, Detail, Icon\r\n" +
                                            "    FROM LichSuChinhSua\r\n" +
                                            "    ORDER BY Timestamp DESC;\r\n" +
                                            "END;\r\n" +
                                            "GO\r\n";
                        System.IO.File.AppendAllText(storeSqlPath, appendText, System.Text.Encoding.Unicode);
                    }
                }

                // 3. Xóa Database_Logs.sql dư thừa
                if (System.IO.File.Exists(logsSqlPath))
                {
                    System.IO.File.Delete(logsSqlPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi tự động cập nhật SQL files: " + ex.Message);
            }
        }

        public static string GetLocalIpAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch {}
            return "127.0.0.1";
        }

        public static void LogAccess(string userName, string location)
        {
            try
            {
                string uName = string.IsNullOrEmpty(userName) ? "Khách/Ẩn danh" : userName;
                string ip = GetLocalIpAddress();
                string device = System.Environment.MachineName;
                string loc = string.IsNullOrEmpty(location) ? "Hệ thống nội bộ (Local)" : location;

                LogBus.InsertAccessLog(uName, ip, device, loc);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi ghi log truy cập: " + ex.Message);
            }
        }

        public static void LogEdit(string userName, string action, string detail, string icon = "✏️")
        {
            try
            {
                string uName = string.IsNullOrEmpty(userName) ? "Hệ thống" : userName;
                LogBus.InsertEditLog(uName, action, detail, icon);
                
                // Trigger notification event for UI updates
                OnNewEditLog?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi ghi log chỉnh sửa: " + ex.Message);
            }
        }

        public static List<AccessLog> GetAccessLogs()
        {
            var list = new List<AccessLog>();
            try
            {
                DataTable dt = LogBus.GetAllAccessLogs();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new AccessLog
                    {
                        Timestamp = row["Timestamp"] != DBNull.Value ? Convert.ToDateTime(row["Timestamp"]) : DateTime.Now,
                        User = row["UserName"] != DBNull.Value ? row["UserName"].ToString() : "Khách/Ẩn danh",
                        IpAddress = row["IpAddress"] != DBNull.Value ? row["IpAddress"].ToString() : "",
                        DeviceName = row["DeviceName"] != DBNull.Value ? row["DeviceName"].ToString() : "",
                        Location = row["Location"] != DBNull.Value ? row["Location"].ToString() : ""
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi tải lịch sử truy cập: " + ex.Message);
            }
            return list;
        }

        public static List<EditLog> GetEditLogs()
        {
            var list = new List<EditLog>();
            try
            {
                DataTable dt = LogBus.GetAllEditLogs();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new EditLog
                    {
                        Timestamp = row["Timestamp"] != DBNull.Value ? Convert.ToDateTime(row["Timestamp"]) : DateTime.Now,
                        User = row["UserName"] != DBNull.Value ? row["UserName"].ToString() : "Hệ thống",
                        Action = row["Action"] != DBNull.Value ? row["Action"].ToString() : "",
                        Detail = row["Detail"] != DBNull.Value ? row["Detail"].ToString() : "",
                        Icon = row["Icon"] != DBNull.Value ? row["Icon"].ToString() : "✏️"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi tải lịch sử chỉnh sửa: " + ex.Message);
            }
            return list;
        }

        public static event Action OnNewEditLog;
    }
}
