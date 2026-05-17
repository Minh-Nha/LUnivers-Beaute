USE LUnivers_Beaute;
GO

-- =========================================================================
-- KIỂM KÊ KHO - Tạo phiếu kiểm kê hàng loạt từ JSON
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_TaoPhieuKiemKe
    @MaCuaHang VARCHAR(20),
    @ChiTietJSON NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Parse JSON and insert each item
        INSERT INTO KiemKe (MaCuaHang, MaLo, SoLuongHeThong, SoLuongThucTe, NgayKiem)
        SELECT 
            @MaCuaHang,
            j.MaLo,
            j.SoLuongHeThong,
            j.SoLuongThucTe,
            GETDATE()
        FROM OPENJSON(@ChiTietJSON)
        WITH (
            MaLo INT '$.MaLo',
            SoLuongHeThong INT '$.SoLuongHeThong',
            SoLuongThucTe INT '$.SoLuongThucTe'
        ) j;

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
