CREATE TABLE [acc].[Account]
(
    [AccountId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Customer profile / Identity User
    [AccountNumber] NVARCHAR(20) NOT NULL,
    [AccountType] VARCHAR(20) NOT NULL, -- Savings, Checking, Loan
    [Balance] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [Currency] VARCHAR(3) NOT NULL DEFAULT 'USD',
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Suspended, Dormant, Closed
    [InterestRate] DECIMAL(5, 4) NOT NULL DEFAULT 0.0000,
    [OpenedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [ClosedDate] DATETIME2(7) NULL,
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedDate] DATETIME2(7) NULL,
    [RowVersion] ROWVERSION NOT NULL,

    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([AccountId] ASC),
    CONSTRAINT [UQ_Account_AccountNumber] UNIQUE NONCLUSTERED ([AccountNumber] ASC),
    CONSTRAINT [CK_Account_Balance] CHECK ([Balance] >= 0.0000 OR [AccountType] = 'Loan')
);
GO
