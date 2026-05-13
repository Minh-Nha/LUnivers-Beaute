USE LUnivers_Beaute;
GO

-- =========================================================================
-- HÓA ĐƠN - Hiển thị tất cả hóa đơn (lọc theo cửa hàng & khoảng ngày)
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllHoaDon
    @MaCuaHang VARCHAR(20) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
AS
BEGIN
    SELECT 
        hd.MaHoaDon,
        ch.TenCuaHang,
        nv.HoTen AS NhanVienLap,
        ISNULL(kh.HoTen, N'Khách lẻ') AS KhachHang,
        km.TenChuongTrinh AS KhuyenMai,
        FORMAT(hd.NgayLap, 'dd/MM/yyyy HH:mm') AS NgayLap,
        hd.PhuongThucThanhToan,
        FORMAT(hd.TongTien, 'N0') + N' ₫' AS TongTien,
        FORMAT(hd.KhachCanTra, 'N0') + N' ₫' AS KhachCanTra
    FROM HoaDon hd
    LEFT JOIN CuaHang ch ON hd.MaCuaHang = ch.MaCuaHang
    LEFT JOIN NhanVien nv ON hd.MaNhanVien = nv.MaNhanVien
    LEFT JOIN KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    LEFT JOIN KhuyenMai km ON hd.MaKhuyenMai = km.MaKhuyenMai
    WHERE 
        (@MaCuaHang IS NULL OR hd.MaCuaHang = @MaCuaHang)
        AND (@TuNgay IS NULL OR CAST(hd.NgayLap AS DATE) >= @TuNgay)
        AND (@DenNgay IS NULL OR CAST(hd.NgayLap AS DATE) <= @DenNgay)
    ORDER BY hd.NgayLap DESC;
END;
GO
