USE LUnivers_Beaute;
GO

-- XÓA DỮ LIỆU CŨ ĐỂ CHẠY LẠI KHÔNG BỊ LỖI TRÙNG KHÓA
DELETE FROM HuySanPham;
DELETE FROM ChamCong;
DELETE FROM KiemKe;
DELETE FROM ChiTietPhieuTraHang;
DELETE FROM PhieuTraHang;
DELETE FROM ChiTietHoaDon;
DELETE FROM HoaDon;
DELETE FROM KhuyenMai;
DELETE FROM ChiTietNhapKho;
DELETE FROM PhieuNhapKho;
DELETE FROM TonKho;
DELETE FROM LoSanXuat;
DELETE FROM SanPham;
DELETE FROM KhachHang;
DELETE FROM NhanVien;
DELETE FROM NhaCungCap;
DELETE FROM CuaHang;
DELETE FROM ThuongHieu;
DELETE FROM DanhMuc;

-- Reset identity seeds
DBCC CHECKIDENT ('DanhMuc', RESEED, 0);
DBCC CHECKIDENT ('ThuongHieu', RESEED, 0);
DBCC CHECKIDENT ('NhaCungCap', RESEED, 0);
DBCC CHECKIDENT ('KhachHang', RESEED, 0);
DBCC CHECKIDENT ('LoSanXuat', RESEED, 0);
DBCC CHECKIDENT ('KhuyenMai', RESEED, 0);
DBCC CHECKIDENT ('KiemKe', RESEED, 0);
DBCC CHECKIDENT ('ChamCong', RESEED, 0);
DBCC CHECKIDENT ('HuySanPham', RESEED, 0);
GO

-- 1. DanhMuc (20 rows)
INSERT INTO DanhMuc (TenDanhMuc) VALUES 
(N'Son môi'), (N'Kem dưỡng da'), (N'Sữa rửa mặt'), (N'Nước hoa'), (N'Mascara'),
(N'Phấn nền'), (N'Tẩy trang'), (N'Kem chống nắng'), (N'Serum'), (N'Mặt nạ'),
(N'Xịt khoáng'), (N'Dưỡng tóc'), (N'Sữa tắm'), (N'Dầu gội'), (N'Kem mắt'),
(N'Tẩy tế bào chết'), (N'Nước hoa hồng'), (N'Cushion'), (N'Chì kẻ mày'), (N'Má hồng');

-- 2. ThuongHieu (20 rows)
INSERT INTO ThuongHieu (TenThuongHieu, QuocGia) VALUES 
(N'Lancôme', N'Pháp'), (N'La Roche-Posay', N'Pháp'), (N'Dior', N'Pháp'), (N'Chanel', N'Pháp'), (N'MAC', N'Mỹ'),
(N'Obagi', N'Mỹ'), (N'CeraVe', N'Mỹ'), (N'Maybelline', N'Mỹ'), (N'Anessa', N'Nhật Bản'), (N'SK-II', N'Nhật Bản'),
(N'Shiseido', N'Nhật Bản'), (N'Innisfree', N'Hàn Quốc'), (N'Laneige', N'Hàn Quốc'), (N'Sulwhasoo', N'Hàn Quốc'), (N'Vichy', N'Pháp'),
(N'L''Oréal', N'Pháp'), (N'Estée Lauder', N'Mỹ'), (N'Kiehl''s', N'Mỹ'), (N'The Ordinary', N'Canada'), (N'Bioderma', N'Pháp');

