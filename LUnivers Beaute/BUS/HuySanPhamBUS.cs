using System.Data;
using DAL;

namespace BUS
{
    public class HuySanPhamBUS
    {
        private HuySanPhamDAL _dal = new HuySanPhamDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
