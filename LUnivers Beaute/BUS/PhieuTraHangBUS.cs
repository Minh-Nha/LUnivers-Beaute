using System;
using System.Data;
using DAL;

namespace BUS
{
    public class PhieuTraHangBUS
    {
        private PhieuTraHangDAL dal = new PhieuTraHangDAL();

        public DataTable GetAllPhieuTraHang()
        {
            return dal.GetAllPhieuTraHang();
        }

        public DataTable GetChiTietPhieuTra(string maPhieuTra)
        {
            return dal.GetChiTietPhieuTra(maPhieuTra);
        }

        public bool InsertPhieuTraHang(string maPhieuTra, string maHoaDon, DateTime ngayTra, string lyDo, string jsonChiTiet)
        {
            return dal.InsertPhieuTraHang(maPhieuTra, maHoaDon, ngayTra, lyDo, jsonChiTiet);
        }
    }
}
