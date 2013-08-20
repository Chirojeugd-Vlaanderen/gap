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


-- versie 1.6?

ALTER TABLE grp.Groep ADD StopDatum DateTime NULL;

-- Wijzigingen voor 1.9

ALTER TABLE pers.Persoon ADD SeNaam AS SOUNDEX(Naam);
ALTER TABLE pers.Persoon ADD SeVoornaam AS SOUNDEX(VoorNaam);

CREATE INDEX IX_Persoon_SoundEx ON pers.Persoon(Naam,Voornaam);
CREATE INDEX IX_Persoon_SoundEx2 ON pers.Persoon(Voornaam,Naam);
use [KipAdmin]
GO
GRANT DELETE ON [lid].[Aansluiting] TO [KipSyncRole]
GO


DROP INDEX [IX_Lid_AfdelingsJaarID] ON [lid].[Kind]
ALTER TABLE lid.Kind ALTER COLUMN afdelingsJaarID integer not null;
CREATE NONCLUSTERED INDEX [IX_Lid_AfdelingsJaarID] ON [lid].[Kind]([afdelingsJaarID] ASC);

-- versie 1.5


ALTER TABLE biv.BivakOverzicht ALTER COLUMN b_naam VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN b_straat VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN b_gemeente VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN b_land VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN b_tel VARCHAR(160);

ALTER TABLE biv.BivakOverzicht ALTER COLUMN s_naam VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN s_straat VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN s_gemeente VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN s_land VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN s_tel VARCHAR(160);

ALTER TABLE biv.BivakOverzicht ALTER COLUMN u_naam VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN u_straat VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN u_gemeente VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN u_land VARCHAR(80);
ALTER TABLE biv.BivakOverzicht ALTER COLUMN u_tel VARCHAR(160);

ALTER TABLE biv.BivakOverzicht ALTER COLUMN b_begin DATETIME;
ALTER TABLE biv.BivakOverzicht ALTER COLUMN b_eind DATETIME;
ALTER TABLE biv.BivakOverzicht ALTER COLUMN s_begin DATETIME;
ALTER TABLE biv.BivakOverzicht ALTER COLUMN s_eind DATETIME;
ALTER TABLE biv.BivakOverzicht ALTER COLUMN u_begin DATETIME;
ALTER TABLE biv.BivakOverzicht ALTER COLUMN u_eind DATETIME;
ALTER TABLE biv.BivakOverzicht ALTER COLUMN stempel DATETIME;

ALTER TABLE [biv].[BivakOverzicht] DROP CONSTRAINT [DF_kipBivak_datum]
ALTER TABLE biv.BivakOverzicht ALTER COLUMN datum DATETIME;
ALTER TABLE [biv].[BivakOverzicht] ADD  CONSTRAINT [DF_kipBivak_datum]  DEFAULT (getdate()) FOR [datum]

