use LUnivers_Beaute
go

CREATE OR ALTER PROCEDURE sp_GetSanPhamBanHang
    @MaCuaHang VARCHAR(20) = NULL,
    @SearchTerm NVARCHAR(100) = NULL,
    @MaDanhMuc INT = NULL
AS
BEGIN
    SELECT 
        sp.MaSanPham,
        sp.TenSanPham,
        th.TenThuongHieu,
        sp.GiaNiemYet,
        ISNULL(SUM(tk.SoLuongTon), 0) AS SoLuongTon
    FROM SanPham sp
    LEFT JOIN ThuongHieu th ON sp.MaThuongHieu = th.MaThuongHieu
    LEFT JOIN LoSanXuat lsx ON sp.MaSanPham = lsx.MaSanPham 
                             AND lsx.HanSuDung >= CAST(GETDATE() AS DATE)
    LEFT JOIN TonKho tk ON lsx.MaLo = tk.MaLo AND (@MaCuaHang IS NULL OR tk.MaCuaHang = @MaCuaHang)
    WHERE sp.TrangThai = 1
      AND (@SearchTerm IS NULL OR sp.TenSanPham LIKE N'%' + @SearchTerm + N'%' OR sp.MaSanPham LIKE N'%' + @SearchTerm + N'%')
      AND (@MaDanhMuc IS NULL OR @MaDanhMuc = 0 OR sp.MaDanhMuc = @MaDanhMuc)
    GROUP BY sp.MaSanPham, sp.TenSanPham, th.TenThuongHieu, sp.GiaNiemYet
    HAVING ISNULL(SUM(tk.SoLuongTon), 0) > 0
    ORDER BY sp.TenSanPham;
END;
GO
