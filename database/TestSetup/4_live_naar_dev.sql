-- STAP 4: migreer database van live naar dev

-- Hier moeten alle aanpassingen komen aan de database die nodig zijn om de live-db
-- om te zetten naar de db die gebruikt wordt in dev.

USE [master]
GO


-- users:

-- eerst service-user uit DB deleten, dan terug koppelen
-- aan user op deze server.

USE [gap_tst]
GO


CREATE USER [KIPDORP\gapservice_tst] FOR LOGIN [KIPDORP\gapservice_tst]
GO
EXEC sp_addrolemember N'db_datareader', N'KIPDORP\gapservice_tst'
GO
EXEC sp_addrolemember N'GapRole', N'KIPDORP\gapservice_tst'
GO


CREATE PROCEDURE auth.spWillekeurigeGroepToekennen (@login varchar(40)) AS
-- Doel: Kent gebruikersrecht voor willekeurige actieve groep
-- toe aan user met gegeven login. (Enkel voor debugging purposes)
BEGIN 
	DECLARE @stamnr as varchar(10);

	set @stamnr = (select top 1 g.Code from grp.Groep g where g.StopDatum is null order by newid());
	exec auth.spGebruikersRechtToekennen @stamnr, @login
END


-- echte werk:

USE gap_tst
GO


-- Wijzigingen voor 1.9.1

GRANT UPDATE ON verz.PersoonsVerzekering TO GapSuperRole
GO
GRANT UPDATE ON verz.PersoonsVerzekering TO GapRole
GO
GRANT DELETE ON auth.GavSchap TO GapSuperRole
GO
GRANT DELETE ON auth.GavSchap TO GapRole
GO
GRANT UPDATE ON adr.BelgischAdres TO GapRole
GO
GRANT UPDATE ON abo.Abonnement TO GapSuperRole
GO
GRANT UPDATE ON abo.Abonnement TO GapRole
GO

use [KipAdmin_tst]
GO
GRANT DELETE ON [lid].[Aansluiting] TO [KipSyncRole]
GO

