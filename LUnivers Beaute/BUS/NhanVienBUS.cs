using System.Data;
using DAL;

namespace BUS
{
    public class NhanVienBUS
    {
        private NhanVienDAL _dal = new NhanVienDAL();

        public DataTable GetAll(string? timKiem = null, string? maCuaHang = null, int? trangThai = null)
        {
            return _dal.GetAll(timKiem, maCuaHang, trangThai);
        }

        public int Insert(string hoTen, string soDienThoai, string vaiTro, string tenDangNhap, string matKhau, string maCuaHang, bool trangThai)
        {
            return _dal.Insert(hoTen, soDienThoai, vaiTro, tenDangNhap, matKhau, maCuaHang, trangThai);
        }

        public int Update(string maNhanVien, string hoTen, string soDienThoai, string vaiTro, string tenDangNhap, string maCuaHang, bool trangThai)
        {
            return _dal.Update(maNhanVien, hoTen, soDienThoai, vaiTro, tenDangNhap, maCuaHang, trangThai);
        }

        public int Delete(string maNhanVien)
        {
            return _dal.Delete(maNhanVien);
        }
    }
}
