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

