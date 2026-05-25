using System;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace LUnivers_Beaute.Services
{
    public class CloudinaryService
    {
        private Cloudinary _cloudinary;

        public CloudinaryService()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (File.Exists(configPath))
                {
                    string jsonString = File.ReadAllText(configPath);
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        var cloudConfig = doc.RootElement.GetProperty("Cloudinary");
                        string cloudName = cloudConfig.GetProperty("CloudName").GetString();
                        string apiKey = cloudConfig.GetProperty("ApiKey").GetString();
                        string apiSecret = cloudConfig.GetProperty("ApiSecret").GetString();

                        Account account = new Account(cloudName, apiKey, apiSecret);
                        _cloudinary = new Cloudinary(account);
                        _cloudinary.Api.Secure = true;
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file config.json!", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc cấu hình Cloudinary: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task<string> UploadImageAsync(string filePath)
        {
            if (_cloudinary == null)
            {
                MessageBox.Show("Cloudinary chưa được cấu hình đúng. Vui lòng kiểm tra config.json.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if (!File.Exists(filePath))
                return null;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(filePath),
                Folder = "LUniversBeaute_Products",
                Transformation = new Transformation()
                    .Width(1280).Height(1280).Crop("limit") // Giới hạn kích thước tối đa là HD (1280x1280)
                    .Quality("auto") // Tự động nén chất lượng (AI của Cloudinary)
                    .FetchFormat("auto") // Tự động chọn định dạng WebP/AVIF để tối ưu dung lượng
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            return null;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl) || _cloudinary == null)
                return false;

            try
            {
                // URL dạng: https://res.cloudinary.com/.../upload/v1234/Folder/filename.ext
                // Ta cần lấy "Folder/filename" làm publicId
                Uri uri = new Uri(imageUrl);
                string path = uri.AbsolutePath;
                
                int uploadIndex = path.IndexOf("upload/");
                if (uploadIndex == -1) return false;

                string afterUpload = path.Substring(uploadIndex + 7);
                // Sau upload/ có thể là phiên bản v123...
                int firstSlash = afterUpload.IndexOf('/');
                if (firstSlash == -1) return false;

                string publicIdWithExt = afterUpload.Substring(firstSlash + 1);
                int lastDot = publicIdWithExt.LastIndexOf('.');
                string publicId = lastDot != -1 ? publicIdWithExt.Substring(0, lastDot) : publicIdWithExt;

                // Để an toàn với URL được escape
                publicId = Uri.UnescapeDataString(publicId);

                var delParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(delParams);

                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }
    }
}
