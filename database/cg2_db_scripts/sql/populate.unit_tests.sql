-- Creeer testgroep voor unittests, met deze testdata:
-- @testHuidigWerkJaar en @testVorigWerkJaar
--
-- Een Groep (en ChiroGroep) met code @testGroepCode
-- Een GroepsWerkJaar in @testHuidigWerkJaar
-- Afdelingen met codes @testAfdelingsAfkorting1 en @testAfdelingsAfkorting12
-- AfdelingsWerkJaar en voor afdeling met code @testAfdelingsAfkorting1 in @testHuidigWerkJaar en @testVorigWerkJaar
-- Gelieerde Testpersonen:
--		 @testPersoonVoorNaam1 @testPersoonNaam1, @testPersoon2VoorNaam en @testPersoonNaam1, @testPersoon3VoorNaam en @testPersoonNaam1
--
-- 2 test-GAV's, en maakt de eerste GAV van testgroep1.
--
-- 1ste en 2de gelieerde persoon mogen geen lid zijn, want daarmee wordt geexperimenteerd in de ledentests
-- 3de gelieerde persoon is leiding
--
-- 1 testcategorie voor testgroep1, waaraan de 1ste gelieerde persoon worden toegevoegd
-- 2de testcategorie met de drie gelieerde personen

-- Datums worden in gegeven in het formaat DD/MM/YYYY
-- We definiteren dit hier (in de documentatie) omdat hier beneden we het gebruiken.
SET DateFormat dmy;

-- Dit script doet het volgende: 
--
-- 1°  Creatie van een ChiroGroep, 'St.-Unittestius' met als stamnummer 'tst/0001'
		DECLARE @testGroepID AS INT;
		DECLARE @testGroepCode AS CHAR(10); 					SET @testGroepCode='tst/0001';
		DECLARE @testGroepNaam AS VARCHAR(160);					SET @testGroepNaam='St.-Unittestius';

-- 2° Voor deze ChiroGroep maken we 2 groepswerkjaren aan, 
--    1 voor dit jaar en 1 voor het vorige jaar.
		DECLARE @testGroepsHuidigWerkJaarID AS INT;
		DECLARE @testHuidigWerkJaar AS INT;						SET @testHuidigWerkJaar=2009;
		DECLARE @testVorigGroepsWerkJaarID AS INT;
		DECLARE @testVorigWerkJaar AS INT;						SET @testVorigWerkJaar=2008;

-- 3° De ChiroGroep heeft een eigen gedefinieerde namen voor een afdeling:
--    'Unittestjes' genaamd, met afkoring 'UK'
		DECLARE @testAfdeling1ID AS INT;
		DECLARE @testAfdelingsNaam1 AS VARCHAR(50);				SET @testAfdelingsNaam1 ='Unittestjes';
		DECLARE @testAfdelingsAfkorting1 AS VARCHAR(10);		SET @testAfdelingsAfkorting1='UT';
		
--    En heeft een afdeling Speelkwi's met afkoring 'SK'
		DECLARE @testAfdeling2ID AS INT;
		DECLARE @testAfdelingsNaam2 AS VARCHAR(50);				SET @testAfdelingsNaam2 ='Speelkwi''s';
		DECLARE @testAfdelingsAfkorting2 AS VARCHAR(10);		SET @testAfdelingsAfkorting2='SK';

-- 4° De eerste afdeling, Unittestjes, mappen we met een officiele Chiro afdeling
--    namelijk met de Ribbels.
--    Dit doen we voor Huidig Werkjaar en Vorig Werkjaar, en geven telkens de start en eind van de geboorte jaren.

        DECLARE @testOfficieleAfdelingID1 AS INT; 
		DECLARE @testOfficieleAfdelingNaam1 AS VARCHAR(50);		SET @testOfficieleAfdelingNaam1 = 'Ribbels';

		DECLARE @testHuidigGeboorteJaarVanAfdelingID1 AS INT;	SET @testHuidigGeboorteJaarVanAfdelingID1=2003;
		DECLARE @testHuidigGeboorteJaarTotAfdelingID1 AS INT;	SET @testHuidigGeboorteJaarTotAfdelingID1=2004;
		
		DECLARE @testVorigGeboorteJaarVanAfdelingID1 AS INT;	SET @testVorigGeboorteJaarVanAfdelingID1=2003;
		DECLARE @testVorigGeboorteJaarTotAfdelingID1 AS INT;	SET @testVorigGeboorteJaarTotAfdelingID1=2004;

