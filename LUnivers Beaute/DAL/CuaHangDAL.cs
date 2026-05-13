using System.Data;

namespace DAL
{
    public class CuaHangDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllCuaHang");
        }
    }
}
