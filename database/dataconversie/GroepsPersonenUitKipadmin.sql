CREATE PROCEDURE data.spGroepsPersonenUitKipadmin @stamNr VARCHAR(10) AS
-- vult de gegevens van een groep aan met die in Kipadmin.

DECLARE @groepID AS INTEGER
DECLARE @huidigWj AS INTEGER


SET @huidigWj = (SELECT WerkJaar FROM Kipadmin.Dbo.HuidigWerkJaar)

-- Deze SP heeft een dubbelzinnige naam; updatet de groepsinfo als die al bestaat.
exec @groepID = data.spNieuweGroepUitKipadmin @stamNr


--------------------------
-- personen overzetten ---
--------------------------
-- Ik importeer enkel personen uit het recentste werkjaar.

PRINT 'Personen overzetten'
PRINT '-------------------'

PRINT '-> Reeds bestaande personen aanpassen.'
-- reeds bestaande personen updaten
--

-- zowel joinen op ad-nr als op naam, voornaam, geboortedatum


UPDATE pGap
	SET pGap.Naam = p.Naam, 
		pGap.VoorNaam = p.VoorNaam, 
		pGap.GeboorteDatum = p.GeboorteDatum,  -- als dat hier crasht, is dat waarschijnlijk owv slechte groepsgegevens 
		pGap.Geslacht = p.Geslacht,
		pGap.AdNummer = p.AdNr
	FROM KipAdmin.dbo.kipPersoon p JOIN KipadMin.dbo.kipLidLeidKad lk 
			ON (p.AdNr = lk.AdNr)
		JOIN grp.Groep g 
			ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
		LEFT OUTER JOIN pers.Persoon pGap on (pGap.AdNummer = p.AdNr
					OR pGap.Naam = p.Naam COLLATE SQL_Latin1_General_CP1_CI_AS
						AND pGap.VoorNaam = p.VoorNaam COLLATE SQL_Latin1_General_CP1_CI_AS
						AND pGap.GeboorteDatum = p.GeboorteDatum)
		WHERE g.GroepID = @groepID AND lk.WerkJaar = @huidigWj


-- nog niet geimporteerde personen importeren
-- omdat de ad-nrs sowieso geupdatet zijn, volstaat het hier enkel op ad-nr te joinen

PRINT '-> Nog niet geimporteerde personen importeren.'

INSERT INTO pers.Persoon(AdNummer, Naam, VoorNaam, GeboorteDatum, Geslacht)
	SELECT DISTINCT p.AdNr, 
                    p.Naam, 
                    p.VoorNaam, 
		            CASE WHEN p.GeboorteDatum > '01/01/1900' THEN p.GeboorteDatum ELSE null END AS GeboorteDatum,  
                    -- Anders faalt de conversie datetime to smalldatetime
                    p.Geslacht
		FROM KipAdmin.dbo.kipPersoon p JOIN KipadMin.dbo.kipLidLeidKad lk 
				ON p.AdNr = lk.AdNr
			JOIN grp.Groep g 
				ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
		WHERE g.GroepID = @groepID AND lk.WerkJaar = @huidigWj
			AND NOT EXISTS (SELECT 1 FROM pers.Persoon 
										WHERE AdNummer = p.AdNr)

-------------------------------------
--- personen aan groepen koppelen ---
-------------------------------------
--
-- het lijkt me interessant om enkel de personen van het huidige werkjaar 'actief' te koppelen,
-- en de 'oudere' op inactief te zetten.  Momenteel gaat dat nog niet, want er is geen actief-veld
-- voor gelieerde persoon.
--
-- We mogen niet de ID van personen gebruiken, deze kunnen verschillend zijn tussen de 2 databases, 
-- maar het AdNummer is wel uniek voor elke persoon.

PRINT 'Personen aan groep linken.'
PRINT '---------------------------'

-- enkel nog niet geimporteerde personen.
INSERT INTO pers.GelieerdePersoon(GroepID, PersoonID, ChiroLeefTijd)
	SELECT DISTINCT g.GroepID, p.PersoonID, 0
		FROM KipAdmin.dbo.kipLidLeidKad lk JOIN grp.Groep g 
				ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
			JOIN pers.Persoon p 
				ON lk.AdNr = p.AdNummer
		WHERE g.GroepID = @groepID AND lk.WerkJaar = @huidigWj
			AND NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon 
									WHERE GroepID = g.GroepID 
									AND PersoonID = p.PersoonID)

