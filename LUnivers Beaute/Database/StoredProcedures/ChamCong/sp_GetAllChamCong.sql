USE LUnivers_Beaute;
GO

-- =========================================================================
-- CHẤM CÔNG - Hiển thị chấm công theo nhân viên & khoảng ngày
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
