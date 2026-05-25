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

        public DataTable GetSanPhamBanHang(string maCuaHang, string searchTerm = null, int? maDanhMuc = null)
        {
            var prmCuaHang = new SqlParameter("@MaCuaHang", (object)maCuaHang ?? DBNull.Value);
            var prmSearch = new SqlParameter("@SearchTerm", (object)searchTerm ?? DBNull.Value);
            var prmDanhMuc = new SqlParameter("@MaDanhMuc", (object)maDanhMuc ?? DBNull.Value);
            return DatabaseHelpers.GetData("sp_GetSanPhamBanHang", prmCuaHang, prmSearch, prmDanhMuc);
        }

        public DataTable GetSanPhamBanHangPaged(string maCuaHang, string searchTerm, int? maDanhMuc, int pageNumber, int pageSize, out int totalRecords)
        {
            var prmCuaHang = new SqlParameter("@MaCuaHang", (object)maCuaHang ?? DBNull.Value);
            var prmSearch = new SqlParameter("@SearchTerm", (object)searchTerm ?? DBNull.Value);
            var prmDanhMuc = new SqlParameter("@MaDanhMuc", (object)maDanhMuc ?? DBNull.Value);
            var prmPageNumber = new SqlParameter("@PageNumber", pageNumber);
            var prmPageSize = new SqlParameter("@PageSize", pageSize);
            
            var prmTotalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int);
            prmTotalRecords.Direction = ParameterDirection.Output;

            DataTable dt = DatabaseHelpers.GetData("sp_GetSanPhamBanHang_Paged", prmCuaHang, prmSearch, prmDanhMuc, prmPageNumber, prmPageSize, prmTotalRecords);
            
            if (prmTotalRecords.Value != DBNull.Value && prmTotalRecords.Value != null)
            {
                totalRecords = Convert.ToInt32(prmTotalRecords.Value);
            }
            else
            {
                totalRecords = 0;
            }

            return dt;
        }

        public int TaoHoaDon(string maHoaDon, string maCuaHang, string maNhanVien, int? maKhachHang, int? maKhuyenMai, string phuongThucThanhToan, string chiTietJSON)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_TaoHoaDon",
                new SqlParameter("@MaHoaDon", maHoaDon),
                new SqlParameter("@MaCuaHang", maCuaHang),
                new SqlParameter("@MaNhanVien", maNhanVien),
                new SqlParameter("@MaKhachHang", (object)maKhachHang ?? DBNull.Value),
                new SqlParameter("@MaKhuyenMai", (object)maKhuyenMai ?? DBNull.Value),
                new SqlParameter("@PhuongThucThanhToan", phuongThucThanhToan),
                new SqlParameter("@ChiTietJSON", chiTietJSON));
        }
    }
}
