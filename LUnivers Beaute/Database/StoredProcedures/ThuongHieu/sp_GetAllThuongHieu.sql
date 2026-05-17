USE LUnivers_Beaute;
GO

-- =========================================================================
-- THƯƠNG HIỆU - Hiển thị tất cả thương hiệu
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllThuongHieu
    @SearchTerm NVARCHAR(50) = NULL
AS
BEGIN
    SELECT MaThuongHieu, TenThuongHieu, QuocGia
    FROM ThuongHieu
    WHERE (@SearchTerm IS NULL OR 
           TenThuongHieu LIKE N'%' + @SearchTerm + N'%' OR
           QuocGia LIKE N'%' + @SearchTerm + N'%')
    ORDER BY MaThuongHieu;
END;
GO
