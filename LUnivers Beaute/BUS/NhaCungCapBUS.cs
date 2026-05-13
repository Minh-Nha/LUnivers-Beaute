using System.Data;
using DAL;

namespace BUS
{
    public class NhaCungCapBUS
    {
        private NhaCungCapDAL _dal = new NhaCungCapDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
