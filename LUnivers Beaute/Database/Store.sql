USE LUnivers_Beaute;
GO

-- =========================================================================
-- STORED PROCEDURES: HIỂN THỊ DỮ LIỆU TẤT CẢ CÁC BẢNG
-- L'Univers Beauté Management System
-- =========================================================================

-- =========================================================================
-- 1. DANH MỤC
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllDanhMuc
AS
BEGIN
    SELECT MaDanhMuc, TenDanhMuc
    FROM DanhMuc
    ORDER BY MaDanhMuc;
END;
GO

-- =========================================================================
-- 2. THƯƠNG HIỆU
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllThuongHieu
AS
BEGIN
    SELECT MaThuongHieu, TenThuongHieu, QuocGia
    FROM ThuongHieu
    ORDER BY MaThuongHieu;
END;
GO

-- =========================================================================
-- 3. CỬA HÀNG
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllCuaHang
AS
BEGIN
    SELECT 
        MaCuaHang,
        TenCuaHang,
        DiaChi,
        SoDienThoai,
        CASE WHEN TrangThai = 1 THEN N'Đang hoạt động' ELSE N'Ngừng hoạt động' END AS TrangThai
    FROM CuaHang
    ORDER BY MaCuaHang;
END;
GO

-- =========================================================================
-- 4. NHÀ CUNG CẤP
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllNhaCungCap
AS
BEGIN
    SELECT MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi
    FROM NhaCungCap
    ORDER BY MaNhaCungCap;
END;
GO

-- =========================================================================
-- 5. NHÂN VIÊN
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllNhanVien
AS
BEGIN
    SELECT 
        nv.MaNhanVien,
        nv.HoTen,
        nv.SoDienThoai,
        nv.VaiTro,
        nv.TenDangNhap,
        ch.TenCuaHang,
        CASE WHEN nv.TrangThai = 1 THEN N'Đang làm' ELSE N'Nghỉ việc' END AS TrangThai
    FROM NhanVien nv
    LEFT JOIN CuaHang ch ON nv.MaCuaHang = ch.MaCuaHang
    ORDER BY nv.MaNhanVien;
END;
GO

-- =========================================================================
-- 6. KHÁCH HÀNG
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllKhachHang
AS
BEGIN
    SELECT 
        MaKhachHang,
        HoTen,
        SoDienThoai,
        DiemTichLuy,
        CASE WHEN TrangThai = 1 THEN N'Hoạt động' ELSE N'Khóa' END AS TrangThai
    FROM KhachHang
    ORDER BY MaKhachHang;
END;
GO

-- =========================================================================
-- 7. SẢN PHẨM
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllSanPham
AS
BEGIN
    SELECT 
        sp.MaSanPham,
        sp.HinhAnh,
        sp.TenSanPham,
        dm.TenDanhMuc,
        th.TenThuongHieu,
        ncc.TenNhaCungCap,
        sp.DonViTinh,
        FORMAT(sp.GiaNiemYet, 'N0') + N' ₫' AS GiaNiemYet,
        CASE WHEN sp.TrangThai = 1 THEN N'Đang bán' ELSE N'Ngừng bán' END AS TrangThai
    FROM SanPham sp
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    LEFT JOIN NhaCungCap ncc ON sp.MaNhaCungCap = ncc.MaNhaCungCap
    ORDER BY sp.MaSanPham;
END;
GO

-- =========================================================================
-- 8. LÔ SẢN XUẤT
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllLoSanXuat
AS
BEGIN
    SELECT 
        lsx.MaLo,
        sp.TenSanPham,
        lsx.MaSanPham,
        lsx.SoLo,
        FORMAT(lsx.NgaySanXuat, 'dd/MM/yyyy') AS NgaySanXuat,
        FORMAT(lsx.HanSuDung, 'dd/MM/yyyy') AS HanSuDung,
        CASE 
            WHEN lsx.HanSuDung < CAST(GETDATE() AS DATE) THEN N'Hết hạn'
            WHEN DATEDIFF(DAY, GETDATE(), lsx.HanSuDung) <= 90 THEN N'Sắp hết hạn'
            ELSE N'Còn hạn' END AS TinhTrangHSD
    FROM LoSanXuat lsx
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    ORDER BY lsx.MaLo;
END;
GO

