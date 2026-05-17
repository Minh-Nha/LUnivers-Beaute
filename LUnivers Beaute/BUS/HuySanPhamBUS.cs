using System;
using System.Data;
using DAL;

namespace BUS
{
    public class HuySanPhamBUS
    {
        private HuySanPhamDAL dal = new HuySanPhamDAL();

        public DataTable GetAllHuySanPham()
        {
            return dal.GetAllHuySanPham();
        }

        public bool InsertHuySanPham(string maCuaHang, string maNhanVien, int maLo, int soLuong, DateTime ngayHuy, string lyDo)
        {
            return dal.InsertHuySanPham(maCuaHang, maNhanVien, maLo, soLuong, ngayHuy, lyDo);
        }

        public bool AutoHuySanPhamHetHan(string maCuaHang, string maNhanVien)
        {
            return dal.AutoHuySanPhamHetHan(maCuaHang, maNhanVien);
        }

        public DataTable GetSanPhamChoHuy(string maCuaHang)
        {
            return dal.GetSanPhamChoHuy(maCuaHang);
        }
    }
}
