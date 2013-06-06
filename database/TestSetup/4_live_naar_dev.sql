-- STAP 4: migreer database van live naar dev

-- Hier moeten alle aanpassingen komen aan de database die nodig zijn om de live-db
-- om te zetten naar de db die gebruikt wordt in dev.

USE [master]
GO

ALTER DATABASE [Gap_Dev] SET COMPATIBILITY_LEVEL = 100
GO

ALTER DATABASE [Gap_Dev] SET RECOVERY SIMPLE WITH NO_WAIT
GO

USE gap_dev
GO

ALTER TABLE grp.Groep ADD StopDatum DateTime NULL;

USE [kipadmin_dev]

GRANT SELECT ON biv.BivakOverzicht TO GapSuperRole
GRANT SELECT ON grp.ChiroGroep TO GapSuperRole
GRANT SELECT ON dbo.HuidigWerkJaar TO GapSuperRole