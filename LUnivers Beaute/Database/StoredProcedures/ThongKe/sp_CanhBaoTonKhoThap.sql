USE LUnivers_Beaute;
GO

-- =========================================================================
-- DASHBOARD: Cảnh báo tồn kho thấp
-- =========================================================================
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
