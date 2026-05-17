USE LUnivers_Beaute;
GO

-- =========================================================================
-- KIỂM KÊ KHO - Lấy danh sách tồn kho theo cửa hàng để kiểm kê
-- =========================================================================
CREATE OR ALTER PROCEDURE sp_GetTonKhoForKiemKe
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
