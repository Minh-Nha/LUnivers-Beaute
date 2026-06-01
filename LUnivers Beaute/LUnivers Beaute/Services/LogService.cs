using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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
        private static readonly string AccessLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "access_logs.json");
        private static readonly string EditLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "edit_logs.json");
        private static readonly object FileLock = new object();

        static LogService()
        {
            SeedInitialLogs();
        }

        private static void SeedInitialLogs()
        {
            lock (FileLock)
            {
                if (!File.Exists(AccessLogPath))
                {
                    var initialAccess = new List<AccessLog>();
                    File.WriteAllText(AccessLogPath, JsonSerializer.Serialize(initialAccess, new JsonSerializerOptions { WriteIndented = true }));
                }

                if (!File.Exists(EditLogPath))
                {
                    var initialEdits = new List<EditLog>();
                    File.WriteAllText(EditLogPath, JsonSerializer.Serialize(initialEdits, new JsonSerializerOptions { WriteIndented = true }));
                }
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
            lock (FileLock)
            {
                try
                {
                    var logs = GetAccessLogs();
                    logs.Insert(0, new AccessLog
                    {
                        Timestamp = DateTime.Now,
                        User = string.IsNullOrEmpty(userName) ? "Khách/Ẩn danh" : userName,
                        IpAddress = GetLocalIpAddress(),
                        DeviceName = System.Environment.MachineName,
                        Location = string.IsNullOrEmpty(location) ? "Hệ thống nội bộ (Local)" : location
                    });

                    // Giới hạn 100 dòng mới nhất
                    if (logs.Count > 100) logs.RemoveRange(100, logs.Count - 100);

                    File.WriteAllText(AccessLogPath, JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true }));
                }
                catch { }
            }
        }

        public static void LogEdit(string userName, string action, string detail, string icon = "✏️")
        {
            lock (FileLock)
            {
                try
                {
                    var logs = GetEditLogs();
                    logs.Insert(0, new EditLog
                    {
                        Timestamp = DateTime.Now,
                        User = string.IsNullOrEmpty(userName) ? "Hệ thống" : userName,
                        Action = action,
                        Detail = detail,
                        Icon = icon
                    });

                    // Giới hạn 100 dòng mới nhất
                    if (logs.Count > 100) logs.RemoveRange(100, logs.Count - 100);

                    File.WriteAllText(EditLogPath, JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true }));
                    
                    // Trigger notification event for UI updates
                    OnNewEditLog?.Invoke();
                }
                catch { }
            }
        }

        public static List<AccessLog> GetAccessLogs()
        {
            lock (FileLock)
            {
                try
                {
                    if (File.Exists(AccessLogPath))
                    {
                        string json = File.ReadAllText(AccessLogPath);
                        return JsonSerializer.Deserialize<List<AccessLog>>(json) ?? new List<AccessLog>();
                    }
                }
                catch { }
                return new List<AccessLog>();
            }
        }

        public static List<EditLog> GetEditLogs()
        {
            lock (FileLock)
            {
                try
                {
                    if (File.Exists(EditLogPath))
                    {
                        string json = File.ReadAllText(EditLogPath);
                        return JsonSerializer.Deserialize<List<EditLog>>(json) ?? new List<EditLog>();
                    }
                }
                catch { }
                return new List<EditLog>();
            }
        }

        public static event Action OnNewEditLog;
    }
}
