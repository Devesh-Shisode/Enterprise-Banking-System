CREATE FUNCTION [auth].[fn_GetPasswordExpiry]
(
    @UserId UNIQUEIDENTIFIER,
    @ExpiryDays INT = 90
)
RETURNS DATETIME2(7)
AS
BEGIN
    DECLARE @CreatedDate DATETIME2(7);
    DECLARE @UpdatedDate DATETIME2(7);
    DECLARE @ExpiryDate DATETIME2(7);

    SELECT @CreatedDate = [CreatedDate], @UpdatedDate = [UpdatedDate]
    FROM [auth].[User]
    WHERE [UserId] = @UserId;

    SET @ExpiryDate = DATEADD(day, @ExpiryDays, ISNULL(@UpdatedDate, @CreatedDate));
    RETURN @ExpiryDate;
END
GO
