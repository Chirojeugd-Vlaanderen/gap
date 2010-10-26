--- Importeer personen leden uit kaderploegen in KIPADMIN
--- Voor kadergroepen importeren we alle leden (kadermedewerkers) ooit

USE gap
GO

-- Stap 1: Maak alle personen ooit in een kaderploeg die nog niet in GAP zitten aan in GAP.
-- Ik kijk enkel naar AD-nummers.  
-- In praktijk gaan er bepaalde personen die ik nu ga aanmaken, al in de database zitten,
-- maar zonder AD-nummer.  Dat is bewust.  Geen AD-nummer => geen updates van adressen.
-- Ik wil vermijden dat bepaalde groepen ineens updates van adressen krijgen omdat
-- een bepaalde persoon plots een ad-nummer krijgt.

INSERT INTO pers.Persoon(AdNummer, Naam, VoorNaam, GeboorteDatum, Geslacht)
SELECT DISTINCT 
	kp.AdNr, 
	kp.Naam, 
	kp.VoorNaam, 
	CASE WHEN kp.GeboorteDatum > '01/01/1900' THEN kp.GeboorteDatum ELSE null END AS GeboorteDatum,
	kp.Geslacht
FROM Kipadmin.dbo.kipPersoon kp
JOIN Kipadmin.lid.Lid kl ON kp.AdNr = kl.AdNr
JOIN Kipadmin.grp.ChiroGroep kcg ON kl.GroepID = kcg.GroepID
WHERE 
	(kcg.Type='G' OR kcg.Type='V') 
	AND kp.VoorNaam <> 'Onbekend'
	AND NOT EXISTS
	(
		SELECT 1 FROM pers.Persoon p WHERE p.AdNummer=kp.AdNr
	)
GO

-- Stap 2: Kaderpersonen aan de juiste (kader)groepen koppelen

INSERT INTO pers.GelieerdePersoon(GroepID, PersoonID, ChiroLeeftijd)
SELECT DISTINCT g.GroepID, p.PersoonID, 0 AS ChiroLeefTijd
FROM Kipadmin.dbo.kipPersoon kp
JOIN Kipadmin.lid.Lid kl ON kp.AdNr = kl.AdNr
JOIN Kipadmin.grp.ChiroGroep kcg ON kl.GroepID = kcg.GroepID
JOIN grp.Groep g on kcg.StamNr = g.code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN pers.Persoon p on kl.AdNr = p.AdNummer
WHERE (kcg.Type='G' OR kcg.Type='V') 
AND NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon WHERE PersoonID = p.PersoonID AND GroepID = g.GroepID)
GO

--
-- Stap 3: Matchen van relevante adressen
--
-- Adressen die matchen met onze officiele stratenlijst overnemen
-- de andere adressen worden jammer genoeg genegeerd
--
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
GO

-- We kennen in Kipadmin geen Bus noch PostCode.
-- De matching tussen een CRAB AdresID (Adressen bewaard in nieuwe CG2 database, gebaseerd op CRAB)
-- gebeurd via StraatNaam, SubGemeente en PostNr .

INSERT INTO #Adres 
	SELECT distinct NULL AS Bus, 
					CASE core.ufnEnkelCijfers(ka.Nr) COLLATE SQL_Latin1_General_CP1_CI_AI WHEN '' THEN NULL ELSE CAST(core.ufnEnkelCijfers(ka.Nr) AS INT) END AS HuisNr,
					NULL AS PostCode, 
					s.StraatNaamID, 
					sg.WoonPlaatsID 
	FROM kipAdmin.dbo.kipAdres ka 
	JOIN adr.StraatNaam s ON ka.Straat COLLATE SQL_Latin1_General_CP1_CI_AI = s.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNummer as varchar(5))
	JOIN adr.WoonPlaats sg ON Gemeente COLLATE SQL_Latin1_General_CP1_CI_AI = sg.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNummer as varchar(5))
	JOIN kipAdmin.dbo.kipWoont kw ON kw.AdresId = ka.AdresId
	JOIN Kipadmin.lid.Lid kl ON kw.AdNr = kl.AdNr
	JOIN Kipadmin.grp.ChiroGroep kcg ON kl.GroepID = kcg.GroepID
	WHERE (kcg.Type='G' OR kcg.Type='V') 
GO


-- de adressen 
-- Venstraat 48, 2240 Viersel en
-- Venstraat 48, 2240 Zandhoven
-- mogen verschillend in de database zitten, hoewel het eigenlijk hetzelfde
-- adres is (Viersel is deelgemeente van Zandhoven).  De gebruiker mag kiezen
-- of hij fusiegemeente of deelgemeente gebruikt, en het is niet omdat groep A
-- met deelgemeenten werkt, groep B dat ook moet doen.

