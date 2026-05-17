use LUnivers_Beaute
go



CREATE OR ALTER PROCEDURE sp_GetAllSanPham
    @TimKiem NVARCHAR(200) = NULL,
    @MaDanhMuc INT = NULL,
    @MaThuongHieu INT = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        sp.MaSanPham,
        sp.HinhAnh,
        sp.TenSanPham,
        sp.MaDanhMuc,
        dm.TenDanhMuc,
        sp.MaThuongHieu,
        th.TenThuongHieu,
        sp.MaNhaCungCap,
        ncc.TenNhaCungCap,
        sp.DonViTinh,
        sp.GiaNiemYet,
        CASE WHEN sp.TrangThai = 1 THEN N'Đang bán' ELSE N'Ngừng bán' END AS TrangThaiStr,
        sp.TrangThai
    FROM SanPham sp
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    LEFT JOIN NhaCungCap ncc ON sp.MaNhaCungCap = ncc.MaNhaCungCap
    WHERE 
        (@TimKiem IS NULL OR @TimKiem = '' OR sp.TenSanPham LIKE N'%' + @TimKiem + N'%' OR sp.MaSanPham LIKE '%' + @TimKiem + '%')
        AND (@MaDanhMuc IS NULL OR @MaDanhMuc = 0 OR sp.MaDanhMuc = @MaDanhMuc)
        AND (@MaThuongHieu IS NULL OR @MaThuongHieu = 0 OR sp.MaThuongHieu = @MaThuongHieu)
        AND (@TrangThai IS NULL OR @TrangThai = -1 OR sp.TrangThai = @TrangThai)
    ORDER BY sp.MaSanPham;
END;
GO
