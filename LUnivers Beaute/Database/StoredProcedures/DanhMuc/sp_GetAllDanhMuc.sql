USE LUnivers_Beaute;
GO

-- =========================================================================
-- DANH MỤC - Hiển thị tất cả danh mục sản phẩm
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllDanhMuc
    @SearchTerm NVARCHAR(50) = NULL
AS
BEGIN
    SELECT MaDanhMuc, TenDanhMuc
    FROM DanhMuc
    WHERE (@SearchTerm IS NULL OR TenDanhMuc LIKE N'%' + @SearchTerm + N'%')
    ORDER BY MaDanhMuc;
END;
GO
