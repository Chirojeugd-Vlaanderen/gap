use gap

-- Creeer testgroep voor unittests, met deze testdata:
-- @testHuidigWerkJaar en @testVorigWerkJaar
--
-- Een Groep (en ChiroGroep) met code @testGroepCode
-- Een GroepsWerkJaar in @testHuidigWerkJaar
-- Afdelingen met codes 
--		@testAfdelingsAfkorting1
--		@testAfdelingsAfkorting2
--		@testAfdelingsAfkorting3 - **deze mag niet actief zijn in het huidig werkjaar**
-- AfdelingsWerkJaar en voor afdeling met code @testAfdelingsAfkorting1 in @testHuidigWerkJaar en @testVorigWerkJaar
--
-- 5 test GelieerdePersonen
--     Gegevens in @testPersoonVoorNaami @testPersoonNaami @testPersoonGeboorteDatum, @testPersoonGeslacht
--
--   GelieerdePersoon1 is geen lid, en zit in Categorie1, en Categorie 2
--   GelieerdePersoon2 is ook geen lid, zit in Categorie2.
--   GelieerdePersoon3 is leiding van Afdeling1 en Afdeling2, 
--     zit in Categorie2 en heeft functies contactpersoon en eindredactie boekje
--   GelieerdePersoon4 is ook leiding, met functie 'eindredactie boekje'.  Was ook leiding
--	   in vorig werkjaar.
--   GelieerdePersoon5 is lid bij de 'unittestjes'.
--
-- GelieerdePersoon 3  heeft een voorkeurs(post)adres, e-mailadres, maar geen telefoonnummer
--					   en nog een ander postadres, maar dan zonder voorkeur
--
-- GelieerdePersoon 4  heeft geen voorkeurs(post)adres, maar wel een e-mailadres en een telefoonnummer
--
-- 2 test-GAV's, en maakt de eerste GAV van testgroep1.
--
-- Datums worden in gegeven in het formaat DD/MM/YYYY
-- We definiteren dit hier (in de documentatie) omdat hier beneden we het gebruiken.
SET DateFormat dmy;

		DECLARE @contactPersoonCode AS VARCHAR(5);				SET @contactPersoonCode='CP';

-- Dit script doet het volgende: 
--

-- 0  Creeren van een testgewest

		DECLARE @testGewestID AS INT;
		DECLARE @testGewestCode AS CHAR(10); 					SET @testGewestCode='tst/0000';
		DECLARE @testGewestNaam AS VARCHAR(160);				SET @testGewestNaam='Gewest Test';


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
		DECLARE @testAfdelingsJaar1ID AS INT;
		
--    En heeft een afdeling Speelkwi's met afkoring 'SK'
		DECLARE @testAfdeling2ID AS INT;
		DECLARE @testAfdelingsNaam2 AS VARCHAR(50);				SET @testAfdelingsNaam2 ='Speelkwi''s';
		DECLARE @testAfdelingsAfkorting2 AS VARCHAR(10);		SET @testAfdelingsAfkorting2='SK';
		DECLARE @testAfdelingsJaar2ID AS INT;

--     De afdeling 'onbestaandjes'

		DECLARE @testAfdeling3ID AS INT;
		DECLARE @testAfdelingsNaam3 AS VARCHAR(50);				SET @testAfdelingsNaam3 ='Onbestaandjes';
		DECLARE @testAfdelingsAfkorting3 AS VARCHAR(10);		SET @testAfdelingsAfkorting3='OB';

-- 4° afdelingen activeren

        DECLARE @testOfficieleAfdelingID1 AS INT; 
		DECLARE @testOfficieleAfdelingNaam1 AS VARCHAR(50);		SET @testOfficieleAfdelingNaam1 = 'Ribbels';

		DECLARE @testHuidigGeboorteJaarVanAfdelingID1 AS INT;	SET @testHuidigGeboorteJaarVanAfdelingID1=2003;
		DECLARE @testHuidigGeboorteJaarTotAfdelingID1 AS INT;	SET @testHuidigGeboorteJaarTotAfdelingID1=2004;
		
		DECLARE @testVorigGeboorteJaarVanAfdelingID1 AS INT;	SET @testVorigGeboorteJaarVanAfdelingID1=2003;
		DECLARE @testVorigGeboorteJaarTotAfdelingID1 AS INT;	SET @testVorigGeboorteJaarTotAfdelingID1=2004;

        DECLARE @testOfficieleAfdelingID2 AS INT; 
		DECLARE @testOfficieleAfdelingNaam2 AS VARCHAR(50);		SET @testOfficieleAfdelingNaam2 = 'Speelclub';

		DECLARE @testHuidigGeboorteJaarVanAfdelingID2 AS INT;	SET @testHuidigGeboorteJaarVanAfdelingID2=2003;
		DECLARE @testHuidigGeboorteJaarTotAfdelingID2 AS INT;	SET @testHuidigGeboorteJaarTotAfdelingID2=2004;
		
		DECLARE @testVorigGeboorteJaarVanAfdelingID2 AS INT;	SET @testVorigGeboorteJaarVanAfdelingID2=2003;
		DECLARE @testVorigGeboorteJaarTotAfdelingID2 AS INT;	SET @testVorigGeboorteJaarTotAfdelingID2=2004;


