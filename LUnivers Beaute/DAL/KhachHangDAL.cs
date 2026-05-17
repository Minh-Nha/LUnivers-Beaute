using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class KhachHangDAL
    {
        public DataTable GetAll(string? timKiem = null, int? trangThai = null)
        {
            return DatabaseHelpers.GetData("sp_GetAllKhachHang",
                new SqlParameter("@TimKiem", (object?)timKiem ?? System.DBNull.Value),
                new SqlParameter("@TrangThai", (object?)trangThai ?? System.DBNull.Value));
        }

        public int Insert(string hoTen, string soDienThoai, int diemTichLuy, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertKhachHang",
                new SqlParameter("@HoTen", hoTen),
                new SqlParameter("@SoDienThoai", soDienThoai),
                new SqlParameter("@DiemTichLuy", diemTichLuy),
                new SqlParameter("@TrangThai", trangThai));
        }

        public int Update(int maKhachHang, string hoTen, string soDienThoai, int diemTichLuy, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateKhachHang",
                new SqlParameter("@MaKhachHang", maKhachHang),
                new SqlParameter("@HoTen", hoTen),
                new SqlParameter("@SoDienThoai", soDienThoai),
                new SqlParameter("@DiemTichLuy", diemTichLuy),
                new SqlParameter("@TrangThai", trangThai));
        }

        public int Delete(int maKhachHang)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteKhachHang",
                new SqlParameter("@MaKhachHang", maKhachHang));
        }
    }
}
