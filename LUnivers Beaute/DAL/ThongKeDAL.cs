using DTO;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class ThongKeDAL
    {
        public List<TopKhachHangDTO> GetTopKhachHang()
        {
            List<TopKhachHangDTO> list = new List<TopKhachHangDTO>();
            DataTable dt = DatabaseHelpers.GetData("sp_TopKhachHangMuaNhieuNhat");
            
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new TopKhachHangDTO
                {
                    HoTen = row["HoTen"].ToString(),
                    SoDienThoai = row["SoDienThoai"].ToString(),
                    TongTienMua = Convert.ToDecimal(row["TongTienMua"])
                });
            }
            return list;
        }

        public List<DoanhThuNgayDTO> GetDoanhThu7NgayQua()
        {
            List<DoanhThuNgayDTO> list = new List<DoanhThuNgayDTO>();
            DataTable dt = DatabaseHelpers.GetData("sp_DoanhThu7NgayQua");
            
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DoanhThuNgayDTO
                {
                    Ngay = Convert.ToDateTime(row["Ngay"]),
                    DoanhThu = Convert.ToDecimal(row["DoanhThu"])
                });
            }
            return list;
        }
    }
}
