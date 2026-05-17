using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class ChamCongDAL
    {
        public DataTable GetAll(DateTime? tuNgay = null, DateTime? denNgay = null, string? maNhanVien = null)
        {
            return DatabaseHelpers.GetData("sp_GetAllChamCong",
                new SqlParameter("@TuNgay", (object?)tuNgay ?? System.DBNull.Value),
                new SqlParameter("@DenNgay", (object?)denNgay ?? System.DBNull.Value),
                new SqlParameter("@MaNhanVien", (object?)maNhanVien ?? System.DBNull.Value));
        }

        public int Insert(string maNhanVien, DateTime ngayLam, TimeSpan gioVao, TimeSpan? gioRa = null)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertChamCong",
                new SqlParameter("@MaNhanVien", maNhanVien),
                new SqlParameter("@NgayLam", ngayLam.Date),
                new SqlParameter("@GioVao", gioVao),
                new SqlParameter("@GioRa", (object?)gioRa ?? System.DBNull.Value));
        }

        public int Update(int maCC, string maNhanVien, DateTime ngayLam, TimeSpan gioVao, TimeSpan? gioRa = null)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateChamCong",
                new SqlParameter("@MaCC", maCC),
                new SqlParameter("@MaNhanVien", maNhanVien),
                new SqlParameter("@NgayLam", ngayLam.Date),
                new SqlParameter("@GioVao", gioVao),
                new SqlParameter("@GioRa", (object?)gioRa ?? System.DBNull.Value));
        }

        public int Delete(int maCC)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteChamCong",
                new SqlParameter("@MaCC", maCC));
        }
    }
}
