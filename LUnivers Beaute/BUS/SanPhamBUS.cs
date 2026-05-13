using System.Data;
using DAL;

namespace BUS
{
    public class SanPhamBUS
    {
        private SanPhamDAL _dal = new SanPhamDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