-- 5° We maken nu een aantal testpersonen aan, ze zijn allen famillie van elkaar (eg: zelfde Naam):
--    (Noot: Ook zelfde geboortedatum en geslacht) 

		DECLARE @testPersoon1ID AS INT;
		DECLARE @testPersoon1Naam AS VARCHAR(160);				SET @testPersoon1Naam = 'Bosmans';
		DECLARE @testPersoon1VoorNaam AS VARCHAR(60);			SET @testPersoon1VoorNaam = 'Jos';
		DECLARE @testPersoon1GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon1GeboorteDatum = '30/11/1959';
		DECLARE @testPersoon1Geslacht AS INT;						SET @testPersoon1Geslacht=1;
		

		DECLARE @testPersoon2ID AS INT;
		DECLARE @testPersoon2Naam AS VARCHAR(160);				SET @testPersoon2Naam = @testPersoon1Naam
		DECLARE @testPersoon2VoorNaam AS VARCHAR(60);			SET @testPersoon2VoorNaam = 'Irène';
		DECLARE @testPersoon2GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon2GeboorteDatum = '30/11/2003';
		DECLARE @testPersoon2Geslacht AS BIT;						SET @testPersoon2Geslacht=1;
		
		DECLARE @testPersoon3ID AS INT;
		DECLARE @testPersoon3Naam AS VARCHAR(160);				SET @testPersoon3Naam = @testPersoon1Naam
		DECLARE @testPersoon3VoorNaam AS VARCHAR(60);			SET @testPersoon3VoorNaam = 'Eugène';
		DECLARE @testPersoon3GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon3GeboorteDatum = '30/11/1959';
		DECLARE @testPersoon3Geslacht AS BIT;						SET @testPersoon3Geslacht=1;

		DECLARE @testPersoon4ID AS INT;
		DECLARE @testPersoon4Naam AS VARCHAR(160);				SET @testPersoon4Naam = @testPersoon1Naam
		DECLARE @testPersoon4VoorNaam AS VARCHAR(60);			SET @testPersoon4VoorNaam = 'Yvonne';
		DECLARE @testPersoon4GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon4GeboorteDatum = '20/06/1959';
		DECLARE @testPersoon4Geslacht AS BIT;						SET @testPersoon4Geslacht=2;

		DECLARE @testPersoon5ID AS INT;
		DECLARE @testPersoon5Naam AS VARCHAR(160);				SET @testPersoon5Naam = 'De Kleine';
		DECLARE @testPersoon5VoorNaam AS VARCHAR(60);			SET @testPersoon5VoorNaam = 'Benjamin';
		DECLARE @testPersoon5GeboorteDatum AS SMALLDATETIME; 	SET @testPersoon5GeboorteDatum = '20/06/2004';
		DECLARE @testPersoon5Geslacht AS BIT;						SET @testPersoon5Geslacht=2;

-- 6° We maken de aangemaakte testpersonen lid van de zelfde Groep, (Unittestjes), 
--    Alle test personen zitten in de juiste leeftijdsgroep.

		DECLARE @testGelieerdePersoon1ID AS INT;
		DECLARE @testPersoon1ChiroLeeftijd AS INT;				SET @testPersoon1ChiroLeeftijd=0;
		DECLARE @testGelieerdePersoon2ID AS INT;
		DECLARE @testPersoon2ChiroLeeftijd AS INT;				SET @testPersoon2ChiroLeeftijd=0;
		DECLARE @testGelieerdePersoon3ID AS INT;
		DECLARE @testPersoon3ChiroLeeftijd AS INT;				SET @testPersoon3ChiroLeeftijd=0;
		DECLARE @testGelieerdePersoon4ID AS INT;
		DECLARE @testPersoon4ChiroLeeftijd AS INT;				SET @testPersoon4ChiroLeeftijd=0;
		DECLARE @testGelieerdePersoon5ID AS INT;
		DECLARE @testPersoon5ChiroLeeftijd AS INT;				SET @testPersoon5ChiroLeeftijd=0;

