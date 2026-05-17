using System.Data;
using DAL;

namespace BUS
{
    public class SanPhamBUS
    {
        private SanPhamDAL _dal = new SanPhamDAL();

        public DataTable GetAll(string timKiem = null, int? maDanhMuc = null, int? maThuongHieu = null, int? trangThai = null)
        {
            return _dal.GetAll(timKiem, maDanhMuc, maThuongHieu, trangThai);
        }

        public int Insert(string maSanPham, string tenSanPham, int? maDanhMuc, int? maThuongHieu, int? maNhaCungCap, string hinhAnh, string donViTinh, decimal giaNiemYet, bool trangThai)
        {
            return _dal.Insert(maSanPham, tenSanPham, maDanhMuc, maThuongHieu, maNhaCungCap, hinhAnh, donViTinh, giaNiemYet, trangThai);
        }

        public int Update(string maSanPham, string tenSanPham, int? maDanhMuc, int? maThuongHieu, int? maNhaCungCap, string hinhAnh, string donViTinh, decimal giaNiemYet, bool trangThai)
        {
            return _dal.Update(maSanPham, tenSanPham, maDanhMuc, maThuongHieu, maNhaCungCap, hinhAnh, donViTinh, giaNiemYet, trangThai);
        }

        public int Delete(string maSanPham)
        {
            return _dal.Delete(maSanPham);
        }
    }
}
