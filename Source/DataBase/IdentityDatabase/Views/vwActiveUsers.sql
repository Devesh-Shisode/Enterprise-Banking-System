CREATE VIEW [auth].[vwActiveUsers] AS
SELECT [UserId], [UserName], [Email], [PhoneNumber], [LastLoginDate]
FROM [auth].[User]
WHERE [IsActive] = 1 AND [IsDeleted] = 0;
GO
