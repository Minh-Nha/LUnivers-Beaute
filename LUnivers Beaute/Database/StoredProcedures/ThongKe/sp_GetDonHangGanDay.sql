USE LUnivers_Beaute;
GO

-- =========================================================================
-- DASHBOARD: Đơn hàng gần đây
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetDonHangGanDay
    @TopN INT = 5,
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT TOP (@TopN)
        hd.MaHoaDon,
        ISNULL(kh.HoTen, N'Khách lẻ') AS KhachHang,
        FORMAT(hd.NgayLap, 'dd/MM/yyyy HH:mm') AS NgayLap,
        hd.PhuongThucThanhToan,
        FORMAT(hd.TongTien, 'N0') + N' ₫' AS TongTien
    FROM HoaDon hd
    LEFT JOIN KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    WHERE (@MaCuaHang IS NULL OR hd.MaCuaHang = @MaCuaHang)
    ORDER BY hd.NgayLap DESC;
END;
GO
