-- rechten op de dev-db via SQL authenticatie
-- (omdat de developers typisch niet aan het werken zijn met hun kipdorplogin)

-- !! Kijk alle lijnen die met USE beginnen goed na, om te zien of je wel
-- op de juiste database werkt!

USE Master
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'CgDevelop')
	CREATE LOGIN CgDevelop WITH PASSWORD=N'Chirogroep!', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'CgApp')
	CREATE LOGIN CgApp WITH PASSWORD=N'doemaariets', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO


USE [gap_local]
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


USE [gap_local]

GRANT SELECT ON adr.Adres TO GapSuperRole
GRANT SELECT ON adr.BelgischAdres TO GapSuperRole
GRANT SELECT ON adr.BuitenlandsAdres TO GapSuperRole
GRANT SELECT ON biv.Plaats TO GapSuperRole
GRANT SELECT ON biv.Deelnemer TO GapSuperRole
GRANT SELECT ON grp.GroepsWerkJaar TO GapSuperRole
GRANT SELECT ON biv.Uitstap TO GapSuperRole
GRANT SELECT ON adr.StraatNaam TO GapSuperRole
GRANT SELECT ON adr.WoonPlaats TO GapSuperRole
GRANT SELECT ON grp.Groep TO GapSuperRole
GRANT SELECT ON grp.KaderGroep TO GapSuperRole
GRANT SELECT ON grp.ChiroGroep TO GapSuperRole
GRANT SELECT ON pers.Persoon TO GapSuperRole
GRANT SELECT ON pers.GelieerdePersoon TO GapSuperRole
GRANT SELECT ON pers.CommunicatieType TO GapSuperRole
GRANT SELECT ON pers.CommunicatieVorm TO GapSuperRole

