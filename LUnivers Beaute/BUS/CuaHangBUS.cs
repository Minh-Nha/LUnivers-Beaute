using System.Data;
using DAL;

namespace BUS
{
    public class CuaHangBUS
    {
        private CuaHangDAL _dal = new CuaHangDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
