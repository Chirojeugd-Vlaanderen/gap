-- STAP 4: migreer database van live naar dev

-- Hier moeten alle aanpassingen komen aan de database die nodig zijn om de live-db
-- om te zetten naar de db die gebruikt wordt in dev.

USE [master]
GO

ALTER DATABASE [Gap_Dev] SET COMPATIBILITY_LEVEL = 100
GO

ALTER DATABASE [Gap_Dev] SET RECOVERY SIMPLE WITH NO_WAIT
GO

USE kipadmin_tst;

GRANT SELECT ON kipWoont TO GapSuperRole
GO
GRANT SELECT ON kipPersoon TO GapSuperRole
GO

USE gap_tst
GRANT SELECT ON pers.Persoon TO GapSuperRole
GO
GRANT SELECT ON adr.Adres TO GapSuperRole
GO
GRANT SELECT ON adr.BelgischAdres TO GapSuperRole
GO
GRANT SELECT ON adr.BuitenlandsAdres TO GapSuperRole
GO
GRANT SELECT ON pers.PersoonsAdres TO GapSuperRole
GO
GRANT SELECT ON pers.GelieerdePersoon TO GapSuperRole
GO
GRANT SELECT ON adr.StraatNaam TO GapSuperRole
GO
GRANT SELECT ON adr.WoonPlaats TO GapSuperRole
GO

