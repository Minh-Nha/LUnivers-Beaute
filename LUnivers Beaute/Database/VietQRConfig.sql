IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VietQRConfig' and xtype='U')
BEGIN
    CREATE TABLE VietQRConfig (
        Id INT PRIMARY KEY DEFAULT 1,
        BankCode VARCHAR(50),
        AccountNumber VARCHAR(50),
        AccountName NVARCHAR(255)
    );
    INSERT INTO VietQRConfig (Id, BankCode, AccountNumber, AccountName) VALUES (1, '', '', '');
END
GO

CREATE OR ALTER PROCEDURE sp_GetVietQRConfig
AS
BEGIN
    SELECT TOP 1 BankCode, AccountNumber, AccountName FROM VietQRConfig;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateVietQRConfig
    @BankCode VARCHAR(50),
    @AccountNumber VARCHAR(50),
    @AccountName NVARCHAR(255)
AS
BEGIN
    UPDATE VietQRConfig
    SET BankCode = @BankCode,
        AccountNumber = @AccountNumber,
        AccountName = @AccountName
    WHERE Id = 1;
END
GO
