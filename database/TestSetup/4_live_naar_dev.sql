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

-- Wijzigingen voor 1.9

ALTER TABLE pers.Persoon ADD SeNaam AS SOUNDEX(Naam);
ALTER TABLE pers.Persoon ADD SeVoornaam AS SOUNDEX(VoorNaam);

CREATE INDEX IX_Persoon_SoundEx ON pers.Persoon(Naam,Voornaam);
CREATE INDEX IX_Persoon_SoundEx2 ON pers.Persoon(Voornaam,Naam);