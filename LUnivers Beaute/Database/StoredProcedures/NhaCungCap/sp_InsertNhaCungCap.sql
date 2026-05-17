use LUnivers_Beaute
go


CREATE OR ALTER PROCEDURE sp_InsertNhaCungCap
    @TenNhaCungCap NVARCHAR(150),
    @SoDienThoai VARCHAR(15),
    @DiaChi NVARCHAR(255) = NULL
AS
BEGIN
    INSERT INTO NhaCungCap (TenNhaCungCap, SoDienThoai, DiaChi)
    VALUES (@TenNhaCungCap, @SoDienThoai, @DiaChi);
    
    SELECT SCOPE_IDENTITY() AS MaNhaCungCap;
END;
GO
