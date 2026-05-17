use LUnivers_Beaute
go



CREATE OR ALTER PROCEDURE sp_InsertSanPham
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
    INSERT INTO SanPham (MaSanPham, TenSanPham, MaDanhMuc, MaThuongHieu, MaNhaCungCap, HinhAnh, DonViTinh, GiaNiemYet, TrangThai)
    VALUES (@MaSanPham, @TenSanPham, @MaDanhMuc, @MaThuongHieu, @MaNhaCungCap, @HinhAnh, ISNULL(@DonViTinh, N'Cái'), @GiaNiemYet, ISNULL(@TrangThai, 1));
END;
GO
