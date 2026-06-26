CREATE TABLE [cust].[CustomerAddress]
(
    [AddressId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [AddressType] VARCHAR(20) NOT NULL, -- Permanent, Correspondence
    [AddressLine1] NVARCHAR(100) NOT NULL,
    [AddressLine2] NVARCHAR(100) NULL,
    [City] NVARCHAR(50) NOT NULL,
    [State] NVARCHAR(50) NOT NULL,
    [PostalCode] VARCHAR(15) NOT NULL,
    [Country] NVARCHAR(50) NOT NULL DEFAULT 'India',
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_CustomerAddress] PRIMARY KEY CLUSTERED ([AddressId] ASC),
    CONSTRAINT [FK_CustomerAddress_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [cust].[Customer] ([CustomerId]) ON DELETE CASCADE
);
GO
