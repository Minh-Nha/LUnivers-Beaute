using System.Data;

namespace DAL
{
    public class KhachHangDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllKhachHang");
        }
    }
}
