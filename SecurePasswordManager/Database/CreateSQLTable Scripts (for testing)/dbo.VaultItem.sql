CREATE TABLE [dbo].[VaultItem] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Name]     VARCHAR (100) NOT NULL,
    [UserName] VARCHAR (100) NOT NULL,
    [Password] VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

