USE LUnivers_Beaute;
GO

-- =========================================================================
-- KIỂM KÊ KHO - Lấy tất cả phiếu kiểm kê
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllKiemKe
AS
BEGIN
    SELECT 
        kk.MaKiemKe,
        ch.TenCuaHang,
        sp.TenSanPham,
        lsx.SoLo,
        kk.SoLuongHeThong,
        kk.SoLuongThucTe,
        kk.SoLuongLech,
        FORMAT(kk.NgayKiem, 'dd/MM/yyyy HH:mm') AS NgayKiem,
        CASE 
            WHEN kk.SoLuongLech = 0 THEN N'Khớp'
            WHEN kk.SoLuongLech > 0 THEN N'Thừa'
            ELSE N'Thiếu'
        END AS TinhTrang
    FROM KiemKe kk
    LEFT JOIN CuaHang ch ON kk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN LoSanXuat lsx ON kk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    ORDER BY kk.NgayKiem DESC;
END;
GO
