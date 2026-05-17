using System.Data;
using DAL;

namespace BUS
{
    public class ThuongHieuBUS
    {
        private ThuongHieuDAL _dal = new ThuongHieuDAL();

        public DataTable GetAll(string searchTerm = null)
        {
            return _dal.GetAll(searchTerm);
        }

        public int Insert(string tenThuongHieu, string quocGia) => _dal.Insert(tenThuongHieu, quocGia);
        public int Update(int maThuongHieu, string tenThuongHieu, string quocGia) => _dal.Update(maThuongHieu, tenThuongHieu, quocGia);
        public int Delete(int maThuongHieu) => _dal.Delete(maThuongHieu);
    }
}
