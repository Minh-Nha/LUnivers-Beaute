using System.Data;
using DAL;

namespace BUS
{
    public class ThuongHieuBUS
    {
        private ThuongHieuDAL _dal = new ThuongHieuDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
