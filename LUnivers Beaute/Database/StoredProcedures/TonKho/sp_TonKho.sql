USE LUnivers_Beaute;
GO

CREATE OR ALTER PROCEDURE sp_GetTonKho
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