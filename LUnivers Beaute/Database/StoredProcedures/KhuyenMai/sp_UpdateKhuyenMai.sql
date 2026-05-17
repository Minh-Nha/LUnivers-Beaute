USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_UpdateKhuyenMai
    @MaKhuyenMai INT,
    @TenChuongTrinh NVARCHAR(100),
    @LoaiGiam NVARCHAR(10),
    @GiaTriGiam DECIMAL(18,2),
    @ApDungTheo NVARCHAR(50),
    @MaDanhMuc INT = NULL,
    @MaSanPham VARCHAR(50) = NULL,
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @TrangThai BIT
AS
BEGIN
    UPDATE KhuyenMai
    SET 
        TenChuongTrinh = @TenChuongTrinh,
        LoaiGiam = @LoaiGiam,
        GiaTriGiam = @GiaTriGiam,
        ApDungTheo = @ApDungTheo,
        MaDanhMuc = @MaDanhMuc,
        MaSanPham = @MaSanPham,
        NgayBatDau = @NgayBatDau,
        NgayKetThuc = @NgayKetThuc,
        TrangThai = @TrangThai
    WHERE MaKhuyenMai = @MaKhuyenMai;
END;
GO
