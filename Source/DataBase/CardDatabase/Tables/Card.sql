CREATE TABLE [card].[Card]
(
    [CardId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AccountId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Account database
    [CustomerId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Customer database
    [CardNumber] NVARCHAR(19) NOT NULL,
    [CardType] VARCHAR(20) NOT NULL, -- Debit, Credit
    [Provider] VARCHAR(20) NOT NULL, -- Visa, MasterCard, RuPay
    [ExpiryDate] DATE NOT NULL,
    [CVVHash] NVARCHAR(200) NOT NULL,
    [PinHash] NVARCHAR(200) NOT NULL,
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Blocked, Suspended, Expired
    [CreditLimit] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [CashLimit] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [CurrentBalance] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_Card] PRIMARY KEY CLUSTERED ([CardId] ASC),
    CONSTRAINT [UQ_Card_CardNumber] UNIQUE NONCLUSTERED ([CardNumber] ASC)
);
GO