-- 3. CuaHang (20 rows)
INSERT INTO CuaHang (MaCuaHang, TenCuaHang, DiaChi, SoDienThoai) VALUES 
('CH001', N'L''Univers Quận 1', N'123 Lê Lợi, Q.1, TP.HCM', '0901234567'),
('CH002', N'L''Univers Quận 3', N'456 Nguyễn Đình Chiểu, Q.3, TP.HCM', '0901234568'),
('CH003', N'L''Univers Quận 7', N'789 Nguyễn Văn Linh, Q.7, TP.HCM', '0901234569'),
('CH004', N'L''Univers Thủ Đức', N'101 Võ Văn Ngân, TP. Thủ Đức', '0901234570'),
('CH005', N'L''Univers Bình Thạnh', N'202 Phan Đăng Lưu, Q. Bình Thạnh', '0901234571'),
('CH006', N'L''Univers Gò Vấp', N'303 Quang Trung, Q. Gò Vấp', '0901234572'),
('CH007', N'L''Univers Tân Bình', N'404 Cộng Hòa, Q. Tân Bình', '0901234573'),
('CH008', N'L''Univers Hoàn Kiếm', N'12 Hai Bà Trưng, Q. Hoàn Kiếm, Hà Nội', '0901234574'),
('CH009', N'L''Univers Đống Đa', N'34 Chùa Bộc, Q. Đống Đa, Hà Nội', '0901234575'),
('CH010', N'L''Univers Ba Đình', N'56 Kim Mã, Q. Ba Đình, Hà Nội', '0901234576'),
('CH011', N'L''Univers Hải Phòng', N'78 Lạch Tray, Ngô Quyền, Hải Phòng', '0901234577'),
('CH012', N'L''Univers Đà Nẵng', N'90 Hùng Vương, Hải Châu, Đà Nẵng', '0901234578'),
('CH013', N'L''Univers Cần Thơ', N'12 Mậu Thân, Ninh Kiều, Cần Thơ', '0901234579'),
('CH014', N'L''Univers Biên Hòa', N'34 Phạm Văn Thuận, Biên Hòa', '0901234580'),
('CH015', N'L''Univers Vũng Tàu', N'56 Ba Cu, TP. Vũng Tàu', '0901234581'),
('CH016', N'L''Univers Nha Trang', N'78 Trần Phú, TP. Nha Trang', '0901234582'),
('CH017', N'L''Univers Huế', N'90 Lê Lợi, TP. Huế', '0901234583'),
('CH018', N'L''Univers Buôn Ma Thuột', N'12 Phan Chu Trinh, Buôn Ma Thuột', '0901234584'),
('CH019', N'L''Univers Quy Nhơn', N'34 Nguyễn Huệ, TP. Quy Nhơn', '0901234585'),
('CH020', N'L''Univers Long Xuyên', N'56 Trần Hưng Đạo, Long Xuyên', '0901234586');

-- 4. NhaCungCap (20 rows)
INSERT INTO NhaCungCap (TenNhaCungCap, SoDienThoai, DiaChi) VALUES 
(N'Công ty Mỹ phẩm Sài Gòn', '0281234567', N'Quận 1, TP.HCM'),
(N'Dược phẩm Hậu Giang', '0291234567', N'Cần Thơ'),
(N'Phân phối Estée Lauder VN', '0287654321', N'Quận 3, TP.HCM'),
(N'Mỹ phẩm Shiseido Việt Nam', '0281112223', N'Quận 7, TP.HCM'),
(N'Công ty TNHH L''Oreal VN', '0284445556', N'Quận 1, TP.HCM'),
(N'Mỹ phẩm LG Vina', '0287778889', N'Quận 2, TP.HCM'),
(N'Rohto-Mentholatum VN', '0274123456', N'Bình Dương'),
(N'Mỹ phẩm Menard VN', '0289990001', N'Quận 1, TP.HCM'),
(N'Công ty TNHH AmorePacific VN', '0282223334', N'Quận 3, TP.HCM'),
(N'Mỹ phẩm Oriflame VN', '0285556667', N'Quận 1, TP.HCM'),
(N'Công ty TNHH Beiersdorf VN', '0288889990', N'Quận 7, TP.HCM'),
(N'Mỹ phẩm Watsons VN', '0281212121', N'Quận 1, TP.HCM'),
(N'Guardian Việt Nam', '0283434343', N'Quận 4, TP.HCM'),
(N'Hasaki Beauty & Spa', '0285656565', N'Quận 10, TP.HCM'),
(N'Beauty Garden Distribution', '0287878787', N'Quận Tân Bình, TP.HCM'),
(N'Sammi Shop Distribution', '0289090909', N'Quận Đống Đa, Hà Nội'),
(N'Nuty Cosmetics VN', '0281313131', N'Quận 1, TP.HCM'),
(N'Mint Cosmetics Distribution', '0241414141', N'Quận Ba Đình, Hà Nội'),
(N'CocoShop Distribution', '0241515151', N'Quận Cầu Giấy, Hà Nội'),
(N'Lam Thảo Cosmetics Distribution', '0281616161', N'Quận Bình Thạnh, TP.HCM');

