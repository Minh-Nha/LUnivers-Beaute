USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_InsertThuongHieu
    @TenThuongHieu NVARCHAR(100),
    @QuocGia NVARCHAR(50) = NULL
AS
BEGIN
    INSERT INTO ThuongHieu (TenThuongHieu, QuocGia)
    VALUES (@TenThuongHieu, ISNULL(@QuocGia, N'Chưa xác định'));
    
    SELECT SCOPE_IDENTITY() AS MaThuongHieu;
END;
GO
