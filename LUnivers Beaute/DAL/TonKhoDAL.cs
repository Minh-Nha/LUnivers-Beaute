using System.Data;

namespace DAL
{
    public class TonKhoDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetTonKho");
        }

        public DataTable SearchAndSort(string keyword, string status, string sortColumn, string sortOrder, string maCuaHang = null, int? maDanhMuc = null, int? tuSoLuong = null, int? denSoLuong = null)
        {
            return DatabaseHelpers.GetData("sp_GetTonKho",
                new Microsoft.Data.SqlClient.SqlParameter("@MaCuaHang", string.IsNullOrEmpty(maCuaHang) ? (object)System.DBNull.Value : maCuaHang),
                new Microsoft.Data.SqlClient.SqlParameter("@SearchKeyword", (object)keyword ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", (object)status ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", maDanhMuc.HasValue ? (object)maDanhMuc.Value : System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@TuSoLuong", tuSoLuong.HasValue ? (object)tuSoLuong.Value : System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@DenSoLuong", denSoLuong.HasValue ? (object)denSoLuong.Value : System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@SortColumn", (object)sortColumn ?? "SoLuongTon"),
                new Microsoft.Data.SqlClient.SqlParameter("@SortOrder", (object)sortOrder ?? "ASC"));
        }
    }
}
