CREATE PROCEDURE [auth].[usp_User_Create]
    @UserName NVARCHAR(50),
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(500),
    @PhoneNumber NVARCHAR(20) = NULL,
    @StatusId INT = 1,
    @CreatedBy UNIQUEIDENTIFIER = NULL,
    @NewUserId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @NewUserId = NEWID();

    INSERT INTO [auth].[User]
        ([UserId], [UserName], [Email], [PasswordHash], [PhoneNumber], [StatusId], [CreatedBy])
    VALUES
        (@NewUserId, @UserName, @Email, @PasswordHash, @PhoneNumber, @StatusId, @CreatedBy);
END
GO
