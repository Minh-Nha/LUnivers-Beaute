using DTO;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class ThongKeDAL
    {
        public List<TopKhachHangDTO> GetTopKhachHang()
        {
            List<TopKhachHangDTO> list = new List<TopKhachHangDTO>();
            DataTable dt = DatabaseHelpers.GetData("sp_TopKhachHangMuaNhieuNhat");
            
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new TopKhachHangDTO
                {
                    HoTen = row["HoTen"].ToString(),
                    SoDienThoai = row["SoDienThoai"].ToString(),
                    TongTienMua = Convert.ToDecimal(row["TongTienMua"])
                });
            }
            return list;
        }

        public List<DoanhThuNgayDTO> GetDoanhThu7NgayQua()
        {
            List<DoanhThuNgayDTO> list = new List<DoanhThuNgayDTO>();
            DataTable dt = DatabaseHelpers.GetData("sp_DoanhThu7NgayQua");
            
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DoanhThuNgayDTO
                {
                    Ngay = Convert.ToDateTime(row["Ngay"]),
                    DoanhThu = Convert.ToDecimal(row["DoanhThu"])
                });
            }
            return list;
        }

        public DataTable GetTopSellingProducts()
        {
            string query = @"
                SELECT TOP 5 
                    sp.TenSanPham, 
                    th.TenThuongHieu, 
                    COALESCE(SUM(ct.SoLuong), 0) AS TongDaBan, 
                    sp.GiaNiemYet AS Gia,
                    COALESCE(SUM(tk.SoLuongTon), 0) AS TongTon
                FROM SanPham sp
                LEFT JOIN LoSanXuat lsx ON sp.MaSanPham = lsx.MaSanPham
                LEFT JOIN ChiTietHoaDon ct ON ct.MaLo = lsx.MaLo
                LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
                LEFT JOIN TonKho tk ON tk.MaLo = lsx.MaLo
                GROUP BY sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, sp.GiaNiemYet
                ORDER BY TongDaBan DESC, sp.TenSanPham ASC";
            return DatabaseHelpers.ExecuteQuery(query);
        }

        public DataTable GetLowStockProducts()
        {
            string query = @"
                SELECT TOP 5 
                    sp.TenSanPham, 
                    th.TenThuongHieu, 
                    tk.SoLuongTon, 
                    ch.TenCuaHang,
                    CASE 
                        WHEN tk.SoLuongTon = 0 THEN N'Hết hàng'
                        ELSE N'Sắp hết hàng' 
                    END AS TinhTrang
                FROM TonKho tk
                JOIN LoSanXuat lsx ON tk.MaLo = lsx.MaLo
                JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
                LEFT JOIN CuaHang ch ON tk.MaCuaHang = ch.MaCuaHang
                LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
                WHERE tk.SoLuongTon <= 30
                ORDER BY tk.SoLuongTon ASC, sp.TenSanPham ASC";
            return DatabaseHelpers.ExecuteQuery(query);
        }

        public DataTable GetDashboardOverviewStats()
        {
            string query = @"
                SELECT 
                    COALESCE(SUM(CASE WHEN CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE) THEN TongTien ELSE 0 END), 0) AS DoanhThuHomNay,
                    COALESCE(SUM(CASE WHEN MONTH(NgayLap) = MONTH(GETDATE()) AND YEAR(NgayLap) = YEAR(GETDATE()) THEN TongTien ELSE 0 END), 0) AS DoanhThuThang,
                    COUNT(CASE WHEN CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE) THEN 1 END) AS DonHangHomNay,
                    (SELECT COUNT(*) FROM KhachHang) AS TongKhachHang
                FROM HoaDon";
            return DatabaseHelpers.ExecuteQuery(query);
        }
    }
}
