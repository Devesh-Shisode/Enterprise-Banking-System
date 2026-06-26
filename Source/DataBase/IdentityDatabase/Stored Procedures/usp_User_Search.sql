CREATE PROCEDURE [auth].[usp_User_Search]
    @SearchTerm NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [UserId], [UserName], [Email], [PhoneNumber], [IsActive], [CreatedDate]
    FROM [auth].[User]
    WHERE ([UserName] LIKE '%' + @SearchTerm + '%' OR [Email] LIKE '%' + @SearchTerm + '%' OR @SearchTerm IS NULL)
      AND [IsDeleted] = 0
    ORDER BY [UserName]
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
