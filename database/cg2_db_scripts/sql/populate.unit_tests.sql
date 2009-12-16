-- Creeer testgroep voor unittests, met deze testdata:
-- @testWerkJaar en @testVorigWerkJaar
--
-- Een Groep (en ChiroGroep) met code @testGroepCode
-- Een GroepsWerkJaar in @testWerkJaar
-- Afdelingen met codes @testAfdelingsCode en @testAfdelingsCode2
-- AfdelingsWerkJaar en voor afdeling met code @testAfdelingsCode in @testWerkJaar en @testVorigWerkJaar
-- Gelieerde Testpersonen:
--		 @testPersoonVoornaam @testPersoonNaam, @testPersoon2VoorNaam en @testPersoonNaam, @testPersoon3VoorNaam en @testPersoonNaam
--
-- 2 test-GAV's, en maakt de eerste GAV van testgroep1.
--
-- 1ste en 2de gelieerde persoon mogen geen lid zijn, want daarmee wordt geexperimenteerd in de ledentests
-- 3de gelieerde persoon is leiding
--
-- 1 testcategorie voor testgroep1, waaraan de 1ste gelieerde persoon worden toegevoegd
-- 2de testcategorie met de drie gelieerde personen


DECLARE @testGroepCode AS CHAR(10);
DECLARE @testAfdelingsCode AS VARCHAR(10);
DECLARE @testAfdelingsCode2 AS VARCHAR(10);
DECLARE @testWerkJaar AS INT;
DECLARE @testVorigWerkJaar AS INT;
DECLARE @testOfficieleAfdelingID AS INT;
DECLARE @testPersoonNaam AS VARCHAR(160)
DECLARE @testPersoonVoorNaam AS VARCHAR(60)
DECLARE @testPersoon2VoorNaam AS VARCHAR(60)
DECLARE @testPersoon3VoorNaam AS VARCHAR(60)
DECLARE @testGav1Login AS VARCHAR(40);
DECLARE @testGav2Login AS VARCHAR(40);
DECLARE @testCategorieCode AS VARCHAR(10);
DECLARE @testCategorie2Code AS VARCHAR(10);


SET @testGroepCode='tst/0001';
SET @testAfdelingsCode='UT';
SET @testAfdelingsCode2='SK';
SET @testWerkJaar=2009;
SET @testVorigWerkJaar=2008;
SET @testOfficieleAfdelingID=1;		-- officiele afdeling voor testafdeling in testwerkjaar
SET @testPersoonNaam = 'Bosmans';
SET @testPersoonVoorNaam = 'Jos';
SET @testPersoon2VoorNaam = 'Irène';
SET @testPersoon3VoorNaam = 'Eugène';
SET @testGav1Login = 'Yvonne';
SET @testGAV2Login='Yvette';
SET @testCategorieCode = 'last';
SET @testCategorie2Code = 'peulen';

DECLARE @testGroepID AS INT;
DECLARE @testAfdelingID AS INT;
DECLARE @testAfdeling2ID AS INT;
DECLARE @testGroepsWerkJaarID AS INT;
DECLARE @testVorigGroepsWerkJaarID AS INT;
DECLARE @testPersoonID AS INT;
DECLARE @testPersoon2ID AS INT;
DECLARE @testPersoon3ID AS INT;
DECLARE @testGelieerdePersoonID AS INT;
DECLARE @testGelieerdePersoon2ID AS INT;
DECLARE @testGelieerdePersoon3ID AS INT;
DECLARE @testAfdelingsJaarID AS INT;
DECLARE @testVorigAfdelingsJaarID AS INT;
DECLARE @testGav1ID AS INT;
DECLARE @testCategorieID AS INT;
DECLARE @testCategorie2ID AS INT;
DECLARE @testLid3ID AS INT;

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
--- Groepswerkjaren creeren 
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

IF NOT EXISTS (SELECT 1 FROM grp.GroepsWerkJaar WHERE GroepID = @testGroepID AND WerkJaar = @testVorigWerkJaar)
BEGIN
	INSERT INTO grp.GroepsWerkJaar(WerkJaar, GroepID)
	VALUES (@testVorigWerkJaar, @testGroepID);

	SET @testVorigGroepsWerkJaarID = scope_identity();
END
ELSE
BEGIN
	SET @testVorigGroepsWerkJaarID = (SELECT GroepsWerkJaarID FROM grp.GroepsWerkJaar WHERE GroepID = @testGroepID AND WerkJaar = @testVorigWerkJaar)
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
	SET @testAfdelingsJaarID = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SET @testAfdelingsJaarID = (SELECT AfdelingsJaarID FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsWerkJaarID AND AfdelingID = @testAfdelingID)
END

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testVorigGroepsWerkJaarID AND AfdelingID = @testAfdelingID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID)
	VALUES (2003, 2004, @testAfdelingID, @testVorigGroepsWerkJaarID, @testOfficieleAfdelingID);
	SET @testVorigAfdelingsJaarID = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SET @testVorigAfdelingsJaarID = (SELECT AfdelingsJaarID FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testVorigGroepsWerkJaarID AND AfdelingID = @testAfdelingID)
END

---
--- Testpersonen maken 
---

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoonVoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoonNaam, @testPersoonVoorNaam, '30/11/1959', 1)
	SET @testPersoonID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoonID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoonVoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoon2VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoonNaam, @testPersoon2VoorNaam, '30/11/1959', 1)
	SET @testPersoon2ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon2ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoon2VoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoon3VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoonNaam, @testPersoon3VoorNaam, '30/11/1959', 1)
	SET @testPersoon3ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon3ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoonNaam AND VoorNaam = @testPersoon3VoorNaam);
END;

