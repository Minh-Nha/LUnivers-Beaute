using System.Data;
using DAL;

namespace BUS
{
    public class KhuyenMaiBUS
    {
        private KhuyenMaiDAL _dal = new KhuyenMaiDAL();

        public DataTable GetAll(string tuKhoa = null, int? maDanhMuc = null, string loaiGiam = null, System.DateTime? tuNgay = null, System.DateTime? denNgay = null, int trangThaiLoc = 0)
        {
            return _dal.GetAll(tuKhoa, maDanhMuc, loaiGiam, tuNgay, denNgay, trangThaiLoc);
        }

        public bool Insert(string tenChuongTrinh, string loaiGiam, double giaTriGiam, string apDungTheo, object maDanhMuc, object maSanPham, System.DateTime ngayBatDau, System.DateTime ngayKetThuc, bool trangThai)
        {
            return _dal.Insert(tenChuongTrinh, loaiGiam, giaTriGiam, apDungTheo, maDanhMuc, maSanPham, ngayBatDau, ngayKetThuc, trangThai);
        }

        public bool Update(int maKhuyenMai, string tenChuongTrinh, string loaiGiam, double giaTriGiam, string apDungTheo, object maDanhMuc, object maSanPham, System.DateTime ngayBatDau, System.DateTime ngayKetThuc, bool trangThai)
        {
            return _dal.Update(maKhuyenMai, tenChuongTrinh, loaiGiam, giaTriGiam, apDungTheo, maDanhMuc, maSanPham, ngayBatDau, ngayKetThuc, trangThai);
        }

        public bool Delete(int maKhuyenMai)
        {
            return _dal.Delete(maKhuyenMai);
        }
    }
}
