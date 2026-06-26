CREATE TABLE [notif].[NotificationPreference]
(
    [PreferenceId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Customer database
    [EmailEnabled] BIT NOT NULL DEFAULT 1,
    [SMSEnabled] BIT NOT NULL DEFAULT 1,
    [PushEnabled] BIT NOT NULL DEFAULT 1,
    [MarketingConsent] BIT NOT NULL DEFAULT 0,

    CONSTRAINT [PK_NotificationPreference] PRIMARY KEY CLUSTERED ([PreferenceId] ASC),
    CONSTRAINT [UQ_NotificationPreference_CustomerId] UNIQUE NONCLUSTERED ([CustomerId] ASC)
);
GO
