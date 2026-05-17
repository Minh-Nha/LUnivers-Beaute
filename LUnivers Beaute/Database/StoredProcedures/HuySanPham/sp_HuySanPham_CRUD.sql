USE LUnivers_Beaute;
GO

-- 1. Lấy danh sách Hủy Sản Phẩm
CREATE OR ALTER PROCEDURE sp_GetAllHuySanPham
AS
BEGIN
    SELECT h.MaHuy, 
           c.TenCuaHang, 
           n.HoTen AS NhanVienHuy, 
           sp.TenSanPham + ' - Lô: ' + CAST(l.MaLo AS VARCHAR) AS TenSanPham, 
           h.SoLuong, 
           h.NgayHuy, 
           h.LyDo
    FROM HuySanPham h
    JOIN CuaHang c ON h.MaCuaHang = c.MaCuaHang
    JOIN NhanVien n ON h.MaNhanVien = n.MaNhanVien
    JOIN LoSanXuat l ON h.MaLo = l.MaLo
    JOIN SanPham sp ON l.MaSanPham = sp.MaSanPham
    ORDER BY h.NgayHuy DESC;
END
GO

-- 2. Thêm phiếu hủy sản phẩm
CREATE OR ALTER PROCEDURE sp_InsertHuySanPham
    @MaCuaHang VARCHAR(20),
    @MaNhanVien VARCHAR(20),
    @MaLo INT,
    @SoLuong INT,
    @NgayHuy DATETIME,
    @LyDo NVARCHAR(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Thêm phiếu hủy
        INSERT INTO HuySanPham (MaCuaHang, MaNhanVien, MaLo, SoLuong, NgayHuy, LyDo)
        VALUES (@MaCuaHang, @MaNhanVien, @MaLo, @SoLuong, @NgayHuy, @LyDo);

        -- Trừ số lượng tồn kho
        IF EXISTS (SELECT 1 FROM TonKho WHERE MaCuaHang = @MaCuaHang AND MaLo = @MaLo)
        BEGIN
            UPDATE TonKho
            SET SoLuongTon = SoLuongTon - @SoLuong
            WHERE MaCuaHang = @MaCuaHang AND MaLo = @MaLo;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 3. Lấy danh sách sản phẩm chờ hủy (Sản phẩm đã quá hạn nhưng vẫn còn tồn kho)
CREATE OR ALTER PROCEDURE sp_GetSanPhamChoHuy
    @MaCuaHang VARCHAR(20)
AS
BEGIN
    SELECT tk.MaLo, 
           tk.MaCuaHang,
           sp.TenSanPham, 
           l.SoLo, 
           tk.SoLuongTon, 
           l.HanSuDung
    FROM TonKho tk
    JOIN LoSanXuat l ON tk.MaLo = l.MaLo
    JOIN SanPham sp ON l.MaSanPham = sp.MaSanPham
    WHERE tk.MaCuaHang = @MaCuaHang 
      AND tk.SoLuongTon > 0 
      AND l.HanSuDung < GETDATE()
    ORDER BY l.HanSuDung ASC;
END
GO
