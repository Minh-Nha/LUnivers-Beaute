using System;
using DTO;
using DAL;

namespace BUS
{
    public class VietQRConfigBUS
    {
        public static VietQRConfigDTO GetVietQRConfig()
        {
            return VietQRConfigDAL.GetVietQRConfig();
        }

        public static bool UpdateVietQRConfig(VietQRConfigDTO config)
        {
            return VietQRConfigDAL.UpdateVietQRConfig(config);
        }
    }
}
