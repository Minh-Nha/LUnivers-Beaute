use LUnivers_Beaute
go

CREATE OR ALTER PROCEDURE sp_DeleteNhaCungCap
    @MaNhaCungCap INT
AS
BEGIN
    DELETE FROM NhaCungCap
    WHERE MaNhaCungCap = @MaNhaCungCap;
END;
GO
