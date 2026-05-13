USE LUnivers_Beaute;
GO

-- =========================================================================
-- CHI TIẾT NHẬP KHO - Hiển thị chi tiết phiếu nhập kho
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetChiTietNhapKho
    @MaPhieuNhap VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        ctnk.MaPhieuNhap,
        sp.TenSanPham,
        lsx.SoLo,
        FORMAT(lsx.NgaySanXuat, 'dd/MM/yyyy') AS NgaySanXuat,
        FORMAT(lsx.HanSuDung, 'dd/MM/yyyy') AS HanSuDung,
        ctnk.SoLuong,
        FORMAT(ctnk.GiaNhap, 'N0') + N' ₫' AS GiaNhap,
        FORMAT(ctnk.SoLuong * ctnk.GiaNhap, 'N0') + N' ₫' AS ThanhTien
    FROM ChiTietNhapKho ctnk
    LEFT JOIN LoSanXuat lsx ON ctnk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaPhieuNhap IS NULL OR ctnk.MaPhieuNhap = @MaPhieuNhap)
    ORDER BY ctnk.MaPhieuNhap;
END;
GO
