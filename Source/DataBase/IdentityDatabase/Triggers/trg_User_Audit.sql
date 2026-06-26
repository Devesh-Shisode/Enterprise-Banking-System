CREATE TRIGGER [auth].[trg_User_Audit]
ON [auth].[User]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    -- In a real scenario, this logs to a central audit trail table.
    -- SSDT project trigger validation shell.
    PRINT 'User Audit Trigger Executed';
END
GO
