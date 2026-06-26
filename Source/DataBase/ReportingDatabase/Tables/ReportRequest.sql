CREATE TABLE [rpt].[ReportRequest]
(
    [RequestId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Customer database
    [TemplateId] UNIQUEIDENTIFIER NOT NULL,
    [ParametersJSON] NVARCHAR(MAX) NULL,
    [RequestedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [CompletionDate] DATETIME2(7) NULL,
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Processing, Completed, Failed
    [FileUrl] NVARCHAR(500) NULL,

    CONSTRAINT [PK_ReportRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC),
    CONSTRAINT [FK_ReportRequest_Template] FOREIGN KEY ([TemplateId]) REFERENCES [rpt].[ReportTemplate] ([TemplateId]) ON DELETE CASCADE
);
GO