-- 7° De ChiroGroep (Unittestjes) definieerd ook een aantal categorieen (persoons)

		DECLARE @testCategorie1ID AS INT;
		DECLARE @testCategorie1Naam AS VARCHAR(80);				SET @testCategorie1Naam= 'Vervelende mensen';
		DECLARE @testCategorie1Code AS VARCHAR(10);				SET @testCategorie1Code = 'last';

		DECLARE @testCategorie2ID AS INT;
		DECLARE @testCategorie2Naam AS VARCHAR(80);				SET @testCategorie2Naam= 'Peulengaleis';		
		DECLARE @testCategorie2Code AS VARCHAR(10); 			SET @testCategorie2Code = 'peulen';

		DECLARE @testCategorie3ID AS INT;
		DECLARE @testCategorie3Naam AS VARCHAR(80);				SET @testCategorie3Naam = 'Roddeltantes';
		DECLARE @testCategorie3Code AS VARCHAR(10);				SET @testCategorie3Code = 'bla';

-- 8° We gaan de persoons categorien bevolken:
--       					testCategorie1ID  	testCategorie2ID	testCategorie3
--       testPersoon1ID 			JA					JA
--       testPersoon2ID									JA
--       testPersoon3ID									JA

-- 9° Leden / Leiding aanmaken: 
--    GelieerdePersoon 3, 4 en 5 worden lid
		DECLARE @testLid3ID AS INT;
		DECLARE @testLid3NonActief AS BIT;						SET @testLid3NonActief = 0;
		DECLARE	@testLid3Verwijderd	AS BIT;						SET @testLid3Verwijderd	= 0;
		DECLARE @testLid3LidGeldBetaald AS BIT;					SET @testLid3LidGeldBetaald = 0;
		DECLARE @testLid3VolgendWerkjaar AS SMALLINT;			SET @testLid3VolgendWerkjaar = 0;

		DECLARE @testLid4ID AS INT;
		DECLARE @testLid4NonActief AS BIT;						SET @testLid4NonActief = 0;
		DECLARE	@testLid4Verwijderd	AS BIT;						SET @testLid4Verwijderd	= 0;
		DECLARE @testLid4LidGeldBetaald AS BIT;					SET @testLid4LidGeldBetaald = 0;
		DECLARE @testLid4VolgendWerkjaar AS SMALLINT;			SET @testLid4VolgendWerkjaar = 0;
		DECLARE @testVorigLid4ID AS INT;

		DECLARE @testLid5ID AS INT;
		DECLARE @testLid5NonActief AS BIT;						SET @testLid5NonActief = 0;
		DECLARE	@testLid5Verwijderd	AS BIT;						SET @testLid5Verwijderd	= 0;
		DECLARE @testLid5LidGeldBetaald AS BIT;					SET @testLid5LidGeldBetaald = 0;
		DECLARE @testLid5VolgendWerkjaar AS SMALLINT;			SET @testLid5VolgendWerkjaar = 0;


-- 9.1 functies
--   We maken een eigen functie: 'redactie boekje'

		DECLARE @testFunctieID AS INT;
		DECLARE @testFunctieMaxAantal AS INT;					SET @testFunctieMaxAantal = 2;
		DECLARE @testFunctieCode AS VARCHAR(5);					SET @testFunctieCode='RED';
		DECLARE @testFunctieNaam AS VARCHAR(80);				SET @testFunctieNaam='Redactie boekje';

-- 10° We gaan een aantal users maken, 2 users: Yvonne en Yvette.
--       
		DECLARE @testGav1ID AS INT;
		DECLARE @testGav1Login AS VARCHAR(40);					SET @testGav1Login = 'Yvonne';
		DECLARE @testGav2ID AS INT;
		DECLARE @testGav2Login AS VARCHAR(40);					SET @testGav2Login='Yvette';

-- 11° Toekennen van gebruikersrechten.
--     Enkel Yvonne (Gav1ID) krijgt toegang tot Unittestjes ChiroGroep.


DECLARE @testVorigAfdelingsJaarID AS INT;


-- 12  toekennen van adressen en communicatievormen

	DECLARE @testAdresID AS INT;		SET @testAdresID = 53;				-- gewoon een testadres
	DECLARE @testAdres2ID AS INT;		SET @testAdres2ID = 55;
	DECLARE @testPersoonsAdresID AS INT;
	DECLARE @testTel AS VARCHAR(40);	SET @testTel = '03-231 07 95';		-- een testtelefoonnr
	DECLARE @testMail AS VARCHAR(80);	SET @testMail = 'info@chiro.be';	-- een testmailadres

