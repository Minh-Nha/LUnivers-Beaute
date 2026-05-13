using System.Data;
using DAL;

namespace BUS
{
    public class HoaDonBUS
    {
        private HoaDonDAL _dal = new HoaDonDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
        
        public DataTable GetChiTiet(string maHoaDon)
        {
            return _dal.GetChiTiet(maHoaDon);
        }

        public DataTable GetDonHangGanDay(int topN = 5)
        {
            return _dal.GetDonHangGanDay(topN);
        }
    }
}
