using System.Data;
using DAL;

namespace BUS
{
    public class PhieuTraHangBUS
    {
        private PhieuTraHangDAL _dal = new PhieuTraHangDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
        
        public DataTable GetChiTiet(string maPhieuTra)
        {
            return _dal.GetChiTiet(maPhieuTra);
        }
    }
}