--
-- Ophalen van gegevens die standaard in de database zitten
--

-- Officiele afdeling horende bij afdeling bepaald door AFDELINGID
SET @testOfficieleAfdelingID1 = (SELECT officieleAfdelingID FROM lid.OfficieleAfdeling WHERE Naam=@testOfficieleAfdelingNaam1)
SET @testOfficieleAfdelingID2 = (SELECT officieleAfdelingID FROM lid.OfficieleAfdeling WHERE Naam=@testOfficieleAfdelingNaam2)

---
--- 0  Testgewest maken
---

IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@testGewestCode)
BEGIN
	INSERT INTO grp.Groep(Naam, Code)
	VALUES(@testGewestNaam, @testGewestCode);

	SET @testGewestID = scope_identity();
END
ELSE
BEGIN
	SET @testGewestID = (SELECT GroepID FROM grp.Groep WHERE Code=@testGewestCode)
END

IF NOT EXISTS (SELECT 1 FROM grp.KaderGroep WHERE KaderGroepID = @testGewestID)
BEGIN
	INSERT INTO grp.KaderGroep(KaderGroepID, Niveau) VALUES(@testGewestID, 8);
END


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
	INSERT INTO grp.ChiroGroep(ChiroGroepID, KaderGroepID) VALUES(@testGroepID, @testGewestID);
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
IF NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE ChiroGroepID = @testGroepID AND afkorting = @testAfdelingsAfkorting1)
BEGIN
	INSERT INTO lid.Afdeling(ChiroGroepID, AfdelingsNaam, Afkorting)
	VALUES(@testGroepID, @testAfdelingsNaam1, @testAfdelingsAfkorting1);
	SET @testAfdeling1ID = scope_identity();
END
ELSE
BEGIN
	SET @testAfdeling1ID = (SELECT AfdelingID FROM lid.Afdeling WHERE ChiroGroepID = @testGroepID AND afkorting = @testAfdelingsAfkorting1)
END

IF NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE ChiroGroepID = @testGroepID AND afkorting = @testAfdelingsAfkorting2)
BEGIN
	INSERT INTO lid.Afdeling(ChiroGroepID, AfdelingsNaam, Afkorting)
	VALUES(@testGroepID, @testAfdelingsNaam2, @testAfdelingsAfkorting2);
	SET @testAfdeling2ID = scope_identity();
END
ELSE
BEGIN
	SET @testAfdeling2ID = (SELECT AfdelingID FROM lid.Afdeling WHERE ChiroGroepID = @testGroepID AND afkorting = @testAfdelingsAfkorting2)
END

IF NOT EXISTS (SELECT 1 FROM lid.Afdeling WHERE ChiroGroepID = @testGroepID AND afkorting = @testAfdelingsAfkorting3)
BEGIN
	INSERT INTO lid.Afdeling(ChiroGroepID, AfdelingsNaam, Afkorting)
	VALUES(@testGroepID, @testAfdelingsNaam3, @testAfdelingsAfkorting3);
	SET @testAfdeling2ID = scope_identity();
END
ELSE
BEGIN
	SET @testAfdeling3ID = (SELECT AfdelingID FROM lid.Afdeling WHERE ChiroGroepID = @testGroepID AND afkorting = @testAfdelingsAfkorting3)
END


---
--- 4° AfdelingsJaren
---

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsHuidigWerkJaarID AND AfdelingID = @testAfdeling1ID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID, Geslacht)
	VALUES (@testHuidigGeboorteJaarVanAfdelingID1, @testHuidigGeboorteJaarTotAfdelingID1, @testAfdeling1ID, @testGroepsHuidigWerkJaarID, @testOfficieleAfdelingID1, 3);
	SET @testAfdelingsJaar1ID = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SET @testAfdelingsJaar1ID = (SELECT AfdelingsJaarID FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsHuidigWerkJaarID AND AfdelingID = @testAfdeling1ID)
END

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsHuidigWerkJaarID AND AfdelingID = @testAfdeling2ID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID, Geslacht)
	VALUES (@testHuidigGeboorteJaarVanAfdelingID2, @testHuidigGeboorteJaarTotAfdelingID2, @testAfdeling2ID, @testGroepsHuidigWerkJaarID, @testOfficieleAfdelingID2, 3);
	SET @testAfdelingsJaar2ID = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SET @testAfdelingsJaar2ID = (SELECT AfdelingsJaarID FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testGroepsHuidigWerkJaarID AND AfdelingID = @testAfdeling2ID)