-- =========================================================================
-- 9. PHIẾU NHẬP KHO
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllPhieuNhapKho
AS
BEGIN
    SELECT 
        pnk.MaPhieuNhap,
        ch.TenCuaHang,
        nv.HoTen AS NhanVienNhap,
        FORMAT(pnk.NgayNhap, 'dd/MM/yyyy HH:mm') AS NgayNhap,
        FORMAT(pnk.TongTienNhap, 'N0') + N' ₫' AS TongTienNhap
    FROM PhieuNhapKho pnk
    LEFT JOIN CuaHang ch ON pnk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN NhanVien nv ON pnk.MaNhanVien = nv.MaNhanVien
    ORDER BY pnk.NgayNhap DESC;
END;
GO

-- =========================================================================
-- 10. CHI TIẾT NHẬP KHO
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetChiTietNhapKho
    @MaPhieuNhap VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        ctnk.MaPhieuNhap,
        sp.TenSanPham,
        lsx.SoLo,
        FORMAT(lsx.NgaySanXuat, 'dd/MM/yyyy') AS NgaySanXuat,
        FORMAT(lsx.HanSuDung, 'dd/MM/yyyy') AS HanSuDung,
        ctnk.SoLuong,
        FORMAT(ctnk.GiaNhap, 'N0') + N' ₫' AS GiaNhap,
        FORMAT(ctnk.SoLuong * ctnk.GiaNhap, 'N0') + N' ₫' AS ThanhTien
    FROM ChiTietNhapKho ctnk
    LEFT JOIN LoSanXuat lsx ON ctnk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaPhieuNhap IS NULL OR ctnk.MaPhieuNhap = @MaPhieuNhap)
    ORDER BY ctnk.MaPhieuNhap;
END;
GO

-- =========================================================================
-- 11. TỒN KHO
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllTonKho
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        ch.TenCuaHang,
        sp.MaSanPham,
        sp.TenSanPham,
        dm.TenDanhMuc,
        th.TenThuongHieu,
        lsx.SoLo,
        FORMAT(lsx.NgaySanXuat, 'dd/MM/yyyy') AS NgaySanXuat,
        FORMAT(lsx.HanSuDung, 'dd/MM/yyyy') AS HanSuDung,
        tk.SoLuongTon,
        CASE 
            WHEN tk.SoLuongTon = 0 THEN N'Hết hàng'
            WHEN tk.SoLuongTon <= 10 THEN N'Sắp hết'
            ELSE N'Còn hàng' END AS TinhTrangKho
    FROM TonKho tk
    LEFT JOIN CuaHang ch ON tk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN LoSanXuat lsx ON tk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    WHERE (@MaCuaHang IS NULL OR tk.MaCuaHang = @MaCuaHang)
    ORDER BY tk.SoLuongTon ASC;
END;
GO

-- =========================================================================
-- 12. KHUYẾN MÃI
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

-- =========================================================================
-- 13. HÓA ĐƠN
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllHoaDon
    @MaCuaHang VARCHAR(20) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
AS
BEGIN
    SELECT 
        hd.MaHoaDon,
        ch.TenCuaHang,
        nv.HoTen AS NhanVienLap,
        ISNULL(kh.HoTen, N'Khách lẻ') AS KhachHang,
        km.TenChuongTrinh AS KhuyenMai,
        FORMAT(hd.NgayLap, 'dd/MM/yyyy HH:mm') AS NgayLap,
        hd.PhuongThucThanhToan,
        FORMAT(hd.TongTien, 'N0') + N' ₫' AS TongTien,
        FORMAT(hd.KhachCanTra, 'N0') + N' ₫' AS KhachCanTra
    FROM HoaDon hd
    LEFT JOIN CuaHang ch ON hd.MaCuaHang = ch.MaCuaHang
    LEFT JOIN NhanVien nv ON hd.MaNhanVien = nv.MaNhanVien
    LEFT JOIN KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    LEFT JOIN KhuyenMai km ON hd.MaKhuyenMai = km.MaKhuyenMai
    WHERE 
        (@MaCuaHang IS NULL OR hd.MaCuaHang = @MaCuaHang)
        AND (@TuNgay IS NULL OR CAST(hd.NgayLap AS DATE) >= @TuNgay)
        AND (@DenNgay IS NULL OR CAST(hd.NgayLap AS DATE) <= @DenNgay)
    ORDER BY hd.NgayLap DESC;
