using System.Data;
using DAL;

namespace BUS
{
    public class CuaHangBUS
    {
        private CuaHangDAL _dal = new CuaHangDAL();

        public DataTable GetAll(string? timKiem = null, int? trangThai = null)
        {
            return _dal.GetAll(timKiem, trangThai);
        }

        public int Insert(string tenCuaHang, string diaChi, string soDienThoai, bool trangThai)
        {
            return _dal.Insert(tenCuaHang, diaChi, soDienThoai, trangThai);
        }

        public int Update(string maCuaHang, string tenCuaHang, string diaChi, string soDienThoai, bool trangThai)
        {
            return _dal.Update(maCuaHang, tenCuaHang, diaChi, soDienThoai, trangThai);
        }

        public int Delete(string maCuaHang)
        {
            return _dal.Delete(maCuaHang);
        }
    }
}
