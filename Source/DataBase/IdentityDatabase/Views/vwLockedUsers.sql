CREATE VIEW [auth].[vwLockedUsers] AS
SELECT [UserId], [UserName], [Email], [FailedLoginAttempts], [LockoutEnd]
FROM [auth].[User]
WHERE [LockoutEnd] IS NOT NULL AND [LockoutEnd] > SYSUTCDATETIME();
GO
