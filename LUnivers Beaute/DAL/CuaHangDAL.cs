using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class CuaHangDAL
    {
        public DataTable GetAll(string? timKiem = null, int? trangThai = null)
        {
            return DatabaseHelpers.GetData("sp_GetAllCuaHang",
                new SqlParameter("@TimKiem", (object?)timKiem ?? System.DBNull.Value),
                new SqlParameter("@TrangThai", (object?)trangThai ?? System.DBNull.Value));
        }

        public int Insert(string tenCuaHang, string diaChi, string soDienThoai, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertCuaHang",
                new SqlParameter("@TenCuaHang", tenCuaHang),
                new SqlParameter("@DiaChi", diaChi),
                new SqlParameter("@SoDienThoai", soDienThoai),
                new SqlParameter("@TrangThai", trangThai));
        }

        public int Update(string maCuaHang, string tenCuaHang, string diaChi, string soDienThoai, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateCuaHang",
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@TenCuaHang", tenCuaHang),
                new SqlParameter("@DiaChi", diaChi),
                new SqlParameter("@SoDienThoai", soDienThoai),
                new SqlParameter("@TrangThai", trangThai));
        }

        public int Delete(string maCuaHang)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteCuaHang",
                new SqlParameter("@MaCuaHang", maCuaHang));
        }
    }
}
