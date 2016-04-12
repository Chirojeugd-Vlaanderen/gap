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


--  Dit is de procedure voor nieuwe groep, zoals ze was (#5011)
ALTER PROCEDURE [data].[spNieuweGroepUitKipadmin] @stamNr VARCHAR(10), @werkJaar INT AS
-- vult de gegevens van een groep aan met die in Kipadmin.

DECLARE @groepID AS INTEGER
DECLARE @verdCode AS INTEGER
DECLARE @geslacht AS INTEGER
DECLARE @groepsWjID AS INTEGER

SET @verdCode = (SELECT Verd_Code FROM Kipadmin.dbo.Groep WHERE StamNr = @stamnr)
SET @geslacht = (SELECT CASE Soort WHEN 'J' THEN 1 WHEN 'M' THEN 2 ELSE 3 END FROM Kipadmin.Dbo.Groep WHERE StamNr = @stamNr)

----------------------
-- groep overzetten --
----------------------

PRINT 'Chirogroep Overzeten'
PRINT '--------------------'
IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@stamNr)
BEGIN
	-- De Chiro Groep bestaat nog niet in de database, 
	-- de chirogroep dan gewoon invoegen.
	INSERT INTO grp.Groep(Naam, Code, OprichtingsJaar, WebSite)
		SELECT Naam, StamNr, Jr_Aanslui, lower(HomePage) 
		FROM KipAdmin.dbo.Groep
			WHERE StamNr = @stamNr
	SET @groepID = scope_identity();
END
ELSE
BEGIN
	-- De ChiroGroep bestaat al in de database, de GroepID opvragen in de CG2 database
	SET @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@stamNr)

	-- Opvragen van Naam, Oprichtingsjaar en Website.
	-- Hier veronderstellen we dat de Naam, Oprichtingsjaar en Website in KipAdmin 
	-- steeds van betere kwaliteit zijn dan de al geimporteerde.
	
    -- LET OP! De groepid's in kipadmin verschillen van de groepid's in GAP!

	UPDATE dst
		SET dst.Naam = g.Naam, 
			dst.OprichtingsJaar = YEAR(g.StartDatum), 
			dst.WebSite = lower(g.WebSite)
	FROM grp.Groep dst 
	JOIN Kipadmin.grp.ChiroGroep cg on dst.Code = cg.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS
	JOIN Kipadmin.grp.Groep g on cg.GroepID = g.GroepID
	WHERE dst.GroepID = @groepID
END

-- Maak ook chirogroep aan als die niet bestaat.

IF NOT EXISTS (SELECT 1 FROM grp.ChiroGroep WHERE ChiroGroepID = @groepID)
BEGIN
    INSERT INTO grp.ChiroGroep(ChiroGroepID, Plaats, KaderGroepID)
		SELECT @groepID, Gemeente, gg.GroepID
		FROM KipAdmin.grp.ChiroGroep cg
		JOIN KipAdmin.dbo.Gewest gew on cg.GewestNr = gew.Nr
		JOIN grp.Groep gg on gg.Code = gew.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS
			WHERE cg.StamNr = @stamNr
END

PRINT 'Chirogroep ingevoegd/aangepast.'
PRINT ''


----------------------------------------------------------------------
-- voor het gemak meteen een groepswerkjaar maken voor dit werkjaar --
----------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM grp.GroepsWerkJaar gwj 
				WHERE gwj.GroepID = @groepID AND gwj.WerkJaar = @werkJaar)
BEGIN
	INSERT INTO grp.GroepsWerkJaar(GroepID, WerkJaar)
		VALUES(@groepID, @werkJaar)
	SET @groepsWjID = scope_identity();
END
ELSE
BEGIN
	SET @groepsWjID = (SELECT GroepsWerkJaarID FROM grp.GroepsWerkJaar gwj 
					  WHERE gwj.GroepID = @groepID AND gwj.WerkJaar = @werkJaar)
END

----------------------------------------------------------------------
-- standaardafdelingen
----------------------------------------------------------------------

-- eventueel ribbels (met groepsgekozen naam)
-- 3 letters voor ribbelcode, zo is er geen clash met de standaardafdelingen

