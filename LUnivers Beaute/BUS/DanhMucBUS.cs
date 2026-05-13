using System.Data;
using DAL;

namespace BUS
{
    public class DanhMucBUS
    {
        private DanhMucDAL _dal = new DanhMucDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
