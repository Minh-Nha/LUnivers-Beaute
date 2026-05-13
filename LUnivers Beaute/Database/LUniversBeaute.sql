/* =========================================================================
HỆ QUẢN TRỊ CƠ SỞ DỮ LIỆU: MEDICARE SYSTEM MANAGEMENT (FINAL VERSION)
QUY MÔ: 18 BẢNG | ĐẦY ĐỦ RÀNG BUỘC | TỰ ĐỘNG HÓA KHO & TÍCH ĐIỂM | CÓ HÌNH ẢNH
=========================================================================
*/
CREATE DATABASE LUnivers_Beaute;
GO
USE LUnivers_Beaute;
GO

-- =========================================================================
-- PHẦN 1: TẠO BẢNG & RÀNG BUỘC (TABLES & CONSTRAINTS)
-- =========================================================================

-- 1. Danh mục sản phẩm
CREATE TABLE DanhMuc (
    MaDanhMuc INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL CONSTRAINT UC_TenDanhMuc UNIQUE
);

-- 2. Thương hiệu
CREATE TABLE ThuongHieu (
    MaThuongHieu INT IDENTITY(1,1) PRIMARY KEY,
    TenThuongHieu NVARCHAR(100) NOT NULL CONSTRAINT UC_TenThuongHieu UNIQUE,
    QuocGia NVARCHAR(50) DEFAULT N'Chưa xác định'
);

-- 3. Cửa hàng
CREATE TABLE CuaHang (
    MaCuaHang VARCHAR(20) PRIMARY KEY,
    TenCuaHang NVARCHAR(100) NOT NULL CONSTRAINT UC_TenCuaHang UNIQUE,
    DiaChi NVARCHAR(255) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL CONSTRAINT UC_SDT_CuaHang UNIQUE CHECK (SoDienThoai LIKE '[0-9]%'),
    TrangThai BIT DEFAULT 1
);

-- 4. Nhà cung cấp
CREATE TABLE NhaCungCap (
    MaNhaCungCap INT IDENTITY(1,1) PRIMARY KEY,
    TenNhaCungCap NVARCHAR(150) NOT NULL CONSTRAINT UC_TenNCC UNIQUE,
    SoDienThoai VARCHAR(15) NOT NULL CONSTRAINT UC_SDT_NCC UNIQUE CHECK (SoDienThoai LIKE '[0-9]%'),
    DiaChi NVARCHAR(255)
);

-- 5. Nhân viên
CREATE TABLE NhanVien (
    MaNhanVien VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL CONSTRAINT UC_SDT_NhanVien UNIQUE CHECK (SoDienThoai LIKE '[0-9]%'),
    MaCuaHang VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES CuaHang(MaCuaHang),
    VaiTro NVARCHAR(50) DEFAULT N'Nhân viên',
    TenDangNhap VARCHAR(50) NOT NULL CONSTRAINT UC_Login UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    TrangThai BIT DEFAULT 1
);

-- 6. Khách hàng
CREATE TABLE KhachHang (
    MaKhachHang INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL CONSTRAINT UC_SDT_KhachHang UNIQUE CHECK (SoDienThoai LIKE '[0-9]%'),
    DiemTichLuy INT DEFAULT 0 CONSTRAINT CHK_Diem CHECK (DiemTichLuy >= 0),
    TrangThai BIT DEFAULT 1
);

-- 7. Sản phẩm
CREATE TABLE SanPham (
    MaSanPham VARCHAR(50) PRIMARY KEY,
    TenSanPham NVARCHAR(200) NOT NULL CONSTRAINT UC_TenSanPham UNIQUE,
    MaDanhMuc INT FOREIGN KEY REFERENCES DanhMuc(MaDanhMuc),
    MaThuongHieu INT FOREIGN KEY REFERENCES ThuongHieu(MaThuongHieu),
    MaNhaCungCap INT FOREIGN KEY REFERENCES NhaCungCap(MaNhaCungCap),
    HinhAnh NVARCHAR(MAX),
    DonViTinh NVARCHAR(20) DEFAULT N'Cái',
    GiaNiemYet DECIMAL(18,2) NOT NULL CHECK (GiaNiemYet > 0),
    TrangThai BIT DEFAULT 1
);

