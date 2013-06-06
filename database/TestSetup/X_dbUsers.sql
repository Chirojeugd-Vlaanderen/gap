-- rechten op de dev-db via SQL authenticatie
-- (omdat de developers typisch niet aan het werken zijn met hun kipdorplogin)
USE Master
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'CgDevelop')
	CREATE LOGIN CgDevelop WITH PASSWORD=N'Chirogroep!', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'CgApp')
	CREATE LOGIN CgApp WITH PASSWORD=N'doemaariets', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO


USE [gap_dev]
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CgApp')
	DROP USER [CgApp];

CREATE USER [CgApp] FOR LOGIN [CgApp];

EXEC sp_addrolemember N'GapRole', N'CgApp'
GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CgDevelop')
	DROP USER CgDevelop;
	
CREATE USER [CgDevelop] FOR LOGIN [CgDevelop];

EXEC sp_addrolemember N'db_owner', N'CgDevelop';
GO

CREATE USER [gapsuper] FOR LOGIN [gapsuper]
GO


EXEC sp_addrolemember N'GapSuperRole', N'gapsuper'
GO

GRANT SELECT ON diag.vVerlorenBivakken TO GapSuperRole;