---
--- Personen lieren aan testgroep 
---

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoonID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoonID, @testGroepID, 0);
	SET @testGelieerdePersoonID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoonID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoonID AND GroepID = @testGroepID)
END

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon2ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon2ID, @testGroepID, 0);
	SET @testGelieerdePersoon2ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoon2ID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon2ID AND GroepID = @testGroepID)
END

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon3ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon3ID, @testGroepID, 0);
	SET @testGelieerdePersoon3ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoon3ID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon3ID AND GroepID = @testGroepID)
END


-- 
-- Users creeren 
--

IF NOT EXISTS (SELECT 1 FROM auth.GAV WHERE Login=@testGav1Login)
BEGIN
	INSERT INTO auth.GAV(Login) VALUES(@testGav1Login);
	SET @testGav1ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGAV1ID =  (SELECT GavID FROM auth.GAV WHERE Login=@testGav1Login)
END

IF NOT EXISTS (SELECT 1 FROM auth.GAV WHERE Login=@testGav2Login)
BEGIN
	INSERT INTO auth.GAV(Login) VALUES(@testGav2Login);
END

--
-- Gebruikersrecht
--

IF NOT EXISTS (SELECT 1 FROM auth.GebruikersRecht WHERE GavID=@testGav1ID AND GroepID=@testGroepID)
BEGIN
	INSERT INTO auth.GebruikersRecht(GavID, GroepID) VALUES(@testGav1ID, @testGroepID)
END

--
-- TestCategorieen maken voor testgroep 1
--

IF NOT EXISTS (SELECT 1 FROM core.Categorie WHERE Code=@testCategorieCode)
BEGIN
	INSERT INTO core.Categorie(Naam, Code, GroepID) VALUES('Vervelende mensen', @testCategorieCode, @testGroepID);
	SET @testCategorieID = scope_identity();
END
ELSE
BEGIN
	SET @testCategorieID = (SELECT CategorieID FROM core.Categorie WHERE Code=@testCategorieCode)
END

IF NOT EXISTS (SELECT 1 FROM core.Categorie WHERE Code=@testCategorie2Code)
BEGIN
	INSERT INTO core.Categorie(Naam, Code, GroepID) VALUES('Peulengaleis', @testCategorie2Code, @testGroepID);
	SET @testCategorie2ID = scope_identity();
END
ELSE
BEGIN
	SET @testCategorie2ID = (SELECT CategorieID FROM core.Categorie WHERE Code=@testCategorie2Code)
END


--
-- Testcategorieen bevolken
--

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoonID AND CategorieID=@testCategorieID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoonID, @testCategorieID);
END;

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoonID AND CategorieID=@testCategorie2ID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoonID, @testCategorie2ID);
END;

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoon2ID AND CategorieID=@testCategorie2ID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoon2ID, @testCategorie2ID);
END;

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoon3ID AND CategorieID=@testCategorie2ID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoon3ID, @testCategorie2ID);
END;

-- leden maken
--

IF NOT EXISTS (SELECT 1 FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon3ID AND GroepsWerkJaarID = @testGroepsWerkJaarID)
BEGIN
	INSERT INTO lid.Lid(GelieerdePersoonID, GroepsWerkJaarID, NonActief, Verwijderd, LidGeldBetaald, VolgendWerkJaar) VALUES (@testGelieerdePersoon3ID, @testGroepsWerkJaarID, 0, 0, 0, 0)
	SET @testLid3ID = scope_identity();
END
ELSE
BEGIN
	SET @testLid3ID = (SELECT LidID FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon3ID AND GroepsWerkJaarID = @testGroepsWerkJaarID)
END

IF NOT EXISTS (SELECT 1 FROM lid.Leiding WHERE LeidingID = @testLid3ID)
BEGIN
	INSERT INTO lid.Leiding(LeidingID) VALUES (@testLid3ID)
END

PRINT 'TestGroepID: ' + CAST(@testGroepID AS VARCHAR(10));
PRINT 'TestAfdeling1ID: ' + CAST(@testAfdelingID AS VARCHAR(10));
PRINT 'TestAfdeling2ID: ' + CAST(@testAfdeling2ID AS VARCHAR(10));
PRINT 'TestGroepsWerkJaarID: ' + CAST(@testGroepsWerkJaarID AS VARCHAR(10));
PRINT 'TestVorigGroepsWerkJaarID: ' + CAST(@testVorigGroepsWerkJaarID AS VARCHAR(10));
PRINT 'TestAdfelingsJaarID: ' + CAST(@testAfdelingsJaarID AS VARCHAR(10));
PRINT 'TestVorigAdfelingsJaarID: ' + CAST(@testVorigAfdelingsJaarID AS VARCHAR(10));
PRINT 'TestPersoonID: ' + CAST(@testPersoonID AS VARCHAR(10))
PRINT 'TestGelieerdePersoonID: ' + CAST(@testGelieerdePersoonID AS VARCHAR(10))
PRINT 'TestPersoon2ID: ' + CAST(@testPersoon2ID AS VARCHAR(10))
PRINT 'TestGelieerdePersoon2ID: ' + CAST(@testGelieerdePersoon2ID AS VARCHAR(10))
PRINT 'TestPersoon3ID: ' + CAST(@testPersoon3ID AS VARCHAR(10))
PRINT 'TestGelieerdePersoon3ID: ' + CAST(@testGelieerdePersoon3ID AS VARCHAR(10))
PRINT 'TestGav1ID: ' + CAST(@testGav1ID AS VARCHAR(10))
PRINT 'TestCategorieID: ' + CAST(@testCategorieID AS VARCHAR(10))
PRINT 'TestCategorie2ID: ' + CAST(@testCategorie2ID AS VARCHAR(10))
PRINT 'TestLid3ID: ' + CAST(@testLid3ID AS VARCHAR(10))

GO
