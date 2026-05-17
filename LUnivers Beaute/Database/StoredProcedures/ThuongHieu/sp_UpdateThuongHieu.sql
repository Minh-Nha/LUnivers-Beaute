USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_UpdateThuongHieu
    @MaThuongHieu INT,
    @TenThuongHieu NVARCHAR(100),
    @QuocGia NVARCHAR(50) = NULL
AS
BEGIN
    UPDATE ThuongHieu
    SET TenThuongHieu = @TenThuongHieu,
        QuocGia = ISNULL(@QuocGia, N'Chưa xác định')
    WHERE MaThuongHieu = @MaThuongHieu;
END;
GO
