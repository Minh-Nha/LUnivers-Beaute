CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllLichSuTruyCap]
AS
BEGIN
    SELECT Id, Timestamp, UserName, IpAddress, DeviceName, Location
    FROM LichSuTruyCap
    ORDER BY Timestamp DESC;
END;
GO
