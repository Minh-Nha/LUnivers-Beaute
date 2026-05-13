USE LUnivers_Beaute;
GO

-- =========================================================================
-- CHI TIẾT PHIẾU TRẢ HÀNG - Hiển thị chi tiết phiếu trả hàng
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetChiTietPhieuTraHang
    @MaPhieuTra VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        ctpt.MaPhieuTra,
        sp.TenSanPham,
        lsx.SoLo,
        ctpt.SoLuongTra,
        FORMAT(ctpt.SoTienHoan, 'N0') + N' ₫' AS SoTienHoan
    FROM ChiTietPhieuTraHang ctpt
    LEFT JOIN LoSanXuat lsx ON ctpt.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaPhieuTra IS NULL OR ctpt.MaPhieuTra = @MaPhieuTra)
    ORDER BY ctpt.MaPhieuTra;
END;
GO
