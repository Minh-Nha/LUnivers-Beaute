USE LUnivers_Beaute;
GO

-- =========================================================================
-- KIỂM KÊ KHO - Xóa phiếu kiểm kê
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_DeleteKiemKe
    @MaKiemKe INT
AS
BEGIN
    DELETE FROM KiemKe WHERE MaKiemKe = @MaKiemKe;
END;
GO
