using System.Data;

namespace DAL
{
    public class SanPhamDAL
    {
        public DataTable GetAll(string timKiem = null, int? maDanhMuc = null, int? maThuongHieu = null, int? trangThai = null)
        {
            return DatabaseHelpers.GetData("sp_GetAllSanPham",
                new Microsoft.Data.SqlClient.SqlParameter("@TimKiem", (object)timKiem ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", (object)maDanhMuc ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaThuongHieu", (object)maThuongHieu ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", (object)trangThai ?? System.DBNull.Value));
        }

        public int Insert(string maSanPham, string tenSanPham, int? maDanhMuc, int? maThuongHieu, int? maNhaCungCap, string hinhAnh, string donViTinh, decimal giaNiemYet, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertSanPham",
                new Microsoft.Data.SqlClient.SqlParameter("@MaSanPham", maSanPham),
                new Microsoft.Data.SqlClient.SqlParameter("@TenSanPham", tenSanPham),
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", (object)maDanhMuc ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaThuongHieu", (object)maThuongHieu ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaNhaCungCap", (object)maNhaCungCap ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@HinhAnh", (object)hinhAnh ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@DonViTinh", donViTinh),
                new Microsoft.Data.SqlClient.SqlParameter("@GiaNiemYet", giaNiemYet),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", trangThai));
        }

        public int Update(string maSanPham, string tenSanPham, int? maDanhMuc, int? maThuongHieu, int? maNhaCungCap, string hinhAnh, string donViTinh, decimal giaNiemYet, bool trangThai)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateSanPham",
                new Microsoft.Data.SqlClient.SqlParameter("@MaSanPham", maSanPham),
                new Microsoft.Data.SqlClient.SqlParameter("@TenSanPham", tenSanPham),
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", (object)maDanhMuc ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaThuongHieu", (object)maThuongHieu ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaNhaCungCap", (object)maNhaCungCap ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@HinhAnh", (object)hinhAnh ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@DonViTinh", donViTinh),
                new Microsoft.Data.SqlClient.SqlParameter("@GiaNiemYet", giaNiemYet),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", trangThai));
        }

        public int Delete(string maSanPham)
        {
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteSanPham",
                new Microsoft.Data.SqlClient.SqlParameter("@MaSanPham", maSanPham));
        }
    }
}
