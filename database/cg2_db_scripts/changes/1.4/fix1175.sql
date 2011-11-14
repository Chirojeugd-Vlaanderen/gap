USE [gap]
GO

/****** Object:  StoredProcedure [data].[spChiroGroepenFusioneren]    Script Date: 11/14/2011 15:25:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER procedure [data].[spChiroGroepenFusioneren] (@werkjaar as int, @stamnr1 as varchar(10), @stamnr2 as varchar(10), @fusiestamnr as varchar(10), @naam as varchar(160)) as
-- bedoeling: chirogroepen met stamnummers @stamnr1 en @stamnr2 samenvoegen 
-- afdelingen, leden en nationale functies worden mee overgezet (aparte afdelingen voor oorspronkelijke groepen)
-- TODO: eigen functies en categorieen nog niet
begin



BEGIN TRAN

DECLARE @groepID1 AS INTEGER; SET @groepID1 = (SELECT GroepID FROM grp.Groep WHERE CODE=@StamNr1);
DECLARE @groepID2 AS INTEGER; SET @groepID2 = (SELECT GroepID FROM grp.Groep WHERE CODE=@StamNr2);
DECLARE @groepID AS INTEGER;
DECLARE @groepsWjID AS INTEGER;

--
-- groep maken
--

IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE CODE=@fusiestamnr)
BEGIN
	INSERT INTO grp.Groep (Naam, Code, OprichtingsJaar)
	SELECT @naam AS Naam, @fusieStamNr AS Code, MIN(OprichtingsJaar) FROM grp.Groep WHERE GroepID IN (@groepID1, @groepID2)
	SET @groepID = scope_identity();
END
ELSE
BEGIN
	SET @groepID = (SELECT groepID FROM grp.Groep WHERE CODE=@fusieStamNr);
END

IF NOT EXISTS (SELECT 1 FROM grp.ChiroGroep WHERE ChiroGroepID=@groepID)
BEGIN
	INSERT INTO grp.ChiroGroep(ChiroGroepID, Plaats, KaderGroepID)
	SELECT @groepID, MAX(Plaats), MAX(KaderGroepID) FROM grp.ChiroGroep WHERE ChiroGroepID IN (@groepID1, @groepID2)
END

--
-- groepswerkjaar maken, indien nog niet bestaat
--

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

--
-- Personen lieren aan nieuwe groep
--

INSERT INTO pers.GelieerdePersoon(GroepID, PersoonID, ChiroLeefTijd, VoorkeursAdresID)
SELECT @groepID, PersoonID, MIN(ChiroLeefTijd), MIN(VoorkeursAdresID)
FROM pers.GelieerdePersoon gp1
WHERE gp1.GroepID IN (@groepID1, @groepID2)
AND NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon gp2 WHERE gp2.GroepID=@groepID AND gp2.PersoonID=gp1.PersoonID)
GROUP BY(PersoonID)

--
-- Communicatie overnemen
--

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

---
--- nieuwe afdelingen maken (voodoo)
---

declare @afdelingsInfo TABLE
(
	AfdelingsNaam varchar(50),
	Afkorting varchar(10),
	ChiroGroepID int,
	Geslacht int
)

declare @afdelingsNamen TABLE
(
	ChiroGroepID int,
	OudeNaam varchar(50),
	NieuweNaam varchar(50)
)
	
declare @afdelingsAfkortingen table
(
	ChiroGroepID int,
	OudeAfk varchar(10),
	NieuweAfk varchar(10)
)

insert into @afdelingsInfo
select AfdelingsNaam, Afkorting, ChiroGroepID, Geslacht from lid.Afdeling a
join lid.AfdelingsJaar aj on a.AfdelingID=aj.AfdelingID
join grp.GroepsWerkJaar gwj on gwj.GroepsWerkJaarID=aj.GroepsWerkJaarID
where a.ChiroGroepID in (@groepid1, @groepid2) and gwj.werkjaar=@werkjaar

-- nieuwe afdelingen. Van elke groep worden de afdelingen overgenomen.
-- conflicterende namen worden opgevangen door een ' J'- of ' M'-suffix
-- (als we geslachten kunnen bepalen), anderes gewoon ' 1' of ' 2'

insert into @afdelingsNamen
select ai1.ChiroGroepID, ai1.AfdelingsNaam as OudeNaam,
case when ai2.AfdelingsNaam is null then ai1.Afdelingsnaam
when ai1.Geslacht<>ai2.Geslacht and ai1.Geslacht=1 then ai1.AfdelingsNaam + ' J'
when ai1.Geslacht<>ai2.Geslacht and ai1.Geslacht=2 then ai1.AfdelingsNaam + ' M'
when ai1.Geslacht=ai2.Geslacht and ai1.ChiroGroepID < ai2.ChiroGroepID then ai1.AfdelingsNaam + ' 1'
else ai1.AfdelingsNaam + ' 2' end as NieuweNaam
from @afdelingsInfo ai1 
left outer join @afdelingsinfo ai2 on ai1.AfdelingsNaam=ai2.AfdelingsNaam and ai1.ChiroGroepID <> ai2.ChiroGroepID

-- gelijkaardig voor afkorting

insert into @afdelingsAfkortingen
select ai1.ChiroGroepID, ai1.Afkorting as OudeAfk,
case when ai2.Afkorting is null then ai1.Afkorting
when ai1.Geslacht<>ai2.Geslacht and ai1.Geslacht=1 then ai1.Afkorting + 'J'
when ai1.Geslacht<>ai2.Geslacht and ai1.Geslacht=2 then ai1.Afkorting + 'M'
when ai1.Geslacht=ai2.Geslacht and ai1.ChiroGroepID < ai2.ChiroGroepID then ai1.Afkorting + '1'
else ai1.Afkorting + '2' end as NieuweNaam
from @afdelingsInfo ai1 
left outer join @afdelingsinfo ai2 on ai1.Afkorting=ai2.Afkorting and ai1.ChiroGroepID <> ai2.ChiroGroepID

insert into lid.Afdeling(AfdelingsNaam, Afkorting, ChiroGroepID)
select an.NieuweNaam as AfdelingsNaam, ak.NieuweAfk as Afkorting, @groepid as ChiroGroepID 
from lid.Afdeling a
join @afdelingsNamen an on a.ChiroGroepID=an.ChiroGroepID  and a.AfdelingsNaam=an.OudeNaam
join @afdelingsAfkortingen ak on a.ChiroGroepID=ak.ChiroGroepID and a.Afkorting=ak.OudeAfk
where a.ChiroGroepID in (@groepID1, @groepID2)
and not exists (select 1 from lid.Afdeling a where a.AfdelingsNaam=an.NieuweNaam and a.ChiroGroepID = @groepID) 

-- 
-- afdelingsjaren
--

declare @afdMap table
(
	OudAfdID integer,
	NieuwAfdID integer
);

insert into @afdMap(OudAfdID, NieuwAfdID)
select aOud.AfdelingID as OudAfdID, aNieuw.AfdelingID as NieuwAfdID
from lid.Afdeling aOud
join @afdelingsNamen an on aOud.ChiroGroepID=an.ChiroGroepID  and aOud.AfdelingsNaam=an.OudeNaam
join @afdelingsAfkortingen ak on aOud.ChiroGroepID=ak.ChiroGroepID and aOud.Afkorting=ak.OudeAfk
join lid.Afdeling aNieuw on aNieuw.AfdelingsNaam = an.NieuweNaam and aNieuw.ChiroGroepID = @groepID
where aOud.ChiroGroepID in (@groepID1, @groepID2)

insert into lid.AfdelingsJaar(GeboorteJaarTot, GeboorteJaarVan, AfdelingID, Geslacht, GroepsWerkJaarID, OfficieleAfdelingID)
select aj.GeboortejaarTot, aj.GeboorteJaarVan, am.NieuwAfdID as AfdelingID, aj.Geslacht, @groepswjid as GroepsWerkJaarID, aj.OfficieleAfdelingID
from @afdMap am
join lid.AfdelingsJaar aj on am.OudAfdID = aj.AfdelingID
join grp.GroepsWerkJaar gwj on aj.GroepsWerkJaarID = gwj.GroepsWerkJaarID
where gwj.WerkJaar=2010 and not exists (select 1 from lid.AfdelingsJaar aj2 where aj2.AfdelingID=am.NieuwAfdID and aj2.GroepsWerkJaarID=@groepswjid)


--
-- leden
--

insert into lid.Lid(LidGeldBetaald, NonActief, Verwijderd, VolgendWerkJaar, GroepsWerkJaarID, GelieerdePersoonID, EindeInstapPeriode)
select distinct max(cast(l.LidgeldBetaald as int)), min(cast(l.NonActief as int)), min(cast(l.Verwijderd as int)), 0, @groepswjid as GroepsWerkJaarID, gpNieuw.GelieerdePersoonID, min(l.EindeInstapPeriode)
from lid.Lid l 
join grp.GroepsWerkJaar gwj on l.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join pers.GelieerdePersoon gpOud on l.GelieerdePersoonID = gpOud.GelieerdePersoonID
join pers.GelieerdePersoon gpNieuw on gpOud.PersoonID = gpNieuw.PersoonID and gpNieuw.GroepID=@groepID
where gwj.GroepID in (@groepid1, @groepid2) and gwj.WerkJaar = @werkjaar
and not exists (select 1 from lid.lid l2 where l2.GelieerdePersoonID = gpNieuw.GelieerdePersoonID and l2.GroepsWerkJaarID=@groepswjid)
group by gpNieuw.gelieerdepersoonid

insert into lid.Kind(KindID, AfdelingsJaarID)
select lNieuw.LidID, ajNieuw.AfdelingsJaarID from lid.Kind k
join lid.Lid lOud on k.KindID = lOud.LidID
join grp.GroepsWerkJaar gwj on lOud.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join pers.GelieerdePersoon gpOud on lOud.GelieerdePersoonID = gpOud.GelieerdePersoonID
join pers.GelieerdePersoon gpNieuw on gpNieuw.PersoonID = gpOud.PersoonID and gpNieuw.GroepID = @groepID
join lid.Lid lNieuw on lNieuw.GelieerdePersoonID = gpNieuw.GelieerdePersoonID and lNieuw.GroepsWerkJaarID = @groepswjid
join lid.AfdelingsJaar ajOud on k.AfdelingsJaarID = ajOud.AfdelingsJaarID
join @afdMap am on ajOud.AfdelingID = am.OudAfdID
join lid.AfdelingsJaar ajNieuw on am.NieuwAfdID = ajNieuw.AfdelingID and ajNieuw.GroepsWerkJaarID = @groepswjid
where gwj.GroepID in (@groepid1, @groepid2) and gwj.WerkJaar = @werkjaar
and not exists (select 1 from lid.Kind where KindID = lNieuw.LidID)
and not exists (select 1 from lid.Leiding where LeidingID = lNieuw.LidID)

insert into lid.Leiding(LeidingID)
select distinct lNieuw.LidID from lid.Leiding lei
join lid.Lid lOud on lei.LeidingID = lOud.LidID
join grp.GroepsWerkJaar gwj on lOud.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join pers.GelieerdePersoon gpOud on lOud.GelieerdePersoonID = gpOud.GelieerdePersoonID
join pers.GelieerdePersoon gpNieuw on gpNieuw.PersoonID = gpOud.PersoonID and gpNieuw.GroepID = @groepID
join lid.Lid lNieuw on lNieuw.GelieerdePersoonID = gpNieuw.GelieerdePersoonID and lNieuw.GroepsWerkJaarID = @groepswjid
where gwj.GroepID in (@groepid1, @groepid2) and gwj.WerkJaar = @werkjaar
and not exists (select 1 from lid.Kind where KindID = lNieuw.LidID)
and not exists (select 1 from lid.Leiding where LeidingID = lNieuw.LidID)

-- voorlopig enkel nationaal gedefinieerde functies overnemen

insert into lid.LidFunctie(LidID, FunctieID)
select distinct lNieuw.LidID, lf.FunctieID from lid.LidFunctie lf
join lid.Functie f on lf.FunctieID = f.FunctieID
join lid.Lid lOud on lf.LidID = lOud.LidID
join grp.GroepsWerkJaar gwj on lOud.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join pers.GelieerdePersoon gpOud on lOud.GelieerdePersoonID = gpOud.GelieerdePersoonID
join pers.GelieerdePersoon gpNieuw on gpNieuw.PersoonID = gpOud.PersoonID and gpNieuw.GroepID = @groepID
join lid.Lid lNieuw on lNieuw.GelieerdePersoonID = gpNieuw.GelieerdePersoonID and lNieuw.GroepsWerkJaarID = @groepswjid
where gwj.GroepID in (@groepid1, @groepid2) and gwj.WerkJaar = @werkjaar
and f.GroepID is null
and not exists (select 1 from lid.LidFunctie lf where lf.LidID = lNieuw.LidID and lf.FunctieID = f.FunctieID)

-- leiding in afdelingsjaar

insert into lid.LeidingInAfdelingsJaar(AfdelingsJaarID, LeidingID)
select distinct ajNieuw.AfdelingsJaarID as AfdelingsJaarID, lNieuw.LidID as LeidingID from 
lid.LeidingInAfdelingsJaar laj
join lid.Lid lOud on laj.LeidingID = lOud.LidID
join grp.GroepsWerkJaar gwj on lOud.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join pers.GelieerdePersoon gpOud on lOud.GelieerdePersoonID = gpOud.GelieerdePersoonID
join pers.GelieerdePersoon gpNieuw on gpNieuw.PersoonID = gpOud.PersoonID and gpNieuw.GroepID = @groepID
join lid.Lid lNieuw on lNieuw.GelieerdePersoonID = gpNieuw.GelieerdePersoonID and lNieuw.GroepsWerkJaarID = @groepswjid
join lid.AfdelingsJaar ajOud on laj.AfdelingsJaarID = ajOud.AfdelingsJaarID
join @afdMap am on ajOud.AfdelingID = am.OudAfdID
join lid.AfdelingsJaar ajNieuw on am.NieuwAfdID = ajNieuw.AfdelingID and ajNieuw.GroepsWerkJaarID = @groepswjid
where gwj.GroepID in (@groepid1, @groepid2) and gwj.WerkJaar = @werkjaar
and not exists (select 1 from lid.LeidingInAfdelingsJaar laj2 where laj2.LeidingID = lNieuw.LidID and laj2.AfdelingsJaarID = ajNieuw.AfdelingsJaarID)

-- niet-vervallen gebruikersrecht overzetten

insert into auth.GebruikersRecht(GavID, GroepID, VervalDatum)
select GavID, @groepid as GroepID, max(VervalDatum) as VervalDatum
from auth.GebruikersRecht gr1
where Vervaldatum >= getDate() and groepID in (@groepid1, @groepid2)
and not exists (select 1 from auth.GebruikersRecht gr2 where gr2.GavID=gr1.GavID and gr2.GroepID=@groepID)
group by GavID

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


GO


