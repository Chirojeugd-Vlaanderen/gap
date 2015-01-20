--   Wijzigingen in de GAP-database voor issue #844
--   Copyright 2014,2015 Chirojeugd-Vlaanderen vzw
--
--   Licensed under the Apache License, Version 2.0 (the "License");
--   you may not use this file except in compliance with the License.
--   You may obtain a copy of the License at
--
--       http://www.apache.org/licenses/LICENSE-2.0
--
--   Unless required by applicable law or agreed to in writing, software
--   distributed under the License is distributed on an "AS IS" BASIS,
--   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
--   See the License for the specific language governing permissions and
--   limitations under the License.


use gap_local;

CREATE TABLE auth.GebruikersRechtv2(
	GebruikersRechtV2ID int IDENTITY(1,1) NOT NULL,
	PersoonID INT NOT NULL,
	GroepID INT NOT NULL,
	VervalDatum DATETIME NULL,
	PersoonsPermissies INT NOT NULL DEFAULT 0,
	GroepsPermissies INT NOT NULL DEFAULT 0,
	AfdelingsPermissies INT NOT NULL DEFAULT 0,
	IedereenPermissies INT NOT NULL DEFAULT 0,
	Versie TIMESTAMP NULL,
 CONSTRAINT PK_GebruikersrechtV2 PRIMARY KEY(GebruikersRechtV2ID)
);
GO

ALTER TABLE auth.GebruikersRechtV2 ADD CONSTRAINT FK_GebruikersRechtV2_Persoon FOREIGN KEY(PersoonID)
REFERENCES pers.Persoon(PersoonID);
GO

ALTER TABLE auth.GebruikersRechtV2 ADD CONSTRAINT FK_GebruikersRechtV2_Groep FOREIGN KEY(GroepID)
REFERENCES grp.Groep(GroepID);
GO

CREATE UNIQUE INDEX AK_GebruikersRechtV2_PersoonID_GroepID ON auth.GebruikersRechtV2(PersoonID, GroepID);
GO

INSERT INTO auth.GebruikersRechtV2(PersoonID, GroepID, VervalDatum, GroepsPermissies, IedereenPermissies)
SELECT gs.PersoonID, gr.GroepID, max(gr.VervalDatum) AS VervalDatum, 3 as GroepsPermissies, 3 as IedereenPermissies
FROM auth.GebruikersRecht gr
JOIN auth.Gav gav on gr.GavID = gav.GavID
JOIN auth.GavSchap gs on gav.GavID = gs.GavID
GROUP BY gs.PersoonID, gr.GroepID
GO

CREATE INDEX IDX_GebruikersRechtV2_GroepID ON auth.GebruikersRechtV2(GroepID, VervalDatum) INCLUDE(PersoonID);
GO

CREATE INDEX IDX_GebruikersRechtV2_PersoonID ON auth.GebruikersRechtV2(PersoonID, VervalDatum) INCLUDE(GroepID);
GO


DROP TABLE auth.GavSchap;
GO
DROP TABLE auth.GebruikersRecht;
GO
DROP TABLE auth.Gav;
GO

