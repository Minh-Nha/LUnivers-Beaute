using System;
using System.Data;
using DAL;

namespace BUS
{
    public class ChamCongBUS
    {
        private ChamCongDAL _dal = new ChamCongDAL();

        public DataTable GetAll(DateTime? tuNgay = null, DateTime? denNgay = null, string? maNhanVien = null)
        {
            return _dal.GetAll(tuNgay, denNgay, maNhanVien);
        }

        public int Insert(string maNhanVien, DateTime ngayLam, TimeSpan gioVao, TimeSpan? gioRa = null)
        {
            return _dal.Insert(maNhanVien, ngayLam, gioVao, gioRa);
        }

        public int Update(int maCC, string maNhanVien, DateTime ngayLam, TimeSpan gioVao, TimeSpan? gioRa = null)
        {
            return _dal.Update(maCC, maNhanVien, ngayLam, gioVao, gioRa);
        }

        public int Delete(int maCC)
        {
            return _dal.Delete(maCC);
        }
    }
}