-- 8. Lô sản xuất
CREATE TABLE LoSanXuat (
    MaLo INT IDENTITY(1,1) PRIMARY KEY,
    MaSanPham VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES SanPham(MaSanPham),
    SoLo VARCHAR(50) NOT NULL,
    NgaySanXuat DATE,
    HanSuDung DATE NOT NULL,
    CONSTRAINT UC_SanPham_Lo UNIQUE (MaSanPham, SoLo),
    CONSTRAINT CHK_HSD CHECK (HanSuDung > NgaySanXuat)
);

-- 9. Phiếu nhập kho
CREATE TABLE PhieuNhapKho (
    MaPhieuNhap VARCHAR(20) PRIMARY KEY,
    MaCuaHang VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES CuaHang(MaCuaHang),
    MaNhanVien VARCHAR(20) NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    NgayNhap DATETIME DEFAULT GETDATE(),
    TongTienNhap DECIMAL(18,2) DEFAULT 0 CHECK (TongTienNhap >= 0)
);

-- 10. Chi tiết nhập kho
CREATE TABLE ChiTietNhapKho (
    MaPhieuNhap VARCHAR(20) FOREIGN KEY REFERENCES PhieuNhapKho(MaPhieuNhap),
    MaLo INT FOREIGN KEY REFERENCES LoSanXuat(MaLo),
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    GiaNhap DECIMAL(18,2) NOT NULL CHECK (GiaNhap > 0),
    PRIMARY KEY (MaPhieuNhap, MaLo)
);

-- 11. Tồn kho
CREATE TABLE TonKho (
    MaCuaHang VARCHAR(20) FOREIGN KEY REFERENCES CuaHang(MaCuaHang),
    MaLo INT FOREIGN KEY REFERENCES LoSanXuat(MaLo),
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0),
    PRIMARY KEY (MaCuaHang, MaLo)
);

-- 12. Khuyến mãi
CREATE TABLE KhuyenMai (
    MaKhuyenMai INT IDENTITY(1,1) PRIMARY KEY,
    TenChuongTrinh NVARCHAR(150) NOT NULL CONSTRAINT UC_TenKM UNIQUE,
    GiaTriGiam DECIMAL(18,2) NOT NULL CHECK (GiaTriGiam >= 0),
    LoaiGiam VARCHAR(20) CHECK (LoaiGiam IN ('%', 'VND')), 
    ApDungTheo VARCHAR(20) CHECK (ApDungTheo IN ('HoaDon', 'DanhMuc', 'SanPham')) DEFAULT 'HoaDon',
    MaDanhMuc INT FOREIGN KEY REFERENCES DanhMuc(MaDanhMuc),
    MaSanPham VARCHAR(50) FOREIGN KEY REFERENCES SanPham(MaSanPham),
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    CONSTRAINT CHK_ThoiGianKM CHECK (NgayKetThuc >= NgayBatDau),
    TrangThai BIT DEFAULT 1
);

-- 13. Hóa đơn
CREATE TABLE HoaDon (
    MaHoaDon VARCHAR(20) PRIMARY KEY,
    MaCuaHang VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES CuaHang(MaCuaHang),
    MaNhanVien VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    MaKhachHang INT FOREIGN KEY REFERENCES KhachHang(MaKhachHang),
    MaKhuyenMai INT FOREIGN KEY REFERENCES KhuyenMai(MaKhuyenMai),
    NgayLap DATETIME DEFAULT GETDATE(),
    PhuongThucThanhToan NVARCHAR(50) DEFAULT N'Tiền mặt',
    TongTien DECIMAL(18,2) DEFAULT 0 CHECK (TongTien >= 0),
    KhachCanTra DECIMAL(18,2) DEFAULT 0,
    CONSTRAINT CHK_ThanhToan_Logic CHECK (KhachCanTra <= TongTien)
);

-- 14. Chi tiết hóa đơn
CREATE TABLE ChiTietHoaDon (
    MaHoaDon VARCHAR(20) FOREIGN KEY REFERENCES HoaDon(MaHoaDon),
    MaLo INT FOREIGN KEY REFERENCES LoSanXuat(MaLo),
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
    ThanhTien AS (SoLuong * DonGia), 
    PRIMARY KEY (MaHoaDon, MaLo)
);

-- 15. Phiếu trả hàng
CREATE TABLE PhieuTraHang (
    MaPhieuTra VARCHAR(20) PRIMARY KEY,
    MaHoaDon VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHoaDon),
    NgayTra DATETIME DEFAULT GETDATE(),
    LyDo NVARCHAR(255) NOT NULL
);

