using DAL;
using DTO;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace BUS
{
    public class ThongKeBUS
    {
        private ThongKeDAL _thongKeDAL = new ThongKeDAL();

        public List<TopKhachHangDTO> GetTopKhachHangMuaNhieuNhat()
        {
            var data = _thongKeDAL.GetTopKhachHang();
            if (data == null || data.Count == 0) return data;

            decimal maxTien = data.Max(x => x.TongTienMua);
            if (maxTien == 0) maxTien = 1; // Prevent divide by zero

            // Colors for Top 3
            string[] colors = { "#E8956D", "#5B9BD5", "#9C27B0" };
            string[] bgColors = { "#FFF0E6", "#E3F2FD", "#F3E5F5" };
            string[] avatars = { "👸", "👩", "👱‍♀️" };

            for (int i = 0; i < data.Count; i++)
            {
                data[i].ProgressValue = (double)(data[i].TongTienMua / maxTien) * 150; // Map to 150px max width
                if (data[i].ProgressValue < 10) data[i].ProgressValue = 10; // min width
                
                int colorIndex = i % colors.Length;
                data[i].ProgressColor = colors[colorIndex];
                data[i].AvatarBgColor = bgColors[colorIndex];
                data[i].Avatar = avatars[colorIndex];
            }

            return data;
        }

        public List<DoanhThuNgayDTO> GetDoanhThu7NgayQua()
        {
            return _thongKeDAL.GetDoanhThu7NgayQua();
        }

        public DataTable GetTopSellingProducts()
        {
            return _thongKeDAL.GetTopSellingProducts();
        }

        public DataTable GetLowStockProducts()
        {
            return _thongKeDAL.GetLowStockProducts();
        }

        public DataTable GetDashboardOverviewStats()
        {
            return _thongKeDAL.GetDashboardOverviewStats();
        }
    }
}
