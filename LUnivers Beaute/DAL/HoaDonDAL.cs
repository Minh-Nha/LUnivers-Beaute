using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class HoaDonDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllHoaDon");
        }
        
        public DataTable GetChiTiet(string maHoaDon)
        {
            return DatabaseHelpers.GetData("sp_GetChiTietHoaDon", new SqlParameter("@MaHoaDon", maHoaDon));
        }

        public DataTable GetDonHangGanDay(int topN = 5)
        {
            return DatabaseHelpers.GetData("sp_GetDonHangGanDay", new SqlParameter("@TopN", topN));
        }
    }
}
