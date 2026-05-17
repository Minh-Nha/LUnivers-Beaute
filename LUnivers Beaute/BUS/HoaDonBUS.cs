using System.Data;
using DAL;

namespace BUS
{
    public class HoaDonBUS
    {
        private HoaDonDAL _dal = new HoaDonDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
        
        public DataTable GetChiTiet(string maHoaDon)
        {
            return _dal.GetChiTiet(maHoaDon);
        }

        public DataTable GetDonHangGanDay(int topN = 5)
        {
            return _dal.GetDonHangGanDay(topN);
        }

        public DataTable GetSanPhamBanHang(string maCuaHang, string searchTerm = null, int? maDanhMuc = null)
        {
            return _dal.GetSanPhamBanHang(maCuaHang, searchTerm, maDanhMuc);
        }

        public int TaoHoaDon(string maHoaDon, string maCuaHang, string maNhanVien, int? maKhachHang, int? maKhuyenMai, string phuongThucThanhToan, string chiTietJSON)
        {
            return _dal.TaoHoaDon(maHoaDon, maCuaHang, maNhanVien, maKhachHang, maKhuyenMai, phuongThucThanhToan, chiTietJSON);
        }
    }
}
