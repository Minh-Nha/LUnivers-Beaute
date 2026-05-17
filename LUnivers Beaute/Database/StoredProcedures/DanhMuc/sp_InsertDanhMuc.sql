CREATE OR ALTER PROCEDURE sp_InsertDanhMuc
    @TenDanhMuc NVARCHAR(100)
AS
BEGIN
    INSERT INTO DanhMuc (TenDanhMuc)
    VALUES (@TenDanhMuc);
    
    SELECT SCOPE_IDENTITY() AS MaDanhMuc;
END;
GO
