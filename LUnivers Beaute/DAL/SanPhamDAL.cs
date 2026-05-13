using System.Data;

namespace DAL
{
    public class SanPhamDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllSanPham");
        }
    }
}
