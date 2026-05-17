USE LUnivers_Beaute;
GO

-- =========================================================================
-- CỬA HÀNG - CRUD Stored Procedures
-- =========================================================================

-- 1. GET ALL
CREATE OR ALTER PROCEDURE sp_GetAllCuaHang
    @TimKiem NVARCHAR(100) = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        MaCuaHang,
        TenCuaHang,
        DiaChi,
        SoDienThoai,
        CASE WHEN TrangThai = 1 THEN N'Hoạt động' ELSE N'Ngừng hoạt động' END AS TrangThai
    FROM CuaHang
    WHERE (@TimKiem IS NULL OR TenCuaHang LIKE N'%' + @TimKiem + N'%' OR MaCuaHang LIKE '%' + @TimKiem + '%' OR SoDienThoai LIKE '%' + @TimKiem + '%')
      AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
    ORDER BY MaCuaHang DESC;
END;
GO

-- 2. INSERT
CREATE OR ALTER PROCEDURE sp_InsertCuaHang
    @TenCuaHang NVARCHAR(100),
    @DiaChi NVARCHAR(255),
    @SoDienThoai VARCHAR(15),
    @TrangThai BIT = 1
AS
BEGIN
    -- Kiểm tra lỗi unique
    IF EXISTS (SELECT 1 FROM CuaHang WHERE TenCuaHang = @TenCuaHang)
    BEGIN
        RAISERROR(N'Tên cửa hàng đã tồn tại!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM CuaHang WHERE SoDienThoai = @SoDienThoai)
    BEGIN
        RAISERROR(N'Số điện thoại đã tồn tại!', 16, 1);
        RETURN;
    END

    DECLARE @MaCuaHang VARCHAR(20);
    DECLARE @MaxNum INT = 0;
    
    SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaCuaHang, 3, LEN(MaCuaHang) - 2) AS INT)), 0)
    FROM CuaHang
    WHERE MaCuaHang LIKE 'CH%' AND ISNUMERIC(SUBSTRING(MaCuaHang, 3, LEN(MaCuaHang) - 2)) = 1;
    
    SET @MaCuaHang = 'CH' + RIGHT('000' + CAST(@MaxNum + 1 AS VARCHAR), 3);

    INSERT INTO CuaHang (MaCuaHang, TenCuaHang, DiaChi, SoDienThoai, TrangThai)
    VALUES (@MaCuaHang, @TenCuaHang, @DiaChi, @SoDienThoai, @TrangThai);
    
    SELECT @MaCuaHang AS MaCuaHang;
END;
GO

-- 3. UPDATE
CREATE OR ALTER PROCEDURE sp_UpdateCuaHang
    @MaCuaHang VARCHAR(20),
    @TenCuaHang NVARCHAR(100),
    @DiaChi NVARCHAR(255),
    @SoDienThoai VARCHAR(15),
    @TrangThai BIT
AS
BEGIN
    -- Kiểm tra lỗi unique
    IF EXISTS (SELECT 1 FROM CuaHang WHERE TenCuaHang = @TenCuaHang AND MaCuaHang != @MaCuaHang)
    BEGIN
        RAISERROR(N'Tên cửa hàng đã tồn tại!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM CuaHang WHERE SoDienThoai = @SoDienThoai AND MaCuaHang != @MaCuaHang)
    BEGIN
        RAISERROR(N'Số điện thoại đã tồn tại!', 16, 1);
        RETURN;
    END

    UPDATE CuaHang
    SET TenCuaHang = @TenCuaHang,
        DiaChi = @DiaChi,
        SoDienThoai = @SoDienThoai,
        TrangThai = @TrangThai
    WHERE MaCuaHang = @MaCuaHang;
END;
GO

-- 4. DELETE
CREATE OR ALTER PROCEDURE sp_DeleteCuaHang
    @MaCuaHang VARCHAR(20)
AS
BEGIN
    DELETE FROM CuaHang WHERE MaCuaHang = @MaCuaHang;
END;
GO
