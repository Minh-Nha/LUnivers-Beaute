using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class PhieuNhapKhoDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllPhieuNhapKho");
        }
        
        public DataTable GetChiTiet(string maPhieuNhap)
        {
            return DatabaseHelpers.GetData("sp_GetChiTietNhapKho", new SqlParameter("@MaPhieuNhap", maPhieuNhap));
        }
    }
}
