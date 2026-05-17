USE LUnivers_Beaute;
GO

-- 1. Lấy danh sách Phiếu Trả Hàng
CREATE OR ALTER PROCEDURE sp_GetAllPhieuTraHang
AS
BEGIN
    SELECT MaPhieuTra, MaHoaDon, NgayTra, LyDo
    FROM PhieuTraHang
    ORDER BY NgayTra DESC;
END
GO

-- 2. Lấy danh sách Chi Tiết Phiếu Trả theo mã
CREATE OR ALTER PROCEDURE sp_GetChiTietPhieuTra
    @MaPhieuTra VARCHAR(20)
AS
BEGIN
    SELECT 
        sp.TenSanPham, 
        c.MaLo AS SoLo, 
        c.SoLuongTra, 
        c.SoTienHoan
    FROM ChiTietPhieuTraHang c
    JOIN LoSanXuat l ON c.MaLo = l.MaLo
    JOIN SanPham sp ON l.MaSanPham = sp.MaSanPham
    WHERE c.MaPhieuTra = @MaPhieuTra;
END
GO

-- 3. Tạo mới Phiếu Trả Hàng & Chi tiết
-- Sử dụng JSON để truyền danh sách chi tiết (Tương tự như Nhập Kho và Bán Hàng)
CREATE OR ALTER PROCEDURE sp_InsertPhieuTraHang
    @MaPhieuTra VARCHAR(20),
    @MaHoaDon VARCHAR(20),
    @NgayTra DATETIME,
    @LyDo NVARCHAR(255),
    @JsonChiTiet NVARCHAR(MAX)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Insert Phiếu Trả
        INSERT INTO PhieuTraHang (MaPhieuTra, MaHoaDon, NgayTra, LyDo)
        VALUES (@MaPhieuTra, @MaHoaDon, @NgayTra, @LyDo);

        -- 2. Insert Chi tiết phiếu trả từ JSON
        INSERT INTO ChiTietPhieuTraHang (MaPhieuTra, MaLo, SoLuongTra, SoTienHoan)
        SELECT 
            @MaPhieuTra,
            JSON_VALUE(value, '$.MaLo'),
            JSON_VALUE(value, '$.SoLuongTra'),
            JSON_VALUE(value, '$.SoTienHoan')
        FROM OPENJSON(@JsonChiTiet);

        -- 3. Cập nhật lại số lượng Tồn Kho (Vì khách trả hàng thì nhập lại vào kho)
        -- Chú ý: Ở đây ta cần biết cửa hàng nào xuất hóa đơn này để cộng lại tồn kho cho đúng cửa hàng đó.
        DECLARE @MaCuaHang VARCHAR(20);
        SELECT @MaCuaHang = MaCuaHang FROM HoaDon WHERE MaHoaDon = @MaHoaDon;

        -- Tăng số lượng tồn kho cho từng lô
        UPDATE tk
        SET tk.SoLuongTon = tk.SoLuongTon + c.SoLuongTra
        FROM TonKho tk
        INNER JOIN (
            SELECT 
                CAST(JSON_VALUE(value, '$.MaLo') AS INT) AS MaLo,
                CAST(JSON_VALUE(value, '$.SoLuongTra') AS INT) AS SoLuongTra
            FROM OPENJSON(@JsonChiTiet)
        ) c ON tk.MaLo = c.MaLo
        WHERE tk.MaCuaHang = @MaCuaHang;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
