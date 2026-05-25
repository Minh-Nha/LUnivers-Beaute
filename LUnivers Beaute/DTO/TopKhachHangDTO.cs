using System;

namespace DTO
{
    public class TopKhachHangDTO
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public decimal TongTienMua { get; set; }
        
        // For UI purposes
        public string DisplayTongTien => $"{TongTienMua / 1000000.0m:0.#}M";
        public double ProgressValue { get; set; }
        public string ProgressColor { get; set; }
        public string Avatar { get; set; }
        public string AvatarBgColor { get; set; }
    }
}
