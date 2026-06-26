CREATE TABLE [audit].[AuditLog]
(
    [AuditLogId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApplicationName] VARCHAR(100) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, -- Logical FK pointing to Identity User
    [IpAddress] VARCHAR(45) NULL,
    [ActionName] VARCHAR(100) NOT NULL,
    [EntityName] VARCHAR(100) NULL,
    [EntityId] VARCHAR(50) NULL,
    [OldValueJSON] NVARCHAR(MAX) NULL,
    [NewValueJSON] NVARCHAR(MAX) NULL,
    [Timestamp] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [Severity] VARCHAR(20) NOT NULL DEFAULT 'Info', -- Info, Warning, Critical

    CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([AuditLogId] ASC)
);
GO