-- 5. NhanVien (20 rows)
INSERT INTO NhanVien (MaNhanVien, HoTen, SoDienThoai, MaCuaHang, VaiTro, TenDangNhap, MatKhau) VALUES 
('NV001', N'Nguyễn Văn An', '0912345001', 'CH001', N'Quản lý', 'admin1', '123'),
('NV002', N'Lê Thị Bình', '0912345002', 'CH001', N'Nhân viên', 'nv1', '123'),
('NV003', N'Trần Văn Cường', '0912345003', 'CH002', N'Quản lý', 'admin2', '123'),
('NV004', N'Phạm Thị Dung', '0912345004', 'CH002', N'Nhân viên', 'nv2', '123'),
('NV005', N'Hoàng Văn Em', '0912345005', 'CH003', N'Nhân viên', 'nv3', '123'),
('NV006', N'Vũ Thị Phương', '0912345006', 'CH003', N'Nhân viên', 'nv4', '123'),
('NV007', N'Lý Văn Giang', '0912345007', 'CH004', N'Nhân viên', 'nv5', '123'),
('NV008', N'Đặng Thị Hoa', '0912345008', 'CH005', N'Nhân viên', 'nv6', '123'),
('NV009', N'Bùi Văn Hùng', '0912345009', 'CH006', N'Nhân viên', 'nv7', '123'),
('NV010', N'Ngô Thị Hương', '0912345010', 'CH007', N'Nhân viên', 'nv8', '123'),
('NV011', N'Đỗ Văn Kiên', '0912345011', 'CH008', N'Nhân viên', 'nv9', '123'),
('NV012', N'Hồ Thị Lan', '0912345012', 'CH009', N'Nhân viên', 'nv10', '123'),
('NV013', N'Mai Văn Minh', '0912345013', 'CH010', N'Nhân viên', 'nv11', '123'),
('NV014', N'Đoàn Thị Nga', '0912345014', 'CH011', N'Nhân viên', 'nv12', '123'),
('NV015', N'Trịnh Văn Oanh', '0912345015', 'CH012', N'Nhân viên', 'nv13', '123'),
('NV016', N'Lương Thị Phúc', '0912345016', 'CH013', N'Nhân viên', 'nv14', '123'),
('NV017', N'Phan Văn Quân', '0912345017', 'CH014', N'Nhân viên', 'nv15', '123'),
('NV018', N'Tô Thị Thảo', '0912345018', 'CH015', N'Nhân viên', 'nv16', '123'),
('NV019', N'Quách Văn Uy', '0912345019', 'CH016', N'Nhân viên', 'nv17', '123'),
('NV020', N'Dương Thị Xuân', '0912345020', 'CH017', N'Nhân viên', 'nv18', '123');

-- 6. KhachHang (20 rows)
INSERT INTO KhachHang (HoTen, SoDienThoai, DiemTichLuy) VALUES 
(N'Nguyễn Thị Hồng', '0987654001', 100), (N'Trần Văn Tâm', '0987654002', 250), (N'Lê Minh Tú', '0987654003', 0),
(N'Phạm Hoàng Nam', '0987654004', 500), (N'Vũ Ngọc Anh', '0987654005', 10), (N'Hoàng Thùy Linh', '0987654006', 120),
(N'Đặng Quốc Bảo', '0987654007', 300), (N'Bùi Minh Tuyết', '0987654008', 50), (N'Ngô Văn Tài', '0987654009', 80),
(N'Đỗ Thị Diệu', '0987654010', 15), (N'Hồ Hoàng Hải', '0987654011', 200), (N'Mai Kim Ngân', '0987654012', 400),
(N'Đoàn Văn Khoa', '0987654013', 0), (N'Trịnh Thu Trang', '0987654014', 600), (N'Lương Văn Lộc', '0987654015', 75),
(N'Phan Ngọc Mai', '0987654016', 110), (N'Tô Đình Phước', '0987654017', 20), (N'Quách Thu Hà', '0987654018', 350),
(N'Dương Quốc Trung', '0987654019', 90), (N'Lý Mỹ Hạnh', '0987654020', 5);

