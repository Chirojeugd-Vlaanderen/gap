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

-- replace old tables with new tables.

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

INSERT INTO auth.GebruikersRechtV2(PersoonID, GroepID, VervalDatum, GroepsPermissies, IedereenPermissies, PersoonsPermissies)
SELECT gs.PersoonID, gr.GroepID, max(gr.VervalDatum) AS VervalDatum, 3 as GroepsPermissies, 3 as IedereenPermissies, 1 as PersoonsPermissies
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

-- procedures, vooral voor intranet/extranet

CREATE PROCEDURE [auth].[spGebruikersRechtToekennenAd]
(
	@stamNr VARCHAR(10)
	, @adNr int
)
AS
-- Doel: Geeft login gebruikersrecht op gegeven groep
-- Als de persoon ooit gebruikersrecht had, wordt dit gewoon verlengd.
BEGIN
	-- bepaal de vervaldatum.
	declare @datum datetime
			, @jaar varchar(4)

	-- Vanaf juli gaan we ervan uit dat de login voor degene is
	-- die het volgende werkjaar de groepsadministratie zal regelen.
	IF MONTH(getdate()) >= 7
		set @jaar = cast(year(getdate()) + 1 as varchar)
	ELSE
		set @jaar = cast(year(getdate()) as varchar)

	set @datum =  @jaar + '-11-01';

	-- Verwijder eventueel oud gebruikersrecht.
	DELETE gr FROM auth.GebruikersRechtV2 gr 
		JOIN pers.Persoon p ON gr.PersoonID = p.PersoonID
		JOIN grp.Groep g ON gr.GroepID = g.GroepID
	WHERE p.AdNummer=@adNr AND g.Code=@stamNr;

	INSERT INTO auth.GebruikersRechtV2(PersoonID, GroepID, VervalDatum, GroepsPermissies, IedereenPermissies, PersoonsPermissies)
	SELECT
			p.PersoonID, g.GroepID, @datum, 3, 3, 1 -- Je moet jezelf kunnen zien om toegang te hebben, zie #5618
	FROM
			pers.Persoon p
			, grp.Groep g
	WHERE
			p.AdNummer=@adNr
			AND g.Code=@stamnr

	IF @@ROWCOUNT <= 0
	BEGIN
		RAISERROR('Dat StamNr is niet gevonden in de tabel Groep', 16, 1)
	END
END

GO

CREATE PROCEDURE auth.spAlleGebruikersRechtenOntnemenAd
(
	@adNr INT
)
AS
-- Doel: Ontneemt gebruikersrecht op alle gekoppelde groepen
BEGIN
    UPDATE gr
	SET VervalDatum = DateAdd(day, -1, getdate())
	FROM auth.Gebruikersrechtv2 gr 
	JOIN pers.Persoon p on gr.PersoonID= p.PersoonID
	WHERE p.AdNummer = @adNr
END
GO


CREATE procedure auth.spGebruikersRechtenOphalenAd
(
	@adNr INT
)
AS
-- Doel: gegevens ophalen van de groepen waar de persoon als GAV aan gekoppeld is
BEGIN
	SET NOCOUNT ON;

	SELECT
			g.GroepID
			, Code AS StamNr
			, g.Naam
			, Plaats
			, gr.Vervaldatum
	FROM
			grp.Groep g
			LEFT OUTER JOIN grp.Chirogroep cg
				ON g.GroepID = cg.ChiroGroepID
			INNER JOIN auth.GebruikersrechtV2 gr
				ON g.GroepID = gr.GroepID
			INNER JOIN pers.Persoon gav
				ON gr.PersoonID = gav.PersoonID
	WHERE
			gav.AdNummer = @adNr
			AND gr.GroepsPermissies = 3 AND gr.IedereenPermissies = 3
END

GO


CREATE procedure [auth].[spGebruikersRechtenPerGroepOphalenAd]
(
	@stamnr varchar(8)
)
AS
-- Doel: Personen ophalen met GAV-rechten op de groep
BEGIN
	SET NOCOUNT ON;

	SELECT
			gav.AdNummer
			, gr.Vervaldatum
	FROM
			grp.Groep g
			INNER JOIN auth.GebruikersrechtV2 gr
				ON g.GroepID = gr.GroepID
			INNER JOIN pers.Persoon gav
				ON gr.PersoonID = gav.PersoonID
	WHERE
			Code = @stamnr
			AND gr.Vervaldatum > dateadd(month, -3, getdate())
			AND gr.GroepsPermissies = 3 AND gr.IedereenPermissies = 3
END
GO


CREATE PROCEDURE auth.spGebruikersRechtOntnemenAd
(
	@stamNr VARCHAR(10)
	, @adNr INT
)
AS
-- Doel: Ontneemt gebruikersrecht op gegeven groep
BEGIN
    UPDATE gr
	SET VervalDatum = DateAdd(day, -1, getdate())
	FROM grp.Groep g JOIN auth.GebruikersrechtV2 gr ON g.GroepID = gr.GroepID
					JOIN pers.Persoon gav ON gr.PersoonID = gav.PersoonID
	WHERE g.Code=@stamNr AND gav.AdNummer=@adNr
END
GO


-- procedures voor dev (moet weg uit live db)

CREATE PROCEDURE [auth].[spWillekeurigeGroepToekennenAd]
(
	@adNr int
)
AS
-- Doel: Kent rechten toe aan persoon met gegeven AD-nummer.
-- Als er geen gebruiker is met gegeven AD-nummer, wordt die gemaakt op basis
-- van meegeleverde naam en voornaam.
BEGIN
	DECLARE @stamnr as varchar(10);

	set @stamnr = (select top 1 g.Code from grp.Groep g where g.StopDatum is null order by newid());
	exec auth.spGebruikersRechtToekennenAd @stamnr, @adNr
END
GO

GRANT EXECUTE ON auth.spWillekeurigeGroepToekennenAd TO GapHackRole;
GO
