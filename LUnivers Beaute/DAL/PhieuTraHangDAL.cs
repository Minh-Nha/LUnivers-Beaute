using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class PhieuTraHangDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllPhieuTraHang");
        }
        
        public DataTable GetChiTiet(string maPhieuTra)
        {
            return DatabaseHelpers.GetData("sp_GetChiTietPhieuTraHang", new SqlParameter("@MaPhieuTra", maPhieuTra));
        }
    }
}