-- 7. SanPham (20 rows)
INSERT INTO SanPham (MaSanPham, TenSanPham, MaDanhMuc, MaThuongHieu, MaNhaCungCap, GiaNiemYet) VALUES 
('SP001', N'Son MAC Ruby Woo', 1, 5, 1, 450000),
('SP002', N'Serum Lancôme Advanced Génifique', 9, 1, 3, 2500000),
('SP003', N'Kem chống nắng Anessa Perfect UV', 8, 9, 4, 650000),
('SP004', N'Nước hoa Chanel No.5', 4, 4, 5, 3800000),
('SP005', N'Sữa rửa mặt CeraVe Hydrating', 3, 7, 7, 380000),
('SP006', N'Mặt nạ Innisfree Super Volcanic', 10, 12, 9, 320000),
('SP007', N'Tẩy trang Bioderma Sensibio H2O', 7, 20, 10, 420000),
('SP008', N'Serum The Ordinary Niacinamide 10%', 9, 19, 11, 280000),
('SP009', N'Kem dưỡng La Roche-Posay B5', 2, 2, 5, 350000),
('SP010', N'Phấn nền Dior Forever Skin Glow', 6, 3, 13, 1450000),
('SP011', N'Sữa tắm Vichy Ideal Body', 13, 15, 5, 450000),
('SP012', N'Dầu gội L''Oréal Professionnel', 14, 16, 5, 550000),
('SP013', N'Kem mắt Estée Lauder Night Repair', 15, 17, 3, 1850000),
('SP014', N'Mascara Maybelline Hyper Curl', 5, 8, 8, 160000),
('SP015', N'Xịt khoáng La Roche-Posay Thermal', 11, 2, 5, 320000),
('SP016', N'Nước hoa hồng Thayer Rose Petal', 17, 19, 15, 290000),
('SP017', N'Má hồng 3CE Rose Beige', 20, 12, 9, 350000),
('SP018', N'Chì kẻ mày Innisfree Auto Eyebrow', 19, 12, 9, 120000),
('SP019', N'Cushion Laneige Neo Matte', 18, 13, 9, 850000),
('SP020', N'Tẩy tế bào chết Cure Natural Aqua', 16, 11, 4, 680000);

-- 8. LoSanXuat (20 rows)
INSERT INTO LoSanXuat (MaSanPham, SoLo, NgaySanXuat, HanSuDung) VALUES 
('SP001', 'LOT001', '2024-01-01', '2027-01-01'),
('SP001', 'LOT002', '2024-06-01', '2027-06-01'),
('SP002', 'LOT001', '2024-02-15', '2027-02-15'),
('SP003', 'LOT001', '2024-03-20', '2027-03-20'),
('SP004', 'LOT001', '2024-12-01', '2028-12-01'),
('SP005', 'LOT001', '2024-04-10', '2027-04-10'),
('SP010', 'LOT001', '2024-05-05', '2027-05-05'),
('SP015', 'LOT001', '2024-01-20', '2027-01-20'),
('SP020', 'LOT001', '2024-02-01', '2027-02-01'),
('SP007', 'LOT001', '2024-03-01', '2027-03-01'),
('SP008', 'LOT001', '2024-04-01', '2027-04-01'),
('SP009', 'LOT001', '2024-05-01', '2027-05-01'),
('SP006', 'LOT001', '2024-06-01', '2027-06-01'),
('SP011', 'LOT001', '2024-07-01', '2027-07-01'),
('SP012', 'LOT001', '2024-08-01', '2027-08-01'),
('SP013', 'LOT001', '2024-09-01', '2027-09-01'),
('SP014', 'LOT001', '2024-10-01', '2027-10-01'),
('SP016', 'LOT001', '2024-11-01', '2027-11-01'),
('SP017', 'LOT001', '2024-12-01', '2027-12-01'),
('SP018', 'LOT001', '2025-01-01', '2028-01-01');