-- 16. Chi tiết phiếu trả hàng
CREATE TABLE ChiTietPhieuTraHang (
    MaPhieuTra VARCHAR(20) FOREIGN KEY REFERENCES PhieuTraHang(MaPhieuTra),
    MaLo INT FOREIGN KEY REFERENCES LoSanXuat(MaLo),
    SoLuongTra INT NOT NULL CHECK (SoLuongTra > 0),
    SoTienHoan DECIMAL(18,2) DEFAULT 0,
    PRIMARY KEY (MaPhieuTra, MaLo)
);

-- 17. Kiểm kê
CREATE TABLE KiemKe (
    MaKiemKe INT IDENTITY(1,1) PRIMARY KEY,
    MaCuaHang VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES CuaHang(MaCuaHang),
    MaLo INT NOT NULL FOREIGN KEY REFERENCES LoSanXuat(MaLo),
    SoLuongHeThong INT DEFAULT 0,
    SoLuongThucTe INT DEFAULT 0 CHECK (SoLuongThucTe >= 0),
    SoLuongLech AS (SoLuongThucTe - SoLuongHeThong),
    NgayKiem DATETIME DEFAULT GETDATE()
);

-- 18. Chấm công
CREATE TABLE ChamCong (
    MaCC INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVien VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    NgayLam DATE DEFAULT CAST(GETDATE() AS DATE),
    GioVao TIME NOT NULL,
    GioRa TIME,
    CONSTRAINT CHK_GioLamViec CHECK (GioRa > GioVao)
);

-- 19. Hủy sản phẩm quá hạn
CREATE TABLE HuySanPham (
    MaHuy INT IDENTITY(1,1) PRIMARY KEY,
    MaCuaHang VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES CuaHang(MaCuaHang),
    MaNhanVien VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNhanVien),
    MaLo INT NOT NULL FOREIGN KEY REFERENCES LoSanXuat(MaLo),
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    NgayHuy DATETIME DEFAULT GETDATE(),
    LyDo NVARCHAR(255) DEFAULT N'Sản phẩm quá hạn sử dụng'
);
GO

-- =========================================================================
-- PHẦN 2: TẠO TRIGGER TỰ ĐỘNG (AUTOMATION)
-- =========================================================================

-- 1. Tự động bảo toàn kho khi Nhập hàng (Thêm / Sửa / Xóa)
CREATE TRIGGER trg_NhapHang_TonKho
ON ChiTietNhapKho
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Khôi phục số lượng kho cũ nếu là hành động Xóa / Sửa
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE TonKho
        SET SoLuongTon = SoLuongTon - d.SoLuong
        FROM TonKho tk
        JOIN deleted d ON tk.MaLo = d.MaLo
        JOIN PhieuNhapKho pnk ON d.MaPhieuNhap = pnk.MaPhieuNhap
        WHERE tk.MaCuaHang = pnk.MaCuaHang;
    END

    -- Cập nhật số lượng kho mới nếu là hành động Thêm / Sửa
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        -- Chèn các bản ghi kho bị thiếu
        INSERT INTO TonKho (MaCuaHang, MaLo, SoLuongTon)
        SELECT pnk.MaCuaHang, i.MaLo, 0
        FROM inserted i
        JOIN PhieuNhapKho pnk ON i.MaPhieuNhap = pnk.MaPhieuNhap
        WHERE NOT EXISTS (
            SELECT 1 FROM TonKho tk 
            WHERE tk.MaCuaHang = pnk.MaCuaHang AND tk.MaLo = i.MaLo
        );

        -- Cộng số lượng mới vào
        UPDATE TonKho
        SET SoLuongTon = SoLuongTon + i.SoLuong
        FROM TonKho tk
        JOIN inserted i ON tk.MaLo = i.MaLo
        JOIN PhieuNhapKho pnk ON i.MaPhieuNhap = pnk.MaPhieuNhap
        WHERE tk.MaCuaHang = pnk.MaCuaHang;
    END
END;
GO

