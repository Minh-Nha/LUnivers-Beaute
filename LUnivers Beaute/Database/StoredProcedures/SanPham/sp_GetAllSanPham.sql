USE LUnivers_Beaute;
GO

-- =========================================================================
-- SẢN PHẨM - Hiển thị tất cả sản phẩm
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllSanPham
AS
BEGIN
    SELECT 
        sp.MaSanPham,
        sp.HinhAnh,
        sp.TenSanPham,
        dm.TenDanhMuc,
        th.TenThuongHieu,
        ncc.TenNhaCungCap,
        sp.DonViTinh,
        FORMAT(sp.GiaNiemYet, 'N0') + N' ₫' AS GiaNiemYet,
        CASE WHEN sp.TrangThai = 1 THEN N'Đang bán' ELSE N'Ngừng bán' END AS TrangThai
    FROM SanPham sp
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    LEFT JOIN NhaCungCap ncc ON sp.MaNhaCungCap = ncc.MaNhaCungCap
    ORDER BY sp.MaSanPham;
END;
GO
