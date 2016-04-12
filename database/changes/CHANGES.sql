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

CREATE FUNCTION grp.ufnRaadGeslacht(@stamNr VARCHAR(10)) RETURNS INT AS
-- Bepaalt geslacht (als int) uit stamnummer
BEGIN
	DECLARE @char2 CHAR(1)
	DECLARE @char3 CHAR(1)

	SELECT @char2 = SUBSTRING(@stamNr, 2, 1)
	SELECT @char3 = SUBSTRING(@stamNr, 3, 1)

	IF @char3 = ' ' 
	BEGIN
		RETURN CASE @char2 WHEN 'J' THEN 1 WHEN 'M' THEN 2 ELSE 3 END
	END
	RETURN CASE @char3 WHEN 'J' THEN 1 WHEN 'M' THEN 2 ELSE 3 END
END
GO

CREATE FUNCTION grp.ufnRaadNiveau(@stamNr VARCHAR(10)) RETURNS INT AS
-- Bepaalt niveau (groep=2, gewest=8, verbond=32, nationaal=128) op basis van stamnr
BEGIN
	IF LEFT(@stamNr, 3) = 'NAT' RETURN 128
	-- Dit is niet helemaal waterdicht, maar is voorlopig goed.
	IF RIGHT(@stamNr, 3) = '000' RETURN 32
	IF RIGHT(@stamNr, 2) = '00' RETURN 8
	IF SUBSTRING(@stamNr, 4, 1) = '/' RETURN 2
	RETURN 128
END
GO

CREATE FUNCTION grp.ufnRaadGewest(@stamNr VARCHAR(10)) RETURNS INT AS
-- Bepaalt (GroepID van) gewest uit stamnummer
BEGIN
	DECLARE @char3 CHAR(1)
	DECLARE @leftPart VARCHAR(10)
	DECLARE @midPart VARCHAR(10)
	DECLARE @gewestStamnr VARCHAR(10)

	SELECT @char3 = SUBSTRING(@stamNr, 3, 1)

	IF @char3 = ' ' 
	BEGIN
		SELECT @leftPart = LEFT(@stamNr, 1)
		SELECT @midPart = SUBSTRING(@stamNr, 3, 4)
	END
	ELSE
	BEGIN
		SELECT @leftPart = LEFT(@stamNr, 2)
		SELECT @midPart = SUBSTRING(@stamNr, 4, 3)
	END
	SELECT @gewestStamnr = @leftPart + 'G' + @midPart + '00'
	RETURN (SELECT GroepID FROM grp.Groep WHERE Code = @gewestStamnr)
END
GO

CREATE FUNCTION grp.ufnRaadVerbond(@stamNr VARCHAR(10)) RETURNS INT AS
-- Bepaalt (GroepID van) verbond op basis stamnummer groep
BEGIN
	DECLARE @gewestStamnr VARCHAR(10)
	DECLARE @verbondStamnr VARCHAR(10)

	SET @gewestStamnr = (SELECT Code FROM grp.Groep WHERE GroepID = grp.ufnRaadGewest(@stamNr))

	IF LEFT(@gewestStamnr,1) = 'O'
	BEGIN
		SELECT @verbondStamnr = LEFT(@gewestStamnr, 5) + '000'
	END
	ELSE
	BEGIN
		SELECT @verbondStamnr = LEFT(@gewestStamnr, 4) + '0000'
	END
	RETURN (SELECT GroepID FROM grp.Groep WHERE Code = @verbondStamnr)
END
GO

-- Aangepaste procedure voor nieuwe groep (#5011)
-- Drop oude, maak nieuwe

DROP PROCEDURE data.spNieuweGroepUitKipadmin
GO

CREATE PROCEDURE [data].[spNieuweGroep] 
	@stamNr VARCHAR(10), 
	@naam VARCHAR(160),
	@plaats VARCHAR(60), 
	@werkJaar INT AS
-- Maakt een nieuwe ploeg in GAP

DECLARE @groepID AS INTEGER
DECLARE @geslacht AS INTEGER
DECLARE @groepsWjID AS INTEGER
DECLARE @niveau AS INTEGER

SET @geslacht = grp.ufnRaadGeslacht(@stamNr)
SET @niveau = grp.ufnRaadNiveau(@stamNr)

-----------------
-- groep maken --
-----------------

PRINT 'Groep maken/vinden'
PRINT '------------------'
IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@stamNr)
BEGIN
	-- De groep bestaat nog niet in de database. Creeer.
	INSERT INTO grp.Groep(Naam, Code, OprichtingsJaar)
		SELECT @naam, @stamnr, @werkJaar 
	SET @groepID = scope_identity();
END
ELSE
BEGIN
	-- De Groep bestaat al in de database, de GroepID opvragen in de CG2 database
	SET @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@stamNr)

	-- Opvragen van Naam, Oprichtingsjaar en Website.
	-- Hier veronderstellen we dat meegegeven Naam, Oprichtingsjaar en Website 
	-- steeds van betere kwaliteit zijn dan de al bestaande.

	UPDATE dst
		SET dst.Naam = @naam, 
			dst.OprichtingsJaar = @werkJaar
	FROM grp.Groep dst 
	WHERE dst.GroepID = @groepID
END

-- Maak ook chirogroep of kadergroep aan als die niet bestaat.

IF @niveau = 2
BEGIN
	IF NOT EXISTS (SELECT 1 FROM grp.ChiroGroep WHERE ChiroGroepID = @groepID)
	BEGIN
		INSERT INTO grp.ChiroGroep(ChiroGroepID, Plaats, KaderGroepID)
			SELECT @groepID, @plaats, grp.ufnRaadGewest(@stamNr)
	END
	----------------------------------------------------------------------
	-- standaardafdelingen
	----------------------------------------------------------------------

	IF (SELECT COUNT(*) FROM lid.Afdeling WHERE ChiroGroepID = @groepID) = 0
	BEGIN
		INSERT INTO lid.Afdeling(AfdelingsNaam, Afkorting, ChiroGroepID)
		VALUES	('ribbels', 'RI', @groepID),
				('speelclub', 'SP', @groepID),
				('rakwi''s', 'RA', @groepID),
				('tito''s', 'TI', @groepID),
				('keti''s', 'KE', @groepID),
				('aspi''s', 'AS', @groepID);
	END
	PRINT 'Chirogroep met standaardafdeling ingevoegd/aangepast.'
	-- Geen afdelingsjaren dus, omdat een nieuwe Chirogroep waarschijnlijk geen
	-- regelmatige afdelingsverdeling heeft.
END
ELSE
BEGIN
	IF NOT EXISTS (SELECT 1 FROM grp.KaderGroep WHERE KaderGroepID = @groepID)
	BEGIN
		INSERT INTO grp.KaderGroep(KaderGroepID, Niveau, ParentID)
			SELECT @groepID, @niveau, CASE @niveau WHEN 8 THEN grp.ufnRaadVerbond(@stamNr) ELSE NULL END
	END
	PRINT 'Kadergroep ingevoegd/aangepast.'
END

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

PRINT 'GroepID: ' + CAST(@GroepID AS VARCHAR(10))
PRINT 'GroepsWerkJaarID: ' + CAST(@groepsWjID AS VARCHAR(10))

RETURN @GroepID
GO


