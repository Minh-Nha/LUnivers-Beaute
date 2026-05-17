USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_InsertKhuyenMai
    @TenChuongTrinh NVARCHAR(100),
    @LoaiGiam NVARCHAR(10),
    @GiaTriGiam DECIMAL(18,2),
    @ApDungTheo NVARCHAR(50),
    @MaDanhMuc INT = NULL,
    @MaSanPham VARCHAR(50) = NULL,
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @TrangThai BIT = 1
AS
BEGIN
    INSERT INTO KhuyenMai (TenChuongTrinh, LoaiGiam, GiaTriGiam, ApDungTheo, MaDanhMuc, MaSanPham, NgayBatDau, NgayKetThuc, TrangThai)
    VALUES (@TenChuongTrinh, @LoaiGiam, @GiaTriGiam, @ApDungTheo, @MaDanhMuc, @MaSanPham, @NgayBatDau, @NgayKetThuc, @TrangThai);
END;
GO
