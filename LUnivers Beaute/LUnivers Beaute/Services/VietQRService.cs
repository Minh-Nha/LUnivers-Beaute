using System;
using System.IO;
using System.Text.Json;

namespace LUnivers_Beaute.Services
{
    public class VietQRService
    {
        public static string GenerateQRUrl(decimal amount, string invoiceId)
        {
            string bankCode = "";
            string accNum = "";
            string accName = "";

            var config = BUS.VietQRConfigBUS.GetVietQRConfig();
            if (config != null)
            {
                bankCode = config.BankCode;
                accNum = config.AccountNumber;
                accName = config.AccountName;
            }

            if (string.IsNullOrEmpty(accNum) || string.IsNullOrEmpty(bankCode))
            {
                throw new Exception("Vui lòng cấu hình tài khoản VietQR trong phần Cài đặt trước khi thanh toán bằng chuyển khoản.");
            }

            string encodedName = Uri.EscapeDataString(accName);
            string encodedInfo = Uri.EscapeDataString(invoiceId);
            string url = $"https://img.vietqr.io/image/{bankCode}-{accNum}-print.png?amount={amount}&addInfo={encodedInfo}&accountName={encodedName}";

            return url;
        }

        public static async System.Threading.Tasks.Task<System.Collections.Generic.List<BankModel>> GetBanksAsync()
        {
            var banks = new System.Collections.Generic.List<BankModel>();
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var response = await client.GetStringAsync("https://api.vietqr.io/v2/banks");
                    using (JsonDocument doc = JsonDocument.Parse(response))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("code", out JsonElement code) && code.GetString() == "00")
                        {
                            var dataArray = root.GetProperty("data").EnumerateArray();
                            foreach (var item in dataArray)
                            {
                                banks.Add(new BankModel
                                {
                                    Bin = item.GetProperty("bin").GetString(),
                                    Code = item.GetProperty("code").GetString(),
                                    ShortName = item.GetProperty("shortName").GetString(),
                                    Name = item.GetProperty("name").GetString()
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                // Return empty list on failure
            }
            return banks;
        }
    }

    public class BankModel
    {
        public string Bin { get; set; }
        public string Code { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string DisplayName => $"{ShortName} - {Name}";
    }
}
