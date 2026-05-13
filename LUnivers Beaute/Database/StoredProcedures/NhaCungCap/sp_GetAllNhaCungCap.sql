USE LUnivers_Beaute;
GO

-- =========================================================================
-- NHÀ CUNG CẤP - Hiển thị tất cả nhà cung cấp
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllNhaCungCap
AS
BEGIN
    SELECT MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi
    FROM NhaCungCap
    ORDER BY MaNhaCungCap;
END;
GO
