using System.Data;
using DAL;

namespace BUS
{
    public class NhaCungCapBUS
    {
        private NhaCungCapDAL _dal = new NhaCungCapDAL();

        public DataTable GetAll(string searchTerm = null)
        {
            return _dal.GetAll(searchTerm);
        }

        public int Insert(string tenNhaCungCap, string soDienThoai, string diaChi) => _dal.Insert(tenNhaCungCap, soDienThoai, diaChi);
        public int Update(int maNhaCungCap, string tenNhaCungCap, string soDienThoai, string diaChi) => _dal.Update(maNhaCungCap, tenNhaCungCap, soDienThoai, diaChi);
        public int Delete(int maNhaCungCap) => _dal.Delete(maNhaCungCap);
    }
}