END

-- ook een van vorig werkjaar

IF NOT EXISTS (SELECT 1 FROM lid.AfdelingsJaar WHERE GroepsWerkJaarID = @testVorigGroepsWerkJaarID AND AfdelingID = @testAfdeling1ID)
BEGIN
	INSERT INTO lid.AfdelingsJaar(GeboorteJaarVan, GeboorteJaarTot, AfdelingID, GroepsWerkJaarID, OfficieleAfdelingID, Geslacht)
	VALUES (@testVorigGeboorteJaarVanAfdelingID1, @testVorigGeboorteJaarTotAfdelingID1, @testAfdeling1ID, @testVorigGroepsWerkJaarID, @testOfficieleAfdelingID1, 3);
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
	VALUES(@testPersoon1Naam, @testPersoon1VoorNaam, @testPersoon1GeboorteDatum, @testPersoon1Geslacht)
	SET @testPersoon1ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon1ID = (SELECT Min(PersoonID) FROM pers.Persoon WHERE Naam = @testPersoon1Naam AND VoorNaam = @testPersoon1VoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoon2Naam AND VoorNaam = @testPersoon2VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoon2Naam, @testPersoon2VoorNaam, @testPersoon2GeboorteDatum, @testPersoon2Geslacht)
	SET @testPersoon2ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon2ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoon2Naam AND VoorNaam = @testPersoon2VoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoon3Naam AND VoorNaam = @testPersoon3VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoon3Naam, @testPersoon3VoorNaam, @testPersoon3GeboorteDatum, @testPersoon3Geslacht)
	SET @testPersoon3ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon3ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoon3Naam AND VoorNaam = @testPersoon3VoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoon4Naam AND VoorNaam = @testPersoon4VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoon4Naam, @testPersoon4VoorNaam, @testPersoon4GeboorteDatum, @testPersoon4Geslacht)
	SET @testPersoon4ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon4ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoon4Naam AND VoorNaam = @testPersoon4VoorNaam);
END;

IF NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE Naam = @testPersoon5Naam AND VoorNaam = @testPersoon5VoorNaam)
BEGIN
	INSERT INTO pers.Persoon(Naam, VoorNaam, GeboorteDatum, Geslacht)
	VALUES(@testPersoon5Naam, @testPersoon5VoorNaam, @testPersoon5GeboorteDatum, @testPersoon5Geslacht)
	SET @testPersoon5ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testPersoon5ID = (SELECT PersoonID FROM pers.Persoon WHERE Naam = @testPersoon5Naam AND VoorNaam = @testPersoon5VoorNaam);
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

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon4ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon4ID, @testGroepID, @testPersoon4ChiroLeeftijd);
	SET @testGelieerdePersoon4ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoon4ID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon4ID AND GroepID = @testGroepID)
END

IF NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon5ID AND GroepID = @testGroepID)
BEGIN
	INSERT INTO pers.GelieerdePersoon(PersoonID, GroepID, ChiroLeeftijd)
	VALUES (@testPersoon5ID, @testGroepID, @testPersoon5ChiroLeeftijd);
	SET @testGelieerdePersoon5ID = SCOPE_IDENTITY();
END
ELSE
BEGIN
	SET @testGelieerdePersoon5ID=(SELECT GelieerdePersoonID FROM pers.GelieerdePersoon WHERE PersoonID = @testPersoon5ID AND GroepID = @testGroepID)
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

IF NOT EXISTS (SELECT 1 FROM core.Categorie WHERE Code=@testCategorie3Code)
BEGIN
	INSERT INTO core.Categorie(Naam, Code, GroepID) VALUES(@testCategorie3Naam , @testCategorie3Code, @testGroepID);
	SET @testCategorie3ID = scope_identity();
END
ELSE
BEGIN
	SET @testCategorie3ID = (SELECT CategorieID FROM core.Categorie WHERE Code=@testCategorie3Code)
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

-- Gelieerde persoon 3 is dit jaar leiding

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

IF NOT EXISTS (SELECT 1 FROM lid.LeidingInAfdelingsJaar WHERE LeidingID=@testLid3ID AND AfdelingsJaarID=@testAfdelingsJaar1ID)
BEGIN
  INSERT INTO lid.LeidingInAfdelingsJaar(LeidingID, AfdelingsJaarID) VALUES(@testLid3ID, @testAfdelingsJaar1ID)
END

