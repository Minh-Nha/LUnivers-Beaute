USE LUnivers_Beaute;
GO

-- =========================================================================
-- KHÁCH HÀNG - Hiển thị tất cả khách hàng
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllKhachHang
AS
BEGIN
    SELECT 
        MaKhachHang,
        HoTen,
        SoDienThoai,
        DiemTichLuy,
        CASE WHEN TrangThai = 1 THEN N'Hoạt động' ELSE N'Khóa' END AS TrangThai
    FROM KhachHang
    ORDER BY MaKhachHang;
END;
GO
