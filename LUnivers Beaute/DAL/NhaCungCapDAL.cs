using System.Data;

namespace DAL
{
    public class NhaCungCapDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllNhaCungCap");
        }
    }
}
