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

ALTER TABLE grp.GroepsWerkjaar ADD Datum DATETIME DEFAULT getdate();
GO

UPDATE grp.GroepsWerkjaar SET Datum = '2016-08-23' WHERE Werkjaar = 2016;
GO
-- Rechten om groepswerkjaar te verwijderen (zie #5379)
GRANT DELETE on grp.GroepsWerkJaar TO GapRole;