IF NOT EXISTS (SELECT 1 FROM lid.LeidingInAfdelingsJaar WHERE LeidingID=@testLid3ID AND AfdelingsJaarID=@testAfdelingsJaar2ID)
BEGIN
  INSERT INTO lid.LeidingInAfdelingsJaar(LeidingID, AfdelingsJaarID) VALUES(@testLid3ID, @testAfdelingsJaar2ID)
END

-- Gelieerde persoon 4 is dit jaar leiding

IF NOT EXISTS (SELECT 1 FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon4ID AND GroepsWerkJaarID = @testGroepsHuidigWerkJaarID)
BEGIN
	INSERT INTO lid.Lid(GelieerdePersoonID, GroepsWerkJaarID, NonActief, Verwijderd, LidGeldBetaald, VolgendWerkJaar) 
		VALUES (@testGelieerdePersoon4ID, @testGroepsHuidigWerkJaarID, @testLid4NonActief, @testLid4Verwijderd, @testLid4LidGeldBetaald, @testLid4VolgendWerkjaar)
	SET @testLid4ID = scope_identity();
END
ELSE
BEGIN
	SET @testLid4ID = (SELECT LidID FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon4ID AND GroepsWerkJaarID = @testGroepsHuidigWerkJaarID)
END

IF NOT EXISTS (SELECT 1 FROM lid.Leiding WHERE LeidingID = @testLid4ID)
BEGIN
	INSERT INTO lid.Leiding(LeidingID) VALUES (@testLid4ID)
END

-- Gelieerde persoon 4 was ook vorig jaar leiding

IF NOT EXISTS (SELECT 1 FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon4ID AND GroepsWerkJaarID = @testVorigGroepsWerkJaarID)
BEGIN
	INSERT INTO lid.Lid(GelieerdePersoonID, GroepsWerkJaarID, NonActief, Verwijderd, LidGeldBetaald, VolgendWerkJaar) 
		VALUES (@testGelieerdePersoon4ID, @testVorigGroepsWerkJaarID, @testLid4NonActief, @testLid4Verwijderd, @testLid4LidGeldBetaald, @testLid4VolgendWerkjaar)
	SET @testVorigLid4ID = scope_identity();
END
ELSE
BEGIN
	SET @testVorigLid4ID = (SELECT LidID FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon4ID AND GroepsWerkJaarID = @testVorigGroepsWerkJaarID)
END

IF NOT EXISTS (SELECT 1 FROM lid.Leiding WHERE LeidingID = @testVorigLid4ID)
BEGIN
	INSERT INTO lid.Leiding(LeidingID) VALUES (@testVorigLid4ID)
END




IF NOT EXISTS (SELECT 1 FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon5ID AND GroepsWerkJaarID = @testGroepsHuidigWerkJaarID)
BEGIN
	INSERT INTO lid.Lid(GelieerdePersoonID, GroepsWerkJaarID, NonActief, Verwijderd, LidGeldBetaald, VolgendWerkJaar) 
		VALUES (@testGelieerdePersoon5ID, @testGroepsHuidigWerkJaarID, @testLid5NonActief, @testLid5Verwijderd, @testLid5LidGeldBetaald, @testLid5VolgendWerkjaar)
	SET @testLid5ID = scope_identity();
END
ELSE
BEGIN
	SET @testLid5ID = (SELECT LidID FROM lid.Lid WHERE GelieerdePersoonID = @testGelieerdePersoon5ID AND GroepsWerkJaarID = @testGroepsHuidigWerkJaarID)
END

IF NOT EXISTS (SELECT 1 FROM lid.Kind WHERE KindID = @testLid5ID)
BEGIN
  INSERT INTO lid.Kind(KindID, AfdelingsJaarID) VALUES(@testLid5ID, @testAfdeling1ID);
END

-- 
-- 9.1 functies
-- 

-- eigen functie maken

IF NOT EXISTS (SELECT 1 FROM lid.Functie WHERE Code=@testFunctieCode AND GroepID=@testGroepID)
BEGIN
	INSERT INTO lid.Functie(Naam, Code, GroepID, MaxAantal, MinAantal, Niveau, WerkJaarVan, WerkJaarTot)
	VALUES(@testFunctieNaam, @testFunctieCode, @testGroepID, @testFunctieMaxAantal, 0, 6, 0, 0)
	SET @testFunctieID = scope_identity();
END
ELSE
BEGIN
	SET @testFunctieID = (SELECT FunctieID FROM lid.Functie WHERE Code=@testFunctieCode AND GroepID=@testGroepID)
END

