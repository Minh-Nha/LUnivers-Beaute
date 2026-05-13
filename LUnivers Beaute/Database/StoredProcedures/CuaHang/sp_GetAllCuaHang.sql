USE LUnivers_Beaute;
GO

-- =========================================================================
-- CỬA HÀNG - Hiển thị tất cả cửa hàng
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllCuaHang
AS
BEGIN
    SELECT 
        MaCuaHang,
        TenCuaHang,
        DiaChi,
        SoDienThoai,
        CASE WHEN TrangThai = 1 THEN N'Đang hoạt động' ELSE N'Ngừng hoạt động' END AS TrangThai
    FROM CuaHang
    ORDER BY MaCuaHang;
END;
GO