-- 2. Tự động bảo toàn kho khi Bán hàng (Thêm / Sửa / Xóa)
CREATE TRIGGER trg_BanHang_TonKho
ON ChiTietHoaDon
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Kiểm tra hạn sử dụng nếu có hàng mới được thêm vào
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        IF EXISTS (
            SELECT 1 FROM inserted i
            JOIN LoSanXuat lsx ON i.MaLo = lsx.MaLo
            WHERE lsx.HanSuDung < CAST(GETDATE() AS DATE)
        )
        BEGIN
             RAISERROR (N'Lỗi: Phát hiện sản phẩm trong hóa đơn đã qua hạn sử dụng!', 16, 1);
             ROLLBACK TRANSACTION;
             RETURN;
        END
    END

    -- Hoàn lại kho nếu hóa đơn bị Sửa / Xóa
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE TonKho
        SET SoLuongTon = SoLuongTon + d.SoLuong
        FROM TonKho tk
        JOIN deleted d ON tk.MaLo = d.MaLo
        JOIN HoaDon hd ON d.MaHoaDon = hd.MaHoaDon
        WHERE tk.MaCuaHang = hd.MaCuaHang;
    END

    -- Trừ đi kho nếu hóa đơn được Thêm / Sửa
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE TonKho
        SET SoLuongTon = SoLuongTon - i.SoLuong
        FROM TonKho tk
        JOIN inserted i ON tk.MaLo = i.MaLo
        JOIN HoaDon hd ON i.MaHoaDon = hd.MaHoaDon
        WHERE tk.MaCuaHang = hd.MaCuaHang;
    END

    -- Ngăn chặn nếu kho bị âm
    IF EXISTS (SELECT 1 FROM TonKho WHERE SoLuongTon < 0)
    BEGIN
        RAISERROR (N'Lỗi: Số lượng tồn kho không đủ để xuất bán!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 3. Đồng bộ Tổng Tiền & Khách Cần Trả trong Hóa Đơn
CREATE TRIGGER trg_HoaDon_DongBoTien
ON ChiTietHoaDon
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Cập nhật TongTien
    UPDATE HoaDon
    SET TongTien = T.TotalAmount
    FROM HoaDon hd
    JOIN (
        SELECT MaHoaDon, ISNULL(SUM(ThanhTien), 0) as TotalAmount
        FROM ChiTietHoaDon
        WHERE MaHoaDon IN (SELECT DISTINCT MaHoaDon FROM inserted UNION SELECT DISTINCT MaHoaDon FROM deleted)
        GROUP BY MaHoaDon
    ) T ON hd.MaHoaDon = T.MaHoaDon;

    -- Update những hóa đơn mất hết CT HoaDon bị SUM=NULL
    UPDATE HoaDon
    SET TongTien = 0
    WHERE MaHoaDon IN (SELECT DISTINCT MaHoaDon FROM inserted UNION SELECT DISTINCT MaHoaDon FROM deleted) 
          AND MaHoaDon NOT IN (SELECT DISTINCT MaHoaDon FROM ChiTietHoaDon);

    -- Cập nhật KhachCanTra dựa vào khuyến mãi
    UPDATE HoaDon
    SET KhachCanTra = CASE 
        WHEN km.LoaiGiam = '%' THEN hd.TongTien - (hd.TongTien * km.GiaTriGiam / 100)
        WHEN km.LoaiGiam = 'VND' THEN hd.TongTien - km.GiaTriGiam
        ELSE hd.TongTien
    END
    FROM HoaDon hd
    LEFT JOIN KhuyenMai km ON hd.MaKhuyenMai = km.MaKhuyenMai
    WHERE hd.MaHoaDon IN (SELECT DISTINCT MaHoaDon FROM inserted UNION SELECT DISTINCT MaHoaDon FROM deleted);

    -- Đảm bảo KhachCanTra không bị âm
    UPDATE HoaDon
    SET KhachCanTra = 0
    WHERE KhachCanTra < 0 AND MaHoaDon IN (SELECT DISTINCT MaHoaDon FROM inserted UNION SELECT DISTINCT MaHoaDon FROM deleted);
END;
GO

-- 4. Tự động nhận kho khi phiếu xuất bị trả lại (Thêm / Sửa / Xóa)
CREATE TRIGGER trg_TraHang_CongKho
ON ChiTietPhieuTraHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Hoàn tác kho nếu xóa / sửa phiếu trả
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE TonKho
        SET SoLuongTon = SoLuongTon - d.SoLuongTra
        FROM TonKho tk
        JOIN deleted d ON tk.MaLo = d.MaLo
        JOIN PhieuTraHang pt ON d.MaPhieuTra = pt.MaPhieuTra
        JOIN HoaDon hd ON pt.MaHoaDon = hd.MaHoaDon
        WHERE tk.MaCuaHang = hd.MaCuaHang;
    END

    -- Thêm kho nếu nhận trả hàng
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE TonKho
        SET SoLuongTon = SoLuongTon + i.SoLuongTra
        FROM TonKho tk
        JOIN inserted i ON tk.MaLo = i.MaLo
        JOIN PhieuTraHang pt ON i.MaPhieuTra = pt.MaPhieuTra
        JOIN HoaDon hd ON pt.MaHoaDon = hd.MaHoaDon
        WHERE tk.MaCuaHang = hd.MaCuaHang;
    END
END;
GO

-- 5. Tự động tích điểm cho khách hàng sau khi hóa đơn được thanh toán
CREATE TRIGGER trg_TichDiemKhachHang
ON HoaDon
AFTER INSERT, UPDATE
AS
BEGIN
    -- Chỉ chạy nếu trường KhachCanTra thực sự bị ảnh hưởng
    IF UPDATE(KhachCanTra)
    BEGIN
        UPDATE KhachHang
        SET DiemTichLuy = DiemTichLuy + FLOOR(i.KhachCanTra / 10000) - ISNULL(FLOOR(d.KhachCanTra / 10000), 0)
        FROM KhachHang kh
        JOIN inserted i ON kh.MaKhachHang = i.MaKhachHang
        LEFT JOIN deleted d ON i.MaHoaDon = d.MaHoaDon
        WHERE i.MaKhachHang IS NOT NULL;
    END
END;
GO

-- =========================================================================
-- PHẦN 3: TẠO CHỈ MỤC INDEX CHO PERFOMANCE (OPTIMIZATION)
-- =========================================================================

-- -----------------------------------------------------------------------
-- A. BẢNG SẢN PHẨM (SanPham) - Tra cứu theo danh mục, thương hiệu, NCC
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_SanPham_DanhMuc ON SanPham(MaDanhMuc);
CREATE NONCLUSTERED INDEX IX_SanPham_ThuongHieu ON SanPham(MaThuongHieu);
CREATE NONCLUSTERED INDEX IX_SanPham_NhaCungCap ON SanPham(MaNhaCungCap);
CREATE NONCLUSTERED INDEX IX_SanPham_TrangThai ON SanPham(TrangThai);

-- -----------------------------------------------------------------------
-- B. BẢNG LÔ SẢN XUẤT (LoSanXuat) - Kiểm tra hạn sử dụng & tra cứu SP
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_LoSanXuat_MaSanPham ON LoSanXuat(MaSanPham);
CREATE NONCLUSTERED INDEX IX_LoSanXuat_HanSuDung ON LoSanXuat(HanSuDung);

-- -----------------------------------------------------------------------
-- C. BẢNG NHÂN VIÊN (NhanVien) - Tra cứu theo cửa hàng, SĐT, đăng nhập
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_NhanVien_MaCuaHang ON NhanVien(MaCuaHang);
CREATE NONCLUSTERED INDEX IX_NhanVien_SoDienThoai ON NhanVien(SoDienThoai);
CREATE NONCLUSTERED INDEX IX_NhanVien_TrangThai ON NhanVien(TrangThai);

-- -----------------------------------------------------------------------
-- D. BẢNG KHÁCH HÀNG (KhachHang) - Tra cứu theo SĐT, trạng thái
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_KhachHang_SoDienThoai ON KhachHang(SoDienThoai);
CREATE NONCLUSTERED INDEX IX_KhachHang_TrangThai ON KhachHang(TrangThai);

-- -----------------------------------------------------------------------
-- E. BẢNG TỒN KHO (TonKho) - Cảnh báo tồn kho thấp, lọc theo cửa hàng
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_TonKho_SoLuongTon ON TonKho(SoLuongTon);
CREATE NONCLUSTERED INDEX IX_TonKho_MaLo ON TonKho(MaLo);

-- -----------------------------------------------------------------------
-- F. BẢNG PHIẾU NHẬP KHO (PhieuNhapKho) - Lọc theo cửa hàng, ngày nhập
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_PhieuNhapKho_MaCuaHang ON PhieuNhapKho(MaCuaHang);
CREATE NONCLUSTERED INDEX IX_PhieuNhapKho_MaNhanVien ON PhieuNhapKho(MaNhanVien);
CREATE NONCLUSTERED INDEX IX_PhieuNhapKho_NgayNhap ON PhieuNhapKho(NgayNhap DESC);

-- -----------------------------------------------------------------------
-- G. BẢNG CHI TIẾT NHẬP KHO (ChiTietNhapKho) - Tra cứu theo lô
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_ChiTietNhapKho_MaLo ON ChiTietNhapKho(MaLo);

-- -----------------------------------------------------------------------
-- H. BẢNG HÓA ĐƠN (HoaDon) - Lọc cửa hàng, ngày, nhân viên, khách hàng
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_HoaDon_MaCuaHang ON HoaDon(MaCuaHang);
CREATE NONCLUSTERED INDEX IX_HoaDon_MaNhanVien ON HoaDon(MaNhanVien);
CREATE NONCLUSTERED INDEX IX_HoaDon_MaKhachHang ON HoaDon(MaKhachHang);
CREATE NONCLUSTERED INDEX IX_HoaDon_MaKhuyenMai ON HoaDon(MaKhuyenMai);
CREATE NONCLUSTERED INDEX IX_HoaDon_NgayLap ON HoaDon(NgayLap DESC);
-- Index tổng hợp cho báo cáo doanh thu: lọc theo cửa hàng + ngày
CREATE NONCLUSTERED INDEX IX_HoaDon_CuaHang_NgayLap ON HoaDon(MaCuaHang, NgayLap DESC)
    INCLUDE (KhachCanTra, TongTien);

-- -----------------------------------------------------------------------
-- I. BẢNG CHI TIẾT HÓA ĐƠN (ChiTietHoaDon) - Tra cứu theo lô, thống kê
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_ChiTietHoaDon_MaLo ON ChiTietHoaDon(MaLo);

-- -----------------------------------------------------------------------
-- J. BẢNG PHIẾU TRẢ HÀNG (PhieuTraHang) - Tra cứu theo hóa đơn, ngày
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_PhieuTraHang_MaHoaDon ON PhieuTraHang(MaHoaDon);
CREATE NONCLUSTERED INDEX IX_PhieuTraHang_NgayTra ON PhieuTraHang(NgayTra DESC);

-- -----------------------------------------------------------------------
-- K. BẢNG CHI TIẾT PHIẾU TRẢ HÀNG (ChiTietPhieuTraHang) - Tra cứu lô
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_ChiTietPhieuTraHang_MaLo ON ChiTietPhieuTraHang(MaLo);

-- -----------------------------------------------------------------------
-- L. BẢNG KIỂM KÊ (KiemKe) - Lọc theo cửa hàng, ngày kiểm
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_KiemKe_MaCuaHang ON KiemKe(MaCuaHang);
CREATE NONCLUSTERED INDEX IX_KiemKe_MaLo ON KiemKe(MaLo);
CREATE NONCLUSTERED INDEX IX_KiemKe_NgayKiem ON KiemKe(NgayKiem DESC);

-- -----------------------------------------------------------------------
-- M. BẢNG CHẤM CÔNG (ChamCong) - Lọc theo nhân viên, ngày làm
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_ChamCong_MaNhanVien ON ChamCong(MaNhanVien);
CREATE NONCLUSTERED INDEX IX_ChamCong_NgayLam ON ChamCong(NgayLam DESC);
-- Index tổng hợp cho truy vấn chấm công: lọc NV + khoảng ngày
CREATE NONCLUSTERED INDEX IX_ChamCong_NhanVien_NgayLam ON ChamCong(MaNhanVien, NgayLam DESC);

-- -----------------------------------------------------------------------
-- N. BẢNG HỦY SẢN PHẨM (HuySanPham) - Lọc theo cửa hàng, ngày hủy
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_HuySanPham_MaCuaHang ON HuySanPham(MaCuaHang);
CREATE NONCLUSTERED INDEX IX_HuySanPham_MaNhanVien ON HuySanPham(MaNhanVien);
CREATE NONCLUSTERED INDEX IX_HuySanPham_MaLo ON HuySanPham(MaLo);
CREATE NONCLUSTERED INDEX IX_HuySanPham_NgayHuy ON HuySanPham(NgayHuy DESC);

-- -----------------------------------------------------------------------
-- O. BẢNG KHUYẾN MÃI (KhuyenMai) - Lọc theo danh mục, sản phẩm, ngày
-- -----------------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_KhuyenMai_MaDanhMuc ON KhuyenMai(MaDanhMuc);
CREATE NONCLUSTERED INDEX IX_KhuyenMai_MaSanPham ON KhuyenMai(MaSanPham);
CREATE NONCLUSTERED INDEX IX_KhuyenMai_NgayKetThuc ON KhuyenMai(NgayKetThuc DESC);
CREATE NONCLUSTERED INDEX IX_KhuyenMai_TrangThai ON KhuyenMai(TrangThai);
GO

PRINT 'DATABASE L UNIVERS BEAUTE FINAL UPDATED VERSION ĐÃ SẴN SÀNG!';