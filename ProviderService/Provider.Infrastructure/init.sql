IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ProvidersDb')
BEGIN
	CREATE DATABASE ProvidersDb;
END
GO

USE ProvidersDb;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Providers]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[Providers]
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		[Name] NVARCHAR(200) NOT NULL,
		[Lat] FLOAT NULL,
		[Lon] FLOAT NULL,
		[Available] BIT NOT NULL,
		[Rating] FLOAT NULL
	);
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Providers])
BEGIN
	INSERT INTO [dbo].[Providers] (Id, Name, Lat, Lon, Available, Rating) VALUES
	('11111111-1111-1111-1111-111111111111', 'Proveedor A', -34.6, -58.4, 1, 4.7),
	('22222222-2222-2222-2222-222222222222', 'Proveedor B', -34.61, -58.41, 1, 4.2),
	('33333333-3333-3333-3333-333333333333', 'Proveedor C', -34.62, -58.42, 1, 3.9);
END
GO
