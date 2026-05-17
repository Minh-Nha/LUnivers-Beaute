using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class HuySanPhamDAL
    {
        public DataTable GetAllHuySanPham()
        {
            return DatabaseHelpers.GetData("sp_GetAllHuySanPham");
        }

        public bool InsertHuySanPham(string maCuaHang, string maNhanVien, int maLo, int soLuong, DateTime ngayHuy, string lyDo)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@MaNhanVien", maNhanVien),
                new SqlParameter("@MaLo", maLo),
                new SqlParameter("@SoLuong", soLuong),
                new SqlParameter("@NgayHuy", ngayHuy),
                new SqlParameter("@LyDo", lyDo)
            };
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertHuySanPham", parameters) > 0;
        }

        public bool AutoHuySanPhamHetHan(string maCuaHang, string maNhanVien)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@MaNhanVien", maNhanVien)
            };
            DatabaseHelpers.ExecuteNonQuery("sp_AutoHuySanPhamHetHan", parameters);
            return true;
        }

        public DataTable GetSanPhamChoHuy(string maCuaHang)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaCuaHang", maCuaHang)
            };
            return DatabaseHelpers.GetData("sp_GetSanPhamChoHuy", parameters);
        }
    }
}