-- Die adressen invoeren die nog niet bestaan in de database.
INSERT INTO adr.Adres (Bus,HuisNr,PostCode,StraatNaamID,WoonPlaatsID)
	SELECT Bus,HuisNr,PostCode,StraatNaamID,WoonPlaatsID
		FROM #Adres a
		WHERE NOT EXISTS (SELECT 1 FROM adr.Adres 
							WHERE (Bus is null and a.Bus is null or Bus COLLATE SQL_Latin1_General_CP1_CI_AI =a.Bus) 
								AND (HuisNr is null and a.HuisNr is null or HuisNr = a.HuisNr)
								AND StraatNaamID = a.StraatNaamID
								AND WoonPlaatsID = a.WoonPlaatsID)
GO

DROP TABLE #Adres
GO

-- Stap 4: Koppel personen aan adressen

CREATE TABLE #PersoonsAdres(
	Opmerking TEXT NULL,
	AdresID INT NOT NULL,
	AdresTypeID INT NOT NULL,
	PersoonID INT NOT NULL,
    PRIMARY KEY(PersoonID, AdresID))
GO

INSERT INTO #PersoonsAdres(Opmerking, PersoonId, AdresId, AdresTypeId)
	SELECT distinct '',  p.PersoonID, a.AdresID, max(pat.AdresTypeID)
	FROM kipAdmin.dbo.kipAdres ka 
	JOIN adr.StraatNaam s ON ka.Straat COLLATE SQL_Latin1_General_CP1_CI_AI = s.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(s.PostNummer as varchar(5))
	JOIN adr.WoonPlaats sg ON Gemeente COLLATE SQL_Latin1_General_CP1_CI_AI = sg.Naam 
					AND ka.PostNr COLLATE SQL_Latin1_General_CP1_CI_AI = cast(sg.PostNummer as varchar(5))
	JOIN kipAdmin.dbo.kipWoont kw ON kw.AdresId = ka.AdresId
	JOIN pers.Persoon p ON kw.AdNr = p.AdNummer
	JOIN adr.Adres a ON a.StraatNaamID = s.StraatNaamID 
					AND a.WoonPlaatsID = sg.WoonPlaatsID 
					AND CAST(core.ufnEnkelCijfers(ka.Nr) AS INT) = a.HuisNr
	JOIN kipAdmin.dbo.kipAdresType kat ON kat.AdresTypeId = kw.AdresTypeID
	JOIN pers.AdresType pat	ON pat.Omschrijving = kat.Omschrijving COLLATE SQL_Latin1_General_CP1_CI_AI
	JOIN Kipadmin.lid.Lid kl ON kw.AdNr = kl.AdNr
	JOIN Kipadmin.grp.ChiroGroep kcg ON kl.GroepID = kcg.GroepID
	WHERE (kcg.Type='G' OR kcg.Type='V') 
	GROUP BY p.PersoonId, a.AdresId -- blijkbaar heeft kipadmin personen met meermaals hetzelfde adres, maar dan met een ander type
GO

INSERT INTO pers.PersoonsAdres(Opmerking, PersoonID, AdresID, AdresTypeID) 
	SELECT Opmerking, PersoonID, AdresID, AdresTypeID 
	FROM #PersoonsAdres tmp
	WHERE NOT EXISTS (SELECT 1 FROM pers.PersoonsAdres 
						WHERE PersoonID = tmp.PersoonID 
						AND AdresID = tmp.AdresID)
GO

									
DROP TABLE #PersoonsAdres

--Stap 5: 'Kies' een voorkeursadres voor de gelieerde personen die er nog geen hebben

UPDATE gp1
SET gp1.VoorkeursAdresID = nieuwAdr.PersoonsAdresID
FROM pers.GelieerdePersoon gp1 JOIN
(
SELECT gp.GelieerdePersoonID, MAX(pa.PersoonsAdresID) AS PersoonsAdresID 
FROM pers.GelieerdePersoon gp
JOIN grp.KaderGroep kg ON gp.GroepID = kg.KaderGroepID
JOIN pers.PersoonsAdres pa ON pa.PersoonID = gp.PersoonID
WHERE gp.VoorKeursAdresID IS NULL
GROUP BY gp.GelieerdePersoonID
) nieuwAdr ON gp1.GelieerdePersoonID = nieuwAdr.GelieerdePersoonID
GO


--Stap 6: Communicatie

-- Telefoonnrs: initieel geen rekening houden met formattering.  Wel op 
-- dubbels controleren via 'enkelcijfers'
-- (eventueel achteraf probleemgevallen fixen)

INSERT INTO pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, Nummer, IsVoorOptIn, IsGezinsGebonden, Voorkeur)
SELECT DISTINCT gp.GelieerdePersoonID, kci.ContactTypeID, kci.Info, 0, 0, 0
FROM Kipadmin.dbo.kipPersoon kp
JOIN Kipadmin.dbo.kipContactInfo kci on kci.AdNr = kp.AdNr
JOIN pers.Persoon p on p.AdNummer = kp.AdNr
JOIN pers.GelieerdePersoon gp on p.PersoonID = gp.PersoonID
JOIN grp.KaderGroep kg on gp.GroepID = kg.KaderGroepID
WHERE kci.ContactTypeID = 1
AND Core.UfnEnkelCijfers(info) <> '' 
AND NOT EXISTS (
	SELECT 1 FROM pers.CommunicatieVorm cv 
	WHERE GelieerdePersoonID = gp.GelieerdePersoonID AND core.UfnEnkelCijfers(nummer)=core.UfnEnkelCijfers(info))
