using System.Data;

namespace DAL
{
    public class TonKhoDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetTonKho");
        }

        public DataTable SearchAndSort(string keyword, string status, string sortColumn, string sortOrder)
        {
            return DatabaseHelpers.GetData("sp_GetTonKho",
                new Microsoft.Data.SqlClient.SqlParameter("@SearchKeyword", (object)keyword ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", (object)status ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@SortColumn", (object)sortColumn ?? "SoLuongTon"),
                new Microsoft.Data.SqlClient.SqlParameter("@SortOrder", (object)sortOrder ?? "ASC"));
        }
    }
}