---------------------------------------------
--- probeer relevante adressen te matchen ---
---------------------------------------------
PRINT 'Matchen van Adressen.'
PRINT '---------------------'

-- Adressen die matchen met onze officiele stratenlijst overnemen
-- de andere adressen worden jammer genoeg genegeerd

-- omdat het opvullen van de adressentabel nogal omslachtig is,
-- en we moeten vermijden adressen dubbel toe te voegen, maak ik
-- de adressen in een temporary table, en neem ik daarna de relevante
-- (nog niet bestaande) gegevens over in de definitieve tabel.

CREATE TABLE #Adres(
	Bus VARCHAR(10) NULL,
	HuisNr INT NULL,
	PostCode VARCHAR(10) NULL,
	StraatNaamID INT NOT NULL,
	WoonPlaatsID INT NOT NULL);

-- We kennen in Kipadmin geen Bus noch PostCode.
-- De matching tussen een CRAB AdresID (Adressen bewaard in nieuwe CG2 database, gebaseerd op CRAB)
-- gebeurd via StraatNaam, SubGemeente en PostNr .

INSERT INTO #Adres 
	SELECT distinct NULL AS Bus, 
					CASE kipadmin.dbo.EnkelCijfers(ka.Nr) COLLATE SQL_Latin1_General_CP1_CI_AI WHEN '' THEN NULL ELSE CAST(core.ufnEnkelCijfers(ka.Nr) AS INT) END AS HuisNr,
					NULL AS PostCode, 
					s.StraatNaamID, 
					sg.WoonPlaatsID 
		FROM kipAdmin.dbo.kipAdres ka JOIN adr.StraatNaam s 
				ON ka.Straat COLLATE SQL_Latin1_General_CP1_CI_AI = s.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNummer as varchar(5))
			JOIN adr.WoonPlaats sg 
				ON Gemeente COLLATE SQL_Latin1_General_CP1_CI_AI = sg.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNummer as varchar(5))
			JOIN kipAdmin.dbo.kipWoont kw 
				ON kw.AdresId = ka.AdresId
			JOIN pers.Persoon p 
				ON kw.AdNr = p.AdNummer
			JOIN pers.GelieerdePersoon gp 
				ON p.PersoonID = gp.PersoonID
WHERE gp.GroepID = @groepID 

-- de adressen 
-- Venstraat 48, 2240 Viersel en
-- Venstraat 48, 2240 Zandhoven
-- mogen verschillend in de database zitten, hoewel het eigenlijk hetzelfde
-- adres is (Viersel is deelgemeente van Zandhoven).  De gebruiker mag kiezen
-- of hij fusiegemeente of deelgemeente gebruikt, en het is niet omdat groep A
-- met deelgemeenten werkt, groep B dat ook moet doen.


-- Die adressen invoeren die nog niet bestaan in de database.
INSERT INTO adr.Adres (Bus,HuisNr,PostCode,StraatNaamID,WoonPlaatsID)
	SELECT Bus,
		   HuisNr,
		   PostCode,
		   StraatNaamID,WoonPlaatsID
		FROM #Adres a
		WHERE NOT EXISTS (SELECT 1 FROM adr.Adres 
									WHERE (Bus is null and a.Bus is null or Bus COLLATE SQL_Latin1_General_CP1_CI_AI =a.Bus) 
										AND (HuisNr is null and a.HuisNr is null or HuisNr = a.HuisNr)
										AND StraatNaamID = a.StraatNaamID
										AND WoonPlaatsID = a.WoonPlaatsID)

DROP TABLE #Adres

--------------------------------------------------
--- koppel waar mogelijk personen aan adressen ---
--------------------------------------------------

-- Omdat ik eventueel bestaande koppelingen gemakkelijk wil kunnen updaten,
-- imporeer ik eerst alles in een temporary table.
-- (Update is eigenlijk allen voor adrestype)

-- Kijken of alle AdresTypes die KipAdmin kent ook gekend zijn door CG2, 
-- indien er andere in KipAdmin zitten dan voegen we deze toe in CG2
PRINT 'Controlleren/Toevoegen van AdresTypes'
INSERT INTO pers.AdresType (Omschrijving)
	SELECT distinct Omschrijving 
		FROM kipAdmin.dbo.kipAdresType ka
		WHERE NOT EXISTS (SELECT 1 FROM pers.AdresType cg
									WHERE cg.Omschrijving = ka.Omschrijving COLLATE SQL_Latin1_General_CP1_CI_AI)

	

