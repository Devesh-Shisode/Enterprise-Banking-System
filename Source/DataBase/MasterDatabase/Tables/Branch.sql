CREATE TABLE [mst].[Branch]
(
    [BranchId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [BranchCode] VARCHAR(20) NOT NULL, -- IFSC Code / Branch Routing Number
    [BranchName] NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(200) NOT NULL,
    [City] NVARCHAR(50) NOT NULL,
    [State] NVARCHAR(50) NOT NULL,
    [ContactNumber] VARCHAR(20) NULL,

    CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED ([BranchId] ASC),
    CONSTRAINT [UQ_Branch_BranchCode] UNIQUE NONCLUSTERED ([BranchCode] ASC)
);
GO
