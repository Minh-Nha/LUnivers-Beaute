using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class KiemKeDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllKiemKe");
        }

        public DataTable GetTonKhoForKiemKe(string maCuaHang)
        {
            return DatabaseHelpers.GetData("sp_GetTonKhoForKiemKe",
                new SqlParameter("@MaCuaHang", maCuaHang));
        }

        public bool TaoPhieuKiemKe(string maCuaHang, string chiTietJson)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@ChiTietJSON", chiTietJson)
            };
            return DatabaseHelpers.ExecuteNonQuery("sp_TaoPhieuKiemKe", parameters) != 0;
        }

        public bool Delete(int maKiemKe)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteKiemKe",
                new SqlParameter("@MaKiemKe", maKiemKe)) > 0;
        }
    }
}
