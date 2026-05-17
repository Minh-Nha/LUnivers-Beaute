USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_DeleteThuongHieu
    @MaThuongHieu INT
AS
BEGIN
    DELETE FROM ThuongHieu
    WHERE MaThuongHieu = @MaThuongHieu;
END;
GO
