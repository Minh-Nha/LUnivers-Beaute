using System.Data;
using DAL;

namespace BUS
{
    public class PhieuNhapKhoBUS
    {
        private PhieuNhapKhoDAL _dal = new PhieuNhapKhoDAL();

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }
        
        public DataTable GetChiTiet(string maPhieuNhap)
        {
            return _dal.GetChiTiet(maPhieuNhap);
        }
    }
}
