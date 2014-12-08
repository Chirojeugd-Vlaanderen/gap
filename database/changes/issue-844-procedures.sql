--   Wijzigingen in de GAP-database voor nieuw gebruikersbeheer.
--   Copyright 2014 Johan Vervloet
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


use gap_local_auth
go

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
-- Doel: gegevens ophalen van de groepen waar de persoon als user aan gekoppeld is
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
			AND (gr.Permissies & 0x7f7f) = 0x7f7f
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
			AND (gr.Permissies & 0x7f7f) = 0x7f7f
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