GO

-- E-mail

INSERT INTO pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, Nummer, IsVoorOptIn, IsGezinsGebonden, Voorkeur)
SELECT DISTINCT gp.GelieerdePersoonID, kci.ContactTypeID, kci.Info, 1 - kci.GeenMailings, 0, 0
FROM Kipadmin.dbo.kipPersoon kp
JOIN Kipadmin.dbo.kipContactInfo kci on kci.AdNr = kp.AdNr
JOIN pers.Persoon p on p.AdNummer = kp.AdNr
JOIN pers.GelieerdePersoon gp on p.PersoonID = gp.PersoonID
JOIN grp.KaderGroep kg on gp.GroepID = kg.KaderGroepID
WHERE kci.ContactTypeID = 3
AND ltrim(rtrim(info)) <> '' 
AND NOT EXISTS (
	SELECT 1 FROM pers.CommunicatieVorm cv 
	WHERE GelieerdePersoonID = gp.GelieerdePersoonID AND nummer=info COLLATE SQL_Latin1_General_CP1_CI_AI)
GO


-- Stap 7: Groepswerkjaren aanmaken

INSERT INTO grp.GroepsWerkJaar(GroepID, WerkJaar)
SELECT DISTINCT g.GroepID, kl.WerkJaar
FROM Kipadmin.lid.Lid kl
JOIN Kipadmin.grp.ChiroGroep kcg ON kl.GroepID = kcg.GroepID
JOIN grp.Groep g on kcg.StamNr = g.code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN grp.KaderGroep kg on kg.KaderGroepID = g.GroepID
WHERE kl.WerkJaar < 2010 AND
NOT EXISTS (SELECT 1 FROM grp.GroepsWerkJaar gwj WHERE gwj.GroepID = g.GroepID AND gwj.WerkJaar = kl.WerkJaar)
GO

-- Stap 8: alle leden (kadermedewerkers) overzetten

INSERT INTO lid.Lid(GroepsWerkJaarID, GelieerdePersoonID, IsOvergezet, NonActief, Verwijderd, VolgendWerkJaar)
SELECT DISTINCT gwj.GroepsWerkJaarID, gp.GelieerdePersoonID, 0 as IsOvergezet, 0 as NonActief, 0 as Verwijderd, 0 as VolgendWerkJaar
FROM Kipadmin.dbo.kipPersoon kp
JOIN Kipadmin.lid.Lid kl ON kp.AdNr = kl.AdNr
JOIN Kipadmin.grp.ChiroGroep kcg ON kl.GroepID = kcg.GroepID
JOIN grp.Groep g on kcg.StamNr = g.code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN grp.KaderGroep kg on g.GroepID = kg.KaderGroepID
JOIN pers.Persoon p on kl.AdNr = p.AdNummer
JOIN pers.GelieerdePersoon gp on gp.PersoonID = p.PersoonID and gp.GroepID = kg.KaderGroepID
JOIN grp.GroepsWerkJaar gwj on gwj.GroepID = g.GroepID and gwj.WerkJaar = kl.WerkJaar
WHERE NOT EXISTS (SELECT 1 FROM lid.Lid l WHERE l.GroepsWerkJaarID = gwj.GroepsWerkJaarID AND l.GelieerdePersoonID = gp.GelieerdePersoonID)

INSERT INTO lid.Leiding(LeidingID)
SELECT DISTINCT l.LidID
FROM Kipadmin.dbo.kipPersoon kp
JOIN Kipadmin.lid.Lid kl ON kp.AdNr = kl.AdNr
JOIN Kipadmin.grp.ChiroGroep kcg ON kl.GroepID = kcg.GroepID
JOIN grp.Groep g on kcg.StamNr = g.code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN grp.KaderGroep kg on g.GroepID = kg.KaderGroepID
JOIN pers.Persoon p on kl.AdNr = p.AdNummer
JOIN pers.GelieerdePersoon gp on gp.PersoonID = p.PersoonID and gp.GroepID = kg.KaderGroepID
JOIN grp.GroepsWerkJaar gwj on gwj.GroepID = g.GroepID and gwj.WerkJaar = kl.WerkJaar
JOIN lid.Lid l ON l.GroepsWerkJaarID = gwj.GroepsWerkJaarID AND l.GelieerdePersoonID = gp.GelieerdePersoonID
WHERE NOT EXISTS (SELECT 1 FROM lid.Leiding ld WHERE ld.LeidingID = l.LidID)


