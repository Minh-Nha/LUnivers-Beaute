USE LUnivers_Beaute;
GO

-- =========================================================================
-- DANH MỤC - Hiển thị tất cả danh mục sản phẩm
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllDanhMuc
AS
BEGIN
    SELECT MaDanhMuc, TenDanhMuc
    FROM DanhMuc
    ORDER BY MaDanhMuc;
END;
GO
