CREATE PROCEDURE [auth].[usp_User_Login]
    @UserNameOrEmail NVARCHAR(256),
    @PasswordHash NVARCHAR(500),
    @IsSuccess BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @DbPasswordHash NVARCHAR(500);
    DECLARE @IsActive BIT;
    DECLARE @LockoutEnd DATETIME2(7);

    SET @IsSuccess = 0;

    SELECT @UserId = [UserId], @DbPasswordHash = [PasswordHash], @IsActive = [IsActive], @LockoutEnd = [LockoutEnd]
    FROM [auth].[User]
    WHERE ([UserName] = @UserNameOrEmail OR [Email] = @UserNameOrEmail) AND [IsDeleted] = 0;

    IF @UserId IS NULL
        RETURN;

    IF @IsActive = 0 OR (@LockoutEnd IS NOT NULL AND @LockoutEnd > SYSUTCDATETIME())
        RETURN;

    IF @DbPasswordHash = @PasswordHash
    BEGIN
        SET @IsSuccess = 1;
        UPDATE [auth].[User]
        SET [FailedLoginAttempts] = 0,
            [LastLoginDate] = SYSUTCDATETIME(),
            [LockoutEnd] = NULL
        WHERE [UserId] = @UserId;
    END
    ELSE
    BEGIN
        UPDATE [auth].[User]
        SET [FailedLoginAttempts] = [FailedLoginAttempts] + 1,
            [LockoutEnd] = CASE WHEN [FailedLoginAttempts] + 1 >= 5 THEN DATEADD(minute, 15, SYSUTCDATETIME()) ELSE NULL END
        WHERE [UserId] = @UserId;
    END
END
GO
