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

-- Een view voor actieve abonnementen (zie #5463)
-- Drop view als hij al bestaat, dan kunnen we hem terug maken.
IF OBJECT_ID('diag.vActiefAbonnement', 'V') IS NOT NULL
    DROP VIEW diag.vActiefAbonnement;
GO

CREATE VIEW diag.vActiefAbonnement AS
-- Voor 't gemak zitten hier dubbels bij.
SELECT DISTINCT ab.AbonnementID, gp.PersoonID, ab.Type, p.AdNummer 
FROM abo.Abonnement ab
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
) huidigwj ON ab.GroepsWerkjaarID = huidigwj.GroepsWerkJaarID
JOIN grp.Groep g ON huidigwj.GroepID = g.GroepID
JOIN pers.GelieerdePersoon gp on ab.GelieerdePersoonID = gp.GelieerdePersoonID
JOIN pers.Persoon p on gp.PersoonID = p.PersoonID
WHERE g.StopDatum IS NULL;
GO