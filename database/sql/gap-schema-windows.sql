-- Deze export, gemaakt op een windows-server, kan blijkbaar niet
-- zomaar ingelezen worden met Linux.

/****** Object:  DatabaseRole [GapHackRole]    Script Date: 21/02/2017 10:50:15 ******/
CREATE ROLE [GapHackRole]
GO
/****** Object:  DatabaseRole [GapLogRole]    Script Date: 21/02/2017 10:50:15 ******/
CREATE ROLE [GapLogRole]
GO
/****** Object:  DatabaseRole [GapRole]    Script Date: 21/02/2017 10:50:15 ******/
CREATE ROLE [GapRole]
GO
/****** Object:  DatabaseRole [GapSuperRole]    Script Date: 21/02/2017 10:50:15 ******/
CREATE ROLE [GapSuperRole]
GO
/****** Object:  DatabaseRole [KipSyncRole]    Script Date: 21/02/2017 10:50:15 ******/
CREATE ROLE [KipSyncRole]
GO
ALTER ROLE [db_datareader] ADD MEMBER [GapRole]
GO
/****** Object:  Schema [abo]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [abo]
GO
/****** Object:  Schema [adr]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [adr]
GO
/****** Object:  Schema [auth]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [auth]
GO
/****** Object:  Schema [biv]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [biv]
GO
/****** Object:  Schema [core]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [core]
GO
/****** Object:  Schema [data]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [data]
GO
/****** Object:  Schema [diag]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [diag]
GO
/****** Object:  Schema [grp]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [grp]
GO
/****** Object:  Schema [lid]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [lid]
GO
/****** Object:  Schema [logging]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [logging]
GO
/****** Object:  Schema [pers]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [pers]
GO
/****** Object:  Schema [verz]    Script Date: 21/02/2017 10:50:15 ******/
CREATE SCHEMA [verz]
GO
/****** Object:  StoredProcedure [auth].[spGebruikersRechtenOphalenAd]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


CREATE procedure [auth].[spGebruikersRechtenOphalenAd]
(
	@adNr INT
)
AS
-- Doel: gegevens ophalen van de groepen waar de persoon als GAV aan gekoppeld is
BEGIN
	SET NOCOUNT ON;

	SELECT
			g.GroepID
			, Code AS StamNr
			, g.Naam
			, Plaats
			, gr.Vervaldatum
	FROM
			grp.Groep g
			LEFT OUTER JOIN grp.Chirogroep cg
				ON g.GroepID = cg.ChiroGroepID
			INNER JOIN auth.GebruikersrechtV2 gr
				ON g.GroepID = gr.GroepID
			INNER JOIN pers.Persoon gav
				ON gr.PersoonID = gav.PersoonID
	WHERE
			gav.AdNummer = @adNr
			AND gr.GroepsPermissies = 3 AND gr.IedereenPermissies = 3
END


GO
/****** Object:  StoredProcedure [auth].[spGebruikersRechtenPerGroepOphalenAd]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


CREATE procedure [auth].[spGebruikersRechtenPerGroepOphalenAd]
(
	@stamnr varchar(8)
)
AS
-- Doel: Personen ophalen met GAV-rechten op de groep
BEGIN
	SET NOCOUNT ON;

	SELECT
			gav.AdNummer
			, gr.Vervaldatum
	FROM
			grp.Groep g
			INNER JOIN auth.GebruikersrechtV2 gr
				ON g.GroepID = gr.GroepID
			INNER JOIN pers.Persoon gav
				ON gr.PersoonID = gav.PersoonID
	WHERE
			Code = @stamnr
			AND gr.Vervaldatum > dateadd(month, -3, getdate())
			AND gr.GroepsPermissies = 3 AND gr.IedereenPermissies = 3
END

GO
/****** Object:  StoredProcedure [data].[spChiroGroepenFusioneren]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [data].[spChiroGroepenFusioneren] (@werkjaar as int, @stamnr1 as varchar(10), @stamnr2 as varchar(10), @fusiestamnr as varchar(10), @naam as varchar(160)) as
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
	-- SOM-fichebak 'op de hoogte brengen'
	--

	IF NOT EXISTS (SELECT 1 FROM intranet.grp.ChiroGroepEvolutie 
					WHERE OudeGroepStamNr = @stamnr1 AND NieuweGroepStamNr = @fusiestamnr)
	BEGIN
		INSERT INTO intranet.grp.ChiroGroepEvolutie(OudeGroepID, OudeGroepStamNr, NieuweGroepID, NieuweGroepStamNr)
		VALUES(@groepID1, @stamnr1, @groepID, @fusiestamnr)
	END

	IF NOT EXISTS (SELECT 1 FROM intranet.grp.ChiroGroepEvolutie 
					WHERE OudeGroepStamNr = @stamnr2 AND NieuweGroepStamNr = @fusiestamnr)
	BEGIN
		INSERT INTO intranet.grp.ChiroGroepEvolutie(OudeGroepID, OudeGroepStamNr, NieuweGroepID, NieuweGroepStamNr)
		VALUES(@groepID2, @stamnr2, @groepID, @fusiestamnr)
	END

	--
	-- groepswerkjaar maken, als het nog niet bestaat
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
	-- Personen liëren aan nieuwe groep
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

	INSERT INTO pers.CommunicatieVorm(Nota, Nummer, CommunicatieTypeID, IsGezinsGebonden, Voorkeur, GelieerdePersoonID)
	SELECT 
		Max(cv.Nota), 
		cv.Nummer, 
		cv.CommunicatieTypeID, 
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

	insert into lid.Lid(LidGeldBetaald, Verwijderd, VolgendWerkJaar, GroepsWerkJaarID, GelieerdePersoonID, EindeInstapPeriode)
	select distinct max(cast(l.LidgeldBetaald as int)), min(cast(l.Verwijderd as int)), 0, @groepswjid as GroepsWerkJaarID, gpNieuw.GelieerdePersoonID, min(l.EindeInstapPeriode)
	from lid.Lid l 
	join grp.GroepsWerkJaar gwj on l.GroepsWerkJaarID = gwj.GroepsWerkJaarID
	join pers.GelieerdePersoon gpOud on l.GelieerdePersoonID = gpOud.GelieerdePersoonID
	join pers.GelieerdePersoon gpNieuw on gpOud.PersoonID = gpNieuw.PersoonID and gpNieuw.GroepID=@groepID
	where gwj.GroepID in (@groepid1, @groepid2) and gwj.WerkJaar = @werkjaar
	and not exists (select 1 from lid.lid l2 where l2.GelieerdePersoonID = gpNieuw.GelieerdePersoonID and l2.GroepsWerkJaarID=@groepswjid)
	group by gpNieuw.gelieerdepersoonid

	insert into lid.Kind(KindID, AfdelingsJaarID)
	select lNieuw.LidID, max(ajNieuw.AfdelingsJaarID) from lid.Kind k
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
	group by lNieuw.LidID

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
/****** Object:  StoredProcedure [data].[spDubbelePersoonVerwijderen]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [data].[spDubbelePersoonVerwijderen] (@foutPID as int, @juistPID as int) as
-- alle referenties van persoon met @foutPID veranderen naar die van persoon met @juistPID
begin

BEGIN TRAN

-- probeer de gelieerde personen gekoppeld aan de 'verkeerde' persoon
-- te koppelen aan de juiste

UPDATE gp1
SET gp1.PersoonID = @juistPID
FROM pers.GelieerdePersoon gp1 
WHERE gp1.PersoonID = @foutPID
AND NOT EXISTS
(SELECT 1 FROM pers.GelieerdePersoon gp2 
WHERE gp2.PersoonID = @juistPID
AND gp2.GroepID = gp1.GroepID)


-- voor de gelieerde personen waar dat niet lukt, verleggen we
-- de lidobjecten naar een bestaande gelieerde persoon van de
-- juiste persoon

-- om te vermijden dat we straks actieve leden gaan verwijderen,
-- en inactieve leden gaan behouden, verwijderen we eerst alle
-- inactieve leden gekoppeld aan de 'juiste' persoon.
-- (op die manier worden straks eventuele lidobjecten van de foute
-- persoon proper overgezet naar de juiste)

-- (het nadeel is dat gemergede personen die inactief lid waren,
-- mogelijk een nieuwe probeerperiode krijgen als ze opnieuw lid 
-- worden.)

DELETE lf
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.LidFunctie lf on l.LidID = lf.LidID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE k
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Kind k on l.LidID = k.KindID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE lia
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
JOIN lid.LeidingInAfdelingsJaar lia on ld.LeidingID = lia.LeidingID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE ld
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE l
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

-- nu proberen we bestaande lidobjecten van de dubbele
-- persoon over te zetten naar de originele.  Eventuele
-- inactieve lidobjecten voor de originele staan hiervoor
-- nu niet meer in de weg.

UPDATE foutLid
SET foutLid.GelieerdePersoonID = JuisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp ON fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGP.PersoonID = @foutPID AND juisteGP.PersoonID = @juistPID
AND NOT EXISTS
(SELECT 1 FROM lid.Lid l2 
WHERE l2.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND l2.GroepsWerkJaarID = foutLid.GroepsWerkJaarID)

-- Het zou kunnen dat zowel de dubbele als de originele persoon
-- actieve lidobjecten heeft.  In dat geval zijn die van de dubbele
-- nog steeds aan de dubbele gekoppeld.  Heel jammer, maar die
-- van de dubbele worden dan verwijderd.  (incl. afdelingen en functies)

-- Dit stuk code lijkt zeer hard op wat we hierboven doen voor de
-- inactieve leden van de originele persoon.

DELETE lf
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.LidFunctie lf on l.LidID = lf.LidID
WHERE gp.PersoonID = @foutPID

DELETE k
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Kind k on l.LidID = k.KindID
WHERE gp.PersoonID = @foutPID

DELETE lia
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
JOIN lid.LeidingInAfdelingsJaar lia on ld.LeidingID = lia.LeidingID
WHERE gp.PersoonID = @foutPID

DELETE ld
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
WHERE gp.PersoonID = @foutPID

DELETE l
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
WHERE gp.PersoonID = @foutPID


-- Probeer eerst communicatievormen te verleggen naar juiste persoon

UPDATE fouteCv
SET fouteCv.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN pers.CommunicatieVorm fouteCv on fouteCv.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID
AND NOT EXISTS
(SELECT 1 FROM pers.CommunicatieVorm cv
WHERE cv.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND cv.Nummer = fouteCv.Nummer)

-- Diegene die niet verlegd kunnen worden (omdat nummer al bestaat bij juiste GP)
-- worden verwijderd.  Voorkeur, gezinsgebonden,... gaan verloren.

DELETE cv
FROM pers.GelieerdePersoon fouteGp
JOIN pers.CommunicatieVorm cv on fouteGp.GelieerdePersoonID=cv.GelieerdePersoonID
WHERE fouteGp.PersoonID = @foutPID

-- Probeer categorieen te verleggen naar juiste persoon

UPDATE foutePc
SET foutePc.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN pers.PersoonsCategorie foutePc on foutePc.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID
AND NOT EXISTS
(SELECT 1 FROM pers.PersoonsCategorie pc
WHERE pc.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND pc.CategorieID = foutePc.CategorieID)

-- verwijder overgebleven categorieen van foute gelieerde persoon

DELETE pc
FROM pers.GelieerdePersoon fouteGp
JOIN pers.PersoonsCategorie pc on fouteGp.GelieerdePersoonID=pc.GelieerdePersoonID
WHERE fouteGp.PersoonID = @foutPID

-- probeer deelnemers te verleggen

UPDATE fouteDn
SET fouteDn.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN biv.Deelnemer fouteDn on fouteDn.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID

-- foute abonnementen

UPDATE fouteAb
SET fouteAb.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN abo.Abonnement fouteAb on fouteAb.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID

-- op dit moment kunnen de foute gelieerde personen verdwijnen

DELETE FROM pers.GelieerdePersoon
WHERE PersoonID = @foutPID

-- Probeer de adressen te verleggen naar de goede persoon

UPDATE foutPa
SET foutPa.PersoonID = @juistPID
FROM pers.PersoonsAdres foutPa
WHERE foutPa.PersoonID = @foutPID
AND NOT EXISTS
(SELECT 1 FROM pers.PersoonsAdres pa
WHERE pa.PersoonID = @juistPID
AND pa.AdresID = foutPa.AdresID)

-- De adressen die al aan de goede persoon gekoppeld waren, mogen weg van de
-- foute persoon.  We verliezen wel de opmerking

-- eerst nog eens kijken of het te verwijderen persoonsadres geen voorkeursadres
-- is van een gelieerde persoon.  Zo ja: wijzigen

UPDATE gp
SET gp.VoorkeursAdresID = juistePa.PersoonsAdresID
FROM pers.PersoonsAdres foutePa 
JOIN pers.GelieerdePersoon gp on gp.VoorkeursAdresID = foutePa.PersoonsAdresID
JOIN pers.PersoonsAdres juistePa on juistePa.AdresID = foutePa.AdresID
WHERE foutePa.PersoonID=@foutPID AND juistePa.PersoonID = @juistPID 

DELETE FROM pers.PersoonsAdres WHERE PersoonID=@foutPID

-- Persoonsverzekeringen verleggen
-- (Dit gaat wel problemen geven als we op termijn de verzekeringen aan
-- gelieerde personen gaan koppelen)

UPDATE pv
SET pv.PersoonID = @juistPID
FROM verz.PersoonsVerzekering pv
WHERE pv.PersoonID = @foutPID

-- GAV-schap verleggen

UPDATE gs
SET gs.PersoonID = @juistPID
FROM auth.gavSchap gs
WHERE gs.PersoonID = @foutPID

-- We gaan ervan uit dat de goeie informatie in het juiste
-- persoonsrecord zit
DELETE FROM pers.Persoon WHERE PersoonID=@foutPID

COMMIT TRAN

end




GO
/****** Object:  StoredProcedure [data].[spDubbelePersoonVerwijderenGp]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE procedure [data].[spDubbelePersoonVerwijderenGp] (@foutGPID as int, @juistGPID as int) as
-- alle referenties van persoon met GELIEERDEPERSOONID @foutGPID veranderen naar die van 
-- persoon met GELIEERDEPERSOONID @juistGPID
begin

BEGIN TRAN

DECLARE @juistPID as integer;
set @juistPID = (select PersoonID from pers.GelieerdePersoon where GelieerdePersoonID = @juistGPID);
DECLARE @foutPID as integer;
set @foutPID = (select PersoonID from pers.GelieerdePersoon where GelieerdePersoonID = @foutGPID);


-- probeer de gelieerde personen gekoppeld aan de 'verkeerde' persoon
-- te koppelen aan de juiste

UPDATE gp1
SET gp1.PersoonID = @juistPID
FROM pers.GelieerdePersoon gp1 
WHERE gp1.PersoonID = @foutPID
AND NOT EXISTS
(SELECT 1 FROM pers.GelieerdePersoon gp2 
WHERE gp2.PersoonID = @juistPID
AND gp2.GroepID = gp1.GroepID)


-- voor de gelieerde personen waar dat niet lukt, verleggen we
-- de lidobjecten naar een bestaande gelieerde persoon van de
-- juiste persoon

-- om te vermijden dat we straks actieve leden gaan verwijderen,
-- en inactieve leden gaan behouden, verwijderen we eerst alle
-- inactieve leden gekoppeld aan de 'juiste' persoon.
-- (op die manier worden straks eventuele lidobjecten van de foute
-- persoon proper overgezet naar de juiste)

-- (het nadeel is dat gemergede personen die inactief lid waren,
-- mogelijk een nieuwe probeerperiode krijgen als ze opnieuw lid 
-- worden.)

DELETE lf
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.LidFunctie lf on l.LidID = lf.LidID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE k
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Kind k on l.LidID = k.KindID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE lia
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
JOIN lid.LeidingInAfdelingsJaar lia on ld.LeidingID = lia.LeidingID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE ld
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE l
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

-- nu proberen we bestaande lidobjecten van de dubbele
-- persoon over te zetten naar de originele.  Eventuele
-- inactieve lidobjecten voor de originele staan hiervoor
-- nu niet meer in de weg.

UPDATE foutLid
SET foutLid.GelieerdePersoonID = JuisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp ON fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGP.PersoonID = @foutPID AND juisteGP.PersoonID = @juistPID
AND NOT EXISTS
(SELECT 1 FROM lid.Lid l2 
WHERE l2.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND l2.GroepsWerkJaarID = foutLid.GroepsWerkJaarID)

-- Het zou kunnen dat zowel de dubbele als de originele persoon
-- actieve lidobjecten heeft.  In dat geval zijn die van de dubbele
-- nog steeds aan de dubbele gekoppeld.  Heel jammer, maar die
-- van de dubbele worden dan verwijderd.  (incl. afdelingen en functies)

-- Dit stuk code lijkt zeer hard op wat we hierboven doen voor de
-- inactieve leden van de originele persoon.

DELETE lf
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.LidFunctie lf on l.LidID = lf.LidID
WHERE gp.PersoonID = @foutPID

DELETE k
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Kind k on l.LidID = k.KindID
WHERE gp.PersoonID = @foutPID

DELETE lia
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
JOIN lid.LeidingInAfdelingsJaar lia on ld.LeidingID = lia.LeidingID
WHERE gp.PersoonID = @foutPID

DELETE ld
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
WHERE gp.PersoonID = @foutPID

DELETE l
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
WHERE gp.PersoonID = @foutPID


-- Probeer eerst communicatievormen te verleggen naar juiste persoon

UPDATE fouteCv
SET fouteCv.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN pers.CommunicatieVorm fouteCv on fouteCv.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID
AND NOT EXISTS
(SELECT 1 FROM pers.CommunicatieVorm cv
WHERE cv.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND cv.Nummer = fouteCv.Nummer)

-- Diegene die niet verlegd kunnen worden (omdat nummer al bestaat bij juiste GP)
-- worden verwijderd.  Voorkeur, gezinsgebonden,... gaan verloren.

DELETE cv
FROM pers.GelieerdePersoon fouteGp
JOIN pers.CommunicatieVorm cv on fouteGp.GelieerdePersoonID=cv.GelieerdePersoonID
WHERE fouteGp.PersoonID = @foutPID

-- Probeer categorieen te verleggen naar juiste persoon

UPDATE foutePc
SET foutePc.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN pers.PersoonsCategorie foutePc on foutePc.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID
AND NOT EXISTS
(SELECT 1 FROM pers.PersoonsCategorie pc
WHERE pc.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND pc.CategorieID = foutePc.CategorieID)

-- verwijder overgebleven categorieen van foute gelieerde persoon

DELETE pc
FROM pers.GelieerdePersoon fouteGp
JOIN pers.PersoonsCategorie pc on fouteGp.GelieerdePersoonID=pc.GelieerdePersoonID
WHERE fouteGp.PersoonID = @foutPID

-- probeer deelnemers te verleggen

UPDATE fouteDn
SET fouteDn.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN biv.Deelnemer fouteDn on fouteDn.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID

-- foute abonnementen

UPDATE fouteAb
SET fouteAb.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN abo.Abonnement fouteAb on fouteAb.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID

-- op dit moment kunnen de foute gelieerde personen verdwijnen

DELETE FROM pers.GelieerdePersoon
WHERE PersoonID = @foutPID

-- Probeer de adressen te verleggen naar de goede persoon

UPDATE foutPa
SET foutPa.PersoonID = @juistPID
FROM pers.PersoonsAdres foutPa
WHERE foutPa.PersoonID = @foutPID
AND NOT EXISTS
(SELECT 1 FROM pers.PersoonsAdres pa
WHERE pa.PersoonID = @juistPID
AND pa.AdresID = foutPa.AdresID)

-- De adressen die al aan de goede persoon gekoppeld waren, mogen weg van de
-- foute persoon.  We verliezen wel de opmerking

-- eerst nog eens kijken of het te verwijderen persoonsadres geen voorkeursadres
-- is van een gelieerde persoon.  Zo ja: wijzigen

UPDATE gp
SET gp.VoorkeursAdresID = juistePa.PersoonsAdresID
FROM pers.PersoonsAdres foutePa 
JOIN pers.GelieerdePersoon gp on gp.VoorkeursAdresID = foutePa.PersoonsAdresID
JOIN pers.PersoonsAdres juistePa on juistePa.AdresID = foutePa.AdresID
WHERE foutePa.PersoonID=@foutPID AND juistePa.PersoonID = @juistPID 

DELETE FROM pers.PersoonsAdres WHERE PersoonID=@foutPID

-- Persoonsverzekeringen verleggen
-- (Dit gaat wel problemen geven als we op termijn de verzekeringen aan
-- gelieerde personen gaan koppelen)

UPDATE pv
SET pv.PersoonID = @juistPID
FROM verz.PersoonsVerzekering pv
WHERE pv.PersoonID = @foutPID

-- GAV-schap verleggen

UPDATE gs
SET gs.PersoonID = @juistPID
FROM auth.gavSchap gs
WHERE gs.PersoonID = @foutPID

-- We gaan ervan uit dat de goeie informatie in het juiste
-- persoonsrecord zit
DELETE FROM pers.Persoon WHERE PersoonID=@foutPID

COMMIT TRAN

end





GO
/****** Object:  StoredProcedure [data].[spGroepsWerkJaarVerwijderen_2]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [data].[spGroepsWerkJaarVerwijderen_2](
@stamnr as varchar(10),
@werkjaar as int
)
as
begin
	set nocount on

	-- relatie leiding-afdeling verwijderen voor het opgegeven groepswerkjaar
	delete laj
	from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
	join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
	join lid.leidingInAfdelingsJaar laj on l.LidID = laj.LeidingID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- leiding verwijderen
	delete lei
	from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
	join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
	join lid.leiding lei on l.lidID = lei.LeidingID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- kinderen verwijderen
	delete k
	from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
	join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
	join lid.kind k on l.lidID = k.kindID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- lidfuncties verwijderen
	delete lf
	from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
	join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
	join lid.lidFunctie lf on l.LidID = lf.LidID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- leden uitschrijven
	delete l
	from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
	join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- afdelingsjaren verwijderen
	delete aj
	from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
	join lid.Afdelingsjaar aj on gwj.GroepsWerkJaarID = aj.GroepsWerkJaarID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- contactpersoon van geregistreerde uitstappen verwijderen
	update u
	set u.ContactDeelnemerID = null
	from biv.Uitstap u join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID=gwj.GroepsWerkJaarID
	join grp.Groep g on gwj.GroepID = g.GroepID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- deelnemers verwijderen van geregistreerde uitstappen
	delete d
	from biv.Uitstap u join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID=gwj.GroepsWerkJaarID
	join grp.Groep g on gwj.GroepID = g.GroepID
	join biv.Deelnemer d on d.UitstapID = u.UitstapID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- uitstappen verwijderen
	delete u
	from biv.Uitstap u join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID=gwj.GroepsWerkJaarID
	join grp.Groep g on gwj.GroepID = g.GroepID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- abonnementen verwijderen
	delete a
	from abo.Abonnement a join grp.GroepsWerkJaar gwj on a.GroepsWerkJaarID=gwj.GroepsWerkJaarID
	join grp.Groep g on gwj.GroepID = g.GroepID
	where g.code=@stamnr and gwj.werkjaar=@werkjaar

	-- groepswerkjaar verwijderen
	delete gwj
	from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
	where g.code=@stamnr and gwj.werkjaar=@werkjaar
end

GO
/****** Object:  StoredProcedure [grp].[spNieuweGroep]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [grp].[spNieuweGroep] 
	@stamNr VARCHAR(10), 
	@naam VARCHAR(160),
	@plaats VARCHAR(60), 
	@werkJaar INT AS
-- Maakt een nieuwe ploeg in GAP

DECLARE @groepID AS INTEGER
DECLARE @geslacht AS INTEGER
DECLARE @groepsWjID AS INTEGER
DECLARE @niveau AS INTEGER

SET @geslacht = grp.ufnRaadGeslacht(@stamNr)
SET @niveau = grp.ufnRaadNiveau(@stamNr)

-----------------
-- groep maken --
-----------------

PRINT 'Groep maken/vinden'
PRINT '------------------'
IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@stamNr)
BEGIN
	-- De groep bestaat nog niet in de database. Creeer.
	INSERT INTO grp.Groep(Naam, Code, OprichtingsJaar)
		SELECT @naam, @stamnr, @werkJaar 
	SET @groepID = scope_identity();
END
ELSE
BEGIN
	-- De Groep bestaat al in de database, de GroepID opvragen in de CG2 database
	SET @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@stamNr)

	-- Opvragen van Naam, Oprichtingsjaar en Website.
	-- Hier veronderstellen we dat meegegeven Naam, Oprichtingsjaar en Website 
	-- steeds van betere kwaliteit zijn dan de al bestaande.

	UPDATE dst
		SET dst.Naam = @naam, 
			dst.OprichtingsJaar = @werkJaar
	FROM grp.Groep dst 
	WHERE dst.GroepID = @groepID
END

-- Maak ook chirogroep of kadergroep aan als die niet bestaat.

IF @niveau = 2
BEGIN
	IF NOT EXISTS (SELECT 1 FROM grp.ChiroGroep WHERE ChiroGroepID = @groepID)
	BEGIN
		INSERT INTO grp.ChiroGroep(ChiroGroepID, Plaats, KaderGroepID)
			SELECT @groepID, @plaats, grp.ufnRaadGewest(@stamNr)
	END
	----------------------------------------------------------------------
	-- standaardafdelingen
	----------------------------------------------------------------------

	IF (SELECT COUNT(*) FROM lid.Afdeling WHERE ChiroGroepID = @groepID) = 0
	BEGIN
		INSERT INTO lid.Afdeling(AfdelingsNaam, Afkorting, ChiroGroepID)
		VALUES	('ribbels', 'RI', @groepID),
				('speelclub', 'SP', @groepID),
				('rakwi''s', 'RA', @groepID),
				('tito''s', 'TI', @groepID),
				('keti''s', 'KE', @groepID),
				('aspi''s', 'AS', @groepID);
	END
	PRINT 'Chirogroep met standaardafdeling ingevoegd/aangepast.'
	-- Geen afdelingsjaren dus, omdat een nieuwe Chirogroep waarschijnlijk geen
	-- regelmatige afdelingsverdeling heeft.
END
ELSE
BEGIN
	IF NOT EXISTS (SELECT 1 FROM grp.KaderGroep WHERE KaderGroepID = @groepID)
	BEGIN
		INSERT INTO grp.KaderGroep(KaderGroepID, Niveau, ParentID)
			SELECT @groepID, @niveau, CASE @niveau WHEN 8 THEN grp.ufnRaadVerbond(@stamNr) ELSE NULL END
	END
	PRINT 'Kadergroep ingevoegd/aangepast.'
END

PRINT ''


----------------------------------------------------------------------
-- voor het gemak meteen een groepswerkjaar maken voor dit werkjaar --
----------------------------------------------------------------------

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

PRINT 'GroepID: ' + CAST(@GroepID AS VARCHAR(10))
PRINT 'GroepsWerkJaarID: ' + CAST(@groepsWjID AS VARCHAR(10))

RETURN @GroepID

GO
/****** Object:  StoredProcedure [pers].[spFixVoorkeursAdres]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

create procedure [pers].[spFixVoorkeursAdres] @PersoonID int as
-- DOEL: zorg ervoor dat iedere gelieerde persoon gekoppeld aan de persoon
-- met gegeven PersoonID minstens 1 voorkeursadres heeft. 
-- (aangenomen dat de persoon adressen heeft; anders gebeurt er niets)
update gp1 
set gp1.VoorkeursAdresID = nieuw.PersoonsAdresID
from pers.GelieerdePersoon gp1 join
(
select gp.GelieerdePersoonID, max(pa.PersoonsAdresID) as PersoonsAdresID 
from pers.GelieerdePersoon gp
join pers.PersoonsAdres pa on gp.PersoonID = pa.PersoonID
where gp.persoonID = @PersoonID
	and gp.VoorkeursAdresID is null
group by gp.GelieerdePersoonID
) nieuw on gp1.GelieerdePersoonID = nieuw.GelieerdePersoonID


GO
/****** Object:  UserDefinedFunction [core].[ufnSoundEx]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE FUNCTION [core].[ufnSoundEx]
(
  @tekst VARCHAR(MAX)
)
RETURNS VARCHAR(8) AS
-- Doel: UDF die enkel soundex uitvoert, zodat we deze kunnen gebruiken in
-- entity framework
BEGIN
  RETURN
  (
	SELECT SOUNDEX(@tekst)
  )
END;

GO
/****** Object:  Table [abo].[Abonnement]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [abo].[Abonnement](
	[AbonnementID] [int] IDENTITY(1,1) NOT NULL,
	[GelieerdePersoonID] [int] NOT NULL,
	[PublicatieID] [int] NOT NULL,
	[GroepsWerkJaarID] [int] NOT NULL,
	[AanvraagDatum] [datetime] NOT NULL,
	[Versie] [timestamp] NULL,
	[Type] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AbonnementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [abo].[Publicatie]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [abo].[Publicatie](
	[PublicatieID] [int] IDENTITY(1,1) NOT NULL,
	[Naam] [varchar](80) NOT NULL,
	[Versie] [timestamp] NULL,
	[Actief] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PublicatieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[Adres]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[Adres](
	[Bus] [varchar](10) NULL,
	[HuisNr] [int] NULL,
	[PostCode] [varchar](10) NULL,
	[AdresID] [int] IDENTITY(1,1) NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_Adres] PRIMARY KEY CLUSTERED 
(
	[AdresID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[BelgischAdres]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [adr].[BelgischAdres](
	[BelgischAdresID] [int] NOT NULL,
	[StraatNaamID] [int] NOT NULL,
	[WoonPlaatsID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BelgischAdresID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [adr].[BuitenLandsAdres]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[BuitenLandsAdres](
	[BuitenlandsAdresID] [int] NOT NULL,
	[PostCode] [varchar](16) NULL,
	[Straat] [varchar](80) NOT NULL,
	[WoonPlaats] [varchar](80) NOT NULL,
	[LandID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BuitenlandsAdresID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[Land]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[Land](
	[LandID] [int] IDENTITY(1,1) NOT NULL,
	[Naam] [varchar](80) NOT NULL,
	[IsoCode] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Land_IsoCode] UNIQUE NONCLUSTERED 
(
	[IsoCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[PostNr]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [adr].[PostNr](
	[PostNr] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_PostNr] PRIMARY KEY CLUSTERED 
(
	[PostNr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [adr].[StraatNaam]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[StraatNaam](
	[StraatNaamID] [int] IDENTITY(1,1) NOT NULL,
	[PostNummer] [int] NOT NULL,
	[Naam] [varchar](80) NOT NULL,
	[TaalID] [int] NOT NULL,
	[CrabSubstraatID] [int] NULL,
	[Versie] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StraatNaamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[WoonPlaats]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[WoonPlaats](
	[WoonPlaatsID] [int] IDENTITY(1,1) NOT NULL,
	[PostNummer] [int] NOT NULL,
	[Naam] [varchar](80) NOT NULL,
	[TaalID] [int] NOT NULL,
	[CrabPostKantonID] [int] NULL,
	[Versie] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[WoonPlaatsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [auth].[GebruikersRechtv2]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [auth].[GebruikersRechtv2](
	[GebruikersRechtV2ID] [int] IDENTITY(1,1) NOT NULL,
	[PersoonID] [int] NOT NULL,
	[GroepID] [int] NOT NULL,
	[VervalDatum] [datetime] NULL,
	[PersoonsPermissies] [int] NOT NULL,
	[GroepsPermissies] [int] NOT NULL,
	[AfdelingsPermissies] [int] NOT NULL,
	[IedereenPermissies] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_GebruikersrechtV2] PRIMARY KEY CLUSTERED 
(
	[GebruikersRechtV2ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [biv].[Deelnemer]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [biv].[Deelnemer](
	[DeelnemerID] [int] IDENTITY(1,1) NOT NULL,
	[UitstapID] [int] NOT NULL,
	[GelieerdePersoonID] [int] NOT NULL,
	[IsLogistieker] [bit] NOT NULL,
	[HeeftBetaald] [bit] NOT NULL,
	[MedischeFicheOk] [bit] NOT NULL,
	[Opmerkingen] [text] NULL,
	[Versie] [timestamp] NOT NULL,
 CONSTRAINT [PK_Deelnemer] PRIMARY KEY CLUSTERED 
(
	[DeelnemerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [biv].[Plaats]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [biv].[Plaats](
	[PlaatsID] [int] IDENTITY(1,1) NOT NULL,
	[Naam] [varchar](80) NOT NULL,
	[AdresID] [int] NOT NULL,
	[GelieerdePersoonID] [int] NULL,
	[GroepID] [int] NOT NULL,
	[Versie] [timestamp] NOT NULL,
 CONSTRAINT [PK_Plaats] PRIMARY KEY CLUSTERED 
(
	[PlaatsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [biv].[Uitstap]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [biv].[Uitstap](
	[UitstapID] [int] IDENTITY(1,1) NOT NULL,
	[Naam] [varchar](120) NOT NULL,
	[IsBivak] [bit] NOT NULL,
	[DatumVan] [datetime] NOT NULL,
	[DatumTot] [datetime] NOT NULL,
	[Opmerkingen] [text] NULL,
	[PlaatsID] [int] NULL,
	[GroepsWerkJaarID] [int] NULL,
	[ContactDeelnemerID] [int] NULL,
	[Versie] [timestamp] NOT NULL,
 CONSTRAINT [PK_Uitstap] PRIMARY KEY CLUSTERED 
(
	[UitstapID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [core].[Categorie]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [core].[Categorie](
	[Naam] [varchar](80) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[CategorieID] [int] IDENTITY(1,1) NOT NULL,
	[GroepID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_Categorie] PRIMARY KEY CLUSTERED 
(
	[CategorieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Categorie_GroepID_Code] UNIQUE NONCLUSTERED 
(
	[GroepID] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Categorie_GroepID_Naam] UNIQUE NONCLUSTERED 
(
	[GroepID] ASC,
	[Naam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [core].[Taal]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [core].[Taal](
	[TaalID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](5) NOT NULL,
	[Naam] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TaalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [core].[VrijVeldType]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [core].[VrijVeldType](
	[Naam] [varchar](80) NULL,
	[DataType] [int] NOT NULL,
	[VrijVeldTypeID] [int] NOT NULL,
	[GroepID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_VrijVeldType] PRIMARY KEY CLUSTERED 
(
	[VrijVeldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [grp].[ChiroGroep]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [grp].[ChiroGroep](
	[ChiroGroepID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
	[Plaats] [varchar](60) NULL,
	[KaderGroepID] [int] NOT NULL,
 CONSTRAINT [PK_ChiroGroep] PRIMARY KEY CLUSTERED 
(
	[ChiroGroepID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [grp].[Groep]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [grp].[Groep](
	[Naam] [varchar](160) NOT NULL,
	[Code] [char](10) NULL,
	[OprichtingsJaar] [int] NULL,
	[WebSite] [varchar](160) NULL,
	[Logo] [image] NULL,
	[GroepID] [int] IDENTITY(1,1) NOT NULL,
	[Versie] [timestamp] NULL,
	[StopDatum] [datetime] NULL,
	[AdresID] [int] NULL,
 CONSTRAINT [PK_Groep] PRIMARY KEY CLUSTERED 
(
	[GroepID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [grp].[GroepsAdres]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [grp].[GroepsAdres](
	[AdresID] [int] NOT NULL,
	[GroepID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_GroepsAdres] PRIMARY KEY CLUSTERED 
(
	[GroepID] ASC,
	[AdresID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [grp].[GroepsWerkJaar]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [grp].[GroepsWerkJaar](
	[WerkJaar] [int] NOT NULL,
	[GroepsWerkJaarID] [int] IDENTITY(1,1) NOT NULL,
	[GroepID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
	[Datum] [datetime] NULL,
 CONSTRAINT [PK_GroepsWerkjaar] PRIMARY KEY CLUSTERED 
(
	[GroepsWerkJaarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [grp].[KaderGroep]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [grp].[KaderGroep](
	[KaderGroepID] [int] NOT NULL,
	[Niveau] [int] NOT NULL,
	[ParentID] [int] NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_KaderGroep] PRIMARY KEY CLUSTERED 
(
	[KaderGroepID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [lid].[Afdeling]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [lid].[Afdeling](
	[AfdelingsNaam] [varchar](50) NOT NULL,
	[Afkorting] [varchar](10) NOT NULL,
	[AfdelingID] [int] IDENTITY(1,1) NOT NULL,
	[ChiroGroepID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_Afdeling] PRIMARY KEY CLUSTERED 
(
	[AfdelingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Afdeling_GroepID_AfdelingsNaam] UNIQUE NONCLUSTERED 
(
	[ChiroGroepID] ASC,
	[AfdelingsNaam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Afdeling_GroepID_Afkorting] UNIQUE NONCLUSTERED 
(
	[ChiroGroepID] ASC,
	[Afkorting] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [lid].[AfdelingsJaar]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[AfdelingsJaar](
	[GeboorteJaarTot] [int] NOT NULL,
	[GeboorteJaarVan] [int] NOT NULL,
	[AfdelingsJaarID] [int] IDENTITY(1,1) NOT NULL,
	[AfdelingID] [int] NOT NULL,
	[Geslacht] [int] NOT NULL,
	[GroepsWerkJaarID] [int] NOT NULL,
	[OfficieleAfdelingID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_AfdelingsJaar] PRIMARY KEY CLUSTERED 
(
	[AfdelingsJaarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_AfdelingsJaar] UNIQUE NONCLUSTERED 
(
	[GroepsWerkJaarID] ASC,
	[AfdelingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [lid].[Functie]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [lid].[Functie](
	[Naam] [varchar](80) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[FunctieID] [int] IDENTITY(1,1) NOT NULL,
	[GroepID] [int] NULL,
	[Versie] [timestamp] NULL,
	[MaxAantal] [int] NULL,
	[MinAantal] [int] NOT NULL,
	[WerkJaarVan] [int] NULL,
	[WerkJaarTot] [int] NULL,
	[IsNationaal]  AS (case when [GroepID] IS NULL then CONVERT([bit],'true',0) else CONVERT([bit],'false',0) end),
	[Niveau] [int] NOT NULL,
	[LidType]  AS (([Niveau]&~(1))/(2)&(3)),
 CONSTRAINT [PK_Functie] PRIMARY KEY CLUSTERED 
(
	[FunctieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Functie_GroepID_Code] UNIQUE NONCLUSTERED 
(
	[GroepID] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Functie_GroepID_Naam] UNIQUE NONCLUSTERED 
(
	[GroepID] ASC,
	[Naam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [lid].[Kind]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[Kind](
	[kindID] [int] NOT NULL,
	[afdelingsJaarID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_Kind] PRIMARY KEY CLUSTERED 
(
	[kindID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [lid].[Leiding]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[Leiding](
	[leidingID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_Leiding] PRIMARY KEY CLUSTERED 
(
	[leidingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [lid].[LeidingInAfdelingsJaar]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[LeidingInAfdelingsJaar](
	[AfdelingsJaarID] [int] NOT NULL,
	[LeidingID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_LeidingInAfdelingsJaar] PRIMARY KEY CLUSTERED 
(
	[LeidingID] ASC,
	[AfdelingsJaarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [lid].[Lid]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[Lid](
	[LidgeldBetaald] [bit] NULL,
	[Verwijderd] [bit] NULL,
	[VolgendWerkjaar] [smallint] NULL,
	[LidID] [int] IDENTITY(1,1) NOT NULL,
	[GroepsWerkjaarID] [int] NOT NULL,
	[GelieerdePersoonID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
	[EindeInstapPeriode] [smalldatetime] NULL,
	[UitschrijfDatum] [datetime] NULL,
	[NonActief]  AS (case when [UitschrijfDatum] IS NULL then CONVERT([bit],(0),0) else CONVERT([bit],(1),0) end),
	[IsAangesloten] [bit] NOT NULL,
 CONSTRAINT [PK_Lid] PRIMARY KEY NONCLUSTERED 
(
	[LidID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [lid].[LidFunctie]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[LidFunctie](
	[LidID] [int] NOT NULL,
	[FunctieID] [int] NOT NULL,
 CONSTRAINT [PK_LidFunctie] PRIMARY KEY CLUSTERED 
(
	[LidID] ASC,
	[FunctieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [lid].[OfficieleAfdeling]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [lid].[OfficieleAfdeling](
	[Naam] [varchar](50) NOT NULL,
	[OfficieleAfdelingID] [int] IDENTITY(1,1) NOT NULL,
	[LeefTijdVan] [int] NOT NULL,
	[LeefTijdTot] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_OfficieleAfdeling] PRIMARY KEY CLUSTERED 
(
	[OfficieleAfdelingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_OfficieleAfdeling_Naam] UNIQUE NONCLUSTERED 
(
	[Naam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [logging].[Bericht]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [logging].[Bericht](
	[BerichtID] [int] IDENTITY(1,1) NOT NULL,
	[Tijd] [datetime] NOT NULL,
	[Niveau] [int] NOT NULL,
	[Boodschap] [varchar](max) NULL,
	[StamNummer] [char](10) NULL,
	[AdNummer] [int] NULL,
	[PersoonID] [int] NULL,
	[GebruikerID] [int] NULL,
 CONSTRAINT [PK_Bericht] PRIMARY KEY CLUSTERED 
(
	[BerichtID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[AdresType]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[AdresType](
	[Omschrijving] [varchar](80) NULL,
	[AdresTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_AdresType] PRIMARY KEY CLUSTERED 
(
	[AdresTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[CommunicatieType]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[CommunicatieType](
	[Omschrijving] [varchar](80) NULL,
	[Validatie] [varchar](160) NULL,
	[CommunicatieTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Voorbeeld] [varchar](160) NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_CommunicatieType] PRIMARY KEY CLUSTERED 
(
	[CommunicatieTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[CommunicatieVorm]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[CommunicatieVorm](
	[Nota] [varchar](320) NULL,
	[Nummer] [varchar](160) NOT NULL,
	[CommunicatieVormID] [int] IDENTITY(1,1) NOT NULL,
	[CommunicatieTypeID] [int] NOT NULL,
	[IsGezinsgebonden] [bit] NOT NULL,
	[Voorkeur] [bit] NOT NULL,
	[GelieerdePersoonID] [int] NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_CommunicatieVorm] PRIMARY KEY NONCLUSTERED 
(
	[CommunicatieVormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[GelieerdePersoon]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[GelieerdePersoon](
	[GroepID] [int] NOT NULL,
	[PersoonID] [int] NOT NULL,
	[ChiroLeefTijd] [int] NOT NULL,
	[GelieerdePersoonID] [int] IDENTITY(1,1) NOT NULL,
	[Versie] [timestamp] NULL,
	[VoorkeursAdresID] [int] NULL,
 CONSTRAINT [PK_GelieerdePersoon] PRIMARY KEY CLUSTERED 
(
	[GelieerdePersoonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [pers].[Persoon]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[Persoon](
	[AdNummer] [int] NULL,
	[Naam] [varchar](160) NOT NULL,
	[VoorNaam] [varchar](60) NOT NULL,
	[GeboorteDatum] [datetime] NULL,
	[Geslacht] [int] NOT NULL,
	[SterfDatum] [smalldatetime] NULL,
	[PersoonID] [int] IDENTITY(1,1) NOT NULL,
	[Versie] [timestamp] NULL,
	[AdInAanvraag] [bit] NOT NULL,
	[SeNaam]  AS (soundex([Naam])),
	[SeVoornaam]  AS (soundex([VoorNaam])),
	[NieuwsBrief] [bit] NOT NULL,
 CONSTRAINT [PK_Persoon] PRIMARY KEY CLUSTERED 
(
	[PersoonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[PersoonsAdres]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[PersoonsAdres](
	[Opmerking] [text] NULL,
	[AdresID] [int] NOT NULL,
	[AdresTypeID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
	[PersoonsAdresID] [int] IDENTITY(1,1) NOT NULL,
	[PersoonID] [int] NOT NULL,
 CONSTRAINT [PK_PersoonsAdres] PRIMARY KEY CLUSTERED 
(
	[PersoonsAdresID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [pers].[PersoonsCategorie]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[PersoonsCategorie](
	[GelieerdePersoonID] [int] NOT NULL,
	[CategorieID] [int] NOT NULL,
 CONSTRAINT [PK_PersoonsCategorie] PRIMARY KEY CLUSTERED 
(
	[GelieerdePersoonID] ASC,
	[CategorieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [pers].[PersoonVrijVeld]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[PersoonVrijVeld](
	[Waarde] [varchar](320) NOT NULL,
	[GelieerdePersoonID] [int] NOT NULL,
	[PersoonVrijVeldTypeID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_PersoonVrijVeld] PRIMARY KEY CLUSTERED 
(
	[GelieerdePersoonID] ASC,
	[PersoonVrijVeldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[PersoonVrijVeldType]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[PersoonVrijVeldType](
	[PersoonVrijVeldTypeID] [int] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_PersoonVrijVeldType] PRIMARY KEY CLUSTERED 
(
	[PersoonVrijVeldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [verz].[PersoonsVerzekering]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [verz].[PersoonsVerzekering](
	[PersoonsVerzekeringID] [int] IDENTITY(1,1) NOT NULL,
	[PersoonID] [int] NOT NULL,
	[VerzekeringsTypeID] [int] NOT NULL,
	[Van] [datetime] NOT NULL,
	[Tot] [datetime] NOT NULL,
	[Versie] [timestamp] NULL,
 CONSTRAINT [PK_PersoonsVerzekering] PRIMARY KEY CLUSTERED 
(
	[PersoonsVerzekeringID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [verz].[VerzekeringsType]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [verz].[VerzekeringsType](
	[VerzekeringsTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Naam] [varchar](40) NOT NULL,
	[Omschrijving] [text] NULL,
	[EnkelLeden] [bit] NOT NULL,
	[TotEindeWerkJaar] [bit] NOT NULL,
 CONSTRAINT [PK_VerzekeringsType] PRIMARY KEY CLUSTERED 
(
	[VerzekeringsTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [adr].[vPersoonsAdresInfo]    Script Date: 21/02/2017 10:50:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [adr].[vPersoonsAdresInfo] as
-- voorkeursadres(sen) van een persoon
-- (kunnen er meerdere zijn als een persoon aan meerdere groepen is gekoppeld)
select  p.adnummer,gp.gelieerdepersoonid,gp.persoonid,pa.persoonsadresid, p.naam, p.voornaam,
case when bua.buitenlandsadresid is null then sn.naam else bua.straat end as straatnaam,
a.huisnr, a.bus, 
isnull(wp.PostNummer, sn.postnummer) as postnr,
isnull(bua.postcode, '') as postcode,
case when bua.buitenlandsadresid is null then wp.naam else bua.woonplaats end as woonplaats,
isnull(l.naam,'') as land, pa.versie
 from pers.persoon p
join pers.gelieerdepersoon gp on p.persoonid = gp.persoonid
left outer join pers.persoonsadres pa on gp.voorkeursadresid=pa.persoonsadresid
left outer join adr.adres a on pa.adresid=a.adresid
left outer join adr.belgischadres bea on bea.belgischadresid=a.adresid
left outer join adr.buitenlandsadres bua on bua.buitenlandsadresid=a.adresid
left outer join adr.straatnaam sn on bea.straatnaamid=sn.straatnaamid
left outer join adr.woonplaats wp on bea.woonplaatsid=wp.woonplaatsid
left outer join adr.land l on bua.landid=l.landid

GO
ALTER TABLE [abo].[Abonnement] ADD  DEFAULT (getdate()) FOR [AanvraagDatum]
GO
ALTER TABLE [abo].[Abonnement] ADD  DEFAULT ((1)) FOR [Type]
GO
ALTER TABLE [abo].[Publicatie] ADD  DEFAULT ((1)) FOR [Actief]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [PersoonsPermissies]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [GroepsPermissies]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [AfdelingsPermissies]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [IedereenPermissies]
GO
ALTER TABLE [grp].[GroepsWerkJaar] ADD  DEFAULT (getdate()) FOR [Datum]
GO
ALTER TABLE [lid].[Functie] ADD  DEFAULT ((175)) FOR [Niveau]
GO
ALTER TABLE [lid].[Lid] ADD  DEFAULT ((0)) FOR [IsAangesloten]
GO
ALTER TABLE [logging].[Bericht] ADD  DEFAULT (getdate()) FOR [Tijd]
GO
ALTER TABLE [logging].[Bericht] ADD  DEFAULT ((1)) FOR [Niveau]
GO
ALTER TABLE [pers].[CommunicatieVorm] ADD  CONSTRAINT [DF_CommunicatieVorm_IsGezinsgebonden]  DEFAULT ((0)) FOR [IsGezinsgebonden]
GO
ALTER TABLE [pers].[CommunicatieVorm] ADD  CONSTRAINT [DF_CommunicatieVorm_Voorkeur]  DEFAULT ((0)) FOR [Voorkeur]
GO
ALTER TABLE [pers].[GelieerdePersoon] ADD  CONSTRAINT [DF_GelieerdePersoon_ChiroLeefTijd]  DEFAULT ((0)) FOR [ChiroLeefTijd]
GO
ALTER TABLE [pers].[Persoon] ADD  CONSTRAINT [DF_Persoon_AdInAanvraag]  DEFAULT ((0)) FOR [AdInAanvraag]
GO
ALTER TABLE [pers].[Persoon] ADD  DEFAULT ((0)) FOR [NieuwsBrief]
GO
ALTER TABLE [abo].[Abonnement]  WITH CHECK ADD  CONSTRAINT [FK_Abonnement_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [abo].[Abonnement] CHECK CONSTRAINT [FK_Abonnement_GelieerdePersoon]
GO
ALTER TABLE [abo].[Abonnement]  WITH CHECK ADD  CONSTRAINT [FK_Abonnement_GroepsWerkJaar] FOREIGN KEY([GroepsWerkJaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [abo].[Abonnement] CHECK CONSTRAINT [FK_Abonnement_GroepsWerkJaar]
GO
ALTER TABLE [abo].[Abonnement]  WITH CHECK ADD  CONSTRAINT [FK_Abonnement_Publicatie] FOREIGN KEY([PublicatieID])
REFERENCES [abo].[Publicatie] ([PublicatieID])
GO
ALTER TABLE [abo].[Abonnement] CHECK CONSTRAINT [FK_Abonnement_Publicatie]
GO
ALTER TABLE [adr].[BelgischAdres]  WITH CHECK ADD  CONSTRAINT [FK_BelgischAdres_Adres] FOREIGN KEY([BelgischAdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [adr].[BelgischAdres] CHECK CONSTRAINT [FK_BelgischAdres_Adres]
GO
ALTER TABLE [adr].[BelgischAdres]  WITH CHECK ADD  CONSTRAINT [FK_BelgischAdres_StraatNaam] FOREIGN KEY([StraatNaamID])
REFERENCES [adr].[StraatNaam] ([StraatNaamID])
GO
ALTER TABLE [adr].[BelgischAdres] CHECK CONSTRAINT [FK_BelgischAdres_StraatNaam]
GO
ALTER TABLE [adr].[BelgischAdres]  WITH CHECK ADD  CONSTRAINT [FK_BelgischAdres_WoonPlaats] FOREIGN KEY([WoonPlaatsID])
REFERENCES [adr].[WoonPlaats] ([WoonPlaatsID])
GO
ALTER TABLE [adr].[BelgischAdres] CHECK CONSTRAINT [FK_BelgischAdres_WoonPlaats]
GO
ALTER TABLE [adr].[BuitenLandsAdres]  WITH CHECK ADD  CONSTRAINT [FK_BuitenlandsAdres_Adres] FOREIGN KEY([BuitenlandsAdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [adr].[BuitenLandsAdres] CHECK CONSTRAINT [FK_BuitenlandsAdres_Adres]
GO
ALTER TABLE [adr].[BuitenLandsAdres]  WITH CHECK ADD  CONSTRAINT [FK_BuitenlandsAdres_Land] FOREIGN KEY([LandID])
REFERENCES [adr].[Land] ([LandID])
GO
ALTER TABLE [adr].[BuitenLandsAdres] CHECK CONSTRAINT [FK_BuitenlandsAdres_Land]
GO
ALTER TABLE [adr].[StraatNaam]  WITH CHECK ADD  CONSTRAINT [FK_StraatNaam_PostNummer] FOREIGN KEY([PostNummer])
REFERENCES [adr].[PostNr] ([PostNr])
GO
ALTER TABLE [adr].[StraatNaam] CHECK CONSTRAINT [FK_StraatNaam_PostNummer]
GO
ALTER TABLE [adr].[StraatNaam]  WITH CHECK ADD  CONSTRAINT [FK_StraatNaam_Taal] FOREIGN KEY([TaalID])
REFERENCES [core].[Taal] ([TaalID])
GO
ALTER TABLE [adr].[StraatNaam] CHECK CONSTRAINT [FK_StraatNaam_Taal]
GO
ALTER TABLE [adr].[WoonPlaats]  WITH CHECK ADD  CONSTRAINT [FK_WoonPlaats_PostNummer] FOREIGN KEY([PostNummer])
REFERENCES [adr].[PostNr] ([PostNr])
GO
ALTER TABLE [adr].[WoonPlaats] CHECK CONSTRAINT [FK_WoonPlaats_PostNummer]
GO
ALTER TABLE [adr].[WoonPlaats]  WITH CHECK ADD  CONSTRAINT [FK_WoonPlaats_Taal] FOREIGN KEY([TaalID])
REFERENCES [core].[Taal] ([TaalID])
GO
ALTER TABLE [adr].[WoonPlaats] CHECK CONSTRAINT [FK_WoonPlaats_Taal]
GO
ALTER TABLE [auth].[GebruikersRechtv2]  WITH CHECK ADD  CONSTRAINT [FK_GebruikersRechtV2_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [auth].[GebruikersRechtv2] CHECK CONSTRAINT [FK_GebruikersRechtV2_Groep]
GO
ALTER TABLE [auth].[GebruikersRechtv2]  WITH CHECK ADD  CONSTRAINT [FK_GebruikersRechtV2_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [auth].[GebruikersRechtv2] CHECK CONSTRAINT [FK_GebruikersRechtV2_Persoon]
GO
ALTER TABLE [biv].[Deelnemer]  WITH CHECK ADD  CONSTRAINT [FK_Deelnemer_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [biv].[Deelnemer] CHECK CONSTRAINT [FK_Deelnemer_GelieerdePersoon]
GO
ALTER TABLE [biv].[Deelnemer]  WITH CHECK ADD  CONSTRAINT [FK_Deelnemer_Uitstap] FOREIGN KEY([UitstapID])
REFERENCES [biv].[Uitstap] ([UitstapID])
GO
ALTER TABLE [biv].[Deelnemer] CHECK CONSTRAINT [FK_Deelnemer_Uitstap]
GO
ALTER TABLE [biv].[Plaats]  WITH CHECK ADD  CONSTRAINT [FK_Plaats_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [biv].[Plaats] CHECK CONSTRAINT [FK_Plaats_Adres]
GO
ALTER TABLE [biv].[Plaats]  WITH CHECK ADD  CONSTRAINT [FK_Plaats_GelieerdePersoon_Contact] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [biv].[Plaats] CHECK CONSTRAINT [FK_Plaats_GelieerdePersoon_Contact]
GO
ALTER TABLE [biv].[Plaats]  WITH CHECK ADD  CONSTRAINT [FK_Plaats_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [biv].[Plaats] CHECK CONSTRAINT [FK_Plaats_Groep]
GO
ALTER TABLE [biv].[Uitstap]  WITH CHECK ADD  CONSTRAINT [FK_Uitstap_Deelnemer_Contact] FOREIGN KEY([ContactDeelnemerID])
REFERENCES [biv].[Deelnemer] ([DeelnemerID])
GO
ALTER TABLE [biv].[Uitstap] CHECK CONSTRAINT [FK_Uitstap_Deelnemer_Contact]
GO
ALTER TABLE [biv].[Uitstap]  WITH CHECK ADD  CONSTRAINT [FK_Uitstap_GroepsWerkJaar] FOREIGN KEY([GroepsWerkJaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [biv].[Uitstap] CHECK CONSTRAINT [FK_Uitstap_GroepsWerkJaar]
GO
ALTER TABLE [biv].[Uitstap]  WITH CHECK ADD  CONSTRAINT [FK_Uitstap_Plaats] FOREIGN KEY([PlaatsID])
REFERENCES [biv].[Plaats] ([PlaatsID])
GO
ALTER TABLE [biv].[Uitstap] CHECK CONSTRAINT [FK_Uitstap_Plaats]
GO
ALTER TABLE [core].[Categorie]  WITH CHECK ADD  CONSTRAINT [FK_Categorie_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [core].[Categorie] CHECK CONSTRAINT [FK_Categorie_Groep]
GO
ALTER TABLE [core].[VrijVeldType]  WITH CHECK ADD  CONSTRAINT [FK_VrijVeldType_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [core].[VrijVeldType] CHECK CONSTRAINT [FK_VrijVeldType_Groep]
GO
ALTER TABLE [grp].[ChiroGroep]  WITH CHECK ADD  CONSTRAINT [FK_ChiroGroep_Groep] FOREIGN KEY([ChiroGroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[ChiroGroep] CHECK CONSTRAINT [FK_ChiroGroep_Groep]
GO
ALTER TABLE [grp].[ChiroGroep]  WITH CHECK ADD  CONSTRAINT [FK_ChiroGroep_KaderGroep] FOREIGN KEY([KaderGroepID])
REFERENCES [grp].[KaderGroep] ([KaderGroepID])
GO
ALTER TABLE [grp].[ChiroGroep] CHECK CONSTRAINT [FK_ChiroGroep_KaderGroep]
GO
ALTER TABLE [grp].[Groep]  WITH CHECK ADD  CONSTRAINT [FK_Groep_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [grp].[Groep] CHECK CONSTRAINT [FK_Groep_Adres]
GO
ALTER TABLE [grp].[GroepsAdres]  WITH CHECK ADD  CONSTRAINT [FK_GroepsAdres_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [grp].[GroepsAdres] CHECK CONSTRAINT [FK_GroepsAdres_Adres]
GO
ALTER TABLE [grp].[GroepsAdres]  WITH CHECK ADD  CONSTRAINT [FK_GroepsAdres_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[GroepsAdres] CHECK CONSTRAINT [FK_GroepsAdres_Groep]
GO
ALTER TABLE [grp].[GroepsWerkJaar]  WITH CHECK ADD  CONSTRAINT [FK_GroepsWerkjaar_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[GroepsWerkJaar] CHECK CONSTRAINT [FK_GroepsWerkjaar_Groep]
GO
ALTER TABLE [grp].[KaderGroep]  WITH CHECK ADD  CONSTRAINT [FK_KaderGroep_Groep] FOREIGN KEY([KaderGroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[KaderGroep] CHECK CONSTRAINT [FK_KaderGroep_Groep]
GO
ALTER TABLE [grp].[KaderGroep]  WITH CHECK ADD  CONSTRAINT [FK_KaderGroep_KaderGroep] FOREIGN KEY([ParentID])
REFERENCES [grp].[KaderGroep] ([KaderGroepID])
GO
ALTER TABLE [grp].[KaderGroep] CHECK CONSTRAINT [FK_KaderGroep_KaderGroep]
GO
ALTER TABLE [lid].[Afdeling]  WITH CHECK ADD  CONSTRAINT [FK_Afdeling_ChiroGroep] FOREIGN KEY([ChiroGroepID])
REFERENCES [grp].[ChiroGroep] ([ChiroGroepID])
GO
ALTER TABLE [lid].[Afdeling] CHECK CONSTRAINT [FK_Afdeling_ChiroGroep]
GO
ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_Afdeling] FOREIGN KEY([AfdelingID])
REFERENCES [lid].[Afdeling] ([AfdelingID])
GO
ALTER TABLE [lid].[AfdelingsJaar] CHECK CONSTRAINT [FK_AfdelingsJaar_Afdeling]
GO
ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_GroepsWerkjaar] FOREIGN KEY([GroepsWerkJaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [lid].[AfdelingsJaar] CHECK CONSTRAINT [FK_AfdelingsJaar_GroepsWerkjaar]
GO
ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_OfficieleAfdeling] FOREIGN KEY([OfficieleAfdelingID])
REFERENCES [lid].[OfficieleAfdeling] ([OfficieleAfdelingID])
GO
ALTER TABLE [lid].[AfdelingsJaar] CHECK CONSTRAINT [FK_AfdelingsJaar_OfficieleAfdeling]
GO
ALTER TABLE [lid].[Functie]  WITH CHECK ADD  CONSTRAINT [FK_Functie_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [lid].[Functie] CHECK CONSTRAINT [FK_Functie_Groep]
GO
ALTER TABLE [lid].[Kind]  WITH CHECK ADD  CONSTRAINT [FK_Kind_AfdelingsJaar] FOREIGN KEY([afdelingsJaarID])
REFERENCES [lid].[AfdelingsJaar] ([AfdelingsJaarID])
GO
ALTER TABLE [lid].[Kind] CHECK CONSTRAINT [FK_Kind_AfdelingsJaar]
GO
ALTER TABLE [lid].[Kind]  WITH CHECK ADD  CONSTRAINT [FK_Kind_Lid] FOREIGN KEY([kindID])
REFERENCES [lid].[Lid] ([LidID])
GO
ALTER TABLE [lid].[Kind] CHECK CONSTRAINT [FK_Kind_Lid]
GO
ALTER TABLE [lid].[Leiding]  WITH CHECK ADD  CONSTRAINT [FK_Leiding_Lid] FOREIGN KEY([leidingID])
REFERENCES [lid].[Lid] ([LidID])
GO
ALTER TABLE [lid].[Leiding] CHECK CONSTRAINT [FK_Leiding_Lid]
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_LeidingInAfdelingsJaar_AfdelingsJaar] FOREIGN KEY([AfdelingsJaarID])
REFERENCES [lid].[AfdelingsJaar] ([AfdelingsJaarID])
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar] CHECK CONSTRAINT [FK_LeidingInAfdelingsJaar_AfdelingsJaar]
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_LeidingInAfdelingsJaar_Leiding] FOREIGN KEY([LeidingID])
REFERENCES [lid].[Leiding] ([leidingID])
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar] CHECK CONSTRAINT [FK_LeidingInAfdelingsJaar_Leiding]
GO
ALTER TABLE [lid].[Lid]  WITH CHECK ADD  CONSTRAINT [FK_Lid_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [lid].[Lid] CHECK CONSTRAINT [FK_Lid_GelieerdePersoon]
GO
ALTER TABLE [lid].[Lid]  WITH CHECK ADD  CONSTRAINT [FK_Lid_GroepsWerkjaar] FOREIGN KEY([GroepsWerkjaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [lid].[Lid] CHECK CONSTRAINT [FK_Lid_GroepsWerkjaar]
GO
ALTER TABLE [lid].[LidFunctie]  WITH CHECK ADD  CONSTRAINT [FK_LidFunctie_Functie] FOREIGN KEY([FunctieID])
REFERENCES [lid].[Functie] ([FunctieID])
GO
ALTER TABLE [lid].[LidFunctie] CHECK CONSTRAINT [FK_LidFunctie_Functie]
GO
ALTER TABLE [lid].[LidFunctie]  WITH CHECK ADD  CONSTRAINT [FK_LidFunctie_Lid] FOREIGN KEY([LidID])
REFERENCES [lid].[Lid] ([LidID])
GO
ALTER TABLE [lid].[LidFunctie] CHECK CONSTRAINT [FK_LidFunctie_Lid]
GO
ALTER TABLE [logging].[Bericht]  WITH CHECK ADD  CONSTRAINT [FK_Bericht_Gebruiker] FOREIGN KEY([GebruikerID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [logging].[Bericht] CHECK CONSTRAINT [FK_Bericht_Gebruiker]
GO
ALTER TABLE [logging].[Bericht]  WITH CHECK ADD  CONSTRAINT [FK_Bericht_Groep] FOREIGN KEY([StamNummer])
REFERENCES [grp].[Groep] ([Code])
GO
ALTER TABLE [logging].[Bericht] CHECK CONSTRAINT [FK_Bericht_Groep]
GO
ALTER TABLE [logging].[Bericht]  WITH CHECK ADD  CONSTRAINT [FK_Bericht_Persoon_Id] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [logging].[Bericht] CHECK CONSTRAINT [FK_Bericht_Persoon_Id]
GO
ALTER TABLE [pers].[CommunicatieVorm]  WITH CHECK ADD  CONSTRAINT [FK_CommunicatieVorm_CommunicatieType] FOREIGN KEY([CommunicatieTypeID])
REFERENCES [pers].[CommunicatieType] ([CommunicatieTypeID])
GO
ALTER TABLE [pers].[CommunicatieVorm] CHECK CONSTRAINT [FK_CommunicatieVorm_CommunicatieType]
GO
ALTER TABLE [pers].[CommunicatieVorm]  WITH CHECK ADD  CONSTRAINT [FK_CommunicatieVorm_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [pers].[CommunicatieVorm] CHECK CONSTRAINT [FK_CommunicatieVorm_GelieerdePersoon]
GO
ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [pers].[GelieerdePersoon] CHECK CONSTRAINT [FK_GelieerdePersoon_Groep]
GO
ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [pers].[GelieerdePersoon] CHECK CONSTRAINT [FK_GelieerdePersoon_Persoon]
GO
ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_PersoonsAdres] FOREIGN KEY([VoorkeursAdresID])
REFERENCES [pers].[PersoonsAdres] ([PersoonsAdresID])
GO
ALTER TABLE [pers].[GelieerdePersoon] CHECK CONSTRAINT [FK_GelieerdePersoon_PersoonsAdres]
GO
ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [pers].[PersoonsAdres] CHECK CONSTRAINT [FK_PersoonsAdres_Adres]
GO
ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_AdresType] FOREIGN KEY([AdresTypeID])
REFERENCES [pers].[AdresType] ([AdresTypeID])
GO
ALTER TABLE [pers].[PersoonsAdres] CHECK CONSTRAINT [FK_PersoonsAdres_AdresType]
GO
ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [pers].[PersoonsAdres] CHECK CONSTRAINT [FK_PersoonsAdres_Persoon]
GO
ALTER TABLE [pers].[PersoonsCategorie]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsCategorie_Categorie] FOREIGN KEY([CategorieID])
REFERENCES [core].[Categorie] ([CategorieID])
GO
ALTER TABLE [pers].[PersoonsCategorie] CHECK CONSTRAINT [FK_PersoonsCategorie_Categorie]
GO
ALTER TABLE [pers].[PersoonsCategorie]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsCategorie_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [pers].[PersoonsCategorie] CHECK CONSTRAINT [FK_PersoonsCategorie_GelieerdePersoon]
GO
ALTER TABLE [pers].[PersoonVrijVeld]  WITH CHECK ADD  CONSTRAINT [FK_Persoon_PersoonVrijVeldType] FOREIGN KEY([PersoonVrijVeldTypeID])
REFERENCES [pers].[PersoonVrijVeldType] ([PersoonVrijVeldTypeID])
GO
ALTER TABLE [pers].[PersoonVrijVeld] CHECK CONSTRAINT [FK_Persoon_PersoonVrijVeldType]
GO
ALTER TABLE [pers].[PersoonVrijVeld]  WITH CHECK ADD  CONSTRAINT [FK_PersoonVrijVeld_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [pers].[PersoonVrijVeld] CHECK CONSTRAINT [FK_PersoonVrijVeld_GelieerdePersoon]
GO
ALTER TABLE [pers].[PersoonVrijVeldType]  WITH CHECK ADD  CONSTRAINT [FK_PersoonVrijVeldType_VrijVeldType] FOREIGN KEY([PersoonVrijVeldTypeID])
REFERENCES [core].[VrijVeldType] ([VrijVeldTypeID])
GO
ALTER TABLE [pers].[PersoonVrijVeldType] CHECK CONSTRAINT [FK_PersoonVrijVeldType_VrijVeldType]
GO
ALTER TABLE [verz].[PersoonsVerzekering]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsVerzekering_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [verz].[PersoonsVerzekering] CHECK CONSTRAINT [FK_PersoonsVerzekering_Persoon]
GO
ALTER TABLE [verz].[PersoonsVerzekering]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsVerzekering_VerzekeringsType] FOREIGN KEY([VerzekeringsTypeID])
REFERENCES [verz].[VerzekeringsType] ([VerzekeringsTypeID])
GO
ALTER TABLE [verz].[PersoonsVerzekering] CHECK CONSTRAINT [FK_PersoonsVerzekering_VerzekeringsType]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'De gebruiker van het programma kan afdelingen maken zo veel hij/zij wil.  Als een afdeling bestaat in een werkjaar, dan vormt de combinatie afdeling-werkjaar een ''Afdelingsjaar''.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'Afdeling'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'geboortejaar ''tot en met''' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'AfdelingsJaar', @level2type=N'COLUMN',@level2name=N'GeboorteJaarTot'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'geboortejaar ''vanaf''' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'AfdelingsJaar', @level2type=N'COLUMN',@level2name=N'GeboorteJaarVan'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'De velden ''van'' en ''tot'' geven aan welke leden er standaard in deze afdeling terecht komen.

Als voor ''ketiranten 2007-2008'' van=1991 en tot=1993, dan zullen leden die geboren zijn van 1991 t/m 1993 automatisch ''ketirant'' worden als ze zich aansluiten voor 2007-2008' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'AfdelingsJaar'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Een ''Kind'' is een lid dat geen leiding/kadermedewerker is.  Een kind is altijd gekoppeld aan een ''afdelingsjaar'' voor het werkjaar waarin het lid is.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'Kind'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Een leid(st)er is een speciaal geval van een lid.  Gedurende een werkjaar kan een leid(st)er aan meerdere afdelingsjaren gekoppeld zijn.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'Leiding'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Voor het einde van de instapperiode kunnen leden verwijderd worden.  Dat wordt hier aangevinkt.  Het record wordt niet verwijderd, om op die manier te kunnen onthouden dat de persoon al kandidaatlid is geweest.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'Lid', @level2type=N'COLUMN',@level2name=N'Verwijderd'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Bij een nieuw werkjaar zijn er geen leden meer.  Maar aan de hand van wat er in ''VolgendWerkjaar'' staat, kan het programma aangeven of er nog leden zijn die waarschijnlijk nog ingeschreven moeten worden.

Als een lid van vorig werkjaar als toekomstperspectief ''KomtTerug'' heeft, dan herinnert het programma er aan dat die persoon nog aangesloten moet worden.  Is het toekomstperspectief ''KomtNietTerug'', dan zal het programma dat niet doen.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'Lid', @level2type=N'COLUMN',@level2name=N'VolgendWerkjaar'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Er worden enkel  facturen gemaakt voor dit lid als deze datum gepasseerd is.  Tot dat moment kan de groep beslissen om deze persoon uit te schrijven.  Daarna vanzelfsprekend niet meer.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'Lid', @level2type=N'COLUMN',@level2name=N'EindeInstapPeriode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zowel leiding als ''kinderen'' zijn leden.  Van zodra je als lid in de database zit, ben je ''aangesloten'', en kan je in principe een factuur krijgen.

Een lid is standaard verzekerd voor burgerlijke aansprakelijkheid.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'Lid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'De groepen kunnen ieder werkjaar naar hartelust zelf afdelingen uitvinden.  Maar elke afdeling moet gekoppeld zijn aan een ''officiele afdeling''.  De lijst van officiele afdelingen wordt nationaal beheerd.' , @level0type=N'SCHEMA',@level0name=N'lid', @level1type=N'TABLE',@level1name=N'OfficieleAfdeling'
GO
