USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_DeleteKhuyenMai
    @MaKhuyenMai INT
AS
BEGIN
    -- Prevent deletion if there are associated rows in other tables (if any dependencies exist)
    -- In this schema, KhuyenMai is mostly independent or used in HoaDon. If used in HoaDon, maybe just disable it.
    -- For now, allow soft or hard delete. Hard delete for simplicity if no FK constraint.
    DELETE FROM KhuyenMai WHERE MaKhuyenMai = @MaKhuyenMai;
END;
GO
