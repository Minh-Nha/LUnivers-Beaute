using System.Data;

namespace DAL
{
    public class HuySanPhamDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllHuySanPham");
        }
    }
}