INSERT INTO lid.Afdeling(AfdelingsNaam, Afkorting, ChiroGroepID)
SELECT core.ufnUcFirst(g.Pink_Naam) AS AfdelingsNaam, LEFT(g.Pink_Naam, 3) AS Afkorting, @groepID AS GroepID FROM kipadmin.dbo.Groep g
WHERE StamNr = @stamNr
	AND g.Verd_Code = 2
	AND NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE ChiroGroepID=@groepID AND AfdelingsNaam=g.Pink_Naam  COLLATE SQL_Latin1_General_CP1_CI_AS)

-- andere afdelingen (standaard zelfde namen als officiele afdelingen)

INSERT INTO lid.Afdeling(AfdelingsNaam, Afkorting, ChiroGroepID)
SELECT oa.Naam AS AfdelingsNaam, UPPER(LEFT(oa.Naam, 2)) AS Afkorting, @groepID AS GroepID FROM lid.OfficieleAfdeling oa
WHERE oa.officieleAfdelingID > 1 and oa.officieleAfdelingID < 7  -- speciaal valt weg
	AND NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE ChiroGroepID=@groepID AND AfdelingsNaam=oa.Naam COLLATE SQL_Latin1_General_CP1_CI_AS)

----------------------------------------------------------------------
-- en meteen ook maar afdelngsjaren
-- (mijn 'i' werkt niet meer fatsoenlijk sinds dataclean mijn keyboard
-- kuiste)
----------------------------------------------------------------------

INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
SELECT 
	a.AfdelingID as AfdelingID,
	@geslacht as Geslacht,
	@groepsWjID as GroepsWerkJaarID,
	oa.OfficieleAfdelingID as OfficieleAfdelingID,
	@werkJaar - oa.LeefTijdTot AS GeboorteJaarVan,
	@werkJaar - oa.LeefTijdVan AS GeboorteJaarTot
FROM lid.OfficieleAfdeling oa
JOIN lid.Afdeling a on oa.Naam = a.AfdelingsNaam
WHERE a.ChiroGroepID=@groepID AND oa.OfficieleAfdelingID > 3
AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@groepsWjID AND AfdelingID=a.AfdelingID)

IF (@verdCode = 2) BEGIN
	-- speelclub en rakwi's
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@groepsWjID as GroepsWerkJaarID,
		oa.OfficieleAfdelingID as OfficieleAfdelingID,
		@werkJaar - oa.LeefTijdTot AS GeboorteJaarVan,
		@werkJaar - oa.LeefTijdVan AS GeboorteJaarTot
	FROM lid.OfficieleAfdeling oa
	JOIN lid.Afdeling a on oa.Naam = a.AfdelingsNaam
	WHERE a.ChiroGroepID=@groepID AND oa.OfficieleAfdelingID in (2,3)
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@groepsWjID AND AfdelingID=a.AfdelingID)

   -- ribbels
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@groepsWjID as GroepsWerkJaarID,
		1 as OfficieleAfdelingID,
		@werkJaar - 7 AS GeboorteJaarVan,
		@werkJaar - 6 AS GeboorteJaarTot
	FROM Kipadmin.dbo.Groep g
	JOIN lid.Afdeling a ON g.Pink_Naam = a.AfdelingsNaam COLLATE SQL_Latin1_General_CP1_CI_AS
	WHERE a.ChiroGroepID = @groepID AND g.StamNr = @stamNr
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@groepsWjID AND AfdelingID=a.AfdelingID)
END	
ELSE
BEGIN
	-- rakwi's
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@groepsWjID as GroepsWerkJaarID,
		3 as OfficieleAfdelingID,
		@werkJaar - 11 AS GeboorteJaarVan,
		@werkJaar - 9 AS GeboorteJaarTot
	FROM lid.Afdeling a
	WHERE a.AfdelingsNaam='Rakwi''s' and a.ChiroGroepID = @groepID
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@groepsWjID AND AfdelingID=a.AfdelingID)

	-- speelclub
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@groepsWjID as GroepsWerkJaarID,
		2 as OfficieleAfdelingID,
		@werkJaar - 8 AS GeboorteJaarVan,
		@werkJaar - 6 AS GeboorteJaarTot
	FROM lid.Afdeling a
	WHERE a.AfdelingsNaam='Speelclub' and a.ChiroGroepID = @groepID
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@groepsWjID AND AfdelingID=a.AfdelingID)
END

PRINT 'GroepID: ' + CAST(@GroepID AS VARCHAR(10))
PRINT 'GroepsWerkJaarID: ' + CAST(@groepsWjID AS VARCHAR(10))

RETURN @GroepID
GO


