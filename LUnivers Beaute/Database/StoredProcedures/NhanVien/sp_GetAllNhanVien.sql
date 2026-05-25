ALTER PROCEDURE sp_GetAllNhanVien
    @TimKiem NVARCHAR(100) = NULL,
    @MaCuaHang VARCHAR(20) = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        nv.MaNhanVien,
        nv.HoTen,
        nv.SoDienThoai,
        nv.Email,
        nv.VaiTro,
        nv.TenDangNhap,
        nv.MaCuaHang,
        ch.TenCuaHang,
        CASE WHEN nv.TrangThai = 1 THEN N'Đang làm' ELSE N'Nghỉ việc' END AS TrangThai
    FROM NhanVien nv
    LEFT JOIN CuaHang ch ON nv.MaCuaHang = ch.MaCuaHang
    WHERE (@TimKiem IS NULL OR nv.HoTen LIKE N'%' + @TimKiem + N'%' OR nv.SoDienThoai LIKE '%' + @TimKiem + '%' OR nv.MaNhanVien LIKE '%' + @TimKiem + '%')
      AND (@MaCuaHang IS NULL OR nv.MaCuaHang = @MaCuaHang)
      AND (@TrangThai IS NULL OR nv.TrangThai = @TrangThai)
    ORDER BY nv.MaNhanVien DESC;
END;
GO
