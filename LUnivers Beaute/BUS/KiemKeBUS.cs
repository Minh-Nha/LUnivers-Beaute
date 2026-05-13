using System.Data;
using DAL;

namespace BUS
{
    public class KiemKeBUS
    {
        private KiemKeDAL _dal = new KiemKeDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
