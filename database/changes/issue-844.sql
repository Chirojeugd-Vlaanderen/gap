--   Wijzigingen in de GAP-database voor issue #844
--   Copyright 2014 Chirojeugd-Vlaanderen vzw
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
	Permissies INT NOT NULL,
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

INSERT INTO auth.GebruikersRechtV2(PersoonID, GroepID, VervalDatum, Permissies)
SELECT gs.PersoonID, gr.GroepID, max(gr.VervalDatum) AS VervalDatum, 0x7F7F AS Permissies
FROM auth.GebruikersRecht gr
JOIN auth.Gav gav on gr.GavID = gav.GavID
JOIN auth.GavSchap gs on gav.GavID = gs.GavID
GROUP BY gs.PersoonID, gr.GroepID
GO

DROP TABLE auth.GavSchap;
GO
DROP TABLE auth.GebruikersRecht;
GO
DROP TABLE auth.Gav;
GO

CREATE PROCEDURE [auth].[spGebruikersRechtToekennenAd]
(
	@stamNr VARCHAR(10)
	, @adNr int
)
AS
-- Doel: Geeft login gebruikersrecht op gegeven groep
-- Als de persoon ooit gebruikersrecht had, wordt dit gewoon verlengd.
BEGIN

	INSERT INTO auth.GebruikersRechtV2(PersoonID, GroepID, VervalDatum, Permissies)
	SELECT
			-- voor de vervaldatum doen we maar iets; die wordt straks overschreven.
			-- de permissies overigens ook.
			p.PersoonID, g.GroepID, DateAdd(year, 1, getDate()), 0x7f7f
	FROM
			pers.Persoon p
			, grp.Groep g
	WHERE
			p.AdNummer=@adNr
			AND g.Code=@stamnr
			AND NOT EXISTS (SELECT 1 FROM auth.GebruikersRechtV2 gr
							WHERE gr.PersoonID = p.PersoonID AND gr.GroepID = g.GroepID)

	-- nu de echte vervaldatum.
	declare @datum datetime
			, @jaar varchar(4)

	-- Vanaf juli gaan we ervan uit dat de login voor degene is
	-- die het volgende werkjaar de groepsadministratie zal regelen.
	IF MONTH(getdate()) >= 7
		set @jaar = cast(year(getdate()) + 1 as varchar)
	ELSE
		set @jaar = cast(year(getdate()) as varchar)

	set @datum =  @jaar + '-11-01';

	-- BELANGRIJK! Permissie 0x7f7f is GAV! Alle rechten in de groep.

	UPDATE gr
	SET gr.VervalDatum = @datum, gr.Permissies=0x7f7f
	FROM auth.GebruikersRechtV2 gr JOIN grp.Groep g on gr.GroepID = g.GroepID
	JOIN pers.Persoon p on gr.PersoonID = p.PersoonID
	WHERE p.AdNummer=@adNr AND g.Code=@stamnr
	
	IF @@ROWCOUNT <= 0
	BEGIN
		RAISERROR('Dat StamNr is niet gevonden in de tabel Groep', 16, 1)
	END
END


GO


