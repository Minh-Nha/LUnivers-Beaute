USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_TaoPhieuNhapKho
    @MaPhieuNhap VARCHAR(20),
    @MaCuaHang VARCHAR(20),
    @MaNhanVien VARCHAR(20),
    @TongTienNhap DECIMAL(18,2),
    @ChiTietJSON NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Insert PhieuNhapKho
        INSERT INTO PhieuNhapKho (MaPhieuNhap, MaCuaHang, MaNhanVien, NgayNhap, TongTienNhap)
        VALUES (@MaPhieuNhap, @MaCuaHang, @MaNhanVien, GETDATE(), @TongTienNhap);

        -- 2. Parse JSON into temporary table
        CREATE TABLE #Cart (
            MaSanPham VARCHAR(50),
            SoLuong INT,
            GiaNhap DECIMAL(18,2),
            SoLo VARCHAR(50),
            NgaySanXuat DATE,
            HanSuDung DATE
        );

        INSERT INTO #Cart (MaSanPham, SoLuong, GiaNhap, SoLo, NgaySanXuat, HanSuDung)
        SELECT MaSanPham, SoLuong, GiaNhap, SoLo, NgaySanXuat, HanSuDung
        FROM OPENJSON(@ChiTietJSON)
        WITH (
            MaSanPham VARCHAR(50) '$.MaSanPham',
            SoLuong INT '$.SoLuong',
            GiaNhap DECIMAL(18,2) '$.GiaNhap',
            SoLo VARCHAR(50) '$.SoLo',
            NgaySanXuat DATE '$.NgaySanXuat',
            HanSuDung DATE '$.HanSuDung'
        );

        -- 3. Loop through cart and process
        DECLARE @CurMaSanPham VARCHAR(50);
        DECLARE @CurSoLuong INT;
        DECLARE @CurGiaNhap DECIMAL(18,2);
        DECLARE @CurSoLo VARCHAR(50);
        DECLARE @CurNSX DATE;
        DECLARE @CurHSD DATE;
        DECLARE @NewMaLo INT;

        DECLARE curCart CURSOR LOCAL FOR 
        SELECT MaSanPham, SoLuong, GiaNhap, SoLo, NgaySanXuat, HanSuDung FROM #Cart;

        OPEN curCart;
        FETCH NEXT FROM curCart INTO @CurMaSanPham, @CurSoLuong, @CurGiaNhap, @CurSoLo, @CurNSX, @CurHSD;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- CRITICAL FIX: Reset @NewMaLo before selecting
            SET @NewMaLo = NULL;

            -- Check if LoSanXuat exists (by SoLo and MaSanPham)
            SELECT @NewMaLo = MaLo FROM LoSanXuat WHERE SoLo = @CurSoLo AND MaSanPham = @CurMaSanPham;

            IF @NewMaLo IS NULL
            BEGIN
                -- Create new batch
                INSERT INTO LoSanXuat (MaSanPham, SoLo, NgaySanXuat, HanSuDung)
                VALUES (@CurMaSanPham, @CurSoLo, @CurNSX, @CurHSD);
                
                SET @NewMaLo = SCOPE_IDENTITY();
            END

            -- Insert into ChiTietNhapKho (the trigger trg_NhapHang_TonKho will automatically add to TonKho)
            INSERT INTO ChiTietNhapKho (MaPhieuNhap, MaLo, SoLuong, GiaNhap)
            VALUES (@MaPhieuNhap, @NewMaLo, @CurSoLuong, @CurGiaNhap);

            FETCH NEXT FROM curCart INTO @CurMaSanPham, @CurSoLuong, @CurGiaNhap, @CurSoLo, @CurNSX, @CurHSD;
        END

        CLOSE curCart;
        DEALLOCATE curCart;
        DROP TABLE #Cart;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO