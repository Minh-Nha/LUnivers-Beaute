USE LUnivers_Beaute;
GO

-- =========================================================================
-- HỦY SẢN PHẨM - Hiển thị tất cả sản phẩm đã hủy theo cửa hàng
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllHuySanPham
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        hsp.MaHuy,
        ch.TenCuaHang,
        nv.HoTen AS NhanVienHuy,
        sp.TenSanPham + ISNULL(' / ' + lsx.SoLo, '') AS TenSanPham,
        hsp.SoLuong,
        hsp.LyDo,
        FORMAT(hsp.NgayHuy, 'dd/MM/yyyy HH:mm') AS NgayHuy
    FROM HuySanPham hsp
    LEFT JOIN CuaHang ch ON hsp.MaCuaHang = ch.MaCuaHang
    LEFT JOIN NhanVien nv ON hsp.MaNhanVien = nv.MaNhanVien
    LEFT JOIN LoSanXuat lsx ON hsp.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaCuaHang IS NULL OR hsp.MaCuaHang = @MaCuaHang)
    ORDER BY hsp.NgayHuy DESC;
END;
GO
