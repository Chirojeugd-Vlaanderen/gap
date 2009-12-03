CREATE PROCEDURE data.spGroepUitKipadmin @stamNr VARCHAR(10) AS
-- vult de gegevens van een groep aan met die in Kipadmin.

DECLARE @groepID AS INTEGER

DECLARE @huidigWj AS INTEGER

SET @huidigWj = (SELECT WerkJaar FROM Kipadmin.Dbo.HuidigWerkJaar)

----------------------
-- groep overzetten --
----------------------

IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@stamNr)
BEGIN
	INSERT INTO grp.Groep(Naam, Code, OprichtingsJaar, WebSite)
	SELECT 
		Naam
		, StamNr AS Code
		, Jr_Aanslui AS OprichtingsJaar
		, lower(HomePage) AS WebSite
	FROM KipAdmin.dbo.Groep g
	WHERE g.StamNr = @stamNr
	SET @groepID = scope_identity();
END
ELSE
BEGIN
	SET @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@stamNr)

	UPDATE dst
	SET dst.Naam = g.Naam, dst.OprichtingsJaar = YEAR(g.StartDatum), dst.WebSite = lower(g.WebSite)
	FROM grp.Groep dst 
	JOIN Kipadmin.grp.Groep g on dst.GroepID = g.GroepID
	JOIN Kipadmin.grp.ChiroGroep cg on dst.GroepID = cg.GroepID
	WHERE g.GroepID = @groepID
END

--------------------------
-- personen overzetten ---
--------------------------

-- reeds bestaande personen updaten

UPDATE pGap
SET pGap.Naam = p.Naam, pGap.VoorNaam = p.VoorNaam, pGap.GeboorteDatum = p.GeboorteDatum, pGap.Geslacht = p.Geslacht
FROM KipAdmin.dbo.kipPersoon p
JOIN KipadMin.dbo.kipLidLeidKad lk ON p.AdNr = lk.AdNr
JOIN grp.Groep g ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN pers.Persoon pGap on pGap.AdNummer = p.AdNr
WHERE g.GroepID = @groepID

-- nog niet geimporteerde personen importeren

INSERT INTO pers.Persoon(AdNummer, Naam, VoorNaam, GeboorteDatum, Geslacht)
SELECT DISTINCT
  p.AdNr AS AdNummer
, p.Naam, p.VoorNaam, p.GeboorteDatum, p.Geslacht
FROM KipAdmin.dbo.kipPersoon p
JOIN KipadMin.dbo.kipLidLeidKad lk ON p.AdNr = lk.AdNr
JOIN grp.Groep g ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
WHERE g.GroepID = @groepID
AND NOT EXISTS (SELECT 1 FROM pers.Persoon WHERE AdNummer = p.AdNr)

-------------------------------------
--- personen aan groepen koppelen ---
-------------------------------------

-- het lijkt me interessant om enkel de personen van het huidige werkjaar 'actief' te koppelen,
-- en de 'oudere' op inactief te zetten.  Momenteel gaat dat nog niet, want er is geen actief-veld
-- voor gelieerde persoon.

-- enkel nog niet geimporteerde personen.

INSERT INTO pers.GelieerdePersoon(GroepID, PersoonID, ChiroLeefTijd)
SELECT DISTINCT g.GroepID, p.PersoonID, 0
FROM KipAdmin.dbo.kipLidLeidKad lk
JOIN grp.Groep g ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN pers.Persoon p ON lk.AdNr = p.AdNummer
WHERE g.GroepID = @groepID
AND NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE GroepID = g.GroepID AND PersoonID = p.PersoonID)

---------------------------------------------
--- probeer relevante adressen te matchen ---
---------------------------------------------

-- Adressen die matchen met onze officiele stratenlijst overnemen
-- de andere adressen worden jammer genoeg genegeerd

-- omdat het opvullen van de adressentabel nogal omslachtig is,
-- en we moeten vermijden adressen dubbel toe te voegen, maak ik
-- de adressen in een temporary table, en neem ik daarna de relevante
-- (nog niet bestaande) gegevens over in de definitieve tabel.

