using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace LUnivers_Beaute.Views
{
    public partial class CaiDatView : UserControl
    {
        private string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public CaiDatView()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string jsonString = File.ReadAllText(configPath);
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        if (doc.RootElement.TryGetProperty("Email", out JsonElement emailConfig))
                        {
                            txtSenderEmail.Text = emailConfig.GetProperty("SenderEmail").GetString();
                            txtAppPassword.Password = emailConfig.GetProperty("AppPassword").GetString();
                        }
                        if (doc.RootElement.TryGetProperty("Cloudinary", out JsonElement cloudConfig))
                        {
                            txtCloudName.Text = cloudConfig.GetProperty("CloudName").GetString();
                            txtApiKey.Text = cloudConfig.GetProperty("ApiKey").GetString();
                            txtApiSecret.Password = cloudConfig.GetProperty("ApiSecret").GetString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc cấu hình: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonObject root = new JsonObject();
                if (File.Exists(configPath))
                {
                    string existing = File.ReadAllText(configPath);
                    root = JsonNode.Parse(existing) as JsonObject ?? new JsonObject();
                }

                JsonObject emailNode = new JsonObject();
                emailNode["SenderEmail"] = txtSenderEmail.Text.Trim();
                emailNode["AppPassword"] = txtAppPassword.Password;
                root["Email"] = emailNode;

                JsonObject cloudNode = new JsonObject();
                cloudNode["CloudName"] = txtCloudName.Text.Trim();
                cloudNode["ApiKey"] = txtApiKey.Text.Trim();
                cloudNode["ApiSecret"] = txtApiSecret.Password;
                root["Cloudinary"] = cloudNode;

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(configPath, root.ToJsonString(options));

                MessageBox.Show("Đã lưu cấu hình thành công! Một số thay đổi sẽ có tác dụng trong lần tiếp theo hệ thống gọi API.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu cấu hình: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
