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

        public bool TaoPhieuNhap(string maPhieuNhap, string maCuaHang, string maNhanVien, double tongTienNhap, string chiTietJson)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhieuNhap", maPhieuNhap),
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@MaNhanVien", maNhanVien),
                new SqlParameter("@TongTienNhap", tongTienNhap),
                new SqlParameter("@ChiTietJSON", chiTietJson)
            };
            return DatabaseHelpers.ExecuteNonQuery("sp_TaoPhieuNhapKho", parameters) != 0;
        }
    }
}
