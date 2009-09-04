-- Creeer testgroep voor unittests, met testdata

USE ChiroGroep


DECLARE @testGroepCode AS CHAR(10);
DECLARE @testAfdelingsCode AS VARCHAR(10);
DECLARE @testWerkJaar AS INT;
DECLARE @testOfficieleAfdelingID AS INT;


SET @testGroepCode='tst/0001';
SET @testAfdelingsCode='UT';
SET @testWerkJaar=2009;
SET @testOfficieleAfdelingID=1;		-- officiele afdeling voor testafdeling in testwerkjaar


DECLARE @testGroepID AS INT;
DECLARE @testAfdelingID AS INT;
DECLARE @testGroepsWerkJaarID AS INT;

---
--- Groep en Chirogroep creeren
---

IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@testGroepCode)
BEGIN
	INSERT INTO grp.Groep(Naam, Code)
	VALUES('St.-Unittestius', @testGroepCode);

	SET @testGroepID = scope_identity();
END
ELSE
BEGIN
	SET @testGroepID = (SELECT GroepID FROM grp.Groep WHERE Code=@testGroepCode)
END

IF NOT EXISTS (SELECT 1 FROM grp.ChiroGroep WHERE ChiroGroepID = @testGroepID)
BEGIN
	INSERT INTO grp.ChiroGroep(ChiroGroepID) VALUES(@testGroepID);
END

--- 
--- Groepswerkjaar creeren 
---

IF NOT EXISTS (SELECT 1 FROM grp.GroepsWerkJaar WHERE GroepID = @testGroepID AND WerkJaar = @testWerkJaar)
BEGIN
	INSERT INTO grp.GroepsWerkJaar(WerkJaar, GroepID)
	VALUES (@testWerkJaar, @testGroepID);

	SET @testGroepsWerkJaarID = scope_identity();
END
ELSE
BEGIN
	SET @testGroepsWerkJaarID = (SELECT GroepsWerkJaarID FROM grp.GroepsWerkJaar WHERE GroepID = @testGroepID AND WerkJaar = @testWerkJaar)
END

---
--- Afdeling maken 
---

IF NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsCode)
BEGIN
	INSERT INTO lid.Afdeling(GroepID, AfdelingsNaam, Afkorting)
	VALUES(@testGroepID, 'Unittestjes', @testAfdelingsCode);
	SET @testAfdelingID = scope_identity();
END
ELSE
BEGIN
	SET @testAfdelingID = (SELECT AfdelingID FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsCode)
END

---
--- AfdelingsJaar maken voor net gecreeerde officiele afdeling
---

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsWerkJaarID AND AfdelingID = @testAfdelingID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID)
	VALUES (2003, 2004, @testAfdelingID, @testGroepsWerkJaarID, @testOfficieleAfdelingID);
END


PRINT 'TestGroepID: ' + CAST(@testGroepID AS VARCHAR(10));
PRINT 'TestAfdelingID: ' + CAST(@testAfdelingID AS VARCHAR(10));
PRINT 'TestGroepsWerkJaarID: ' + CAST(@testGroepsWerkJaarID AS VARCHAR(10));


GO