-- contactpersoon
IF NOT EXISTS (SELECT 1 FROM lid.LidFunctie lf JOIN lid.Functie f on lf.FunctieID = f.FunctieID WHERE lf.LidID=@testLid3ID AND f.Code=@contactPersoonCode)
BEGIN
	INSERT INTO lid.LidFunctie(LidID, FunctieID)
	SELECT @testLid3ID, FunctieID FROM lid.Functie WHERE Code=@contactPersoonCode
END

-- eigen functie
IF NOT EXISTS (SELECT 1 FROM lid.LidFunctie lf where lf.FunctieID=@testFunctieID and lf.LidID=@testLid3ID)
BEGIN
	INSERT INTO lid.LidFunctie(LidID, FunctieID) VALUES (@testLid3ID, @testFunctieID)
END

IF NOT EXISTS (SELECT 1 FROM lid.LidFunctie lf where lf.FunctieID=@testFunctieID and lf.LidID=@testLid4ID)
BEGIN
	INSERT INTO lid.LidFunctie(LidID, FunctieID) VALUES (@testLid4ID, @testFunctieID)
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


--
-- 12 adressen enz
--

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsAdres WHERE PersoonID=@testPersoon3ID AND AdresID=@testAdresID)
BEGIN
	INSERT INTO pers.PersoonsAdres(PersoonID, AdresID, AdresTypeID) VALUES(@testPersoon3ID, @testAdresID, 1);
	SET @testPersoonsAdresID=SCOPE_IDENTITY()
END
ELSE
BEGIN
	SET @testPersoonsAdresID= (SELECT PersoonsAdresID FROM pers.PersoonsAdres WHERE PersoonID=@testPersoon3ID AND AdresID=@testAdresID)
END

IF NOT EXISTS (SELECT 1 FROM pers.PersoonsAdres WHERE PersoonID=@testPersoon3ID AND AdresID=@testAdres2ID)
BEGIN
	INSERT INTO pers.PersoonsAdres(PersoonID, AdresID, AdresTypeID) VALUES(@testPersoon3ID, @testAdres2ID, 1);
END

UPDATE pers.GelieerdePersoon SET VoorkeursAdresID=@testPersoonsAdresID WHERE GelieerdePersoonID=@testGelieerdePersoon3ID;

IF NOT EXISTS (SELECT 1 FROM pers.CommunicatieVorm WHERE GelieerdePersoonID=@testGelieerdePersoon4ID AND Nummer=@testTel)
BEGIN
	INSERT INTO pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, Nummer, IsVoorOptIn)
	VALUES(@testGelieerdePersoon4ID, 1, @testTel, 0)
END

IF NOT EXISTS (SELECT 1 FROM pers.CommunicatieVorm WHERE GelieerdePersoonID=@testGelieerdePersoon3ID AND Nummer=@testMail)
BEGIN
	INSERT INTO pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, Nummer, IsVoorOptIn)
	VALUES(@testGelieerdePersoon3ID, 2, @testMail, 0)
END

IF NOT EXISTS (SELECT 1 FROM pers.CommunicatieVorm WHERE GelieerdePersoonID=@testGelieerdePersoon4ID AND Nummer=@testMail)
BEGIN
	INSERT INTO pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, Nummer, IsVoorOptIn)
	VALUES(@testGelieerdePersoon4ID, 2, @testMail, 0)
END