-- 9. PhieuNhapKho (20 rows)
INSERT INTO PhieuNhapKho (MaPhieuNhap, MaCuaHang, MaNhanVien, NgayNhap, TongTienNhap) VALUES 
('PN001', 'CH001', 'NV001', '2024-01-01 08:00', 0),
('PN002', 'CH001', 'NV001', '2024-01-05 09:00', 0),
('PN003', 'CH002', 'NV003', '2024-01-10 10:00', 0),
('PN004', 'CH003', 'NV005', '2024-01-15 11:00', 0),
('PN005', 'CH004', 'NV007', '2024-01-20 12:00', 0),
('PN006', 'CH005', 'NV008', '2024-01-25 13:00', 0),
('PN007', 'CH006', 'NV009', '2024-01-30 14:00', 0),
('PN008', 'CH007', 'NV010', '2024-02-01 15:00', 0),
('PN009', 'CH008', 'NV011', '2024-02-05 16:00', 0),
('PN010', 'CH009', 'NV012', '2024-02-10 08:30', 0),
('PN011', 'CH010', 'NV013', '2024-02-15 09:30', 0),
('PN012', 'CH011', 'NV014', '2024-02-20 10:30', 0),
('PN013', 'CH012', 'NV015', '2024-02-25 11:30', 0),
('PN014', 'CH013', 'NV016', '2024-03-01 12:30', 0),
('PN015', 'CH014', 'NV017', '2024-03-05 13:30', 0),
('PN016', 'CH015', 'NV018', '2024-03-10 14:30', 0),
('PN017', 'CH016', 'NV019', '2024-03-15 15:30', 0),
('PN018', 'CH017', 'NV020', '2024-03-20 16:30', 0),
('PN019', 'CH018', 'NV001', '2024-03-25 08:00', 0),
('PN020', 'CH019', 'NV003', '2024-03-30 09:00', 0);

-- 10. ChiTietNhapKho (20 rows) - Triggers will update PhieuNhapKho.TongTienNhap and TonKho
INSERT INTO ChiTietNhapKho (MaPhieuNhap, MaLo, SoLuong, GiaNhap) VALUES 
('PN001', 1, 100, 300000),
('PN001', 3, 50, 1800000),
('PN002', 4, 80, 450000),
('PN003', 5, 20, 2800000),
('PN004', 6, 150, 250000),
('PN005', 7, 30, 1000000),
('PN006', 8, 200, 200000),
('PN007', 9, 40, 500000),
('PN008', 10, 120, 300000),
('PN009', 11, 200, 150000),
('PN010', 12, 90, 250000),
('PN011', 13, 60, 220000),
('PN012', 14, 110, 320000),
('PN013', 15, 80, 380000),
('PN014', 16, 40, 1300000),
('PN015', 17, 300, 100000),
('PN016', 18, 150, 210000),
('PN017', 19, 100, 180000),
('PN018', 20, 50, 70000),
('PN019', 1, 60, 300000);

-- Note: TonKho is handled by trigger trg_NhapHang_TonKho

-- 11. KhuyenMai (20 rows)
INSERT INTO KhuyenMai (TenChuongTrinh, GiaTriGiam, LoaiGiam, ApDungTheo, NgayBatDau, NgayKetThuc) VALUES 
(N'Chào hè rực rỡ', 10, '%', 'HoaDon', '2024-05-01', '2024-06-30'),
(N'Lễ 30/4 rộn ràng', 50000, 'VND', 'HoaDon', '2024-04-25', '2024-05-05'),
(N'Sale son môi', 15, '%', 'DanhMuc', '2024-03-01', '2024-03-15'), -- MaDanhMuc=1
(N'Dưỡng da ngày mới', 20000, 'VND', 'SanPham', '2024-02-10', '2024-02-20'), -- MaSanPham='SP001'
(N'Black Friday', 30, '%', 'HoaDon', '2024-11-20', '2024-11-30'),
(N'Giáng sinh ấm áp', 20, '%', 'HoaDon', '2024-12-20', '2024-12-31'),
(N'Tết Nguyên Đán', 100000, 'VND', 'HoaDon', '2025-01-15', '2025-02-15'),
(N'Mừng 8/3', 8, '%', 'HoaDon', '2024-03-05', '2024-03-10'),
(N'Lễ tình nhân', 14, '%', 'HoaDon', '2024-02-10', '2024-02-15'),
(N'Thứ 6 may mắn', 5, '%', 'HoaDon', '2024-01-01', '2024-12-31'),
(N'Mùa cưới lung linh', 10, '%', 'DanhMuc', '2024-09-01', '2024-10-31'),
(N'Chào mừng thành viên', 20000, 'VND', 'HoaDon', '2024-01-01', '2024-12-31'),
(N'Cuối tuần vui vẻ', 10000, 'VND', 'HoaDon', '2024-01-01', '2024-12-31'),
(N'Sale thương hiệu Lancome', 12, '%', 'HoaDon', '2024-06-01', '2024-06-07'),
(N'Mùa thu lá bay', 15, '%', 'HoaDon', '2024-08-15', '2024-09-15'),
(N'Phụ nữ là để yêu', 10, '%', 'HoaDon', '2024-10-15', '2024-10-25'),
(N'Ngày của cha', 5, '%', 'HoaDon', '2024-06-15', '2024-06-25'),
(N'Ngày của mẹ', 10, '%', 'HoaDon', '2024-05-10', '2024-05-20'),
(N'Voucher 20k', 20000, 'VND', 'HoaDon', '2024-01-01', '2024-12-31'),
(N'Voucher 50k', 50000, 'VND', 'HoaDon', '2024-01-01', '2024-12-31');

