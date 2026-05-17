USE LUnivers_Beaute;
GO

-- =========================================================================
-- NHÀ CUNG CẤP - Hiển thị tất cả nhà cung cấp
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllNhaCungCap
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SELECT MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi
    FROM NhaCungCap
    WHERE (@SearchTerm IS NULL OR 
           TenNhaCungCap LIKE N'%' + @SearchTerm + N'%' OR
           SoDienThoai LIKE N'%' + @SearchTerm + N'%')
    ORDER BY MaNhaCungCap;
END;
GO
