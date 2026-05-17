USE LUnivers_Beaute;
GO


CREATE OR ALTER PROCEDURE sp_DeleteSanPham
    @MaSanPham VARCHAR(50)
AS
BEGIN
    DELETE FROM SanPham
    WHERE MaSanPham = @MaSanPham;
END;
GO
