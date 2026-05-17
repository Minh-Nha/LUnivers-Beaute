CREATE OR ALTER PROCEDURE sp_UpdateDanhMuc
    @MaDanhMuc INT,
    @TenDanhMuc NVARCHAR(100)
AS
BEGIN
    UPDATE DanhMuc
    SET TenDanhMuc = @TenDanhMuc
    WHERE MaDanhMuc = @MaDanhMuc;
END;
GO
