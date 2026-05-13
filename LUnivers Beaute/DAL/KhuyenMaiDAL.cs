using System.Data;

namespace DAL
{
    public class KhuyenMaiDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllKhuyenMai");
        }
    }
}
