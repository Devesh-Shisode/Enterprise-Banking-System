CREATE TABLE [loan].[LoanRepayment]
(
    [RepaymentId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [LoanId] UNIQUEIDENTIFIER NOT NULL,
    [ScheduledDate] DATE NOT NULL,
    [ActualPaymentDate] DATETIME2(7) NULL,
    [AmountPaid] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [PrincipalComponent] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [InterestComponent] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [PenaltyCharges] DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    [Status] VARCHAR(20) NOT NULL DEFAULT 'Unpaid', -- Paid, Overdue, Unpaid

    CONSTRAINT [PK_LoanRepayment] PRIMARY KEY CLUSTERED ([RepaymentId] ASC),
    CONSTRAINT [FK_LoanRepayment_Loan] FOREIGN KEY ([LoanId]) REFERENCES [loan].[Loan] ([LoanId]) ON DELETE CASCADE
);
GO
