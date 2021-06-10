CREATE TABLE [dbo].[Users] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (100) NULL, 
    [LastName] NVARCHAR(100) NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [PasswordHash] VARBINARY(MAX) NULL,
    [IsAdmin] BIT NOT NULL DEFAULT 0
);

