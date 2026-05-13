USE LUnivers_Beaute;
GO

-- =========================================================================
-- KIỂM KÊ - Hiển thị tất cả phiếu kiểm kê theo cửa hàng
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllKiemKe
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        kk.MaKiemKe,
        ch.TenCuaHang,
        sp.TenSanPham + ISNULL(' / ' + lsx.SoLo, '') AS TenSanPham,
        kk.SoLuongHeThong,
        kk.SoLuongThucTe,
        kk.SoLuongLech,
        FORMAT(kk.NgayKiem, 'dd/MM/yyyy HH:mm') AS NgayKiem,
        CASE 
            WHEN kk.SoLuongLech = 0 THEN N'Khớp'
            WHEN kk.SoLuongLech > 0 THEN N'Thừa'
            ELSE N'Thiếu'
        END AS KetQua
    FROM KiemKe kk
    LEFT JOIN CuaHang ch ON kk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN LoSanXuat lsx ON kk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaCuaHang IS NULL OR kk.MaCuaHang = @MaCuaHang)
    ORDER BY kk.NgayKiem DESC;
END;
GO
