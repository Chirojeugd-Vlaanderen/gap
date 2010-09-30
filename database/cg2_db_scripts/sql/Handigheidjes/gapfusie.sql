alter procedure data.spChiroGroepenFusioneren (@werkjaar as int, @stamnr1 as varchar(10), @stamnr2 as varchar(10), @fusiestamnr as varchar(10), @naam as varchar(160)) as
-- bedoeling: chirogroepen met stamnummers @stamnr1 en @stamnr2 samenvoegen 
begin

-- Enkel gelieerde personen overzetten en leeg groepswerkjaar maken.
-- Al de rest moeten ze manueel doen.


BEGIN TRAN

DECLARE @groepID1 AS INTEGER; SET @groepID1 = (SELECT GroepID FROM grp.Groep WHERE CODE=@StamNr1);
DECLARE @groepID2 AS INTEGER; SET @groepID2 = (SELECT GroepID FROM grp.Groep WHERE CODE=@StamNr2);
DECLARE @groepID AS INTEGER;
DECLARE @groepsWjID AS INTEGER;

-- groep maken, indien die nog niet bestaat

IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE CODE=@fusiestamnr)
BEGIN
	INSERT INTO grp.Groep (Naam, Code, OprichtingsJaar)
	SELECT @naam AS Naam, @fusieStamNr AS Code, MIN(OprichtingsJaar) FROM grp.Groep WHERE GroepID IN (@groepID1, @groepID2)
	SET @groepID = scope_identity();

	INSERT INTO grp.ChiroGroep(ChiroGroepID, Plaats)
	SELECT @groepID, MAX(Plaats) FROM grp.ChiroGroep WHERE ChiroGroepID IN (@groepID1, @groepID2)
END
ELSE
BEGIN
	SET @groepID = (SELECT groepID FROM grp.Groep WHERE CODE=@fusieStamNr);
END

-- groepswerkjaar maken, indien nog niet bestaat

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

-- Standaardafdelingsverdeling

exec data.spStandaardAfdelingsVerdeling @groepsWjID

-- Personen lieren aan nieuwe groep

INSERT INTO pers.GelieerdePersoon(GroepID, PersoonID, ChiroLeefTijd, VoorkeursAdresID)
SELECT @groepID, PersoonID, MIN(ChiroLeefTijd), MIN(VoorkeursAdresID)
FROM pers.GelieerdePersoon gp1
WHERE gp1.GroepID IN (@groepID1, @groepID2)
AND NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon gp2 WHERE gp2.GroepID=@groepID AND gp2.PersoonID=gp1.PersoonID)
GROUP BY(PersoonID)

-- Communicatie overnemen


INSERT INTO pers.CommunicatieVorm(Nota, Nummer, CommunicatieTypeID, IsVoorOptIn, IsGezinsGebonden, Voorkeur, GelieerdePersoonID)
SELECT 
	Max(cv.Nota), 
	cv.Nummer, 
	cv.CommunicatieTypeID, 
	CAST(Max(CAST(cv.IsVoorOptIn AS INT)) AS BIT), 
	CAST(Max(CAST(cv.IsGezinsGebonden AS INT)) AS BIT), 
	CAST(Min(CAST(cv.Voorkeur AS INT)) AS BIT), 
	gpNieuw.GelieerdePersoonID
FROM pers.CommunicatieVorm cv 
JOIN pers.GelieerdePersoon gpOud ON cv.GelieerdePersoonID = gpOud.GelieerdePersoonID
JOIN pers.GelieerdePersoon gpNieuw ON gpOud.PersoonID = gpNieuw.PersoonID
WHERE gpOud.GroepID IN (@groepID1, @groepID2) AND gpNieuw.GroepID = @groepID
AND NOT EXISTS 
(SELECT 1 FROM pers.CommunicatieVorm cv2 
WHERE cv2.GelieerdePersoonID = gpNieuw.GelieerdePersoonID 
	AND cv2.Nummer = cv.Nummer 
	AND cv2.CommunicatieTypeID = cv.CommunicatieTypeID)
GROUP BY cv.Nummer, cv.CommunicatieTypeID, gpNieuw.GelieerdePersoonID

-- Selecteer mogelijke dubbels

SELECT p2.* 
FROM pers.Persoon p2
JOIN pers.GelieerdePersoon gp2 on p2.PersoonID = gp2.PersoonID
JOIN
(
SELECT p.Naam, p.Voornaam, count(*) as aantal FROM
pers.Persoon p
WHERE p.PersoonID IN (SELECT PersoonID FROM pers.GelieerdePersoon WHERE GroepID=@groepID)
GROUP BY p.Naam, p.Voornaam
HAVING count(*) > 1
) prob on p2.Naam=prob.Naam and p2.VoorNaam=prob.Voornaam
WHERE gp2.GroepID IN (@groepID1, @groepID2)

PRINT 'GroepID1 ' + CAST(@groepID1 AS VARCHAR(MAX));
PRINT 'GroepID2 ' + CAST(@groepID2 AS VARCHAR(MAX));

PRINT 'GroepID ' + CAST(@groepID AS VARCHAR(MAX));
PRINT 'GroepsWjID ' + CAST(@groepsWjID AS VARCHAR(MAX));

COMMIT TRAN

end
