CREATE TABLE [dbo].[MasterPassword] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [masterPassword] VARCHAR (100) NOT NULL,
    [UserNAME]       VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

