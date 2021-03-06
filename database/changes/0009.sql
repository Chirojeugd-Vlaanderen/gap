-- Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the
-- top-level directory of this distribution, and at
-- https://gapwiki.chiro.be/copyright
--
-- Licensed under the Apache License, Version 2.0 (the "License");
-- you may not use this file except in compliance with the License.
-- You may obtain a copy of the License at
--
--     http://www.apache.org/licenses/LICENSE-2.0
--
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS,
-- WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
-- See the License for the specific language governing permissions and
-- limitations under the License.

-- Voeg hier de wijzigingen toe die moeten gebeuren aan de database.

-- Een view voor actieve leden (zie #5386)
-- Drop view als hij al bestaat, dan kunnen we hem terug maken.
IF OBJECT_ID('diag.vActiefLid', 'V') IS NOT NULL
    DROP VIEW diag.vActiefLid;
GO

CREATE VIEW diag.vActiefLid AS
SELECT l.LidID, p.PersoonID, g.GroepID, p.AdNummer, g.Code, huidigwj.WerkJaar FROM lid.Lid l
JOIN
(
-- We gaan er even vanuit dat recente groepswerkjaren een hoger ID
-- hebben dan oude groepswerkjaren. We mogen dat eigenlijk niet doen.
-- Maar we doen het toch.
SELECT
MAX(gwj.GroepsWerkJaarID) AS GroepsWerkJaarID,
MAX(gwj.WerkJaar) AS WerkJaar,
gwj.GroepID
FROM grp.GroepsWerkJaar gwj
GROUP BY gwj.GroepID
) huidigwj ON l.GroepsWerkjaarID = huidigwj.GroepsWerkJaarID
JOIN grp.Groep g ON huidigwj.GroepID = g.GroepID
JOIN pers.GelieerdePersoon gp on l.GelieerdePersoonID = gp.GelieerdePersoonID
JOIN pers.Persoon p on gp.PersoonID = p.PersoonID
WHERE l.NonActief = 0 AND g.StopDatum IS NULL;
GO

-- Geen koppeling van loggingtabel aan groeptabel via stamnr. (#5363)
IF (OBJECT_ID('FK_Bericht_Groep', 'F') IS NOT NULL)
  ALTER TABLE [logging].[Bericht] DROP CONSTRAINT [FK_Bericht_Groep];
GO

-- Disable 2 gestopte groepen in GAP (zie ook #5386).

UPDATE grp.Groep SET StopDatum='2015-08-31' WHERE Code in ('BG /0605','OG /1208');
