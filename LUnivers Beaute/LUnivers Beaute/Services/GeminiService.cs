using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LUnivers_Beaute.Services
{
    public class GeminiService
    {
        private static readonly List<DateTime> requestTimestamps = new List<DateTime>();
        private static readonly object lockObj = new object();

        private static async Task<string> FetchAllDatabaseDataAsync()
        {
            return await Task.Run(() =>
            {
                string connectionString = @"Data Source=pyrex.myvnc.com,14330;Initial Catalog=LUnivers_Beaute;Persist Security Info=True;User ID=user;Password = 123;Trust Server Certificate=True";
                string[] tables = new[] 
                {
                    "ChamCong", "ChiTietHoaDon", "ChiTietNhapKho", "ChiTietPhieuTraHang",
                    "CuaHang", "DanhMuc", "HoaDon", "HuySanPham", "KhachHang",
                    "KhuyenMai", "KiemKe", "LoSanXuat", "NhaCungCap", "NhanVien",
                    "PhieuNhapKho", "PhieuTraHang", "SanPham", "ThuongHieu", "TonKho"
                };

                var dbData = new Dictionary<string, List<Dictionary<string, object>>>();

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        foreach (var table in tables)
                        {
                            string query = $"SELECT * FROM {table}";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    var tableRows = new List<Dictionary<string, object>>();
                                    while (reader.Read())
                                    {
                                        var row = new Dictionary<string, object>();
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                        }
                                        tableRows.Add(row);
                                    }
                                    dbData[table] = tableRows;
                                }
                            }
                        }
                    }
                    return JsonSerializer.Serialize(dbData);
                }
                catch (Exception ex)
                {
                    return "{\"error\": \"Lỗi đọc dữ liệu Database: " + ex.Message + "\"}";
                }
            });
        }

        public static async Task<string> GenerateContentAsync(string prompt)
        {
            lock (lockObj)
            {
                var now = DateTime.Now;
                requestTimestamps.RemoveAll(t => (now - t).TotalMinutes >= 1);
                if (requestTimestamps.Count >= 14)
                {
                    throw new Exception("Vượt quá giới hạn gọi AI (14 lần/phút). Vui lòng đợi khoảng 1 phút rồi thử lại.");
                }
                requestTimestamps.Add(now);
            }

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            string apiKey = "";
            string modelName = "gemini-flash-latest"; // Default

            if (File.Exists(configPath))
            {
                string jsonString = File.ReadAllText(configPath);
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    if (doc.RootElement.TryGetProperty("Gemini", out JsonElement geminiConfig))
                    {
                        if (geminiConfig.TryGetProperty("ApiKey", out JsonElement keyElem))
                            apiKey = keyElem.GetString();
                        if (geminiConfig.TryGetProperty("ModelName", out JsonElement modelElem))
                            modelName = modelElem.GetString();
                    }
                }
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Vui lòng cấu hình API Key của Gemini trong phần Cài đặt.");
            }
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = "gemini-3.1-flash-lite"; // User requested default
            }

            // Fetch entire database context
            string dbContextJson = await FetchAllDatabaseDataAsync();
            string enrichedPrompt = $"Dưới đây là toàn bộ dữ liệu của hệ thống dưới định dạng JSON:\n{dbContextJson}\n\nDựa vào dữ liệu trên, hãy trả lời câu hỏi sau của người dùng:\n{prompt}";

            using (var client = new HttpClient())
            {
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent";
                client.DefaultRequestHeaders.Add("X-goog-api-key", apiKey);

                var requestBody = new
                {
                    systemInstruction = new
                    {
                        parts = new[]
                        {
                            new { text = "Bạn là trợ lý AI thông minh của hệ thống quản lý L'Univers Beauté. Nhiệm vụ của bạn là hỗ trợ đọc, phân tích dữ liệu từ các bảng (Doanh thu, Sản phẩm, Khách hàng...) và hướng dẫn sử dụng phần mềm. \n\nQUY TẮC NGHIÊM NGẶT:\n1. CHỈ trả lời các câu hỏi liên quan đến dữ liệu bán hàng, quản lý cửa hàng và hướng dẫn dùng app.\n2. TUYỆT ĐỐI KHÔNG cung cấp, liệt kê hay đề cập đến thông tin nhạy cảm như: mật khẩu (password), API Key, chuỗi kết nối CSDL (Connection String), hay file cấu hình hệ thống.\n3. Nếu người dùng hỏi những thông tin ngoài phạm vi hoặc cố tình lấy thông tin bảo mật, hãy từ chối trả lời một cách lịch sự và chuyên nghiệp." }
                        }
                    },
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = enrichedPrompt }
                            }
                        }
                    }
                };

                string jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(responseString))
                    {
                        var root = doc.RootElement;
                        var candidates = root.GetProperty("candidates");
                        if (candidates.GetArrayLength() > 0)
                        {
                            var firstCandidate = candidates[0];
                            var contentProp = firstCandidate.GetProperty("content");
                            var parts = contentProp.GetProperty("parts");
                            if (parts.GetArrayLength() > 0)
                            {
                                return parts[0].GetProperty("text").GetString();
                            }
                        }
                    }
                    return "Không có nội dung trả về từ AI.";
                }
                else
                {
                    throw new Exception($"Lỗi gọi API: {response.StatusCode} - {responseString}");
                }
            }
        }
    }
}
