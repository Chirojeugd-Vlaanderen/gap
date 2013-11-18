-- STAP 4: migreer database van live naar dev

-- Hier moeten alle aanpassingen komen aan de database die nodig zijn om de live-db
-- om te zetten naar de db die gebruikt wordt in dev.

USE [master]
GO

-- VOOR TEST:

-- database: simple

ALTER DATABASE [gap_tst] SET COMPATIBILITY_LEVEL = 100
GO

ALTER DATABASE [gap_tst] SET RECOVERY SIMPLE WITH NO_WAIT
GO

-- users:

-- eerst service-user uit DB deleten, dan terug koppelen
-- aan user op deze server.

USE [gap_tst]
GO

DROP SCHEMA [kipdorp\gapservice_tst]
GO

DROP USER [KIPDORP\gapservice_tst]
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

-- nodig voor mergen van personen (mogelijk):

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

-- beter voorbeeld voor telefoonnummer en faxnummer

update pers.communicatietype set voorbeeld='0483-67 94 90' where communicatietypeid = 1
update pers.communicatietype set voorbeeld='03-232 15 62' where communicatietypeid = 2

update pers.communicatietype set validatie='^0[0-9]{1,2}\-[0-9]{2,3} ?[0-9]{2} ?[0-9]{2}$|^04[0-9]{2}\-[0-9]{2,3} ?[0-9]{2} ?[0-9]{2}$|^[+][0-9]*$' where communicatietypeid=1
update pers.communicatietype set validatie='^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$' where communicatietypeid in (3,5,6,8)


use [KipAdmin]
GO
GRANT DELETE ON [lid].[Aansluiting] TO [KipSyncRole]
GO

