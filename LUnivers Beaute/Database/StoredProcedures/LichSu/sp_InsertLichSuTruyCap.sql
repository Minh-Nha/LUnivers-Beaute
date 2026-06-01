CREATE OR ALTER PROCEDURE [dbo].[sp_InsertLichSuTruyCap]
    @UserName NVARCHAR(100),
    @IpAddress VARCHAR(50),
    @DeviceName NVARCHAR(100),
    @Location NVARCHAR(250)
AS
BEGIN
    INSERT INTO LichSuTruyCap (UserName, IpAddress, DeviceName, Location)
    VALUES (@UserName, @IpAddress, @DeviceName, @Location);
END;
GO