-- Update KhuyenMai FKs
UPDATE KhuyenMai SET MaDanhMuc = 1 WHERE MaKhuyenMai = 3;
UPDATE KhuyenMai SET MaSanPham = 'SP001' WHERE MaKhuyenMai = 4;

-- 12. HoaDon (20 rows)
INSERT INTO HoaDon (MaHoaDon, MaCuaHang, MaNhanVien, MaKhachHang, MaKhuyenMai, NgayLap, PhuongThucThanhToan) VALUES 
('HD001', 'CH001', 'NV002', 1, 1, '2024-05-10 10:00', N'Tiền mặt'),
('HD002', 'CH001', 'NV002', 2, NULL, '2024-05-11 11:00', N'Chuyển khoản'),
('HD003', 'CH002', 'NV004', 3, 2, '2024-05-12 12:00', N'Thẻ'),
('HD004', 'CH002', 'NV004', 4, NULL, '2024-05-13 13:00', N'Tiền mặt'),
('HD005', 'CH003', 'NV005', 5, 10, '2024-05-14 14:00', N'Chuyển khoản'),
('HD006', 'CH004', 'NV007', 6, NULL, '2024-05-15 15:00', N'Thẻ'),
('HD007', 'CH005', 'NV008', 7, NULL, '2024-05-16 16:00', N'Tiền mặt'),
('HD008', 'CH006', 'NV009', 8, NULL, '2024-05-17 10:30', N'Chuyển khoản'),
('HD009', 'CH007', 'NV010', 9, NULL, '2024-05-18 11:30', N'Thẻ'),
('HD010', 'CH008', 'NV011', 10, NULL, '2024-05-19 12:30', N'Tiền mặt'),
('HD011', 'CH009', 'NV012', 11, NULL, '2024-05-20 13:30', N'Chuyển khoản'),
('HD012', 'CH010', 'NV013', 12, NULL, '2024-05-21 14:30', N'Thẻ'),
('HD013', 'CH011', 'NV014', 13, NULL, '2024-05-22 15:30', N'Tiền mặt'),
('HD014', 'CH012', 'NV015', 14, NULL, '2024-05-23 16:30', N'Chuyển khoản'),
('HD015', 'CH013', 'NV016', 15, NULL, '2024-05-24 10:00', N'Thẻ'),
('HD016', 'CH014', 'NV017', 16, NULL, '2024-05-25 11:00', N'Tiền mặt'),
('HD017', 'CH015', 'NV018', 17, NULL, '2024-05-26 12:00', N'Chuyển khoản'),
('HD018', 'CH016', 'NV019', 18, NULL, '2024-05-27 13:00', N'Thẻ'),
('HD019', 'CH017', 'NV020', 19, NULL, '2024-05-28 14:00', N'Tiền mặt'),
('HD020', 'CH018', 'NV001', 20, NULL, '2024-05-29 15:00', N'Chuyển khoản');

