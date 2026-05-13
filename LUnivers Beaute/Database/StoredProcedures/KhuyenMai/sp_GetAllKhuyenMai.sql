USE LUnivers_Beaute;
GO

-- =========================================================================
-- KHUYẾN MÃI - Hiển thị tất cả chương trình khuyến mãi
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllKhuyenMai
AS
BEGIN
    SELECT 
        km.MaKhuyenMai,
        km.TenChuongTrinh,
        CASE 
            WHEN km.LoaiGiam = '%' THEN CAST(km.GiaTriGiam AS NVARCHAR) + N'%'
            ELSE FORMAT(km.GiaTriGiam, 'N0') + N' ₫'
        END AS MucGiam,
        km.ApDungTheo,
        dm.TenDanhMuc,
        km.MaSanPham,
        FORMAT(km.NgayBatDau, 'dd/MM/yyyy') AS NgayBatDau,
        FORMAT(km.NgayKetThuc, 'dd/MM/yyyy') AS NgayKetThuc,
        CASE 
            WHEN GETDATE() BETWEEN km.NgayBatDau AND km.NgayKetThuc AND km.TrangThai = 1 THEN N'Đang diễn ra'
            WHEN GETDATE() < km.NgayBatDau THEN N'Chưa bắt đầu'
            ELSE N'Đã kết thúc' END AS TinhTrang
    FROM KhuyenMai km
    LEFT JOIN DanhMuc dm ON km.MaDanhMuc = dm.MaDanhMuc
    ORDER BY km.NgayKetThuc DESC;
END;
GO
