USE [LUnivers_Beaute]
GO
/****** Object:  StoredProcedure [dbo].[sp_AutoHuySanPhamHetHan]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 3. Tự động hủy sản phẩm hết hạn (Tự động quét và đẩy vào bảng HuySanPham)
CREATE   PROCEDURE [dbo].[sp_AutoHuySanPhamHetHan]
    @MaCuaHang VARCHAR(20),
    @MaNhanVien VARCHAR(20)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Tìm các lô sản phẩm đã hết hạn và còn tồn kho tại cửa hàng
        DECLARE @ExpiredItems TABLE (
            MaLo INT,
            SoLuongTon INT
        );

        INSERT INTO @ExpiredItems (MaLo, SoLuongTon)
        SELECT tk.MaLo, tk.SoLuongTon
        FROM TonKho tk
        JOIN LoSanXuat l ON tk.MaLo = l.MaLo
        WHERE tk.MaCuaHang = @MaCuaHang 
          AND tk.SoLuongTon > 0 
          AND l.HanSuDung < GETDATE();

        -- Đẩy vào bảng HuySanPham
        INSERT INTO HuySanPham (MaCuaHang, MaNhanVien, MaLo, SoLuong, NgayHuy, LyDo)
        SELECT @MaCuaHang, @MaNhanVien, MaLo, SoLuongTon, GETDATE(), N'Tự động hủy: Sản phẩm đã quá hạn sử dụng'
        FROM @ExpiredItems;

        -- Cập nhật Tồn kho về 0 cho các lô này
        UPDATE TonKho
        SET SoLuongTon = 0
        WHERE MaCuaHang = @MaCuaHang 
          AND MaLo IN (SELECT MaLo FROM @ExpiredItems);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[sp_CanhBaoTonKhoThap]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Cảnh báo tồn kho thấp
CREATE   PROCEDURE [dbo].[sp_CanhBaoTonKhoThap]
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
/****** Object:  StoredProcedure [dbo].[sp_CheckEmailExists]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_CheckEmailExists]
    @Email VARCHAR(100)
AS
BEGIN
    SELECT MaNhanVien, HoTen, TenDangNhap
    FROM NhanVien
    WHERE Email = @Email AND TrangThai = 1;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteChamCong]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 4. DELETE
CREATE   PROCEDURE [dbo].[sp_DeleteChamCong]
    @MaCC INT
AS
BEGIN
    DELETE FROM ChamCong WHERE MaCC = @MaCC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteCuaHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 4. DELETE
CREATE   PROCEDURE [dbo].[sp_DeleteCuaHang]
    @MaCuaHang VARCHAR(20)
AS
BEGIN
    DELETE FROM CuaHang WHERE MaCuaHang = @MaCuaHang;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteDanhMuc]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_DeleteDanhMuc]
    @MaDanhMuc INT
AS
BEGIN
    DELETE FROM DanhMuc
    WHERE MaDanhMuc = @MaDanhMuc;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteKhachHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 4. DELETE
CREATE   PROCEDURE [dbo].[sp_DeleteKhachHang]
    @MaKhachHang INT
AS
BEGIN
    DELETE FROM KhachHang WHERE MaKhachHang = @MaKhachHang;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteKhuyenMai]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_DeleteKhuyenMai]
    @MaKhuyenMai INT
AS
BEGIN
    -- Prevent deletion if there are associated rows in other tables (if any dependencies exist)
    -- In this schema, KhuyenMai is mostly independent or used in HoaDon. If used in HoaDon, maybe just disable it.
    -- For now, allow soft or hard delete. Hard delete for simplicity if no FK constraint.
    DELETE FROM KhuyenMai WHERE MaKhuyenMai = @MaKhuyenMai;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteKiemKe]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- KIỂM KÊ KHO - Xóa phiếu kiểm kê
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_DeleteKiemKe]
    @MaKiemKe INT
AS
BEGIN
    DELETE FROM KiemKe WHERE MaKiemKe = @MaKiemKe;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteNhaCungCap]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_DeleteNhaCungCap]
    @MaNhaCungCap INT
AS
BEGIN
    DELETE FROM NhaCungCap
    WHERE MaNhaCungCap = @MaNhaCungCap;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteNhanVien]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 4. DELETE
CREATE   PROCEDURE [dbo].[sp_DeleteNhanVien]
    @MaNhanVien VARCHAR(20)
AS
BEGIN
    DELETE FROM NhanVien WHERE MaNhanVien = @MaNhanVien;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteSanPham]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[sp_DeleteSanPham]
    @MaSanPham VARCHAR(50)
AS
BEGIN
    DELETE FROM SanPham
    WHERE MaSanPham = @MaSanPham;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteThuongHieu]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_DeleteThuongHieu]
    @MaThuongHieu INT
AS
BEGIN
    DELETE FROM ThuongHieu
    WHERE MaThuongHieu = @MaThuongHieu;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DoanhThu7NgayQua]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_DoanhThu7NgayQua] AS BEGIN DECLARE @Today DATE = CAST(GETDATE() AS DATE); WITH CTE_Dates AS (SELECT TOP 7 DATEADD(day, - (ROW_NUMBER() OVER(ORDER BY object_id) - 1), @Today) AS Ngay FROM sys.all_objects) SELECT D.Ngay, ISNULL(SUM(H.TongTien), 0) AS DoanhThu FROM CTE_Dates D LEFT JOIN HoaDon H ON CAST(H.NgayLap AS DATE) = D.Ngay GROUP BY D.Ngay ORDER BY D.Ngay ASC; END
GO
/****** Object:  StoredProcedure [dbo].[sp_DoanhThuTheoNgay]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Doanh thu theo ngày
CREATE   PROCEDURE [dbo].[sp_DoanhThuTheoNgay]
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
/****** Object:  StoredProcedure [dbo].[sp_GetAllChamCong]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =========================================================================
-- CHẤM CÔNG - CRUD Stored Procedures
-- =========================================================================

-- 1. GET ALL
CREATE   PROCEDURE [dbo].[sp_GetAllChamCong]
    @TuNgay DATE = NULL,
    @DenNgay DATE = NULL,
    @MaNhanVien VARCHAR(20) = NULL
AS
BEGIN
    SELECT 
        cc.MaCC,
        cc.MaNhanVien,
        nv.HoTen,
        cc.NgayLam,
        cc.GioVao,
        cc.GioRa,
        CASE 
            WHEN cc.GioRa IS NOT NULL THEN 
                RIGHT('0' + CAST(DATEDIFF(MINUTE, cc.GioVao, cc.GioRa) / 60 AS VARCHAR), 2) + ':' + 
                RIGHT('0' + CAST(DATEDIFF(MINUTE, cc.GioVao, cc.GioRa) % 60 AS VARCHAR), 2)
            ELSE '' 
        END AS TongGioLam
    FROM ChamCong cc
    INNER JOIN NhanVien nv ON cc.MaNhanVien = nv.MaNhanVien
    WHERE (@MaNhanVien IS NULL OR cc.MaNhanVien = @MaNhanVien)
      AND (@TuNgay IS NULL OR cc.NgayLam >= @TuNgay)
      AND (@DenNgay IS NULL OR cc.NgayLam <= @DenNgay)
    ORDER BY cc.NgayLam DESC, cc.GioVao DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllCuaHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =========================================================================
-- CỬA HÀNG - CRUD Stored Procedures
-- =========================================================================

-- 1. GET ALL
CREATE   PROCEDURE [dbo].[sp_GetAllCuaHang]
    @TimKiem NVARCHAR(100) = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        MaCuaHang,
        TenCuaHang,
        DiaChi,
        SoDienThoai,
        CASE WHEN TrangThai = 1 THEN N'Hoạt động' ELSE N'Ngừng hoạt động' END AS TrangThai
    FROM CuaHang
    WHERE (@TimKiem IS NULL OR TenCuaHang LIKE N'%' + @TimKiem + N'%' OR MaCuaHang LIKE '%' + @TimKiem + '%' OR SoDienThoai LIKE '%' + @TimKiem + '%')
      AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
    ORDER BY MaCuaHang DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllDanhMuc]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- DANH MỤC - Hiển thị tất cả danh mục sản phẩm
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllDanhMuc]
    @SearchTerm NVARCHAR(50) = NULL
AS
BEGIN
    SELECT MaDanhMuc, TenDanhMuc
    FROM DanhMuc
    WHERE (@SearchTerm IS NULL OR TenDanhMuc LIKE N'%' + @SearchTerm + N'%')
    ORDER BY MaDanhMuc;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllHoaDon]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- 13. HÓA ĐƠN
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllHoaDon]
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
/****** Object:  StoredProcedure [dbo].[sp_GetAllHuySanPham]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 1. Lấy danh sách Hủy Sản Phẩm
CREATE   PROCEDURE [dbo].[sp_GetAllHuySanPham]
AS
BEGIN
    SELECT h.MaHuy, 
           c.TenCuaHang, 
           n.HoTen AS NhanVienHuy, 
           sp.TenSanPham + ' - Lô: ' + CAST(l.MaLo AS VARCHAR) AS TenSanPham, 
           h.SoLuong, 
           h.NgayHuy, 
           h.LyDo
    FROM HuySanPham h
    JOIN CuaHang c ON h.MaCuaHang = c.MaCuaHang
    JOIN NhanVien n ON h.MaNhanVien = n.MaNhanVien
    JOIN LoSanXuat l ON h.MaLo = l.MaLo
    JOIN SanPham sp ON l.MaSanPham = sp.MaSanPham
    ORDER BY h.NgayHuy DESC;
END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllKhachHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- KHÁCH HÀNG - CRUD Stored Procedures
-- =========================================================================

-- 1. GET ALL (with search & filter)
CREATE   PROCEDURE [dbo].[sp_GetAllKhachHang]
    @TimKiem NVARCHAR(100) = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        MaKhachHang,
        HoTen,
        SoDienThoai,
        DiemTichLuy,
        CASE WHEN TrangThai = 1 THEN N'Hoạt động' ELSE N'Khóa' END AS TrangThai
    FROM KhachHang
    WHERE (@TimKiem IS NULL OR HoTen LIKE N'%' + @TimKiem + N'%' OR SoDienThoai LIKE '%' + @TimKiem + '%')
      AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
    ORDER BY MaKhachHang DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllKhuyenMai]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_GetAllKhuyenMai]
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
/****** Object:  StoredProcedure [dbo].[sp_GetAllKiemKe]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- KIỂM KÊ KHO - Lấy tất cả phiếu kiểm kê
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllKiemKe]
AS
BEGIN
    SELECT 
        kk.MaKiemKe,
        kk.MaCuaHang,
        ch.TenCuaHang,
        sp.TenSanPham,
        lsx.SoLo,
        kk.SoLuongHeThong,
        kk.SoLuongThucTe,
        kk.SoLuongLech,
        FORMAT(kk.NgayKiem, 'dd/MM/yyyy HH:mm') AS NgayKiem,
        CASE 
            WHEN kk.SoLuongLech = 0 THEN N'Khớp'
            WHEN kk.SoLuongLech > 0 THEN N'Thừa'
            ELSE N'Thiếu'
        END AS TinhTrang
    FROM KiemKe kk
    LEFT JOIN CuaHang ch ON kk.MaCuaHang = ch.MaCuaHang
    LEFT JOIN LoSanXuat lsx ON kk.MaLo = lsx.MaLo
    LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    ORDER BY kk.NgayKiem DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllLoSanXuat]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- LÔ SẢN XUẤT - Hiển thị tất cả lô sản xuất
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllLoSanXuat]
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
/****** Object:  StoredProcedure [dbo].[sp_GetAllNhaCungCap]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- NHÀ CUNG CẤP - Hiển thị tất cả nhà cung cấp
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllNhaCungCap]
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SELECT MaNhaCungCap, TenNhaCungCap, SoDienThoai, DiaChi
    FROM NhaCungCap
    WHERE (@SearchTerm IS NULL OR 
           TenNhaCungCap LIKE N'%' + @SearchTerm + N'%' OR
           SoDienThoai LIKE N'%' + @SearchTerm + N'%')
    ORDER BY MaNhaCungCap;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllNhanVien]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_GetAllNhanVien]
    @TimKiem NVARCHAR(100) = NULL,
    @MaCuaHang VARCHAR(20) = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        nv.MaNhanVien,
        nv.HoTen,
        nv.SoDienThoai,
        nv.Email,
        nv.VaiTro,
        nv.TenDangNhap,
        nv.MaCuaHang,
        ch.TenCuaHang,
        CASE WHEN nv.TrangThai = 1 THEN N'Đang làm' ELSE N'Nghỉ việc' END AS TrangThai
    FROM NhanVien nv
    LEFT JOIN CuaHang ch ON nv.MaCuaHang = ch.MaCuaHang
    WHERE (@TimKiem IS NULL OR nv.HoTen LIKE N'%' + @TimKiem + N'%' OR nv.SoDienThoai LIKE '%' + @TimKiem + '%' OR nv.MaNhanVien LIKE '%' + @TimKiem + '%')
      AND (@MaCuaHang IS NULL OR nv.MaCuaHang = @MaCuaHang)
      AND (@TrangThai IS NULL OR nv.TrangThai = @TrangThai)
    ORDER BY nv.MaNhanVien DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllPhieuNhapKho]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- PHIẾU NHẬP KHO - Hiển thị tất cả phiếu nhập kho
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllPhieuNhapKho]
AS
BEGIN
    SELECT 
        pnk.MaPhieuNhap,
        pnk.MaCuaHang,
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
/****** Object:  StoredProcedure [dbo].[sp_GetAllPhieuTraHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 1. Lấy danh sách Phiếu Trả Hàng
CREATE   PROCEDURE [dbo].[sp_GetAllPhieuTraHang]
AS
BEGIN
    SELECT MaPhieuTra, MaHoaDon, NgayTra, LyDo
    FROM PhieuTraHang
    ORDER BY NgayTra DESC;
END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllSanPham]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [dbo].[sp_GetAllSanPham]
    @TimKiem NVARCHAR(200) = NULL,
    @MaDanhMuc INT = NULL,
    @MaThuongHieu INT = NULL,
    @TrangThai INT = NULL
AS
BEGIN
    SELECT 
        sp.MaSanPham,
        sp.HinhAnh,
        sp.TenSanPham,
        sp.MaDanhMuc,
        dm.TenDanhMuc,
        sp.MaThuongHieu,
        th.TenThuongHieu,
        sp.MaNhaCungCap,
        ncc.TenNhaCungCap,
        sp.DonViTinh,
        sp.GiaNiemYet,
        CASE WHEN sp.TrangThai = 1 THEN N'Đang bán' ELSE N'Ngừng bán' END AS TrangThaiStr,
        sp.TrangThai
    FROM SanPham sp
    LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    LEFT JOIN NhaCungCap ncc ON sp.MaNhaCungCap = ncc.MaNhaCungCap
    WHERE 
        (@TimKiem IS NULL OR @TimKiem = '' OR sp.TenSanPham LIKE N'%' + @TimKiem + N'%' OR sp.MaSanPham LIKE '%' + @TimKiem + '%')
        AND (@MaDanhMuc IS NULL OR @MaDanhMuc = 0 OR sp.MaDanhMuc = @MaDanhMuc)
        AND (@MaThuongHieu IS NULL OR @MaThuongHieu = 0 OR sp.MaThuongHieu = @MaThuongHieu)
        AND (@TrangThai IS NULL OR @TrangThai = -1 OR sp.TrangThai = @TrangThai)
    ORDER BY sp.MaSanPham;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllThuongHieu]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- THƯƠNG HIỆU - Hiển thị tất cả thương hiệu
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllThuongHieu]
    @SearchTerm NVARCHAR(50) = NULL
AS
BEGIN
    SELECT MaThuongHieu, TenThuongHieu, QuocGia
    FROM ThuongHieu
    WHERE (@SearchTerm IS NULL OR 
           TenThuongHieu LIKE N'%' + @SearchTerm + N'%' OR
           QuocGia LIKE N'%' + @SearchTerm + N'%')
    ORDER BY MaThuongHieu;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllTonKho]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- 11. TỒN KHO
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetAllTonKho]
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
/****** Object:  StoredProcedure [dbo].[sp_GetChiTietHoaDon]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- CHI TIẾT HÓA ĐƠN - Hiển thị chi tiết hóa đơn
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetChiTietHoaDon]
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
/****** Object:  StoredProcedure [dbo].[sp_GetChiTietNhapKho]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- CHI TIẾT NHẬP KHO - Hiển thị chi tiết phiếu nhập kho
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetChiTietNhapKho]
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
/****** Object:  StoredProcedure [dbo].[sp_GetChiTietPhieuTra]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 2. Lấy danh sách Chi Tiết Phiếu Trả theo mã
CREATE   PROCEDURE [dbo].[sp_GetChiTietPhieuTra]
    @MaPhieuTra VARCHAR(20)
AS
BEGIN
    SELECT 
        sp.TenSanPham, 
        c.MaLo AS SoLo, 
        c.SoLuongTra, 
        c.SoTienHoan
    FROM ChiTietPhieuTraHang c
    JOIN LoSanXuat l ON c.MaLo = l.MaLo
    JOIN SanPham sp ON l.MaSanPham = sp.MaSanPham
    WHERE c.MaPhieuTra = @MaPhieuTra;
END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetChiTietPhieuTraHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- 16. CHI TIẾT PHIẾU TRẢ HÀNG
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetChiTietPhieuTraHang]
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
/****** Object:  StoredProcedure [dbo].[sp_GetDonHangGanDay]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- THỐNG KÊ BỔ SUNG (DASHBOARD)
-- =========================================================================

-- Đơn hàng gần đây
CREATE   PROCEDURE [dbo].[sp_GetDonHangGanDay]
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
/****** Object:  StoredProcedure [dbo].[sp_GetSanPhamBanHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_GetSanPhamBanHang] @MaCuaHang VARCHAR(20) = NULL, @SearchTerm NVARCHAR(100) = NULL, @MaDanhMuc INT = NULL AS BEGIN SELECT sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, sp.GiaNiemYet, sp.HinhAnh, ISNULL(SUM(tk.SoLuongTon), 0) AS SoLuongTon FROM SanPham sp LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu LEFT JOIN LoSanXuat lsx ON sp.MaSanPham = lsx.MaSanPham AND lsx.HanSuDung >= CAST(GETDATE() AS DATE) LEFT JOIN TonKho tk ON lsx.MaLo = tk.MaLo AND (@MaCuaHang IS NULL OR tk.MaCuaHang = @MaCuaHang) WHERE sp.TrangThai = 1 AND (@SearchTerm IS NULL OR sp.TenSanPham LIKE N'%' + @SearchTerm + N'%' OR sp.MaSanPham LIKE N'%' + @SearchTerm + N'%') AND (@MaDanhMuc IS NULL OR @MaDanhMuc = 0 OR sp.MaDanhMuc = @MaDanhMuc) GROUP BY sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, sp.GiaNiemYet, sp.HinhAnh HAVING ISNULL(SUM(tk.SoLuongTon), 0) > 0 ORDER BY sp.TenSanPham; END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSanPhamBanHang_Paged]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_GetSanPhamBanHang_Paged]
    @MaCuaHang VARCHAR(20) = NULL,
    @SearchTerm NVARCHAR(100) = NULL,
    @MaDanhMuc INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 8,
    @TotalRecords INT OUTPUT
AS 
BEGIN
    SET NOCOUNT ON;
    
    SELECT sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, sp.GiaNiemYet, sp.HinhAnh, ISNULL(SUM(tk.SoLuongTon), 0) AS SoLuongTon
    INTO #TempResult
    FROM SanPham sp 
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu 
    LEFT JOIN LoSanXuat lsx ON sp.MaSanPham = lsx.MaSanPham AND lsx.HanSuDung >= CAST(GETDATE() AS DATE) 
    LEFT JOIN TonKho tk ON lsx.MaLo = tk.MaLo AND (@MaCuaHang IS NULL OR tk.MaCuaHang = @MaCuaHang) 
    WHERE sp.TrangThai = 1 
      AND (@SearchTerm IS NULL OR sp.TenSanPham LIKE N'%' + @SearchTerm + N'%' OR sp.MaSanPham LIKE N'%' + @SearchTerm + N'%') 
      AND (@MaDanhMuc IS NULL OR @MaDanhMuc = 0 OR sp.MaDanhMuc = @MaDanhMuc) 
    GROUP BY sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, sp.GiaNiemYet, sp.HinhAnh 
    HAVING ISNULL(SUM(tk.SoLuongTon), 0) > 0;

    SELECT @TotalRecords = COUNT(*) FROM #TempResult;

    SELECT * 
    FROM #TempResult
    ORDER BY TenSanPham
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    DROP TABLE #TempResult;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetSanPhamChoHuy]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_GetSanPhamChoHuy]
    @MaCuaHang VARCHAR(20)
AS
BEGIN
    SELECT tk.MaLo, 
           tk.MaCuaHang,
           sp.TenSanPham, 
           l.SoLo, 
           tk.SoLuongTon, 
           l.HanSuDung
    FROM TonKho tk
    JOIN LoSanXuat l ON tk.MaLo = l.MaLo
    JOIN SanPham sp ON l.MaSanPham = sp.MaSanPham
    WHERE tk.MaCuaHang = @MaCuaHang 
      AND tk.SoLuongTon > 0 
      AND l.HanSuDung < GETDATE()
    ORDER BY l.HanSuDung ASC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetTonKho]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_GetTonKho]
    @MaCuaHang VARCHAR(20) = NULL,
    @SearchKeyword NVARCHAR(100) = NULL,
    @TrangThai NVARCHAR(50) = NULL,
    @MaDanhMuc INT = NULL,
    @TuSoLuong INT = NULL,
    @DenSoLuong INT = NULL,
    @SortColumn VARCHAR(50) = 'SoLuongTon',
    @SortOrder VARCHAR(4) = 'ASC'
AS
BEGIN
    WITH TonKhoCTE AS (
        SELECT 
            tk.MaCuaHang,
            ch.TenCuaHang,
            tk.MaLo,
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
                WHEN DATEDIFF(day, GETDATE(), lsx.HanSuDung) < 0 THEN N'Đã hết hạn'
                WHEN DATEDIFF(day, GETDATE(), lsx.HanSuDung) <= 30 THEN N'Sắp hết hạn'
                WHEN tk.SoLuongTon <= 30 THEN N'Sắp hết hàng'
                ELSE N'Còn hàng' END AS TinhTrangKho
        FROM TonKho tk
        LEFT JOIN CuaHang ch ON tk.MaCuaHang = ch.MaCuaHang
        LEFT JOIN LoSanXuat lsx ON tk.MaLo = lsx.MaLo
        LEFT JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
        LEFT JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc
        LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
        WHERE (@MaCuaHang IS NULL OR @MaCuaHang = '' OR tk.MaCuaHang = @MaCuaHang)
          AND (@MaDanhMuc IS NULL OR @MaDanhMuc = 0 OR sp.MaDanhMuc = @MaDanhMuc)
          AND (@TuSoLuong IS NULL OR tk.SoLuongTon >= @TuSoLuong)
          AND (@DenSoLuong IS NULL OR tk.SoLuongTon <= @DenSoLuong)
    )
    SELECT * FROM TonKhoCTE
    WHERE 
        (@SearchKeyword IS NULL OR @SearchKeyword = '' OR 
         TenSanPham LIKE N'%' + @SearchKeyword + N'%' OR 
         TenCuaHang LIKE N'%' + @SearchKeyword + N'%' OR 
         SoLo LIKE N'%' + @SearchKeyword + N'%')
        AND (@TrangThai IS NULL OR @TrangThai = N'Tất cả' OR @TrangThai = '' OR TinhTrangKho = @TrangThai)
    ORDER BY 
        CASE WHEN @SortOrder = 'ASC' AND @SortColumn = 'TenCuaHang' THEN TenCuaHang END ASC,
        CASE WHEN @SortOrder = 'DESC' AND @SortColumn = 'TenCuaHang' THEN TenCuaHang END DESC,
        CASE WHEN @SortOrder = 'ASC' AND @SortColumn = 'TenSanPham' THEN TenSanPham END ASC,
        CASE WHEN @SortOrder = 'DESC' AND @SortColumn = 'TenSanPham' THEN TenSanPham END DESC,
        CASE WHEN @SortOrder = 'ASC' AND @SortColumn = 'SoLo' THEN SoLo END ASC,
        CASE WHEN @SortOrder = 'DESC' AND @SortColumn = 'SoLo' THEN SoLo END DESC,
        CASE WHEN @SortOrder = 'ASC' AND @SortColumn = 'NgaySanXuat' THEN NgaySanXuat END ASC,
        CASE WHEN @SortOrder = 'DESC' AND @SortColumn = 'NgaySanXuat' THEN NgaySanXuat END DESC,
        CASE WHEN @SortOrder = 'ASC' AND @SortColumn = 'HanSuDung' THEN HanSuDung END ASC,
        CASE WHEN @SortOrder = 'DESC' AND @SortColumn = 'HanSuDung' THEN HanSuDung END DESC,
        CASE WHEN @SortOrder = 'ASC' AND @SortColumn = 'SoLuongTon' THEN SoLuongTon END ASC,
        CASE WHEN @SortOrder = 'DESC' AND @SortColumn = 'SoLuongTon' THEN SoLuongTon END DESC,
        CASE WHEN @SortOrder = 'ASC' AND @SortColumn = 'TinhTrangKho' THEN TinhTrangKho END ASC,
        CASE WHEN @SortOrder = 'DESC' AND @SortColumn = 'TinhTrangKho' THEN TinhTrangKho END DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetTonKhoForKiemKe]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- KIỂM KÊ KHO - Lấy danh sách tồn kho theo cửa hàng để kiểm kê
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_GetTonKhoForKiemKe]
    @MaCuaHang VARCHAR(20)
AS
BEGIN
    SELECT 
        tk.MaLo,
        sp.MaSanPham,
        sp.TenSanPham,
        lsx.SoLo,
        FORMAT(lsx.HanSuDung, 'dd/MM/yyyy') AS HanSuDung,
        tk.SoLuongTon AS SoLuongHeThong
    FROM TonKho tk
    JOIN LoSanXuat lsx ON tk.MaLo = lsx.MaLo
    JOIN SanPham sp ON lsx.MaSanPham = sp.MaSanPham
    WHERE tk.MaCuaHang = @MaCuaHang
      AND tk.SoLuongTon > 0
    ORDER BY sp.TenSanPham, lsx.SoLo;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertChamCong]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 2. INSERT
CREATE   PROCEDURE [dbo].[sp_InsertChamCong]
    @MaNhanVien VARCHAR(20),
    @NgayLam DATE,
    @GioVao TIME,
    @GioRa TIME = NULL
AS
BEGIN
    -- Bắt lỗi unique (Mỗi nhân viên chỉ có 1 ca trong 1 ngày)
    IF EXISTS (SELECT 1 FROM ChamCong WHERE MaNhanVien = @MaNhanVien AND NgayLam = @NgayLam)
    BEGIN
        RAISERROR(N'Nhân viên này đã được chấm công trong ngày hôm nay!', 16, 1);
        RETURN;
    END

    INSERT INTO ChamCong (MaNhanVien, NgayLam, GioVao, GioRa)
    VALUES (@MaNhanVien, @NgayLam, @GioVao, @GioRa);
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertCuaHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 2. INSERT
CREATE   PROCEDURE [dbo].[sp_InsertCuaHang]
    @TenCuaHang NVARCHAR(100),
    @DiaChi NVARCHAR(255),
    @SoDienThoai VARCHAR(15),
    @TrangThai BIT = 1
AS
BEGIN
    -- Kiểm tra lỗi unique
    IF EXISTS (SELECT 1 FROM CuaHang WHERE TenCuaHang = @TenCuaHang)
    BEGIN
        RAISERROR(N'Tên cửa hàng đã tồn tại!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM CuaHang WHERE SoDienThoai = @SoDienThoai)
    BEGIN
        RAISERROR(N'Số điện thoại đã tồn tại!', 16, 1);
        RETURN;
    END

    DECLARE @MaCuaHang VARCHAR(20);
    DECLARE @MaxNum INT = 0;
    
    SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaCuaHang, 3, LEN(MaCuaHang) - 2) AS INT)), 0)
    FROM CuaHang
    WHERE MaCuaHang LIKE 'CH%' AND ISNUMERIC(SUBSTRING(MaCuaHang, 3, LEN(MaCuaHang) - 2)) = 1;
    
    SET @MaCuaHang = 'CH' + RIGHT('000' + CAST(@MaxNum + 1 AS VARCHAR), 3);

    INSERT INTO CuaHang (MaCuaHang, TenCuaHang, DiaChi, SoDienThoai, TrangThai)
    VALUES (@MaCuaHang, @TenCuaHang, @DiaChi, @SoDienThoai, @TrangThai);
    
    SELECT @MaCuaHang AS MaCuaHang;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertDanhMuc]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_InsertDanhMuc]
    @TenDanhMuc NVARCHAR(100)
AS
BEGIN
    INSERT INTO DanhMuc (TenDanhMuc)
    VALUES (@TenDanhMuc);
    
    SELECT SCOPE_IDENTITY() AS MaDanhMuc;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertHuySanPham]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 2. Thêm phiếu hủy sản phẩm
CREATE   PROCEDURE [dbo].[sp_InsertHuySanPham]
    @MaCuaHang VARCHAR(20),
    @MaNhanVien VARCHAR(20),
    @MaLo INT,
    @SoLuong INT,
    @NgayHuy DATETIME,
    @LyDo NVARCHAR(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Thêm phiếu hủy
        INSERT INTO HuySanPham (MaCuaHang, MaNhanVien, MaLo, SoLuong, NgayHuy, LyDo)
        VALUES (@MaCuaHang, @MaNhanVien, @MaLo, @SoLuong, @NgayHuy, @LyDo);

        -- Trừ số lượng tồn kho
        IF EXISTS (SELECT 1 FROM TonKho WHERE MaCuaHang = @MaCuaHang AND MaLo = @MaLo)
        BEGIN
            UPDATE TonKho
            SET SoLuongTon = SoLuongTon - @SoLuong
            WHERE MaCuaHang = @MaCuaHang AND MaLo = @MaLo;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertKhachHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 2. INSERT
CREATE   PROCEDURE [dbo].[sp_InsertKhachHang]
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @DiemTichLuy INT = 0,
    @TrangThai BIT = 1
AS
BEGIN
    INSERT INTO KhachHang (HoTen, SoDienThoai, DiemTichLuy, TrangThai)
    VALUES (@HoTen, @SoDienThoai, @DiemTichLuy, @TrangThai);
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertKhuyenMai]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_InsertKhuyenMai]
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
/****** Object:  StoredProcedure [dbo].[sp_InsertNhaCungCap]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[sp_InsertNhaCungCap]
    @TenNhaCungCap NVARCHAR(150),
    @SoDienThoai VARCHAR(15),
    @DiaChi NVARCHAR(255) = NULL
AS
BEGIN
    INSERT INTO NhaCungCap (TenNhaCungCap, SoDienThoai, DiaChi)
    VALUES (@TenNhaCungCap, @SoDienThoai, @DiaChi);
    
    SELECT SCOPE_IDENTITY() AS MaNhaCungCap;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertNhanVien]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_InsertNhanVien]
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @VaiTro NVARCHAR(50),
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(255),
    @MaCuaHang VARCHAR(20),
    @Email VARCHAR(100),
    @TrangThai BIT = 1
AS
BEGIN
    DECLARE @MaNhanVien VARCHAR(20);
    DECLARE @MaxNum INT = 0;
    
    SELECT @MaxNum = ISNULL(MAX(CAST(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2) AS INT)), 0)
    FROM NhanVien
    WHERE MaNhanVien LIKE 'NV%' AND ISNUMERIC(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 1;
    
    SET @MaNhanVien = 'NV' + RIGHT('000' + CAST(@MaxNum + 1 AS VARCHAR), 3);

    INSERT INTO NhanVien (MaNhanVien, HoTen, SoDienThoai, VaiTro, TenDangNhap, MatKhau, MaCuaHang, Email, TrangThai)
    VALUES (@MaNhanVien, @HoTen, @SoDienThoai, @VaiTro, @TenDangNhap, @MatKhau, @MaCuaHang, @Email, @TrangThai);
    
    SELECT @MaNhanVien AS MaNhanVien;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertPhieuTraHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 3. Tạo mới Phiếu Trả Hàng & Chi tiết
-- Sử dụng JSON để truyền danh sách chi tiết (Tương tự như Nhập Kho và Bán Hàng)
CREATE   PROCEDURE [dbo].[sp_InsertPhieuTraHang]
    @MaPhieuTra VARCHAR(20),
    @MaHoaDon VARCHAR(20),
    @NgayTra DATETIME,
    @LyDo NVARCHAR(255),
    @JsonChiTiet NVARCHAR(MAX)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Insert Phiếu Trả
        INSERT INTO PhieuTraHang (MaPhieuTra, MaHoaDon, NgayTra, LyDo)
        VALUES (@MaPhieuTra, @MaHoaDon, @NgayTra, @LyDo);

        -- 2. Insert Chi tiết phiếu trả từ JSON
        INSERT INTO ChiTietPhieuTraHang (MaPhieuTra, MaLo, SoLuongTra, SoTienHoan)
        SELECT 
            @MaPhieuTra,
            JSON_VALUE(value, '$.MaLo'),
            JSON_VALUE(value, '$.SoLuongTra'),
            JSON_VALUE(value, '$.SoTienHoan')
        FROM OPENJSON(@JsonChiTiet);

        -- 3. Cập nhật lại số lượng Tồn Kho (Vì khách trả hàng thì nhập lại vào kho)
        -- Chú ý: Ở đây ta cần biết cửa hàng nào xuất hóa đơn này để cộng lại tồn kho cho đúng cửa hàng đó.
        DECLARE @MaCuaHang VARCHAR(20);
        SELECT @MaCuaHang = MaCuaHang FROM HoaDon WHERE MaHoaDon = @MaHoaDon;

        -- Tăng số lượng tồn kho cho từng lô
        UPDATE tk
        SET tk.SoLuongTon = tk.SoLuongTon + c.SoLuongTra
        FROM TonKho tk
        INNER JOIN (
            SELECT 
                CAST(JSON_VALUE(value, '$.MaLo') AS INT) AS MaLo,
                CAST(JSON_VALUE(value, '$.SoLuongTra') AS INT) AS SoLuongTra
            FROM OPENJSON(@JsonChiTiet)
        ) c ON tk.MaLo = c.MaLo
        WHERE tk.MaCuaHang = @MaCuaHang;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertSanPham]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [dbo].[sp_InsertSanPham]
    @MaSanPham VARCHAR(50),
    @TenSanPham NVARCHAR(200),
    @MaDanhMuc INT = NULL,
    @MaThuongHieu INT = NULL,
    @MaNhaCungCap INT = NULL,
    @HinhAnh NVARCHAR(MAX) = NULL,
    @DonViTinh NVARCHAR(20) = N'Cái',
    @GiaNiemYet DECIMAL(18,2),
    @TrangThai BIT = 1
AS
BEGIN
    INSERT INTO SanPham (MaSanPham, TenSanPham, MaDanhMuc, MaThuongHieu, MaNhaCungCap, HinhAnh, DonViTinh, GiaNiemYet, TrangThai)
    VALUES (@MaSanPham, @TenSanPham, @MaDanhMuc, @MaThuongHieu, @MaNhaCungCap, @HinhAnh, ISNULL(@DonViTinh, N'Cái'), @GiaNiemYet, ISNULL(@TrangThai, 1));
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertThuongHieu]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_InsertThuongHieu]
    @TenThuongHieu NVARCHAR(100),
    @QuocGia NVARCHAR(50) = NULL
AS
BEGIN
    INSERT INTO ThuongHieu (TenThuongHieu, QuocGia)
    VALUES (@TenThuongHieu, ISNULL(@QuocGia, N'Chưa xác định'));
    
    SELECT SCOPE_IDENTITY() AS MaThuongHieu;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_Login]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_Login]
    @TenDangNhap VARCHAR(50),
    @MatKhau VARCHAR(255)
AS
BEGIN
    SELECT n.MaNhanVien, n.HoTen, n.VaiTro, n.MaCuaHang, c.TenCuaHang
    FROM NhanVien n
    JOIN CuaHang c ON n.MaCuaHang = c.MaCuaHang
    WHERE n.TenDangNhap = @TenDangNhap 
      AND n.MatKhau = @MatKhau 
      AND n.TrangThai = 1;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_TaoHoaDon]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_TaoHoaDon]
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
/****** Object:  StoredProcedure [dbo].[sp_TaoPhieuKiemKe]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- KIỂM KÊ KHO - Tạo phiếu kiểm kê hàng loạt từ JSON
-- =========================================================================
CREATE   PROCEDURE [dbo].[sp_TaoPhieuKiemKe]
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
/****** Object:  StoredProcedure [dbo].[sp_TaoPhieuNhapKho]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_TaoPhieuNhapKho]
    @MaPhieuNhap VARCHAR(20),
    @MaCuaHang VARCHAR(20),
    @MaNhanVien VARCHAR(20),
    @TongTienNhap DECIMAL(18,2),
    @ChiTietJSON NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Insert PhieuNhapKho
        INSERT INTO PhieuNhapKho (MaPhieuNhap, MaCuaHang, MaNhanVien, NgayNhap, TongTienNhap)
        VALUES (@MaPhieuNhap, @MaCuaHang, @MaNhanVien, GETDATE(), @TongTienNhap);

        -- 2. Parse JSON into temporary table
        CREATE TABLE #Cart (
            MaSanPham VARCHAR(50),
            SoLuong INT,
            GiaNhap DECIMAL(18,2),
            GiaNiemYet DECIMAL(18,2),
            SoLo VARCHAR(50),
            NgaySanXuat DATE,
            HanSuDung DATE
        );

        INSERT INTO #Cart (MaSanPham, SoLuong, GiaNhap, GiaNiemYet, SoLo, NgaySanXuat, HanSuDung)
        SELECT MaSanPham, SoLuong, GiaNhap, GiaNiemYet, SoLo, NgaySanXuat, HanSuDung
        FROM OPENJSON(@ChiTietJSON)
        WITH (
            MaSanPham VARCHAR(50) '$.MaSanPham',
            SoLuong INT '$.SoLuong',
            GiaNhap DECIMAL(18,2) '$.GiaNhap',
            GiaNiemYet DECIMAL(18,2) '$.GiaNiemYet',
            SoLo VARCHAR(50) '$.SoLo',
            NgaySanXuat DATE '$.NgaySanXuat',
            HanSuDung DATE '$.HanSuDung'
        );

        -- 3. Loop through cart and process
        DECLARE @CurMaSanPham VARCHAR(50);
        DECLARE @CurSoLuong INT;
        DECLARE @CurGiaNhap DECIMAL(18,2);
        DECLARE @CurGiaNiemYet DECIMAL(18,2);
        DECLARE @CurSoLo VARCHAR(50);
        DECLARE @CurNSX DATE;
        DECLARE @CurHSD DATE;
        DECLARE @NewMaLo INT;

        DECLARE curCart CURSOR LOCAL FOR 
        SELECT MaSanPham, SoLuong, GiaNhap, GiaNiemYet, SoLo, NgaySanXuat, HanSuDung FROM #Cart;

        OPEN curCart;
        FETCH NEXT FROM curCart INTO @CurMaSanPham, @CurSoLuong, @CurGiaNhap, @CurGiaNiemYet, @CurSoLo, @CurNSX, @CurHSD;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- CRITICAL FIX: Reset @NewMaLo before selecting
            SET @NewMaLo = NULL;

            -- Check if LoSanXuat exists (by SoLo and MaSanPham)
            SELECT @NewMaLo = MaLo FROM LoSanXuat WHERE SoLo = @CurSoLo AND MaSanPham = @CurMaSanPham;

            IF @NewMaLo IS NULL
            BEGIN
                -- Create new batch
                INSERT INTO LoSanXuat (MaSanPham, SoLo, NgaySanXuat, HanSuDung)
                VALUES (@CurMaSanPham, @CurSoLo, @CurNSX, @CurHSD);
                
                SET @NewMaLo = SCOPE_IDENTITY();
            END

            -- Update product listed price if provided
            IF @CurGiaNiemYet IS NOT NULL AND @CurGiaNiemYet > 0
            BEGIN
                UPDATE SanPham SET GiaNiemYet = @CurGiaNiemYet WHERE MaSanPham = @CurMaSanPham;
            END

            -- Insert into ChiTietNhapKho (the trigger trg_NhapHang_TonKho will automatically add to TonKho)
            INSERT INTO ChiTietNhapKho (MaPhieuNhap, MaLo, SoLuong, GiaNhap)
            VALUES (@MaPhieuNhap, @NewMaLo, @CurSoLuong, @CurGiaNhap);

            FETCH NEXT FROM curCart INTO @CurMaSanPham, @CurSoLuong, @CurGiaNhap, @CurGiaNiemYet, @CurSoLo, @CurNSX, @CurHSD;
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
/****** Object:  StoredProcedure [dbo].[sp_TopKhachHangMuaNhieuNhat]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_TopKhachHangMuaNhieuNhat] AS BEGIN SELECT TOP 5 K.HoTen, K.SoDienThoai, ISNULL(SUM(H.TongTien), 0) AS TongTienMua FROM KhachHang K INNER JOIN HoaDon H ON K.MaKhachHang = H.MaKhachHang GROUP BY K.HoTen, K.SoDienThoai ORDER BY TongTienMua DESC END
GO
/****** Object:  StoredProcedure [dbo].[sp_TopSanPhamBanChay]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Top sản phẩm bán chạy
CREATE   PROCEDURE [dbo].[sp_TopSanPhamBanChay]
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
/****** Object:  StoredProcedure [dbo].[sp_UpdateChamCong]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 3. UPDATE
CREATE   PROCEDURE [dbo].[sp_UpdateChamCong]
    @MaCC INT,
    @MaNhanVien VARCHAR(20),
    @NgayLam DATE,
    @GioVao TIME,
    @GioRa TIME = NULL
AS
BEGIN
    -- Bắt lỗi unique cho bản ghi khác
    IF EXISTS (SELECT 1 FROM ChamCong WHERE MaNhanVien = @MaNhanVien AND NgayLam = @NgayLam AND MaCC != @MaCC)
    BEGIN
        RAISERROR(N'Nhân viên này đã được chấm công trong ngày hôm nay!', 16, 1);
        RETURN;
    END

    UPDATE ChamCong
    SET MaNhanVien = @MaNhanVien,
        NgayLam = @NgayLam,
        GioVao = @GioVao,
        GioRa = @GioRa
    WHERE MaCC = @MaCC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateCuaHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- 3. UPDATE
CREATE   PROCEDURE [dbo].[sp_UpdateCuaHang]
    @MaCuaHang VARCHAR(20),
    @TenCuaHang NVARCHAR(100),
    @DiaChi NVARCHAR(255),
    @SoDienThoai VARCHAR(15),
    @TrangThai BIT
AS
BEGIN
    -- Kiểm tra lỗi unique
    IF EXISTS (SELECT 1 FROM CuaHang WHERE TenCuaHang = @TenCuaHang AND MaCuaHang != @MaCuaHang)
    BEGIN
        RAISERROR(N'Tên cửa hàng đã tồn tại!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM CuaHang WHERE SoDienThoai = @SoDienThoai AND MaCuaHang != @MaCuaHang)
    BEGIN
        RAISERROR(N'Số điện thoại đã tồn tại!', 16, 1);
        RETURN;
    END

    UPDATE CuaHang
    SET TenCuaHang = @TenCuaHang,
        DiaChi = @DiaChi,
        SoDienThoai = @SoDienThoai,
        TrangThai = @TrangThai
    WHERE MaCuaHang = @MaCuaHang;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateDanhMuc]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_UpdateDanhMuc]
    @MaDanhMuc INT,
    @TenDanhMuc NVARCHAR(100)
AS
BEGIN
    UPDATE DanhMuc
    SET TenDanhMuc = @TenDanhMuc
    WHERE MaDanhMuc = @MaDanhMuc;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateKhachHang]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 3. UPDATE
CREATE   PROCEDURE [dbo].[sp_UpdateKhachHang]
    @MaKhachHang INT,
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @DiemTichLuy INT,
    @TrangThai BIT
AS
BEGIN
    UPDATE KhachHang
    SET HoTen = @HoTen,
        SoDienThoai = @SoDienThoai,
        DiemTichLuy = @DiemTichLuy,
        TrangThai = @TrangThai
    WHERE MaKhachHang = @MaKhachHang;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateKhuyenMai]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE   PROCEDURE [dbo].[sp_UpdateKhuyenMai]
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
/****** Object:  StoredProcedure [dbo].[sp_UpdateNhaCungCap]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_UpdateNhaCungCap]
    @MaNhaCungCap INT,
    @TenNhaCungCap NVARCHAR(150),
    @SoDienThoai VARCHAR(15),
    @DiaChi NVARCHAR(255) = NULL
AS
BEGIN
    UPDATE NhaCungCap
    SET TenNhaCungCap = @TenNhaCungCap,
        SoDienThoai = @SoDienThoai,
        DiaChi = @DiaChi
    WHERE MaNhaCungCap = @MaNhaCungCap;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateNhanVien]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_UpdateNhanVien]
    @MaNhanVien VARCHAR(20),
    @HoTen NVARCHAR(100),
    @SoDienThoai VARCHAR(15),
    @VaiTro NVARCHAR(50),
    @TenDangNhap VARCHAR(50),
    @MaCuaHang VARCHAR(20),
    @Email VARCHAR(100),
    @TrangThai BIT
AS
BEGIN
    UPDATE NhanVien
    SET HoTen = @HoTen,
        SoDienThoai = @SoDienThoai,
        VaiTro = @VaiTro,
        TenDangNhap = @TenDangNhap,
        MaCuaHang = @MaCuaHang,
        Email = @Email,
        TrangThai = @TrangThai
    WHERE MaNhanVien = @MaNhanVien;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdatePasswordByEmail]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_UpdatePasswordByEmail]
    @Email VARCHAR(100),
    @MatKhau VARCHAR(255)
AS
BEGIN
    UPDATE NhanVien
    SET MatKhau = @MatKhau
    WHERE Email = @Email AND TrangThai = 1;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateSanPham]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_UpdateSanPham]
    @MaSanPham VARCHAR(50),
    @TenSanPham NVARCHAR(200),
    @MaDanhMuc INT = NULL,
    @MaThuongHieu INT = NULL,
    @MaNhaCungCap INT = NULL,
    @HinhAnh NVARCHAR(MAX) = NULL,
    @DonViTinh NVARCHAR(20) = N'Cái',
    @GiaNiemYet DECIMAL(18,2),
    @TrangThai BIT = 1
AS
BEGIN
    UPDATE SanPham
    SET TenSanPham = @TenSanPham,
        MaDanhMuc = @MaDanhMuc,
        MaThuongHieu = @MaThuongHieu,
        MaNhaCungCap = @MaNhaCungCap,
        HinhAnh = @HinhAnh,
        DonViTinh = ISNULL(@DonViTinh, N'Cái'),
        GiaNiemYet = @GiaNiemYet,
        TrangThai = ISNULL(@TrangThai, 1)
    WHERE MaSanPham = @MaSanPham;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateThuongHieu]    Script Date: 5/25/2026 23:39:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sp_UpdateThuongHieu]
    @MaThuongHieu INT,
    @TenThuongHieu NVARCHAR(100),
    @QuocGia NVARCHAR(50) = NULL
AS
BEGIN
    UPDATE ThuongHieu
    SET TenThuongHieu = @TenThuongHieu,
        QuocGia = ISNULL(@QuocGia, N'Chưa xác định')
    WHERE MaThuongHieu = @MaThuongHieu;
END;

GO


-- ==========================================
-- STORED PROCEDURES LỊCH SỬ (Thêm tự động)
-- ==========================================
/****** Object:  StoredProcedure [dbo].[sp_InsertLichSuTruyCap] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertLichSuTruyCap]
    @UserName NVARCHAR(100),
    @IpAddress VARCHAR(50),
    @DeviceName NVARCHAR(100),
    @Location NVARCHAR(250)
AS
BEGIN
    INSERT INTO LichSuTruyCap (UserName, IpAddress, DeviceName, Location)
    VALUES (@UserName, @IpAddress, @DeviceName, @Location);
END;
GO

/****** Object:  StoredProcedure [dbo].[sp_GetAllLichSuTruyCap] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllLichSuTruyCap]
AS
BEGIN
    SELECT Id, Timestamp, UserName, IpAddress, DeviceName, Location
    FROM LichSuTruyCap
    ORDER BY Timestamp DESC;
END;
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertLichSuChinhSua] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertLichSuChinhSua]
    @UserName NVARCHAR(100),
    @Action NVARCHAR(100),
    @Detail NVARCHAR(MAX),
    @Icon NVARCHAR(50)
AS
BEGIN
    INSERT INTO LichSuChinhSua (UserName, Action, Detail, Icon)
    VALUES (@UserName, @Action, @Detail, @Icon);
END;
GO

/****** Object:  StoredProcedure [dbo].[sp_GetAllLichSuChinhSua] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllLichSuChinhSua]
AS
BEGIN
    SELECT Id, Timestamp, UserName, Action, Detail, Icon
    FROM LichSuChinhSua
    ORDER BY Timestamp DESC;
END;
GO