CREATE TABLE #Adres(
	Bus VARCHAR(10) NOT NULL,
	HuisNr INT NOT NULL,
	PostCode VARCHAR(10) NOT NULL,
	StraatID INT NOT NULL,
	SubgemeenteID INT NOT NULL,
    PRIMARY KEY(StraatID, SubgemeenteID, HuisNr, Bus, PostCode));

INSERT INTO #Adres 
SELECT distinct '' AS Bus, CAST(kipadmin.dbo.EnkelCijfers(ka.Nr) AS INT) as HuisNr, '' AS PostCode, s.StraatID, sg.SubgemeenteID FROM kipAdmin.dbo.kipAdres ka 
JOIN adr.Straat s ON ka.Straat COLLATE SQL_Latin1_General_CP1_CI_AI = s.Naam AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNr as varchar(5))
JOIN adr.Subgemeente sg ON Gemeente COLLATE SQL_Latin1_General_CP1_CI_AI = sg.Naam AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNr as varchar(5))
JOIN kipAdmin.dbo.kipWoont kw ON kw.AdresId = ka.AdresId
JOIN pers.Persoon p ON kw.AdNr = p.AdNummer
JOIN pers.GelieerdePersoon gp ON p.PersoonID = gp.PersoonID
WHERE gp.GroepID = @groepID

-- Het zou moeten zijn zoals hieronder uitgecommentarieerd: de adressen 
-- Venstraat 48, 2240 Viersel als
-- Venstraat 48, 2240 Zandhoven
-- mogen verschillend in de database zitten, hoewel het eigenlijk hetzelfde
-- adres is (Viersel is deelgemeente van Zandhoven).  De gebruiker mag kiezen
-- of hij fusiegemeente of deelgemeente gebruikt, en het is niet omdat groep A
-- met deelgemeenten werkt, groep B dat ook moet doen.

--INSERT INTO adr.Adres (Bus,HuisNr,PostCode,StraatID,SubgemeenteID)
--SELECT * FROM #Adres a
--WHERE NOT EXISTS (SELECT 1 FROM adr.Adres WHERE Bus=a.Bus AND HuisNr = a.HuisNr AND PostCode = a.PostCode AND StraatID = a.StraatID AND SubgemeenteID = a.SubgemeenteID)

-- De constraint AK_Adres moet dus nog aangepast worden; voorlopig is het zo:

INSERT INTO adr.Adres (Bus,HuisNr,PostCode,StraatID,SubgemeenteID)
SELECT Bus,HuisNr,max(PostCode),StraatID,max(SubgemeenteID) FROM #Adres a
WHERE NOT EXISTS (SELECT 1 FROM adr.Adres WHERE Bus=a.Bus AND HuisNr = a.HuisNr AND StraatID = a.StraatID)
GROUP BY StraatID, HuisNr, Bus

DROP TABLE #Adres

--------------------------------------------------
--- koppel waar mogelijk personen aan adressen ---
--------------------------------------------------

-- Omdat ik eventueel bestaande koppelingen gemakkelijk wil kunnen updaten,
-- imporeer ik eerst alles in een temporary table.
-- (Update is eigenlijk allen voor adrestype)

CREATE TABLE #PersoonsAdres(
	Opmerking TEXT NULL,
	AdresID INT NOT NULL,
	AdresTypeID INT NOT NULL,
	PersoonID INT NOT NULL,
    PRIMARY KEY(PersoonID, AdresID))

INSERT INTO #PersoonsAdres(Opmerking, PersoonId, AdresId, AdresTypeId)
SELECT distinct
  '' AS OPmerking
  , p.PersoonID as PersoonID
  , a.AdresID as AdresID
  , kw.AdresTypeID as AdresTypeID
