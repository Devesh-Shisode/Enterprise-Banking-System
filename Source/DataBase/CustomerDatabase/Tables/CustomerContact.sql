CREATE TABLE [cust].[CustomerContact]
(
    [ContactId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [ContactType] VARCHAR(20) NOT NULL, -- Email, Mobile, HomePhone, WorkPhone
    [ContactValue] NVARCHAR(100) NOT NULL,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_CustomerContact] PRIMARY KEY CLUSTERED ([ContactId] ASC),
    CONSTRAINT [FK_CustomerContact_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [cust].[Customer] ([CustomerId]) ON DELETE CASCADE
);
GO
