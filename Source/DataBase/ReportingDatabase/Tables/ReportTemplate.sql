CREATE TABLE [rpt].[ReportTemplate]
(
    [TemplateId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ReportName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(250) NULL,
    [Category] VARCHAR(50) NOT NULL, -- Transaction, Audit, Statement
    [QueryDefinition] NVARCHAR(MAX) NOT NULL,

    CONSTRAINT [PK_ReportTemplate] PRIMARY KEY CLUSTERED ([TemplateId] ASC),
    CONSTRAINT [UQ_ReportTemplate_ReportName] UNIQUE NONCLUSTERED ([ReportName] ASC)
);
GO