-- 5° We maken nu een aantal testpersonen aan, ze zijn allen famillie van elkaar (eg: zelfde Naam):
--    (Noot: Ook zelfde geboortedatum en geslacht) 

		DECLARE @testPersoon1ID AS INT;
		DECLARE @testPersoon1Naam AS VARCHAR(160);				SET @testPersoon1Naam = 'Bosmans';
		DECLARE @testPersoon1VoorNaam AS VARCHAR(60);			SET @testPersoon1VoorNaam = 'Jos';
		DECLARE @testPersoon1GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon1GeboorteDatum = '30/11/1959';
		DECLARE @testPersoon1IsMan AS INT;						SET @testPersoon1IsMan=1;
		

		DECLARE @testPersoon2ID AS INT;
		DECLARE @testPersoon2Naam AS VARCHAR(160);				SET @testPersoon2Naam = @testPersoon1Naam
		DECLARE @testPersoon2VoorNaam AS VARCHAR(60);			SET @testPersoon2VoorNaam = 'Irène';
		DECLARE @testPersoon2GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon2GeboorteDatum = '30/11/2003';
		DECLARE @testPersoon2IsMan AS BIT;						SET @testPersoon2IsMan=1;
		
		DECLARE @testPersoon3ID AS INT;
		DECLARE @testPersoon3Naam AS VARCHAR(160);				SET @testPersoon3Naam = @testPersoon1Naam
		DECLARE @testPersoon3VoorNaam AS VARCHAR(60);			SET @testPersoon3VoorNaam = 'Eugène';
		DECLARE @testPersoon3GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon3GeboorteDatum = '30/11/1959';
		DECLARE @testPersoon3IsMan AS BIT;						SET @testPersoon3IsMan=1;

-- 6° We maken de aangemaakte testpersonen lid van de zelfde Groep, (Unittestjes), 
--    Alle test personen zitten in de juiste leeftijdsgroep.

		DECLARE @testGelieerdePersoon1ID AS INT;
		DECLARE @testPersoon1ChiroLeeftijd AS INT;				SET @testPersoon1ChiroLeeftijd=0;
		DECLARE @testGelieerdePersoon2ID AS INT;
		DECLARE @testPersoon2ChiroLeeftijd AS INT;				SET @testPersoon2ChiroLeeftijd=0;
		DECLARE @testGelieerdePersoon3ID AS INT;
		DECLARE @testPersoon3ChiroLeeftijd AS INT;				SET @testPersoon3ChiroLeeftijd=0;

-- 7° De ChiroGroep (Unittestjes) definieerd ook een aantal categorieen (persoons)

		DECLARE @testCategorie1ID AS INT;
		DECLARE @testCategorie1Naam AS VARCHAR(80);				SET @testCategorie1Naam= 'Vervelende mensen';
		DECLARE @testCategorie1Code AS VARCHAR(10);				SET @testCategorie1Code = 'last';

		DECLARE @testCategorie2ID AS INT;
		DECLARE @testCategorie2Naam AS VARCHAR(80);				SET @testCategorie2Naam= 'Peulengaleis';		
		DECLARE @testCategorie2Code AS VARCHAR(10); 			SET @testCategorie2Code = 'peulen';

-- 8° We gaan de persoons categorien bevolken:
--       					testCategorie1ID  	testCategorie2ID
--       testPersoon1ID 			JA					JA
--       testPersoon2ID									JA
--       testPersoon3ID									JA

-- 9° Leden / Leiding aanmaken: 
--    We gaan Persoon3 (Gelieerde) lid maken en is tevens leiding.		
		DECLARE @testLid3ID AS INT;
		DECLARE @testLid3NonActief AS BIT;						SET @testLid3NonActief = 0;
		DECLARE	@testLid3Verwijderd	AS BIT;						SET @testLid3Verwijderd	= 0;
		DECLARE @testLid3LidGeldBetaald AS BIT;					SET @testLid3LidGeldBetaald = 0;
		DECLARE @testLid3VolgendWerkjaar AS SMALLINT;			SET @testLid3VolgendWerkjaar = 0;

