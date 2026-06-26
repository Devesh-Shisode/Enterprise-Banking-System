CREATE TABLE [cust].[Customer]
(
    [CustomerId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK to auth.User
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [DateOfBirth] DATE NOT NULL,
    [Gender] VARCHAR(20) NULL,
    [PAN] VARCHAR(10) NOT NULL,
    [AadhaarNumber] VARCHAR(12) NOT NULL,
    [KYCStatus] VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Verified, Rejected
    [KYCDate] DATETIME2(7) NULL,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedDate] DATETIME2(7) NULL,

    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerId] ASC),
    CONSTRAINT [UQ_Customer_PAN] UNIQUE NONCLUSTERED ([PAN] ASC),
    CONSTRAINT [UQ_Customer_AadhaarNumber] UNIQUE NONCLUSTERED ([AadhaarNumber] ASC)
);
GO
