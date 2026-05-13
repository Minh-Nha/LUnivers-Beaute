USE LUnivers_Beaute;
GO

-- =========================================================================
-- DASHBOARD: Doanh thu theo ngày
-- =========================================================================
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