END;
GO

-- =========================================================================
-- 14. CHI TIẾT HÓA ĐƠN
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetChiTietHoaDon
    @MaHoaDon VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        cthd.MaHoaDon,
        sp.TenSanPham,
        lsx.SoLo,
        cthd.SoLuong,
        FORMAT(cthd.DonGia, 'N0') + N' ₫' AS DonGia,
        FORMAT(cthd.ThanhTien, 'N0') + N' ₫' AS ThanhTien
    FROM ChiTietHoaDon cthd
    LEFT JOIN LoSanXuat lsx ON cthd.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaHoaDon IS NULL OR cthd.MaHoaDon = @MaHoaDon)
    ORDER BY cthd.MaHoaDon;
END;
GO

-- =========================================================================
-- 15. PHIẾU TRẢ HÀNG
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllPhieuTraHang
AS
BEGIN
    SELECT 
        pt.MaPhieuTra,
        pt.MaHoaDon,
        ch.TenCuaHang,
        ISNULL(kh.HoTen, N'Khách lẻ') AS KhachHang,
        FORMAT(pt.NgayTra, 'dd/MM/yyyy HH:mm') AS NgayTra,
        pt.LyDo
    FROM PhieuTraHang pt
    LEFT JOIN HoaDon hd ON pt.MaHoaDon = hd.MaHoaDon
    LEFT JOIN CuaHang ch ON hd.MaCuaHang = ch.MaCuaHang
    LEFT JOIN KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    ORDER BY pt.NgayTra DESC;
END;
GO

-- =========================================================================
-- 16. CHI TIẾT PHIẾU TRẢ HÀNG
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetChiTietPhieuTraHang
    @MaPhieuTra VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        ctpt.MaPhieuTra,
        sp.TenSanPham,
        lsx.SoLo,
        ctpt.SoLuongTra,
        FORMAT(ctpt.SoTienHoan, 'N0') + N' ₫' AS SoTienHoan
    FROM ChiTietPhieuTraHang ctpt
    LEFT JOIN LoSanXuat lsx ON ctpt.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaPhieuTra IS NULL OR ctpt.MaPhieuTra = @MaPhieuTra)
    ORDER BY ctpt.MaPhieuTra;
END;
GO

-- =========================================================================
-- 17. KIỂM KÊ
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllKiemKe
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        kk.MaKiemKe,
        ch.TenCuaHang,
        sp.TenSanPham + ISNULL(' / ' + lsx.SoLo, '') AS TenSanPham,
        kk.SoLuongHeThong,
        kk.SoLuongThucTe,
        kk.SoLuongLech,
        FORMAT(kk.NgayKiem, 'dd/MM/yyyy HH:mm') AS NgayKiem,
        CASE 
            WHEN kk.SoLuongLech = 0 THEN N'Khớp'
            WHEN kk.SoLuongLech > 0 THEN N'Thừa'
            ELSE N'Thiếu'
        END AS KetQua
    FROM KiemKe kk
    LEFT JOIN CuaHang ch ON kk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN LoSanXuat lsx ON kk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaCuaHang IS NULL OR kk.MaCuaHang = @MaCuaHang)
    ORDER BY kk.NgayKiem DESC;
END;
GO

-- =========================================================================
-- 18. CHẤM CÔNG
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllChamCong
    @MaNhanVien VARCHAR(20) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
