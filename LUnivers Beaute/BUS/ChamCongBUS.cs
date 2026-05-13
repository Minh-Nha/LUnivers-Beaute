using System.Data;
using DAL;

namespace BUS
{
    public class ChamCongBUS
    {
        private ChamCongDAL _dal = new ChamCongDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
