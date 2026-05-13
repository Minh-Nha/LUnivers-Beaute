USE LUnivers_Beaute;
GO

-- =========================================================================
-- PHIẾU TRẢ HÀNG - Hiển thị tất cả phiếu trả hàng
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllPhieuTraHang
AS
BEGIN
    SELECT 
        pt.MaPhieuTra,
        pt.MaHoaDon,
        ch.TenCuaHang,
        ISNULL(kh.HoTen, N'Khách lẻ') AS KhachHang,
        FORMAT(pt.NgayTra, 'dd/MM/yyyy HH:mm') AS NgayTra,
        pt.LyDo
    FROM PhieuTraHang pt
    LEFT JOIN HoaDon hd ON pt.MaHoaDon = hd.MaHoaDon
    LEFT JOIN CuaHang ch ON hd.MaCuaHang = ch.MaCuaHang
    LEFT JOIN KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    ORDER BY pt.NgayTra DESC;
END;
GO
