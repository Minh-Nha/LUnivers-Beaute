using System.Data;
using DAL;

namespace BUS
{
    public class KhachHangBUS
    {
        private KhachHangDAL _dal = new KhachHangDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