AS
BEGIN
    SELECT 
        cc.MaCC,
        cc.MaNhanVien,
        nv.HoTen AS NhanVien,
        ch.TenCuaHang,
        FORMAT(cc.NgayLam, 'dd/MM/yyyy') AS NgayLam,
        FORMAT(cc.GioVao, 'HH:mm') AS GioVao,
        FORMAT(cc.GioRa, 'HH:mm') AS GioRa,
        CASE 
            WHEN cc.GioRa IS NOT NULL 
            THEN CAST(DATEDIFF(MINUTE, cc.GioVao, cc.GioRa) / 60 AS VARCHAR) 
                 + N'h ' 
                 + CAST(DATEDIFF(MINUTE, cc.GioVao, cc.GioRa) % 60 AS VARCHAR) 
                 + N'm'
            ELSE N'Chưa ra'
        END AS TongGioLam
    FROM ChamCong cc
    LEFT JOIN NhanVien nv ON cc.MaNhanVien = nv.MaNhanVien
    LEFT JOIN CuaHang ch ON nv.MaCuaHang = ch.MaCuaHang
    WHERE 
        (@MaNhanVien IS NULL OR cc.MaNhanVien = @MaNhanVien)
        AND (@TuNgay IS NULL OR cc.NgayLam >= @TuNgay)
        AND (@DenNgay IS NULL OR cc.NgayLam <= @DenNgay)
    ORDER BY cc.NgayLam DESC, cc.GioVao;
END;
GO

-- =========================================================================
-- 19. HỦY SẢN PHẨM
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetAllHuySanPham
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        hsp.MaHuy,
        ch.TenCuaHang,
        nv.HoTen AS NhanVienHuy,
        sp.TenSanPham + ISNULL(' / ' + lsx.SoLo, '') AS TenSanPham,
        hsp.SoLuong,
        hsp.LyDo,
        FORMAT(hsp.NgayHuy, 'dd/MM/yyyy HH:mm') AS NgayHuy
    FROM HuySanPham hsp
    LEFT JOIN CuaHang ch ON hsp.MaCuaHang = ch.MaCuaHang
    LEFT JOIN NhanVien nv ON hsp.MaNhanVien = nv.MaNhanVien
    LEFT JOIN LoSanXuat lsx ON hsp.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE (@MaCuaHang IS NULL OR hsp.MaCuaHang = @MaCuaHang)
    ORDER BY hsp.NgayHuy DESC;
END;
GO

-- =========================================================================
-- THỐNG KÊ BỔ SUNG (DASHBOARD)
-- =========================================================================

-- Đơn hàng gần đây
CREATE OR ALTER PROCEDURE sp_GetDonHangGanDay
    @TopN INT = 5,
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT TOP (@TopN)
        hd.MaHoaDon,
        ISNULL(kh.HoTen, N'Khách lẻ') AS KhachHang,
        FORMAT(hd.NgayLap, 'dd/MM/yyyy HH:mm') AS NgayLap,
        hd.PhuongThucThanhToan,
        FORMAT(hd.TongTien, 'N0') + N' ₫' AS TongTien
    FROM HoaDon hd
    LEFT JOIN KhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
    WHERE (@MaCuaHang IS NULL OR hd.MaCuaHang = @MaCuaHang)
    ORDER BY hd.NgayLap DESC;
END;
GO

-- Doanh thu theo ngày
CREATE OR ALTER PROCEDURE sp_DoanhThuTheoNgay
    @MaCuaHang VARCHAR(20) = NULL,
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL
AS
BEGIN
    SELECT 
        CAST(NgayLap AS DATE) AS Ngay,
        COUNT(*) AS SoHoaDon,
        FORMAT(SUM(KhachCanTra), 'N0') + N' ₫' AS DoanhThu
    FROM HoaDon
    WHERE 
        (@MaCuaHang IS NULL OR MaCuaHang = @MaCuaHang)
        AND (@TuNgay IS NULL OR CAST(NgayLap AS DATE) >= @TuNgay)
        AND (@DenNgay IS NULL OR CAST(NgayLap AS DATE) <= @DenNgay)
    GROUP BY CAST(NgayLap AS DATE)
    ORDER BY Ngay DESC;
END;
GO

