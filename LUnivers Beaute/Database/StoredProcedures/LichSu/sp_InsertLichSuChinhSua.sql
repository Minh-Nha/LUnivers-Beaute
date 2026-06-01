CREATE OR ALTER PROCEDURE [dbo].[sp_InsertLichSuChinhSua]
    @UserName NVARCHAR(100),
    @Action NVARCHAR(100),
    @Detail NVARCHAR(MAX),
    @Icon NVARCHAR(50)
AS
BEGIN
    INSERT INTO LichSuChinhSua (UserName, Action, Detail, Icon)
    VALUES (@UserName, @Action, @Detail, @Icon);
END;
GO
