use LUnivers_Beaute
go

CREATE OR ALTER PROCEDURE sp_TaoHoaDon
    @MaHoaDon VARCHAR(20),
    @MaCuaHang VARCHAR(20),
    @MaNhanVien VARCHAR(20),
    @MaKhachHang INT = NULL,
    @MaKhuyenMai INT = NULL,
    @PhuongThucThanhToan NVARCHAR(50),
    @ChiTietJSON NVARCHAR(MAX) 
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Insert HoaDon
        INSERT INTO HoaDon (MaHoaDon, MaCuaHang, MaNhanVien, MaKhachHang, MaKhuyenMai, NgayLap, PhuongThucThanhToan)
        VALUES (@MaHoaDon, @MaCuaHang, @MaNhanVien, @MaKhachHang, @MaKhuyenMai, GETDATE(), @PhuongThucThanhToan);

        -- 2. Parse JSON into temporary table
        CREATE TABLE #Cart (
            MaSanPham VARCHAR(50),
            SoLuong INT,
            DonGia DECIMAL(18,2)
        );

        INSERT INTO #Cart (MaSanPham, SoLuong, DonGia)
        SELECT MaSanPham, SoLuong, DonGia
        FROM OPENJSON(@ChiTietJSON)
        WITH (
            MaSanPham VARCHAR(50) '$.MaSanPham',
            SoLuong INT '$.SoLuong',
            DonGia DECIMAL(18,2) '$.DonGia'
        );

        -- 3. Loop through cart and allocate FIFO MaLo
        DECLARE @CurMaSanPham VARCHAR(50);
        DECLARE @CurSoLuong INT;
        DECLARE @CurDonGia DECIMAL(18,2);

        DECLARE curCart CURSOR LOCAL FOR 
        SELECT MaSanPham, SoLuong, DonGia FROM #Cart;

        OPEN curCart;
        FETCH NEXT FROM curCart INTO @CurMaSanPham, @CurSoLuong, @CurDonGia;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @SoLuongCan INT = @CurSoLuong;

            -- Find unexpired batches for this product in this store, ordered by HanSuDung (oldest first)
            DECLARE @CurMaLo INT;
            DECLARE @CurSoLuongTon INT;

            DECLARE curLo CURSOR LOCAL FOR
            SELECT tk.MaLo, tk.SoLuongTon
            FROM TonKho tk
            JOIN LoSanXuat lsx ON tk.MaLo = lsx.MaLo
            WHERE tk.MaCuaHang = @MaCuaHang 
              AND lsx.MaSanPham = @CurMaSanPham
              AND tk.SoLuongTon > 0
              AND lsx.HanSuDung >= CAST(GETDATE() AS DATE)
            ORDER BY lsx.HanSuDung ASC;

            OPEN curLo;
            FETCH NEXT FROM curLo INTO @CurMaLo, @CurSoLuongTon;

            WHILE @@FETCH_STATUS = 0 AND @SoLuongCan > 0
            BEGIN
                DECLARE @SoLuongXuat INT;

                IF @CurSoLuongTon >= @SoLuongCan
                    SET @SoLuongXuat = @SoLuongCan;
                ELSE
                    SET @SoLuongXuat = @CurSoLuongTon;

                -- Insert into ChiTietHoaDon (the trigger trg_BanHang_TonKho will automatically deduct TonKho)
                INSERT INTO ChiTietHoaDon (MaHoaDon, MaLo, SoLuong, DonGia)
                VALUES (@MaHoaDon, @CurMaLo, @SoLuongXuat, @CurDonGia);

                SET @SoLuongCan = @SoLuongCan - @SoLuongXuat;

                FETCH NEXT FROM curLo INTO @CurMaLo, @CurSoLuongTon;
            END

            CLOSE curLo;
            DEALLOCATE curLo;

            IF @SoLuongCan > 0
            BEGIN
                RAISERROR(N'Không đủ số lượng tồn kho cho sản phẩm %s', 16, 1, @CurMaSanPham);
            END

            FETCH NEXT FROM curCart INTO @CurMaSanPham, @CurSoLuong, @CurDonGia;
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
