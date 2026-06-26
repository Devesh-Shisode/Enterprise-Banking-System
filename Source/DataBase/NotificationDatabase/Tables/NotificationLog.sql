CREATE TABLE [notif].[NotificationLog]
(
    [NotificationLogId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Customer database
    [EventCode] VARCHAR(50) NOT NULL,
    [Recipient] NVARCHAR(100) NOT NULL,
    [Subject] NVARCHAR(200) NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [Channel] VARCHAR(20) NOT NULL, -- Email, SMS, Push
    [SentStatus] VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Sent, Failed, Pending
    [ErrorMessage] NVARCHAR(500) NULL,
    [SentDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_NotificationLog] PRIMARY KEY CLUSTERED ([NotificationLogId] ASC)
);
GO
