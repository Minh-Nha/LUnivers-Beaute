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

        public DataTable Login(string tenDangNhap, string matKhau)
        {
            return _dal.Login(tenDangNhap, matKhau);
        }

        public int Insert(string hoTen, string soDienThoai, string vaiTro, string tenDangNhap, string matKhau, string maCuaHang, string email, bool trangThai)
        {
            return _dal.Insert(hoTen, soDienThoai, vaiTro, tenDangNhap, matKhau, maCuaHang, email, trangThai);
        }

        public int Update(string maNhanVien, string hoTen, string soDienThoai, string vaiTro, string tenDangNhap, string maCuaHang, string email, bool trangThai)
        {
            return _dal.Update(maNhanVien, hoTen, soDienThoai, vaiTro, tenDangNhap, maCuaHang, email, trangThai);
        }

        public DataTable CheckEmailExists(string email)
        {
            return _dal.CheckEmailExists(email);
        }

        public int UpdatePasswordByEmail(string email, string matKhauMoi)
        {
            return _dal.UpdatePasswordByEmail(email, matKhauMoi);
        }

        public int Delete(string maNhanVien)
        {
            return _dal.Delete(maNhanVien);
        }
    }
}
