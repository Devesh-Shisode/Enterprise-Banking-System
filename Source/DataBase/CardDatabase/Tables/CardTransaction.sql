CREATE TABLE [card].[CardTransaction]
(
    [CardTxnId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CardId] UNIQUEIDENTIFIER NOT NULL,
    [MerchantName] NVARCHAR(100) NOT NULL,
    [MerchantCategoryCode] VARCHAR(10) NULL,
    [Amount] DECIMAL(18, 4) NOT NULL,
    [Currency] VARCHAR(3) NOT NULL DEFAULT 'USD',
    [TransactionDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Approved', -- Approved, Declined, Reversed

    CONSTRAINT [PK_CardTransaction] PRIMARY KEY CLUSTERED ([CardTxnId] ASC),
    CONSTRAINT [FK_CardTransaction_Card] FOREIGN KEY ([CardId]) REFERENCES [card].[Card] ([CardId]) ON DELETE CASCADE
);
GO
