using System.Data;

namespace DAL
{
    public class NhaCungCapDAL
    {
        public DataTable GetAll(string searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return DatabaseHelpers.GetData("sp_GetAllNhaCungCap");
            return DatabaseHelpers.GetData("sp_GetAllNhaCungCap",
                new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", searchTerm));
        }

        public int Insert(string tenNhaCungCap, string soDienThoai, string diaChi)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertNhaCungCap",
                new Microsoft.Data.SqlClient.SqlParameter("@TenNhaCungCap", tenNhaCungCap),
                new Microsoft.Data.SqlClient.SqlParameter("@SoDienThoai", soDienThoai),
                new Microsoft.Data.SqlClient.SqlParameter("@DiaChi", (object)diaChi ?? System.DBNull.Value));
        }

        public int Update(int maNhaCungCap, string tenNhaCungCap, string soDienThoai, string diaChi)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateNhaCungCap",
                new Microsoft.Data.SqlClient.SqlParameter("@MaNhaCungCap", maNhaCungCap),
                new Microsoft.Data.SqlClient.SqlParameter("@TenNhaCungCap", tenNhaCungCap),
                new Microsoft.Data.SqlClient.SqlParameter("@SoDienThoai", soDienThoai),
                new Microsoft.Data.SqlClient.SqlParameter("@DiaChi", (object)diaChi ?? System.DBNull.Value));
        }

        public int Delete(int maNhaCungCap)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteNhaCungCap",
                new Microsoft.Data.SqlClient.SqlParameter("@MaNhaCungCap", maNhaCungCap));
        }
    }
}
