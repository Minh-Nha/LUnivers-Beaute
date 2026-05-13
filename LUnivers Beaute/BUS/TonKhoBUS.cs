using System.Data;
using DAL;

namespace BUS
{
    public class TonKhoBUS
    {
        private TonKhoDAL _dal = new TonKhoDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
