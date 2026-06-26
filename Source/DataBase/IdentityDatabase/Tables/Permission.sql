CREATE TABLE [auth].[Permission]
(
    [PermissionId] INT IDENTITY(1,1) NOT NULL,
    [PermissionName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(250) NULL,

    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([PermissionId] ASC),
    CONSTRAINT [UQ_Permission_PermissionName] UNIQUE NONCLUSTERED ([PermissionName] ASC)
);
GO
