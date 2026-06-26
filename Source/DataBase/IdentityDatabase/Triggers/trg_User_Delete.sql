CREATE TRIGGER [auth].[trg_User_Delete]
ON [auth].[User]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    -- Soft-delete logic instead of hard DELETE
    UPDATE u
    SET u.[IsDeleted] = 1,
        u.[IsActive] = 0,
        u.[UpdatedDate] = SYSUTCDATETIME()
    FROM [auth].[User] u
    INNER JOIN deleted d ON u.[UserId] = d.[UserId];
END
GO