-- 10° We gaan een aantal users maken, 2 users: Yvonne en Yvette.
--       
		DECLARE @testGav1ID AS INT;
		DECLARE @testGav1Login AS VARCHAR(40);					SET @testGav1Login = 'Yvonne';
		DECLARE @testGav2ID AS INT;
		DECLARE @testGav2Login AS VARCHAR(40);					SET @testGav2Login='Yvette';

-- 11° Toekennen van gebruikersrechten.
--     Enkel Yvonne (Gav1ID) krijgt toegang tot Unittestjes ChiroGroep.



DECLARE @testAfdelingsJaarID AS INT;
DECLARE @testVorigAfdelingsJaarID AS INT;





--
-- Ophalen van gegevens die standaard in de database zitten
--

-- Officiele afdeling horende bij afdeling bepaald door AFDELINGID
SET @testOfficieleAfdelingID1 = (SELECT officieleAfdelingID FROM lid.OfficieleAfdeling WHERE Naam=@testOfficieleAfdelingNaam1)


---
--- 1°: Groep en Chirogroep creeren
---
IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@testGroepCode)
BEGIN
	INSERT INTO grp.Groep(Naam, Code)
	VALUES(@testGroepNaam, @testGroepCode);

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
--- 2°: Groepswerkjaren creeren 
---
IF NOT EXISTS (SELECT 1 FROM grp.GroepsWerkJaar WHERE GroepID = @testGroepID AND WerkJaar = @testHuidigWerkJaar)
BEGIN
	INSERT INTO grp.GroepsWerkJaar(WerkJaar, GroepID)
	VALUES (@testHuidigWerkJaar, @testGroepID);

	SET @testGroepsHuidigWerkJaarID = scope_identity();
END
ELSE
BEGIN
	SET @testGroepsHuidigWerkJaarID = (SELECT GroepsWerkJaarID FROM grp.GroepsWerkJaar WHERE GroepID = @testGroepID AND WerkJaar = @testHuidigWerkJaar)
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
--- 3° Afdelingen maken 
---
IF NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsAfkorting1)
BEGIN
	INSERT INTO lid.Afdeling(GroepID, AfdelingsNaam, Afkorting)
	VALUES(@testGroepID, @testAfdelingsNaam1, @testAfdelingsAfkorting1);
	SET @testAfdeling1ID = scope_identity();
END
ELSE
BEGIN
	SET @testAfdeling1ID = (SELECT AfdelingID FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsAfkorting1)
END

IF NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsAfkorting2)
BEGIN
	INSERT INTO lid.Afdeling(GroepID, AfdelingsNaam, Afkorting)
	VALUES(@testGroepID, @testAfdelingsNaam2, @testAfdelingsAfkorting2);
	SET @testAfdeling2ID = scope_identity();
END
ELSE
BEGIN
	SET @testAfdeling2ID = (SELECT AfdelingID FROM lid.Afdeling WHERE groepID = @testGroepID AND afkorting = @testAfdelingsAfkorting2)
END


---
--- 4° AfdelingsJaar maken voor testafdeling1
---

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsHuidigWerkJaarID AND AfdelingID = @testAfdeling1ID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID)
	VALUES (@testHuidigGeboorteJaarVanAfdelingID1, @testHuidigGeboorteJaarTotAfdelingID1, @testAfdeling1ID, @testGroepsHuidigWerkJaarID, @testOfficieleAfdelingID1);
	SET @testAfdelingsJaarID = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SET @testAfdelingsJaarID = (SELECT AfdelingsJaarID FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsHuidigWerkJaarID AND AfdelingID = @testAfdeling1ID)
END

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testVorigGroepsWerkJaarID AND AfdelingID = @testAfdeling1ID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID)
	VALUES (@testVorigGeboorteJaarVanAfdelingID1, @testVorigGeboorteJaarTotAfdelingID1, @testAfdeling1ID, @testVorigGroepsWerkJaarID, @testOfficieleAfdelingID1);
	SET @testVorigAfdelingsJaarID = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SET @testVorigAfdelingsJaarID = (SELECT AfdelingsJaarID FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testVorigGroepsWerkJaarID AND AfdelingID = @testAfdeling1ID)
END

