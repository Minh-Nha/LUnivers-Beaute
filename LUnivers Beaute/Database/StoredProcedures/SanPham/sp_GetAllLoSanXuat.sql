USE LUnivers_Beaute;
GO

-- =========================================================================
-- LÔ SẢN XUẤT - Hiển thị tất cả lô sản xuất
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
