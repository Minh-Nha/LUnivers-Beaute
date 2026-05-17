using System.Data;
using DAL;

namespace BUS
{
    public class KhachHangBUS
    {
        private KhachHangDAL _dal = new KhachHangDAL();

        public DataTable GetAll(string? timKiem = null, int? trangThai = null)
        {
            return _dal.GetAll(timKiem, trangThai);
        }

        public int Insert(string hoTen, string soDienThoai, int diemTichLuy, bool trangThai)
        {
            return _dal.Insert(hoTen, soDienThoai, diemTichLuy, trangThai);
        }

        public int Update(int maKhachHang, string hoTen, string soDienThoai, int diemTichLuy, bool trangThai)
        {
            return _dal.Update(maKhachHang, hoTen, soDienThoai, diemTichLuy, trangThai);
        }

        public int Delete(int maKhachHang)
        {
            return _dal.Delete(maKhachHang);
        }
    }
}
