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

        public DataTable GetTonKhoForKiemKe(string maCuaHang)
        {
            return _dal.GetTonKhoForKiemKe(maCuaHang);
        }

        public bool TaoPhieuKiemKe(string maCuaHang, string chiTietJson)
        {
            return _dal.TaoPhieuKiemKe(maCuaHang, chiTietJson);
        }

        public bool Delete(int maKiemKe)
        {
            return _dal.Delete(maKiemKe);
        }
    }
}
