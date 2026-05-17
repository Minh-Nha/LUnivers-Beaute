using System.Data;
using DAL;

namespace BUS
{
    public class DanhMucBUS
    {
        private DanhMucDAL _dal = new DanhMucDAL();

        public DataTable GetAll(string searchTerm = null)
        {
            return _dal.GetAll(searchTerm);
        }

        public int Insert(string tenDanhMuc) => _dal.Insert(tenDanhMuc);
        public int Update(int maDanhMuc, string tenDanhMuc) => _dal.Update(maDanhMuc, tenDanhMuc);
        public int Delete(int maDanhMuc) => _dal.Delete(maDanhMuc);
    }
}
