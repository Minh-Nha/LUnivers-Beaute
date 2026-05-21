using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class NhanVienDAL
    {
        public DataTable GetAll(string? timKiem = null, string? maCuaHang = null, int? trangThai = null)
        {
            return DatabaseHelpers.GetData("sp_GetAllNhanVien",
                new SqlParameter("@TimKiem", (object?)timKiem ?? System.DBNull.Value),
                new SqlParameter("@MaCuaHang", (object?)maCuaHang ?? System.DBNull.Value),
                new SqlParameter("@TrangThai", (object?)trangThai ?? System.DBNull.Value));
        }

        public DataTable Login(string tenDangNhap, string matKhau)
        {
            return DatabaseHelpers.GetData("sp_Login",
                new SqlParameter("@TenDangNhap", tenDangNhap),
                new SqlParameter("@MatKhau", matKhau));
        }

        public int Insert(string hoTen, string soDienThoai, string vaiTro, string tenDangNhap, string matKhau, string maCuaHang, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertNhanVien",
                new SqlParameter("@HoTen", hoTen),
                new SqlParameter("@SoDienThoai", soDienThoai),
                new SqlParameter("@VaiTro", vaiTro),
                new SqlParameter("@TenDangNhap", tenDangNhap),
                new SqlParameter("@MatKhau", matKhau),
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@TrangThai", trangThai));
        }

        public int Update(string maNhanVien, string hoTen, string soDienThoai, string vaiTro, string tenDangNhap, string maCuaHang, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateNhanVien",
                new SqlParameter("@MaNhanVien", maNhanVien),
                new SqlParameter("@HoTen", hoTen),
                new SqlParameter("@SoDienThoai", soDienThoai),
                new SqlParameter("@VaiTro", vaiTro),
                new SqlParameter("@TenDangNhap", tenDangNhap),
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@TrangThai", trangThai));
        }

        public int Delete(string maNhanVien)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteNhanVien",
                new SqlParameter("@MaNhanVien", maNhanVien));
        }
    }
}
