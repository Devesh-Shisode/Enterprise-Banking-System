CREATE TABLE [loan].[Loan]
(
    [LoanId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CustomerId] UNIQUEIDENTIFIER NOT NULL, -- Logical FK pointing to Customer database
    [LoanNumber] NVARCHAR(20) NOT NULL,
    [LoanType] VARCHAR(20) NOT NULL, -- Personal, Home, Auto, Education
    [PrincipalAmount] DECIMAL(18, 4) NOT NULL,
    [OutstandingBalance] DECIMAL(18, 4) NOT NULL,
    [InterestRate] DECIMAL(5, 4) NOT NULL DEFAULT 0.0000,
    [TenureMonths] INT NOT NULL,
    [EMI] DECIMAL(18, 4) NOT NULL,
    [DisbursementDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Closed, Defaulted
    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_Loan] PRIMARY KEY CLUSTERED ([LoanId] ASC),
    CONSTRAINT [UQ_Loan_LoanNumber] UNIQUE NONCLUSTERED ([LoanNumber] ASC)
);
GO
