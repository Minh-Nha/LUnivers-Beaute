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

        public bool TaoPhieuNhap(string maPhieuNhap, string maCuaHang, string maNhanVien, double tongTienNhap, string chiTietJson)
        {
            return _dal.TaoPhieuNhap(maPhieuNhap, maCuaHang, maNhanVien, tongTienNhap, chiTietJson);
        }
    }
}
