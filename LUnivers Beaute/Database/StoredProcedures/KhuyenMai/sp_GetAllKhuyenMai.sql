USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_GetAllKhuyenMai
    @TuKhoa NVARCHAR(100) = NULL,
    @MaDanhMuc INT = NULL,
    @LoaiGiam NVARCHAR(10) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL,
    @TrangThaiLoc INT = 0
AS
BEGIN
    SELECT 
        km.MaKhuyenMai,
        km.TenChuongTrinh,
        km.LoaiGiam,
        km.GiaTriGiam,
        CASE 
            WHEN km.LoaiGiam = '%' THEN CAST(km.GiaTriGiam AS NVARCHAR) + N'%'
            ELSE FORMAT(km.GiaTriGiam, 'N0') + N' ₫'
        END AS MucGiam,
        km.ApDungTheo,
        CASE km.ApDungTheo 
            WHEN 'HoaDon' THEN N'Hóa đơn'
            WHEN 'DanhMuc' THEN N'Danh mục'
            WHEN 'SanPham' THEN N'Sản phẩm'
            ELSE km.ApDungTheo
        END AS ApDungTheoHienThi,
        km.MaDanhMuc,
        dm.TenDanhMuc,
        km.MaSanPham,
        km.NgayBatDau AS RawNgayBatDau,
        km.NgayKetThuc AS RawNgayKetThuc,
        FORMAT(km.NgayBatDau, 'dd/MM/yyyy') AS NgayBatDau,
        FORMAT(km.NgayKetThuc, 'dd/MM/yyyy') AS NgayKetThuc,
        km.TrangThai,
        CASE 
            WHEN GETDATE() BETWEEN km.NgayBatDau AND km.NgayKetThuc AND km.TrangThai = 1 THEN N'Đang diễn ra'
            WHEN GETDATE() < km.NgayBatDau THEN N'Chưa bắt đầu'
            ELSE N'Đã kết thúc' END AS TinhTrang
    FROM KhuyenMai km
    LEFT JOIN DanhMuc dm ON km.MaDanhMuc = dm.MaDanhMuc
    WHERE 
        (@TuKhoa IS NULL OR @TuKhoa = '' OR km.TenChuongTrinh LIKE N'%' + @TuKhoa + N'%' OR CAST(km.MaKhuyenMai AS NVARCHAR) LIKE N'%' + @TuKhoa + N'%')
        AND (@MaDanhMuc IS NULL OR @MaDanhMuc = 0 OR km.MaDanhMuc = @MaDanhMuc)
        AND (@LoaiGiam IS NULL OR @LoaiGiam = '' OR km.LoaiGiam = @LoaiGiam)
        AND (@TuNgay IS NULL OR km.NgayKetThuc >= @TuNgay)
        AND (@DenNgay IS NULL OR km.NgayBatDau <= @DenNgay)
        AND (@TrangThaiLoc = 0 
             OR (@TrangThaiLoc = 1 AND GETDATE() BETWEEN km.NgayBatDau AND km.NgayKetThuc AND km.TrangThai = 1)
             OR (@TrangThaiLoc = 2 AND GETDATE() < km.NgayBatDau)
             OR (@TrangThaiLoc = 3 AND (GETDATE() > km.NgayKetThuc OR km.TrangThai = 0))
            )
    ORDER BY km.NgayKetThuc DESC;
END;
GO