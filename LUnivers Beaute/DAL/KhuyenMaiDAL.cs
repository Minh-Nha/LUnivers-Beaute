using System.Data;

namespace DAL
{
    public class KhuyenMaiDAL
    {
        public DataTable GetAll(string tuKhoa = null, int? maDanhMuc = null, string loaiGiam = null, DateTime? tuNgay = null, DateTime? denNgay = null, int trangThaiLoc = 0)
        {
            Microsoft.Data.SqlClient.SqlParameter[] parameters = new Microsoft.Data.SqlClient.SqlParameter[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@TuKhoa", string.IsNullOrEmpty(tuKhoa) ? (object)DBNull.Value : tuKhoa),
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", maDanhMuc ?? (object)DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@LoaiGiam", string.IsNullOrEmpty(loaiGiam) ? (object)DBNull.Value : loaiGiam),
                new Microsoft.Data.SqlClient.SqlParameter("@TuNgay", tuNgay.HasValue ? (object)tuNgay.Value : DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@DenNgay", denNgay.HasValue ? (object)denNgay.Value : DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThaiLoc", trangThaiLoc)
            };
            return DatabaseHelpers.GetData("sp_GetAllKhuyenMai", parameters);
        }

        public bool Insert(string tenChuongTrinh, string loaiGiam, double giaTriGiam, string apDungTheo, object maDanhMuc, object maSanPham, System.DateTime ngayBatDau, System.DateTime ngayKetThuc, bool trangThai)
        {
            Microsoft.Data.SqlClient.SqlParameter[] parameters = new Microsoft.Data.SqlClient.SqlParameter[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@TenChuongTrinh", tenChuongTrinh),
                new Microsoft.Data.SqlClient.SqlParameter("@LoaiGiam", loaiGiam),
                new Microsoft.Data.SqlClient.SqlParameter("@GiaTriGiam", giaTriGiam),
                new Microsoft.Data.SqlClient.SqlParameter("@ApDungTheo", apDungTheo),
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", maDanhMuc ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaSanPham", maSanPham ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@NgayBatDau", ngayBatDau.ToString("yyyy-MM-dd")),
                new Microsoft.Data.SqlClient.SqlParameter("@NgayKetThuc", ngayKetThuc.ToString("yyyy-MM-dd")),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", trangThai ? 1 : 0)
            };
            return DatabaseHelpers.ExecuteNonQuery("sp_InsertKhuyenMai", parameters) > 0;
        }

        public bool Update(int maKhuyenMai, string tenChuongTrinh, string loaiGiam, double giaTriGiam, string apDungTheo, object maDanhMuc, object maSanPham, System.DateTime ngayBatDau, System.DateTime ngayKetThuc, bool trangThai)
        {
            Microsoft.Data.SqlClient.SqlParameter[] parameters = new Microsoft.Data.SqlClient.SqlParameter[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@MaKhuyenMai", maKhuyenMai),
                new Microsoft.Data.SqlClient.SqlParameter("@TenChuongTrinh", tenChuongTrinh),
                new Microsoft.Data.SqlClient.SqlParameter("@LoaiGiam", loaiGiam),
                new Microsoft.Data.SqlClient.SqlParameter("@GiaTriGiam", giaTriGiam),
                new Microsoft.Data.SqlClient.SqlParameter("@ApDungTheo", apDungTheo),
                new Microsoft.Data.SqlClient.SqlParameter("@MaDanhMuc", maDanhMuc ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@MaSanPham", maSanPham ?? System.DBNull.Value),
                new Microsoft.Data.SqlClient.SqlParameter("@NgayBatDau", ngayBatDau.ToString("yyyy-MM-dd")),
                new Microsoft.Data.SqlClient.SqlParameter("@NgayKetThuc", ngayKetThuc.ToString("yyyy-MM-dd")),
                new Microsoft.Data.SqlClient.SqlParameter("@TrangThai", trangThai ? 1 : 0)
            };
            return DatabaseHelpers.ExecuteNonQuery("sp_UpdateKhuyenMai", parameters) > 0;
        }

        public bool Delete(int maKhuyenMai)
        {
            Microsoft.Data.SqlClient.SqlParameter[] parameters = new Microsoft.Data.SqlClient.SqlParameter[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@MaKhuyenMai", maKhuyenMai)
            };
            return DatabaseHelpers.ExecuteNonQuery("sp_DeleteKhuyenMai", parameters) > 0;
        }
    }
}