-- 13. ChiTietHoaDon (20 rows) - Triggers will update HoaDon.TongTien and KhachCanTra, and TonKho
-- Make sure MaLo exists in TonKho (inserted in step 10)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaLo, SoLuong, DonGia) VALUES 
('HD001', 1, 2, 450000),
('HD001', 3, 1, 2500000),
('HD002', 4, 1, 650000),
('HD003', 5, 1, 3800000),
('HD004', 6, 3, 380000),
('HD005', 7, 1, 1450000),
('HD006', 8, 2, 320000),
('HD007', 9, 1, 680000),
('HD008', 10, 2, 420000),
('HD009', 11, 4, 280000),
('HD010', 12, 1, 350000),
('HD011', 13, 2, 320000),
('HD012', 14, 1, 450000),
('HD013', 15, 1, 550000),
('HD014', 16, 1, 1850000),
('HD015', 17, 3, 160000),
('HD016', 18, 1, 320000),
('HD017', 19, 2, 290000),
('HD018', 20, 1, 350000),
('HD019', 1, 1, 450000);

-- 14. PhieuTraHang (20 rows)
INSERT INTO PhieuTraHang (MaPhieuTra, MaHoaDon, NgayTra, LyDo) VALUES 
('PT001', 'HD001', '2024-05-12', N'Sản phẩm bị móp méo'),
('PT002', 'HD003', '2024-05-15', N'Nhầm loại sản phẩm'),
('PT003', 'HD005', '2024-05-18', N'Khách đổi ý'),
('PT004', 'HD007', '2024-05-20', N'Lỗi vòi xịt'),
('PT005', 'HD009', '2024-05-22', N'Sản phẩm không như quảng cáo'),
('PT006', 'HD011', '2024-05-25', N'Hết hạn sử dụng (giả định)'),
('PT007', 'HD013', '2024-05-28', N'Dị ứng da'),
('PT008', 'HD015', '2024-06-01', N'Sản phẩm bị đổ vỡ'),
('PT009', 'HD017', '2024-06-05', N'Nhân viên tư vấn sai'),
('PT010', 'HD019', '2024-06-10', N'Phát hiện hàng giả (giả định)'),
('PT011', 'HD002', '2024-05-13', N'Đổi trả theo chính sách'),
('PT012', 'HD004', '2024-05-16', N'Lỗi đóng gói'),
('PT013', 'HD006', '2024-05-19', N'Mùi hương không hợp'),
('PT014', 'HD008', '2024-05-21', N'Bao bì bị rách'),
('PT015', 'HD010', '2024-05-23', N'Giao nhầm hàng'),
('PT016', 'HD012', '2024-05-26', N'Thiếu phụ kiện đi kèm'),
('PT017', 'HD014', '2024-05-29', N'Hộp bị mở trước'),
('PT018', 'HD016', '2024-06-02', N'Tem chống hàng giả bị rách'),
('PT019', 'HD018', '2024-06-06', N'Sản phẩm có dấu hiệu đã sử dụng'),
('PT020', 'HD020', '2024-06-11', N'Lỗi nhà sản xuất');

-- 15. ChiTietPhieuTraHang (20 rows) - Triggers will update TonKho
INSERT INTO ChiTietPhieuTraHang (MaPhieuTra, MaLo, SoLuongTra, SoTienHoan) VALUES 
('PT001', 1, 1, 405000),
('PT002', 5, 1, 3800000),
('PT003', 7, 1, 1305000),
('PT004', 9, 1, 680000),
('PT005', 11, 1, 280000),
('PT006', 13, 1, 320000),
('PT007', 15, 1, 550000),
('PT008', 17, 1, 160000),
('PT009', 19, 1, 290000),
('PT010', 1, 1, 450000),
('PT011', 4, 1, 650000),
('PT012', 6, 1, 380000),
('PT013', 8, 1, 320000),
('PT014', 10, 1, 420000),
('PT015', 12, 1, 350000),
('PT016', 14, 1, 450000),
('PT017', 16, 1, 1850000),
('PT018', 18, 1, 320000),
('PT019', 20, 1, 350000),
('PT020', 1, 1, 450000);

