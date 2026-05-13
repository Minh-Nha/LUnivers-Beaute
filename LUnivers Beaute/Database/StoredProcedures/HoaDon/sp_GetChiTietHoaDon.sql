USE LUnivers_Beaute;
GO

-- =========================================================================
-- CHI TIẾT HÓA ĐƠN - Hiển thị chi tiết hóa đơn
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetChiTietHoaDon
    @MaHoaDon VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        cthd.MaHoaDon,
        sp.TenSanPham,
        lsx.SoLo,
        cthd.SoLuong,
        FORMAT(cthd.DonGia, 'N0') + N' ₫' AS DonGia,
        FORMAT(cthd.ThanhTien, 'N0') + N' ₫' AS ThanhTien
    FROM ChiTietHoaDon cthd
    LEFT JOIN LoSanXuat lsx ON cthd.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaHoaDon IS NULL OR cthd.MaHoaDon = @MaHoaDon)
    ORDER BY cthd.MaHoaDon;
END;
GO
