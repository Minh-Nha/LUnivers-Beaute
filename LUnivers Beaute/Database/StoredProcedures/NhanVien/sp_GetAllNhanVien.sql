USE LUnivers_Beaute;
GO

-- =========================================================================
-- NHÂN VIÊN - Hiển thị tất cả nhân viên
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllNhanVien
AS
BEGIN
    SELECT 
        nv.MaNhanVien,
        nv.HoTen,
        nv.SoDienThoai,
        nv.VaiTro,
        nv.TenDangNhap,
        ch.TenCuaHang,
        CASE WHEN nv.TrangThai = 1 THEN N'Đang làm' ELSE N'Nghỉ việc' END AS TrangThai
    FROM NhanVien nv
    LEFT JOIN CuaHang ch ON nv.MaCuaHang = ch.MaCuaHang
    ORDER BY nv.MaNhanVien;
END;
GO
