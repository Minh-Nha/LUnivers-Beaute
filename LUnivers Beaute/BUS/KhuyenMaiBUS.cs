using System.Data;
using DAL;

namespace BUS
{
    public class KhuyenMaiBUS
    {
        private KhuyenMaiDAL _dal = new KhuyenMaiDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
    }
}