FROM kipAdmin.dbo.kipAdres ka 
JOIN adr.Straat s ON ka.Straat COLLATE SQL_Latin1_General_CP1_CI_AI = s.Naam AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNr as varchar(5))
JOIN adr.Subgemeente sg ON Gemeente COLLATE SQL_Latin1_General_CP1_CI_AI = sg.Naam AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNr as varchar(5))
JOIN kipAdmin.dbo.kipWoont kw ON kw.AdresId = ka.AdresId
JOIN pers.Persoon p ON kw.AdNr = p.AdNummer
JOIN adr.Adres a ON a.StraatID = s.StraatID AND a.SubgemeenteID = sg.SubgemeenteID AND CAST(kipadmin.dbo.EnkelCijfers(ka.Nr) AS INT) = a.HuisNr
JOIN pers.GelieerdePersoon gp ON p.PersoonID = gp.PersoonID
WHERE gp.GroepID=@groepID

UPDATE pa 
SET pa.AdresTypeID = tmp.AdresTypeID
FROM pers.PersoonsAdres pa JOIN #PersoonsAdres tmp ON pa.PersoonID = tmp.PersoonID AND pa.AdresID = tmp.AdresID

INSERT INTO pers.PersoonsAdres(Opmerking, PersoonID, AdresID, AdresTypeID) 
SELECT Opmerking, PersoonID, AdresID, AdresTypeID FROM #PersoonsAdres tmp
WHERE NOT EXISTS (SELECT 1 FROM pers.PersoonsAdres WHERE PersoonID = tmp.PersoonID AND AdresID = tmp.AdresID)

DROP TABLE #PersoonsAdres

-- Todo: voorkeursadres 
-- (is een koppeling tussen gelieerdepersoon en persoonsadres, maar die tabel hebben we nog niet gemaakt)

-------------------------------------
--- communicatievormen overzetten ---
-------------------------------------

-- voeg de nog niet bestaande communicatietypes toe
-- voorlopig niets als voorkeur

INSERT INTO pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, Nummer, Voorkeur)
SELECT gp.GelieerdePersoonID, ci.ContactTypeID, ci.Info, 0 as voorkeur
FROM pers.Persoon p 
JOIN kipadmin.dbo.kipContactInfo ci ON p.AdNummer = ci.AdNr
JOIN pers.GelieerdePersoon gp on p.PersoonID = gp.PersoonID
WHERE gp.GroepID = @GroepID
AND NOT EXISTS (
	SELECT 1 
	FROM pers.CommunicatieVorm cv2 
	JOIN pers.GelieerdePersoon gp2 ON cv2.GelieerdePersoonID = gp2.GelieerdePersoonID
	JOIN pers.Persoon p2 ON gp2.PersoonID = p2.PersoonID
	WHERE p2.AdNummer = p.AdNummer)

-- update voorkeur op basis van kipadmin

UPDATE cv 
SET VoorKeur = CASE ci.VolgNr WHEN 1 THEN 1 ELSE 0 END
FROM pers.CommunicatieVorm cv 
JOIN pers.GelieerdePersoon gp on cv.GelieerdePersoonID = gp.GelieerdePersoonID
JOIN pers.Persoon p ON gp.PersoonID = p.PersoonID
JOIN kipadmin.dbo.kipContactInfo ci ON p.AdNummer = ci.AdNr AND ci.ContactTypeID = cv.CommunicatieTypeID AND ci.Info COLLATE SQL_Latin1_General_CP1_CI_AI = cv.Nummer
WHERE gp.GroepID = @groepID

-- case VolgNr when 1 then 1 else 0 end as voorkeur

----------------------------------------------------------------------
-- voor het gemak meteen een groepswerkjaar maken voor dit werkjaar --
----------------------------------------------------------------------

INSERT INTO grp.GroepsWerkJaar(GroepID, WerkJaar)
SELECT @groepID, wj.WerkJaar
FROM kipadmin.dbo.HuidigWerkJaar wj
WHERE NOT EXISTS (SELECT 1 FROM grp.GroepsWerkJaar WHERE GroepID = @groepID AND WerkJaar = wj.WerkJaar)


PRINT 'GroepID: ' + CAST(@GroepID AS VARCHAR(10))
