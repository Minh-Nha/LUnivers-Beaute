using System.Data;

namespace DAL
{
    public class DanhMucDAL
    {
        public DataTable GetAll(string searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return DatabaseHelpers.GetData("sp_GetAllDanhMuc");
            return DatabaseHelpers.GetData("sp_GetAllDanhMuc",
                new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", searchTerm));
        }

        public int Insert(string tenDanhMuc)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertDanhMuc",
                new Microsoft.Data.SqlClient.SqlParameter("@TenDanhMuc", tenDanhMuc));
        }

        public int Update(int maDanhMuc, string tenDanhMuc)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateDanhMuc",
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", maDanhMuc),
                new Microsoft.Data.SqlClient.SqlParameter("@TenDanhMuc", tenDanhMuc));
        }

        public int Delete(int maDanhMuc)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteDanhMuc",
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", maDanhMuc));
        }
    }
}
