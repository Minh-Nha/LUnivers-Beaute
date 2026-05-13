USE LUnivers_Beaute;
GO

-- =========================================================================
-- TỒN KHO - Hiển thị tồn kho theo cửa hàng
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllTonKho
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        ch.TenCuaHang,
        sp.MaSanPham,
        sp.TenSanPham,
        dm.TenDanhMuc,
        th.TenThuongHieu,
        lsx.SoLo,
        FORMAT(lsx.NgaySanXuat, 'dd/MM/yyyy') AS NgaySanXuat,
        FORMAT(lsx.HanSuDung, 'dd/MM/yyyy') AS HanSuDung,
        tk.SoLuongTon,
        CASE 
            WHEN tk.SoLuongTon = 0 THEN N'Hết hàng'
            WHEN tk.SoLuongTon <= 10 THEN N'Sắp hết'
            ELSE N'Còn hàng' END AS TinhTrangKho
    FROM TonKho tk
    LEFT JOIN CuaHang ch ON tk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN LoSanXuat lsx ON tk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    WHERE (@MaCuaHang IS NULL OR tk.MaCuaHang = @MaCuaHang)
    ORDER BY tk.SoLuongTon ASC;
END;
GO
