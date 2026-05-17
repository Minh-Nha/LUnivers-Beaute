USE LUnivers_Beaute;
GO

-- =========================================================================
-- NHÂN VIÊN - CRUD Stored Procedures
-- =========================================================================

-- 1. GET ALL (with search & filter)
CREATE OR ALTER PROCEDURE sp_GetAllNhanVien
    @TimKiem NVARCHAR(100) = NULL,
    @MaCuaHang VARCHAR(20) = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        nv.MaNhanVien,
        nv.HoTen,
        nv.SoDienThoai,
        nv.VaiTro,
        nv.TenDangNhap,
        nv.MaCuaHang,
        ch.TenCuaHang,
        CASE WHEN nv.TrangThai = 1 THEN N'Đang làm' ELSE N'Nghỉ việc' END AS TrangThai
    FROM NhanVien nv
    LEFT JOIN CuaHang ch ON nv.MaCuaHang = ch.MaCuaHang
    WHERE (@TimKiem IS NULL OR nv.HoTen LIKE N'%' + @TimKiem + N'%' OR nv.SoDienThoai LIKE '%' + @TimKiem + '%' OR nv.MaNhanVien LIKE '%' + @TimKiem + '%')
      AND (@MaCuaHang IS NULL OR nv.MaCuaHang = @MaCuaHang)
      AND (@TrangThai IS NULL OR nv.TrangThai = @TrangThai)
    ORDER BY nv.MaNhanVien DESC;
END;
GO

-- 2. INSERT (auto-generate MaNhanVien)
CREATE OR ALTER PROCEDURE sp_InsertNhanVien
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @VaiTro NVARCHAR(50),
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(255),
    @MaCuaHang VARCHAR(20),
    @TrangThai BIT = 1
AS
BEGIN
    DECLARE @MaNhanVien VARCHAR(20);
    DECLARE @MaxNum INT = 0;
    
    SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2) AS INT)), 0)
    FROM NhanVien
    WHERE MaNhanVien LIKE 'NV%' AND ISNUMERIC(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 1;
    
    SET @MaNhanVien = 'NV' + RIGHT('000' + CAST(@MaxNum + 1 AS VARCHAR), 3);

    INSERT INTO NhanVien (MaNhanVien, HoTen, SoDienThoai, VaiTro, TenDangNhap, MatKhau, MaCuaHang, TrangThai)
    VALUES (@MaNhanVien, @HoTen, @SoDienThoai, @VaiTro, @TenDangNhap, @MatKhau, @MaCuaHang, @TrangThai);
    
    SELECT @MaNhanVien AS MaNhanVien;
END;
GO

-- 3. UPDATE
CREATE OR ALTER PROCEDURE sp_UpdateNhanVien
    @MaNhanVien VARCHAR(20),
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @VaiTro NVARCHAR(50),
    @TenDangNhap VARCHAR(50),
    @MaCuaHang VARCHAR(20),
    @TrangThai BIT
AS
BEGIN
    UPDATE NhanVien
    SET HoTen = @HoTen,
        SoDienThoai = @SoDienThoai,
        VaiTro = @VaiTro,
        TenDangNhap = @TenDangNhap,
        MaCuaHang = @MaCuaHang,
        TrangThai = @TrangThai
    WHERE MaNhanVien = @MaNhanVien;
END;
GO

-- 4. DELETE
CREATE OR ALTER PROCEDURE sp_DeleteNhanVien
    @MaNhanVien VARCHAR(20)
AS
BEGIN
    DELETE FROM NhanVien WHERE MaNhanVien = @MaNhanVien;
END;
GO
