CREATE TABLE [txn].[TransferDetail]
(
    [TransferId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TransactionId] UNIQUEIDENTIFIER NOT NULL,
    [SourceAccountId] UNIQUEIDENTIFIER NOT NULL,
    [DestinationAccountId] UNIQUEIDENTIFIER NOT NULL,
    [DestinationBankCode] VARCHAR(20) NULL, -- IFSC / SWIFT / BIC
    [Channel] VARCHAR(20) NOT NULL, -- InternetBanking, MobileApp, ATM, Branch

    CONSTRAINT [PK_TransferDetail] PRIMARY KEY CLUSTERED ([TransferId] ASC),
    CONSTRAINT [FK_TransferDetail_Transaction] FOREIGN KEY ([TransactionId]) REFERENCES [txn].[Transaction] ([TransactionId]) ON DELETE CASCADE
);
GO
