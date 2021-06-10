CREATE TABLE [dbo].[UserKeys] (
    [UserId]   INT NOT NULL,
    [PublicKey] VARBINARY(MAX) NOT NULL,
    [PrivateKey] VARBINARY(MAX) NOT NULL,
    [UnsignedPrePublicKey] VARBINARY(MAX) NOT NULL,
    [UnsignedPrePrivateKey] VARBINARY(MAX) NOT NULL,
    [SignedPrePublicKey] VARBINARY(MAX) NOT NULL,
    [SignedPrePrivateKey] VARBINARY(MAX) NOT NULL,
    [Signature] VARBINARY(MAX) NOT NULL,
    [UnsignedPreKeyId] INT NOT NULL,
    [SignedPreKeyId] INT NOT NULL

);