---
--- 5° Testpersonen maken 
---

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoon1Naam AND VoorNaam = @testPersoon1VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoon1Naam, @testPersoon1VoorNaam, @testPersoon1GeboorteDatum, @testPersoon1IsMan)
	SET @testPersoon1ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon1ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoon1Naam AND VoorNaam = @testPersoon1VoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoon2Naam AND VoorNaam = @testPersoon2VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoon2Naam, @testPersoon2VoorNaam, @testPersoon2GeboorteDatum, @testPersoon2IsMan)
	SET @testPersoon2ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon2ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoon2Naam AND VoorNaam = @testPersoon2VoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoon3Naam AND VoorNaam = @testPersoon3VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoon3Naam, @testPersoon3VoorNaam, @testPersoon3GeboorteDatum, @testPersoon3IsMan)
	SET @testPersoon3ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon3ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoon3Naam AND VoorNaam = @testPersoon3VoorNaam);
END;

---
--- 6° Personen lieren aan testgroep 
---

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon1ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon1ID, @testGroepID, @testPersoon1ChiroLeeftijd);
	SET @testGelieerdePersoon1ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoon1ID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon1ID AND GroepID = @testGroepID)
END

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon2ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon2ID, @testGroepID, @testPersoon2ChiroLeeftijd);
	SET @testGelieerdePersoon2ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoon2ID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon2ID AND GroepID = @testGroepID)
END

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon3ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon3ID, @testGroepID, @testPersoon3ChiroLeeftijd);
	SET @testGelieerdePersoon3ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoon3ID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon3ID AND GroepID = @testGroepID)
END

--
-- 7° TestCategorieen maken voor testgroep 1
--

IF NOT EXISTS (SELECT 1 FROM core.Categorie WHERE Code=@testCategorie1Code)
BEGIN
	INSERT INTO core.Categorie(Naam, Code, GroepID) VALUES(@testCategorie1Naam , @testCategorie1Code, @testGroepID);
	SET @testCategorie1ID = scope_identity();
END
ELSE
BEGIN
	SET @testCategorie1ID = (SELECT CategorieID FROM core.Categorie WHERE Code=@testCategorie1Code)
END

IF NOT EXISTS (SELECT 1 FROM core.Categorie WHERE Code=@testCategorie2Code)
BEGIN
	INSERT INTO core.Categorie(Naam, Code, GroepID) VALUES(@testCategorie2Naam , @testCategorie2Code, @testGroepID);
	SET @testCategorie2ID = scope_identity();
END
ELSE
BEGIN
	SET @testCategorie2ID = (SELECT CategorieID FROM core.Categorie WHERE Code=@testCategorie2Code)
END

--
-- 8° Testcategorieen bevolken
--

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoon1ID AND CategorieID=@testCategorie1ID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoon1ID, @testCategorie1ID);
END;

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoon1ID AND CategorieID=@testCategorie2ID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoon1ID, @testCategorie2ID);
END;

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoon2ID AND CategorieID=@testCategorie2ID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoon2ID, @testCategorie2ID);
END;

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsCategorie WHERE GelieerdePersoonID=@testGelieerdePersoon3ID AND CategorieID=@testCategorie2ID)
BEGIN
	INSERT INTO pers.PersoonsCategorie(GelieerdePersoonID, CategorieID) VALUES(@testGelieerdePersoon3ID, @testCategorie2ID);
END;


-- 9° Leden maken
--

IF NOT EXISTS (SELECT 1 FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon3ID AND GroepsWerkJaarID = @testGroepsHuidigWerkJaarID)
BEGIN
	INSERT INTO lid.Lid(GelieerdePersoonID, GroepsWerkJaarID, NonActief, Verwijderd, LidGeldBetaald, VolgendWerkJaar) 
		VALUES (@testGelieerdePersoon3ID, @testGroepsHuidigWerkJaarID, @testLid3NonActief, @testLid3Verwijderd, @testLid3LidGeldBetaald, @testLid3VolgendWerkjaar)
	SET @testLid3ID = scope_identity();
END
ELSE
BEGIN
	SET @testLid3ID = (SELECT LidID FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon3ID AND GroepsWerkJaarID = @testGroepsHuidigWerkJaarID)
END

IF NOT EXISTS (SELECT 1 FROM lid.Leiding WHERE LeidingID = @testLid3ID)
BEGIN
	INSERT INTO lid.Leiding(LeidingID) VALUES (@testLid3ID)
END


-- 
-- 10° Users creeren 
--

IF NOT EXISTS (SELECT 1 FROM auth.GAV WHERE Login=@testGav1Login)
BEGIN
	INSERT INTO auth.GAV(Login) VALUES(@testGav1Login);
	SET @testGav1ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGav1ID =  (SELECT GavID FROM auth.GAV WHERE Login=@testGav1Login)
