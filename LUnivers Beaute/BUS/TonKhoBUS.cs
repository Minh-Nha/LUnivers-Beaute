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

        public DataTable SearchAndSort(string keyword, string status, string sortColumn, string sortOrder)
        {
            return _dal.SearchAndSort(keyword, status, sortColumn, sortOrder);
        }
    }
}
