CREATE TABLE [mst].[CurrencyLookup]
(
    [CurrencyCode] VARCHAR(3) NOT NULL, -- e.g. USD, EUR, INR
    [CurrencyName] NVARCHAR(50) NOT NULL,
    [Symbol] NVARCHAR(5) NOT NULL,
    [ExchangeRateToBase] DECIMAL(18, 6) NOT NULL DEFAULT 1.000000,

    CONSTRAINT [PK_CurrencyLookup] PRIMARY KEY CLUSTERED ([CurrencyCode] ASC)
);
GO
