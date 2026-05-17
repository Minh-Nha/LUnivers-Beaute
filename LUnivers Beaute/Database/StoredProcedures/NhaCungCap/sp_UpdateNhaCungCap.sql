use LUnivers_Beaute
go

CREATE OR ALTER PROCEDURE sp_UpdateNhaCungCap
    @MaNhaCungCap INT,
    @TenNhaCungCap NVARCHAR(150),
    @SoDienThoai VARCHAR(15),
    @DiaChi NVARCHAR(255) = NULL
AS
BEGIN
    UPDATE NhaCungCap
    SET TenNhaCungCap = @TenNhaCungCap,
        SoDienThoai = @SoDienThoai,
        DiaChi = @DiaChi
    WHERE MaNhaCungCap = @MaNhaCungCap;
END;
GO
