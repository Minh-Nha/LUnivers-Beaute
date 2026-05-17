using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class PhieuTraHangDAL
    {
        public DataTable GetAllPhieuTraHang()
        {
            return DatabaseHelpers.GetData("sp_GetAllPhieuTraHang");
        }

        public DataTable GetChiTietPhieuTra(string maPhieuTra)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhieuTra", maPhieuTra)
            };
            return DatabaseHelpers.GetData("sp_GetChiTietPhieuTra", parameters);
        }

        public bool InsertPhieuTraHang(string maPhieuTra, string maHoaDon, DateTime ngayTra, string lyDo, string jsonChiTiet)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhieuTra", maPhieuTra),
                new SqlParameter("@MaHoaDon", maHoaDon),
                new SqlParameter("@NgayTra", ngayTra),
                new SqlParameter("@LyDo", lyDo),
                new SqlParameter("@JsonChiTiet", jsonChiTiet)
            };
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertPhieuTraHang", parameters) > 0;
        }
    }
}
