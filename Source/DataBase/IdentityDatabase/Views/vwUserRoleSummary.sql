CREATE VIEW [auth].[vwUserRoleSummary] AS
SELECT u.[UserId], u.[UserName], r.[RoleId], r.[RoleName]
FROM [auth].[User] u
INNER JOIN [auth].[UserRole] ur ON u.[UserId] = ur.[UserId]
INNER JOIN [auth].[Role] r ON ur.[RoleId] = r.[RoleId];
GO
