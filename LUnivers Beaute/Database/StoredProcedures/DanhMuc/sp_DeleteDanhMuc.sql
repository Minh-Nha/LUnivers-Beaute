CREATE OR ALTER PROCEDURE sp_DeleteDanhMuc
    @MaDanhMuc INT
AS
BEGIN
    DELETE FROM DanhMuc
    WHERE MaDanhMuc = @MaDanhMuc;
END;
GO
