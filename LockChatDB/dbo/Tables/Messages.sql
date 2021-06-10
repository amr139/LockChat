CREATE TABLE [dbo].[Messages] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [SenderId] INT NOT NULL, 
    [ReceiverId] INT NOT NULL,
    [EncryptedText] VARBINARY(MAX) NOT NULL,
    [Stamp] datetimeoffset NOT NULL
);

