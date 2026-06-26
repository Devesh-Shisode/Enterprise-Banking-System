CREATE TABLE [notif].[NotificationTemplate]
(
    [TemplateId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EventCode] VARCHAR(50) NOT NULL,
    [SubjectTemplate] NVARCHAR(200) NOT NULL,
    [BodyTemplate] NVARCHAR(MAX) NOT NULL,
    [Channel] VARCHAR(20) NOT NULL, -- Email, SMS, Push

    CONSTRAINT [PK_NotificationTemplate] PRIMARY KEY CLUSTERED ([TemplateId] ASC),
    CONSTRAINT [UQ_NotificationTemplate_EventCode_Channel] UNIQUE NONCLUSTERED ([EventCode] ASC, [Channel] ASC)
);
GO
