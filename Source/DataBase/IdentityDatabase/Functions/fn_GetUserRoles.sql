CREATE FUNCTION [auth].[fn_GetUserRoles]
(
    @UserId UNIQUEIDENTIFIER
)
RETURNS TABLE
AS
RETURN
(
    SELECT r.[RoleId], r.[RoleName]
    FROM [auth].[Role] r
    INNER JOIN [auth].[UserRole] ur ON r.[RoleId] = ur.[RoleId]
    WHERE ur.[UserId] = @UserId
);
GO