PRINT 'Koppel waar mogelijk personen aan adressen.'
PRINT '-------------------------------------------'
CREATE TABLE #PersoonsAdres(
	Opmerking TEXT NULL,
	AdresID INT NOT NULL,
	AdresTypeID INT NOT NULL,
	PersoonID INT NOT NULL,
    PRIMARY KEY(PersoonID, AdresID))

PRINT 'Insert in tijdelijke tabel'
INSERT INTO #PersoonsAdres(Opmerking, PersoonId, AdresId, AdresTypeId)
	SELECT distinct '',  p.PersoonID, a.AdresID, max(pat.AdresTypeID)
		FROM kipAdmin.dbo.kipAdres ka JOIN adr.StraatNaam s 
				ON ka.Straat COLLATE SQL_Latin1_General_CP1_CI_AI = s.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNummer as varchar(5))
			JOIN adr.WoonPlaats sg 
				ON Gemeente COLLATE SQL_Latin1_General_CP1_CI_AI = sg.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(sg.PostNummer as varchar(5))
			JOIN kipAdmin.dbo.kipWoont kw 
				ON kw.AdresId = ka.AdresId
			JOIN pers.Persoon p 
				ON kw.AdNr = p.AdNummer
			JOIN adr.Adres a 
				ON a.StraatNaamID = s.StraatNaamID 
					AND a.WoonPlaatsID = sg.WoonPlaatsID 
					AND CAST(kipadmin.dbo.EnkelCijfers(ka.Nr) AS INT) = a.HuisNr
			JOIN pers.GelieerdePersoon gp 
				ON p.PersoonID = gp.PersoonID
			JOIN kipAdmin.dbo.kipAdresType kat
				ON kat.AdresTypeId = kw.AdresTypeID
			JOIN pers.AdresType pat
				ON pat.Omschrijving = kat.Omschrijving COLLATE SQL_Latin1_General_CP1_CI_AI
		WHERE gp.GroepID=@groepID
		GROUP BY p.PersoonId, a.AdresId -- blijkbaar heeft kipadmin personen met meermaals hetzelfde adres, maar dan met een ander type

PRINT 'Update AdresTypeID van gekende personen.'
UPDATE pa 
	SET pa.AdresTypeID = tmp.AdresTypeID
		FROM pers.PersoonsAdres pa JOIN #PersoonsAdres tmp 
			ON pa.PersoonID = tmp.PersoonID 
				AND pa.AdresID = tmp.AdresID

				
INSERT INTO pers.PersoonsAdres(Opmerking, PersoonID, AdresID, AdresTypeID) 
	SELECT Opmerking, PersoonID, AdresID, AdresTypeID 
		FROM #PersoonsAdres tmp
			WHERE NOT EXISTS (SELECT 1 FROM pers.PersoonsAdres 
										WHERE PersoonID = tmp.PersoonID 
											AND AdresID = tmp.AdresID)
									
PRINT 'Verwijder tijdelijke tabel'
DROP TABLE #PersoonsAdres

-- Todo: voorkeursadres 
-- (is een koppeling tussen gelieerdepersoon en persoonsadres, maar die tabel hebben we nog niet gemaakt)

-------------------------------------
--- communicatievormen overzetten ---
-------------------------------------
PRINT 'Communicatievormen overzetten'

-- voeg de nog niet bestaande communicatietypes toe
-- voorlopig niets als voorkeur

-- Kijken of alle communicatie types die KipAdmin kent, gekend zijn door CG2, 
-- indien KipAdmin communciatie types heeft die niet gekend wijn door CG2, dan 
-- voegen we deze toe, met geen validatie en zonder voorbeeld.
-- (Dit moet achteraf manueel worden aangepast.)
 INSERT INTO pers.CommunicatieType (Omschrijving)
	SELECT DISTINCT Omschrijving 
		FROM kipAdmin.dbo.KipContactType kip
			WHERE NOT EXISTS (SELECT 1 FROM pers.CommunicatieType cg
									WHERE cg.Omschrijving = kip.Omschrijving COLLATE SQL_Latin1_General_CP1_CI_AI)




PRINT 'GroepID: ' + CAST(@GroepID AS VARCHAR(10))


