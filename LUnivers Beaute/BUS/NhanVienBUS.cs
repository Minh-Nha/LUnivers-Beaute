using System.Data;
using DAL;

namespace BUS
{
    public class NhanVienBUS
    {
        private NhanVienDAL _dal = new NhanVienDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
