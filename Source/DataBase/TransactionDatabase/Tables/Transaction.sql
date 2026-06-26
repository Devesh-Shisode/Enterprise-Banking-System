CREATE TABLE [txn].[Transaction]
(
    [TransactionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AccountId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Account
    [AccountNumber] NVARCHAR(20) NOT NULL,
    [TransactionType] VARCHAR(20) NOT NULL, -- Deposit, Withdrawal, TransferIn, TransferOut
    [Amount] DECIMAL(18, 4) NOT NULL,
    [Currency] VARCHAR(3) NOT NULL DEFAULT 'USD',
    [TransactionDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [Description] NVARCHAR(250) NULL,
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Completed, Failed
    [ReferenceNumber] NVARCHAR(50) NOT NULL,
    [RelatedTransactionId] UNIQUEIDENTIFIER NULL,
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED ([TransactionId] ASC),
    CONSTRAINT [UQ_Transaction_ReferenceNumber] UNIQUE NONCLUSTERED ([ReferenceNumber] ASC),
    CONSTRAINT [CK_Transaction_Amount] CHECK ([Amount] > 0.0000)
);
GO
