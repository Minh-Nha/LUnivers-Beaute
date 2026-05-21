CREATE OR ALTER PROCEDURE [dbo].[sp_Login]
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(255)
AS
BEGIN
    SELECT n.MaNhanVien, n.HoTen, n.VaiTro, n.MaCuaHang, c.TenCuaHang
    FROM NhanVien n
    JOIN CuaHang c ON n.MaCuaHang = c.MaCuaHang
    WHERE n.TenDangNhap = @TenDangNhap 
      AND n.MatKhau = @MatKhau 
      AND n.TrangThai = 1;
END;
GO
