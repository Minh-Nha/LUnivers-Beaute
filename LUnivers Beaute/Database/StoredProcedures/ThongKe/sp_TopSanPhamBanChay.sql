USE LUnivers_Beaute;
GO

-- =========================================================================
-- DASHBOARD: Top sản phẩm bán chạy
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_TopSanPhamBanChay
    @TopN INT = 10,
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT TOP (@TopN)
        sp.MaSanPham,
        sp.TenSanPham,
        th.TenThuongHieu,
        dm.TenDanhMuc,
        SUM(cthd.SoLuong) AS TongBan,
        FORMAT(SUM(cthd.ThanhTien), 'N0') + N' ₫' AS TongDoanhThu
    FROM ChiTietHoaDon cthd
    LEFT JOIN HoaDon hd ON cthd.MaHoaDon = hd.MaHoaDon
    LEFT JOIN LoSanXuat lsx ON cthd.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    WHERE (@MaCuaHang IS NULL OR hd.MaCuaHang = @MaCuaHang)
    GROUP BY sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, dm.TenDanhMuc
    ORDER BY TongBan DESC;
END;
GO
