USE LUnivers_Beaute;
GO

-- =========================================================================
-- CHẤM CÔNG - CRUD Stored Procedures
-- =========================================================================

-- 1. GET ALL
CREATE OR ALTER PROCEDURE sp_GetAllChamCong
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

-- 2. INSERT
CREATE OR ALTER PROCEDURE sp_InsertChamCong
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

-- 3. UPDATE
CREATE OR ALTER PROCEDURE sp_UpdateChamCong
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

-- 4. DELETE
CREATE OR ALTER PROCEDURE sp_DeleteChamCong
    @MaCC INT
AS
BEGIN
    DELETE FROM ChamCong WHERE MaCC = @MaCC;
END;
GO