-- Top sản phẩm bán chạy
CREATE OR ALTER PROCEDURE sp_TopSanPhamBanChay
    @TopN INT = 10,
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT TOP (@TopN)
        sp.MaSanPham,
        sp.TenSanPham,
        th.TenThuongHieu,
        dm.TenDanhMuc,
        SUM(cthd.SoLuong) AS TongBan,
        FORMAT(SUM(cthd.ThanhTien), 'N0') + N' ₫' AS TongDoanhThu
    FROM ChiTietHoaDon cthd
    LEFT JOIN HoaDon hd ON cthd.MaHoaDon = hd.MaHoaDon
    LEFT JOIN LoSanXuat lsx ON cthd.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    WHERE (@MaCuaHang IS NULL OR hd.MaCuaHang = @MaCuaHang)
    GROUP BY sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, dm.TenDanhMuc
    ORDER BY TongBan DESC;
END;
GO

-- Cảnh báo tồn kho thấp
CREATE OR ALTER PROCEDURE sp_CanhBaoTonKhoThap
    @NguongCanhBao INT = 10,
    @MaCuaHang VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        ch.TenCuaHang,
        sp.MaSanPham,
        sp.TenSanPham,
        th.TenThuongHieu,
        lsx.SoLo,
        FORMAT(lsx.HanSuDung, 'dd/MM/yyyy') AS HanSuDung,
        tk.SoLuongTon,
        CASE 
            WHEN tk.SoLuongTon = 0 THEN N'Hết hàng'
            ELSE N'SẮP HẾT'
        END AS MucDoCanhBao
    FROM TonKho tk
    LEFT JOIN CuaHang ch ON tk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN LoSanXuat lsx ON tk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    WHERE 
        tk.SoLuongTon <= @NguongCanhBao
        AND (@MaCuaHang IS NULL OR tk.MaCuaHang = @MaCuaHang)
    ORDER BY tk.SoLuongTon ASC;
END;
GO

PRINT N'========================================';
PRINT N'TẤT CẢ STORED PROCEDURES ĐÃ TẠO XONG!';
PRINT N'========================================';
PRINT N'';
PRINT N'-- DANH SÁCH STORED PROCEDURES:';
PRINT N'EXEC sp_GetAllDanhMuc';
PRINT N'EXEC sp_GetAllThuongHieu';
PRINT N'EXEC sp_GetAllCuaHang';
PRINT N'EXEC sp_GetAllNhaCungCap';
PRINT N'EXEC sp_GetAllNhanVien';
PRINT N'EXEC sp_GetAllKhachHang';
PRINT N'EXEC sp_GetAllSanPham';
PRINT N'EXEC sp_GetAllLoSanXuat';
PRINT N'EXEC sp_GetAllPhieuNhapKho';
PRINT N'EXEC sp_GetChiTietNhapKho @MaPhieuNhap = NULL';
PRINT N'EXEC sp_GetAllTonKho @MaCuaHang = NULL';
PRINT N'EXEC sp_GetAllKhuyenMai';
PRINT N'EXEC sp_GetAllHoaDon @MaCuaHang = NULL, @TuNgay = NULL, @DenNgay = NULL';
PRINT N'EXEC sp_GetChiTietHoaDon @MaHoaDon = NULL';
PRINT N'EXEC sp_GetAllPhieuTraHang';
PRINT N'EXEC sp_GetChiTietPhieuTraHang @MaPhieuTra = NULL';
PRINT N'EXEC sp_GetAllKiemKe @MaCuaHang = NULL';
PRINT N'EXEC sp_GetAllChamCong @MaNhanVien = NULL, @TuNgay = NULL, @DenNgay = NULL';
PRINT N'EXEC sp_GetAllHuySanPham @MaCuaHang = NULL';
PRINT N'EXEC sp_DoanhThuTheoNgay @MaCuaHang = NULL, @TuNgay = NULL, @DenNgay = NULL';
PRINT N'EXEC sp_TopSanPhamBanChay @TopN = 10, @MaCuaHang = NULL';
PRINT N'EXEC sp_CanhBaoTonKhoThap @NguongCanhBao = 10, @MaCuaHang = NULL';
GO


