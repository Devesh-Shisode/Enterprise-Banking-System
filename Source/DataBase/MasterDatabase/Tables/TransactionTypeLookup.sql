CREATE TABLE [mst].[TransactionTypeLookup]
(
    [TypeCode] VARCHAR(20) NOT NULL, -- Deposit, Withdrawal, TransferIn, TransferOut
    [TypeName] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(250) NULL,

    CONSTRAINT [PK_TransactionTypeLookup] PRIMARY KEY CLUSTERED ([TypeCode] ASC)
);
GO
