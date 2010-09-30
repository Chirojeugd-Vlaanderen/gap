create procedure data.spStandaardAfdelingsVerdeling (@gwjId as int) as
-- bedoeling: standaardafdelingsverdeling maken in gegeven groepswerkjaar
-- (5 of 6 afdelingen op basis van kipadmin)
begin


declare @GroepID as integer; set @GroepID = (select GroepID from grp.GroepsWerkJaar where GroepsWerkJaarID=@gwjID);
declare @StamNr as varchar(10); set @StamNr = (select Code from grp.Groep where GroepID = @GroepID);
declare @Geslacht as integer; SET @geslacht = (SELECT CASE Soort WHEN 'J' THEN 1 WHEN 'M' THEN 2 ELSE 3 END FROM Kipadmin.Dbo.Groep WHERE StamNr = @stamNr)
declare @WerkJaar as integer; set @WerkJaar = (select WerkJaar from grp.GroepsWerkJaar where GroepsWerkJaarID=@gwjID);
declare @VerdCode as integer; SET @verdCode = (SELECT Verd_Code FROM Kipadmin.dbo.Groep WHERE StamNr = @stamnr);

----------------------------------------------------------------------
-- standaardafdelingen
----------------------------------------------------------------------

-- eventueel ribbels (met groepsgekozen naam)
-- 3 letters voor ribbelcode, zo is er geen clash met de standaardafdelingen

INSERT INTO lid.Afdeling(AfdelingsNaam, Afkorting, GroepID)
SELECT core.ufnUcFirst(g.Pink_Naam) AS AfdelingsNaam, LEFT(g.Pink_Naam, 3) AS Afkorting, @groepID AS GroepID FROM kipadmin.dbo.Groep g
WHERE StamNr = @stamNr
	AND g.Verd_Code = 2
	AND NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE GroepID=@groepID AND AfdelingsNaam=g.Pink_Naam  COLLATE SQL_Latin1_General_CP1_CI_AS)

-- andere afdelingen (standaard zelfde namen als officiele afdelingen)


INSERT INTO lid.Afdeling(AfdelingsNaam, Afkorting, GroepID)
SELECT oa.Naam AS AfdelingsNaam, UPPER(LEFT(oa.Naam, 2)) AS Afkorting, @groepID AS GroepID FROM lid.OfficieleAfdeling oa
WHERE oa.officieleAfdelingID > 1 and oa.officieleAfdelingID < 7  -- speciaal valt weg
	AND NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE GroepID=@groepID AND AfdelingsNaam=oa.Naam COLLATE SQL_Latin1_General_CP1_CI_AS)

----------------------------------------------------------------------
-- en meteen ook maar afdelingsjaren
----------------------------------------------------------------------

INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
SELECT 
	a.AfdelingID as AfdelingID,
	@geslacht as Geslacht,
	@gwjId as GroepsWerkJaarID,
	oa.OfficieleAfdelingID as OfficieleAfdelingID,
	@werkJaar - oa.LeefTijdTot AS GeboorteJaarVan,
	@werkJaar - oa.LeefTijdVan AS GeboorteJaarTot
FROM lid.OfficieleAfdeling oa
JOIN lid.Afdeling a on oa.Naam = a.AfdelingsNaam
WHERE a.GroepID=@groepID AND oa.OfficieleAfdelingID > 3
AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@gwjId AND AfdelingID=a.AfdelingID)

IF (@verdCode = 2) BEGIN
	-- speelclub en rakwi's
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@gwjId as GroepsWerkJaarID,
		oa.OfficieleAfdelingID as OfficieleAfdelingID,
		@werkJaar - oa.LeefTijdTot AS GeboorteJaarVan,
		@werkJaar - oa.LeefTijdVan AS GeboorteJaarTot
	FROM lid.OfficieleAfdeling oa
	JOIN lid.Afdeling a on oa.Naam = a.AfdelingsNaam
	WHERE a.GroepID=@groepID AND oa.OfficieleAfdelingID in (2,3)
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@gwjId AND AfdelingID=a.AfdelingID)

   -- ribbels
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@gwjId as GroepsWerkJaarID,
		1 as OfficieleAfdelingID,
		@werkJaar - 7 AS GeboorteJaarVan,
		@werkJaar - 6 AS GeboorteJaarTot
	FROM Kipadmin.dbo.Groep g
	JOIN lid.Afdeling a ON g.Pink_Naam = a.AfdelingsNaam COLLATE SQL_Latin1_General_CP1_CI_AS
	WHERE a.GroepID = @groepID AND g.StamNr = @stamNr
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@gwjId AND AfdelingID=a.AfdelingID)
END	
ELSE
BEGIN
	-- rakwi's
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@gwjId as GroepsWerkJaarID,
		3 as OfficieleAfdelingID,
		@werkJaar - 11 AS GeboorteJaarVan,
		@werkJaar - 9 AS GeboorteJaarTot
	FROM lid.Afdeling a
	WHERE a.AfdelingsNaam='Rakwi''s' and a.GroepID = @groepID
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@gwjId AND AfdelingID=a.AfdelingID)

	-- speelclub
	INSERT INTO lid.AfdelingsJaar(AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID, GeboorteJaarVan, GeboorteJaarTot)
	SELECT 
		a.AfdelingID as AfdelingID,
		@geslacht as Geslacht,
		@gwjId as GroepsWerkJaarID,
		2 as OfficieleAfdelingID,
		@werkJaar - 8 AS GeboorteJaarVan,
		@werkJaar - 6 AS GeboorteJaarTot
	FROM lid.Afdeling a
	WHERE a.AfdelingsNaam='Speelclub' and a.GroepID = @groepID
	AND NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID=@gwjId AND AfdelingID=a.AfdelingID)
END

PRINT 'GroepID: ' + CAST(@GroepID AS VARCHAR(10))
PRINT 'GroepsWerkJaarID: ' + CAST(@gwjId AS VARCHAR(10))

end