-- 16. KiemKe (20 rows)
INSERT INTO KiemKe (MaCuaHang, MaLo, SoLuongHeThong, SoLuongThucTe) VALUES 
('CH001', 1, 100, 98), ('CH001', 3, 50, 50), ('CH002', 5, 20, 19), ('CH003', 6, 150, 150),
('CH004', 7, 30, 30), ('CH005', 8, 200, 195), ('CH006', 9, 40, 40), ('CH007', 10, 120, 120),
('CH008', 11, 200, 200), ('CH009', 12, 90, 88), ('CH010', 13, 60, 60), ('CH011', 14, 110, 110),
('CH012', 15, 80, 79), ('CH013', 16, 40, 40), ('CH014', 17, 300, 300), ('CH015', 18, 150, 148),
('CH016', 19, 100, 100), ('CH017', 20, 50, 50), ('CH018', 1, 60, 59), ('CH019', 4, 0, 0);

-- 17. ChamCong (20 rows)
INSERT INTO ChamCong (MaNhanVien, NgayLam, GioVao, GioRa) VALUES 
('NV001', '2024-05-01', '08:00', '17:00'), ('NV002', '2024-05-01', '08:15', '17:15'),
('NV003', '2024-05-01', '07:50', '16:50'), ('NV004', '2024-05-01', '08:05', '17:05'),
('NV005', '2024-05-01', '08:00', '17:00'), ('NV006', '2024-05-01', '08:00', '17:00'),
('NV007', '2024-05-01', '08:30', '17:30'), ('NV008', '2024-05-01', '08:00', '17:00'),
('NV009', '2024-05-01', '08:00', '17:00'), ('NV010', '2024-05-01', '08:00', '17:00'),
('NV011', '2024-05-02', '08:00', '17:00'), ('NV012', '2024-05-02', '08:15', '17:15'),
('NV013', '2024-05-02', '07:50', '16:50'), ('NV014', '2024-05-02', '08:05', '17:05'),
('NV015', '2024-05-02', '08:00', '17:00'), ('NV016', '2024-05-02', '08:00', '17:00'),
('NV017', '2024-05-02', '08:30', '17:30'), ('NV018', '2024-05-02', '08:00', '17:00'),
('NV019', '2024-05-02', '08:00', '17:00'), ('NV020', '2024-05-02', '08:00', '17:00');

-- 18. HuySanPham (20 rows) - Triggers might be needed if not handled, but schema doesn't show trigger for HuySanPham yet.
-- I'll assume we manually update TonKho if needed, but per schema, it's just a log table.
INSERT INTO HuySanPham (MaCuaHang, MaNhanVien, MaLo, SoLuong, LyDo) VALUES 
('CH001', 'NV001', 1, 2, N'Sản phẩm hết hạn'),
('CH001', 'NV002', 3, 1, N'Sản phẩm bị hư hỏng'),
('CH002', 'NV003', 5, 3, N'Lỗi bao bì'),
('CH003', 'NV005', 6, 5, N'Hết hạn sử dụng'),
('CH004', 'NV007', 7, 2, N'Hỏng do vận chuyển'),
('CH005', 'NV008', 8, 10, N'Hết hạn sử dụng'),
('CH006', 'NV009', 9, 1, N'Sản phẩm bị biến chất'),
('CH007', 'NV010', 10, 4, N'Hết hạn'),
('CH008', 'NV011', 11, 6, N'Lỗi nhà sản xuất'),
('CH009', 'NV012', 12, 2, N'Hết hạn sử dụng'),
('CH010', 'NV013', 13, 3, N'Bị đổ vỡ'),
('CH011', 'NV014', 14, 5, N'Hết hạn'),
('CH012', 'NV015', 15, 1, N'Hỏng hóc'),
('CH013', 'NV016', 16, 2, N'Hết hạn sử dụng'),
('CH014', 'NV017', 17, 8, N'Hết hạn'),
('CH015', 'NV018', 18, 4, N'Sản phẩm bị ẩm mốc'),
('CH016', 'NV019', 19, 2, N'Hết hạn sử dụng'),
('CH017', 'NV020', 20, 1, N'Lỗi đóng gói'),
('CH018', 'NV001', 1, 3, N'Hết hạn'),
('CH019', 'NV003', 4, 1, N'Hỏng vòi xịt');

PRINT 'INSERT DATA COMPLETED!';
GO
