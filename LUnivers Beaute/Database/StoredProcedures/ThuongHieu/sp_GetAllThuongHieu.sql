USE LUnivers_Beaute;
GO

-- =========================================================================
-- THƯƠNG HIỆU - Hiển thị tất cả thương hiệu
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllThuongHieu
AS
BEGIN
    SELECT MaThuongHieu, TenThuongHieu, QuocGia
    FROM ThuongHieu
    ORDER BY MaThuongHieu;
END;
GO
