-- Creeer testgroep voor unittests, met testdata

USE ChiroGroep


DECLARE @testGroepCode AS CHAR(10);
DECLARE @testAfdelingsCode AS VARCHAR(10);
DECLARE @testAfdelingsCode2 AS VARCHAR(10);
DECLARE @testWerkJaar AS INT;
DECLARE @testOfficieleAfdelingID AS INT;
DECLARE @testPersoonNaam AS VARCHAR(160)
DECLARE @testPersoonVoorNaam AS VARCHAR(60)
DECLARE @testPersoon2VoorNaam AS VARCHAR(60)


SET @testGroepCode='tst/0001';
SET @testAfdelingsCode='UT';
SET @testAfdelingsCode2='SK';
SET @testWerkJaar=2009;
SET @testOfficieleAfdelingID=1;		-- officiele afdeling voor testafdeling in testwerkjaar
SET @testPersoonNaam = 'Bosmans';
SET @testPersoonVoorNaam = 'Jos';
SET @testPersoon2VoorNaam = 'Irène';


DECLARE @testGroepID AS INT;
DECLARE @testAfdelingID AS INT;
DECLARE @testAfdeling2ID AS INT;
DECLARE @testGroepsWerkJaarID AS INT;
DECLARE @testPersoonID AS INT;
DECLARE @testPersoon2ID AS INT;

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
--- Afdelingen maken 
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

IF NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsCode2)
BEGIN
	INSERT INTO lid.Afdeling(GroepID, AfdelingsNaam, Afkorting)
	VALUES(@testGroepID, 'Speelkwi''s', @testAfdelingsCode2);
	SET @testAfdeling2ID = scope_identity();
END
ELSE
BEGIN
	SET @testAfdeling2ID = (SELECT AfdelingID FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsCode2)
END


---
--- AfdelingsJaar maken voor testafdeling1
---

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsWerkJaarID AND AfdelingID = @testAfdelingID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID)
	VALUES (2003, 2004, @testAfdelingID, @testGroepsWerkJaarID, @testOfficieleAfdelingID);
END

---
--- Testpersonen maken 
---

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoonVoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoonNaam, @testPersoonVoorNaam, '1959-11-30', 1)
	SET @testPersoonID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoonID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoonVoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoon2VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoonNaam, @testPersoon2VoorNaam, '1959-11-30', 1)
	SET @testPersoon2ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon2ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoon2VoorNaam);
END;

---
--- Personen lieren aan testgroep 
---

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoonID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoonID, @testGroepID, 0);
END

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon2ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon2ID, @testGroepID, 0);
END


PRINT 'TestGroepID: ' + CAST(@testGroepID AS VARCHAR(10));
PRINT 'TestAfdeling1ID: ' + CAST(@testAfdelingID AS VARCHAR(10));
PRINT 'TestAfdeling2ID: ' + CAST(@testAfdeling2ID AS VARCHAR(10));
PRINT 'TestGroepsWerkJaarID: ' + CAST(@testGroepsWerkJaarID AS VARCHAR(10));
PRINT 'TestPersoonID: ' + CAST(@testPersoonID AS VARCHAR(10))
PRINT 'TestPersoon2ID: ' + CAST(@testPersoon2ID AS VARCHAR(10))

GO
