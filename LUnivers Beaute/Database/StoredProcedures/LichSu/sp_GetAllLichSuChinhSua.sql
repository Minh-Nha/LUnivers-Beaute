CREATE OR ALTER PROCEDURE [dbo].[sp_GetAllLichSuChinhSua]
AS
BEGIN
    SELECT Id, Timestamp, UserName, Action, Detail, Icon
    FROM LichSuChinhSua
    ORDER BY Timestamp DESC;
END;
GO