PRINT '-'
PRINT 'Voor je deze database kan gebruiken moet je de volgende file update met de gegevens hier onder'
PRINT 'FILE: source:trunk\Solution\TestProjecten\Chiro.Gap.TestDbInfo\TestInfo.cs'
PRINT 'aan te passen constanten: '
PRINT '-'
PRINT 'public const int GROEPID = ' + CAST(@testGroepID AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOONID = ' + CAST(@testGelieerdePersoon1ID AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOON2ID = ' + CAST(@testGelieerdePersoon2ID  AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOON3ID = ' + CAST(@testGelieerdePersoon3ID  AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOON4ID = ' + CAST(@testGelieerdePersoon4ID  AS VARCHAR(10)) + ';';
PRINT 'public const int GELIEERDEPERSOON5ID = ' + CAST(@testGelieerdePersoon5ID  AS VARCHAR(10)) + ';';

DECLARE @GELPERS AS INT;
DECLARE @AantalInCategorie AS INT;

SET @GELPERS = (SELECT count(*) from pers.GelieerdePersoon where GroepID = @testGroepID);
SET @AantalInCategorie = (SELECT COUNT(*) FROM pers.PersoonsCategorie WHERE CategorieID=@testCategorie1ID);

PRINT 'public const int MINAANTALGELPERS = '+ CAST(@GELPERS AS VARCHAR(10)) + ';';
PRINT 'public const int LID3ID = ' + CAST(@testLid3ID AS VARCHAR(10)) + ';';
PRINT 'public const int LID4ID = ' + CAST(@testLid4ID AS VARCHAR(10)) + ';';
PRINT 'public const int CATEGORIEID = ' + CAST(@testCategorie1ID AS VARCHAR(10)) + ';';
PRINT 'public const string CATEGORIECODE = "' + CAST(@testCategorie1Code AS VARCHAR(10)) + '";';
PRINT 'public const int CATEGORIE2ID = ' + CAST(@testCategorie2ID AS VARCHAR(10)) + ';';
PRINT 'public const int CATEGORIE3ID = ' + CAST(@testCategorie3ID AS VARCHAR(10)) + ';';
PRINT 'public const int FUNCTIEID = ' + CAST(@testFunctieID AS VARCHAR(10)) + ';';
PRINT 'public const int AANTALINCATEGORIE = ' + CAST(@aantalInCategorie AS VARCHAR(10)) + ';'
PRINT 'public const int AFDELING1ID = ' + CAST(@testAfdeling1ID AS VARCHAR(10)) + ';';
PRINT 'public const int OFFICIELEAFDELINGID = ' + CAST(@testOfficieleAfdelingID1 AS VARCHAR(10)) + ';';
PRINT 'public const int AFDELINGSJAAR1ID = ' + CAST(@testAfdelingsJaar1ID AS VARCHAR(10)) + ';';
PRINT 'public const int AFDELING1VAN = ' + CAST(@testHuidigGeboorteJaarVanAfdelingID1 AS VARCHAR(10)) + ' ;'
PRINT 'public const int AFDELING1TOT = ' + CAST(@testHuidigGeboorteJaarTotAfdelingID1 AS VARCHAR(10)) + ' ;'
PRINT 'public const int AFDELING2ID = ' + CAST(@testAfdeling2ID AS VARCHAR(10)) + ';';
PRINT 'public const int AFDELINGSJAAR2ID = ' + CAST(@testAfdelingsJaar2ID AS VARCHAR(10)) + ';';
PRINT 'public const int AFDELING2VAN = ' + CAST(@testHuidigGeboorteJaarVanAfdelingID2 AS VARCHAR(10)) + ' ;'
PRINT 'public const int AFDELING2TOT = ' + CAST(@testHuidigGeboorteJaarTotAfdelingID2 AS VARCHAR(10)) + ' ;'
PRINT 'public const int AFDELING3ID = ' + CAST(@testAfdeling3ID AS VARCHAR(10)) + ';';
PRINT 'public const int ADRESID = ' + CAST(@testAdresID AS VARCHAR(10)) + ';';
PRINT 'public const int GROEPSWERKJAARID = ' + CAST(@testGroepsHuidigWerkJaarID AS VARCHAR(10)) + ';';
PRINT 'public const string ZOEKNAAM = "' + CAST(@testPersoon1Naam AS VARCHAR(10)) + '";'; 
PRINT 'public const string GAV1 = "' + CAST(@testGav1Login AS VARCHAR(10)) + '";';
PRINT 'public const string GAV2 = "' + CAST(@testGav2Login AS VARCHAR(10)) + '";';
PRINT '-'
PRINT 'Blijkbaar wordt het onderstaande niet gebruikt in de tests: '
PRINT '-'
PRINT 'TestVorigGroepsWerkJaarID: ' + CAST(@testVorigGroepsWerkJaarID AS VARCHAR(10)) + ';';
PRINT 'TestVorigAdfelingsJaarID: ' + CAST(@testVorigAfdelingsJaarID AS VARCHAR(10)) + ';';
PRINT 'TestPersoonID: ' + CAST(@testPersoon1ID AS VARCHAR(10)) + ';';
PRINT 'TestPersoon2ID: ' + CAST(@testPersoon3ID AS VARCHAR(10)) + ';';
PRINT 'TestPersoon3ID: ' + CAST(@testPersoon3ID AS VARCHAR(10)) + ';';
PRINT 'TestPersoon4ID: ' + CAST(@testPersoon4ID AS VARCHAR(10)) + ';';
PRINT 'TestPersoon5ID: ' + CAST(@testPersoon5ID AS VARCHAR(10)) + ';';
PRINT 'TestGav1ID: ' + CAST(@testGav1ID AS VARCHAR(10)) + ';';


GO