END

IF NOT EXISTS (SELECT 1 FROM auth.GAV WHERE Login=@testGav2Login)
BEGIN
	INSERT INTO auth.GAV(Login) VALUES(@testGav2Login);
END
BEGIN
	SET @testGav2ID =  (SELECT GavID FROM auth.GAV WHERE Login=@testGav2Login)
END

--
-- 11° Gebruikersrecht
--

IF NOT EXISTS (SELECT 1 FROM auth.GebruikersRecht WHERE GavID=@testGav1ID AND GroepID=@testGroepID)
BEGIN
	INSERT INTO auth.GebruikersRecht(GavID, GroepID) VALUES(@testGav1ID, @testGroepID)
END


PRINT '-'
PRINT 'Voor je deze database kan gebruiken moet je de volgende file update met de gegevens hier onder'
PRINT 'FILE: source:trunk\Solution\TestProjecten\Chiro.Gap.Data.Test\TestInfo.cs'
PRINT 'aan te passen constanten: '
PRINT '-'
PRINT 'public const int GROEPID = ' + CAST(@testGroepID AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOONID = ' + CAST(@testGelieerdePersoon1ID AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOON2ID = ' + CAST(@testGelieerdePersoon2ID  AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOON3ID = ' + CAST(@testGelieerdePersoon3ID  AS VARCHAR(10)) + ';';
DECLARE @GELPERS AS INT;
SET @GELPERS = (SELECT count(*) from pers.GelieerdePersoon where GroepID = @testGroepID);
PRINT 'public const int MINAANTALGELPERS = '+ CAST(@GELPERS AS VARCHAR(10)) + ';';
PRINT 'public const int LID3ID = ' + CAST(@testLid3ID AS VARCHAR(10)) + ';';
PRINT 'public const int CATEGORIEID = ' + CAST(@testCategorie1ID AS VARCHAR(10)) + ';';
PRINT 'public const int CATEGORIE2ID = ' + CAST(@testCategorie2ID AS VARCHAR(10)) + ';';
PRINT 'public const int AANTALINCATEGORIE = 1;'
PRINT 'public const int AFDELINGID = ' + CAST(@testAfdeling1ID AS VARCHAR(10)) + ';';
PRINT 'public const int OFFICIELEAFDELINGID = ' + CAST(@testOfficieleAfdelingID1 AS VARCHAR(10)) + ';';
PRINT 'public const int AFDELINGSJAARID = ' + CAST(@testAfdelingsJaarID AS VARCHAR(10)) + ';';
PRINT 'public const int AFDELING2ID = ' + CAST(@testAfdeling2ID AS VARCHAR(10)) + ';';
PRINT 'public const int AFDELING2VAN = 2001;'
PRINT 'public const int AFDELING2TOT = 1998;'
PRINT 'public const int GROEPSWERKJAARID = ' + CAST(@testGroepsHuidigWerkJaarID AS VARCHAR(10)) + ';';
PRINT 'public const string ZOEKNAAM = "' + CAST(@testPersoon1Naam AS VARCHAR(10)) + '";'; 
PRINT 'public const string GAV1 = "' + CAST(@testGav1Login AS VARCHAR(10)) + '";';
PRINT 'public const string GAV2 = ' + CAST(@testGav2Login AS VARCHAR(10)) + '";';
PRINT '-'
PRINT 'Blijkbaar wordt het onderstaande niet meer gebruikt: '
PRINT '-'
PRINT 'TestVorigGroepsWerkJaarID: ' + CAST(@testVorigGroepsWerkJaarID AS VARCHAR(10)) + ';';
PRINT 'TestVorigAdfelingsJaarID: ' + CAST(@testVorigAfdelingsJaarID AS VARCHAR(10)) + ';';
PRINT 'TestPersoonID: ' + CAST(@testPersoon1ID AS VARCHAR(10)) + ';';
PRINT 'TestPersoon2ID: ' + CAST(@testPersoon3ID AS VARCHAR(10)) + ';';
PRINT 'TestPersoon3ID: ' + CAST(@testPersoon3ID AS VARCHAR(10)) + ';';
PRINT 'TestGav1ID: ' + CAST(@testGav1ID AS VARCHAR(10)) + ';';


GO
