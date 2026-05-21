USE LUnivers_Beaute;
GO

-- =========================================================================
-- PHIẾU NHẬP KHO - Hiển thị tất cả phiếu nhập kho
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllPhieuNhapKho
AS
BEGIN
    SELECT 
        pnk.MaPhieuNhap,
        pnk.MaCuaHang,
        ch.TenCuaHang,
        nv.HoTen AS NhanVienNhap,
        FORMAT(pnk.NgayNhap, 'dd/MM/yyyy HH:mm') AS NgayNhap,
        FORMAT(pnk.TongTienNhap, 'N0') + N' ₫' AS TongTienNhap
    FROM PhieuNhapKho pnk
    LEFT JOIN CuaHang ch ON pnk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN NhanVien nv ON pnk.MaNhanVien = nv.MaNhanVien
    ORDER BY pnk.NgayNhap DESC;
END;
GO
