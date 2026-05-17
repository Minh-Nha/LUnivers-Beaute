USE LUnivers_Beaute;
GO

-- =========================================================================
-- KHÁCH HÀNG - CRUD Stored Procedures
-- =========================================================================

-- 1. GET ALL (with search & filter)
CREATE OR ALTER PROCEDURE sp_GetAllKhachHang
    @TimKiem NVARCHAR(100) = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        MaKhachHang,
        HoTen,
        SoDienThoai,
        DiemTichLuy,
        CASE WHEN TrangThai = 1 THEN N'Hoạt động' ELSE N'Khóa' END AS TrangThai
    FROM KhachHang
    WHERE (@TimKiem IS NULL OR HoTen LIKE N'%' + @TimKiem + N'%' OR SoDienThoai LIKE '%' + @TimKiem + '%')
      AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
    ORDER BY MaKhachHang DESC;
END;
GO

-- 2. INSERT
CREATE OR ALTER PROCEDURE sp_InsertKhachHang
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @DiemTichLuy INT = 0,
    @TrangThai BIT = 1
AS
BEGIN
    INSERT INTO KhachHang (HoTen, SoDienThoai, DiemTichLuy, TrangThai)
    VALUES (@HoTen, @SoDienThoai, @DiemTichLuy, @TrangThai);
END;
GO

-- 3. UPDATE
CREATE OR ALTER PROCEDURE sp_UpdateKhachHang
    @MaKhachHang INT,
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @DiemTichLuy INT,
    @TrangThai BIT
AS
BEGIN
    UPDATE KhachHang
    SET HoTen = @HoTen,
        SoDienThoai = @SoDienThoai,
        DiemTichLuy = @DiemTichLuy,
        TrangThai = @TrangThai
    WHERE MaKhachHang = @MaKhachHang;
END;
GO

-- 4. DELETE
CREATE OR ALTER PROCEDURE sp_DeleteKhachHang
    @MaKhachHang INT
AS
BEGIN
    DELETE FROM KhachHang WHERE MaKhachHang = @MaKhachHang;
END;
GO
