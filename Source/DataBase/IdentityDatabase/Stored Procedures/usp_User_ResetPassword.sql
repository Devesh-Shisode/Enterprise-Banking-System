CREATE PROCEDURE [auth].[usp_User_ResetPassword]
    @UserId UNIQUEIDENTIFIER,
    @NewPasswordHash NVARCHAR(500),
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [auth].[User]
    SET [PasswordHash] = @NewPasswordHash,
        [FailedLoginAttempts] = 0,
        [LockoutEnd] = NULL,
        [UpdatedBy] = @UpdatedBy,
        [UpdatedDate] = SYSUTCDATETIME()
    WHERE [UserId] = @UserId AND [IsDeleted] = 0;
END
GO
