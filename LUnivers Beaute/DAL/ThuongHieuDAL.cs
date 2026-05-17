using System.Data;

namespace DAL
{
    public class ThuongHieuDAL
    {
        public DataTable GetAll(string searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return DatabaseHelpers.GetData("sp_GetAllThuongHieu");
            return DatabaseHelpers.GetData("sp_GetAllThuongHieu",
                new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", searchTerm));
        }

        public int Insert(string tenThuongHieu, string quocGia)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertThuongHieu",
                new Microsoft.Data.SqlClient.SqlParameter("@TenThuongHieu", tenThuongHieu),
                new Microsoft.Data.SqlClient.SqlParameter("@QuocGia", (object)quocGia ?? System.DBNull.Value));
        }

        public int Update(int maThuongHieu, string tenThuongHieu, string quocGia)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateThuongHieu",
                new Microsoft.Data.SqlClient.SqlParameter("@MaThuongHieu", maThuongHieu),
                new Microsoft.Data.SqlClient.SqlParameter("@TenThuongHieu", tenThuongHieu),
                new Microsoft.Data.SqlClient.SqlParameter("@QuocGia", (object)quocGia ?? System.DBNull.Value));
        }

        public int Delete(int maThuongHieu)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteThuongHieu",
                new Microsoft.Data.SqlClient.SqlParameter("@MaThuongHieu", maThuongHieu));
        }
    }
}
