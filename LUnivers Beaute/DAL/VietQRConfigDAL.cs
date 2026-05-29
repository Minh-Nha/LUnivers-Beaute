using System;
using System.Data;
using Microsoft.Data.SqlClient;
using DTO;

namespace DAL
{
    public class VietQRConfigDAL
    {
        public static VietQRConfigDTO GetVietQRConfig()
        {
            DataTable dt = DatabaseHelpers.GetData("sp_GetVietQRConfig");
            if (dt != null && dt.Rows.Count > 0)
            {
                return new VietQRConfigDTO
                {
                    BankCode = dt.Rows[0]["BankCode"].ToString(),
                    AccountNumber = dt.Rows[0]["AccountNumber"].ToString(),
                    AccountName = dt.Rows[0]["AccountName"].ToString()
                };
            }
            return null;
        }

        public static bool UpdateVietQRConfig(VietQRConfigDTO config)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@BankCode", config.BankCode ?? ""),
                    new SqlParameter("@AccountNumber", config.AccountNumber ?? ""),
                    new SqlParameter("@AccountName", config.AccountName ?? "")
                };
                return DatabaseHelpers.ExecuteNonQuery("sp_UpdateVietQRConfig", parameters) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
