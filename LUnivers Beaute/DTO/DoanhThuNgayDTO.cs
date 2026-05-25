using System;

namespace DTO
{
    public class DoanhThuNgayDTO
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
        
        // For UI labels
        public string LabelNgay => Ngay.ToString("dd/MM");
    }
}
