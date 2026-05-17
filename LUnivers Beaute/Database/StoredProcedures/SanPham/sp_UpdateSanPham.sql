use LUnivers_Beaute
go

CREATE OR ALTER PROCEDURE sp_UpdateSanPham
    @MaSanPham VARCHAR(50),
    @TenSanPham NVARCHAR(200),
    @MaDanhMuc INT = NULL,
    @MaThuongHieu INT = NULL,
    @MaNhaCungCap INT = NULL,
    @HinhAnh NVARCHAR(MAX) = NULL,
    @DonViTinh NVARCHAR(20) = N'Cái',
    @GiaNiemYet DECIMAL(18,2),
    @TrangThai BIT = 1
AS
BEGIN
    UPDATE SanPham
    SET TenSanPham = @TenSanPham,
        MaDanhMuc = @MaDanhMuc,
        MaThuongHieu = @MaThuongHieu,
        MaNhaCungCap = @MaNhaCungCap,
        HinhAnh = @HinhAnh,
        DonViTinh = ISNULL(@DonViTinh, N'Cái'),
        GiaNiemYet = @GiaNiemYet,
        TrangThai = ISNULL(@TrangThai, 1)
    WHERE MaSanPham = @MaSanPham;
END;
GO
