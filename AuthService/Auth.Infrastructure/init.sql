IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'AuthDb')
BEGIN
	CREATE DATABASE AuthDb;
END
GO

USE AuthDb;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuthUsers]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[AuthUsers]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		[Email] NVARCHAR(256) NOT NULL,
		[FirstName] NVARCHAR(100) NULL,
		[LastName] NVARCHAR(100) NULL,
		[IsActive] BIT NOT NULL DEFAULT(1),
		[CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
	);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserCredentials]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[UserCredentials]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		[UserId] UNIQUEIDENTIFIER NOT NULL,
		[PasswordHash] NVARCHAR(512) NOT NULL,
		[PasswordSalt] NVARCHAR(256) NULL,
		[LastPasswordChange] DATETIME2 NULL,
		CONSTRAINT FK_UserCredentials_User FOREIGN KEY ([UserId]) REFERENCES [dbo].[AuthUsers]([Id])
	);
END
GO

-- Seed: usuario administrador de ejemplo (password debe ser reemplazado en entorno real)
IF NOT EXISTS (SELECT * FROM [dbo].[AuthUsers] WHERE Email = 'admin@asisya.local')
BEGIN
	INSERT INTO [dbo].[AuthUsers] (Id, Email, FirstName, LastName, IsActive)
	VALUES
	('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'admin@asisya.local', 'Admin', 'AsisYa', 1);

	INSERT INTO [dbo].[UserCredentials] (Id, UserId, PasswordHash, PasswordSalt, LastPasswordChange)
	VALUES
	('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'REPLACE_WITH_HASHED_PASSWORD', NULL, SYSUTCDATETIME());
END
GO
