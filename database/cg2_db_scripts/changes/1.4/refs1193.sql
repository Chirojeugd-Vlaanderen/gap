use gap
go
create view diag.vCorresponderendeLeden as
select l.LidID as GapLidID, kipl.ID as KipLidID, gwj.WerkJaar as WerkJaar
from lid.Lid l
join grp.GroepsWerkJaar gwj on l.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join pers.GelieerdePersoon gp on l.GelieerdePersoonID = gp.GelieerdePersoonID
join grp.Groep g on gp.GroepID = g.GroepID
join pers.Persoon p on gp.PersoonID = p.PersoonID
join kipadmin.grp.ChiroGroep kipcg on kipcg.StamNr = g.Code collate SQL_Latin1_General_CP1_CI_AS
join kipadmin.lid.Lid kipl on kipl.WerkJaar = gwj.WerkJaar and kipl.AdNr=p.AdNummer and kipl.GroepID=kipcg.GroepID

go

create view diag.vFunctieProblemen as
select g.Code, p.AdNummer, p.VoorNaam, p.Naam, prob.* from
(
	-- nat fun wel in GAP, niet in KIP

	select cl.GapLidID, f.Code as GapFunctieCode, f.Naam as GapFunctieOmschrijving, null as KipFunctieCode, null as KipFunctieOmschrijving
	from diag.vCorresponderendeLeden cl
	join lid.LidFunctie lf on cl.GapLidID = lf.LidID
	join lid.Functie f on lf.FunctieID = f.FunctieID
	join kipadmin.dbo.HuidigWerkJaar kipwj on cl.werkJaar = kipwj.WerkJaar
	left outer join (
		select kiphf.leidKad as KipLidID, kipf.ID as KipFunctieID, kipf.GapID as GapFunctieID  
		from kipadmin.dbo.kipHeeftFunctie kiphf
		join kipadmin.dbo.kipFunctie kipf on kiphf.Functie = kipf.ID
		where kipf.GapID is not null ) kiphnf on kiphnf.KipLidID = cl.KipLidID and kiphnf.GapFunctieID = f.FunctieID
	where f.IsNationaal = 1 and kiphnf.KipFunctieID is null

	union
	-- nat fun wel in KIP, niet in GAP

	select cl.GapLidID, null as GapFunctieCode, null as GapFunctieOmschrijving, kipf.Code as KipFunctieCode, kipf.Naam as KipFunctieOmschrijving
	from diag.vCorresponderendeLeden cl
	join kipadmin.dbo.kipHeeftFunctie kiphf on cl.KipLidID = kiphf.LeidKad
	join kipadmin.dbo.kipFunctie kipf on kiphf.Functie = kipf.ID
	join kipadmin.dbo.HuidigWerkJaar kipwj on cl.werkJaar = kipwj.WerkJaar
	left outer join (
		select lf.LidID as GapLidID, f.FunctieID as GapFunctieID
		from lid.LidFunctie lf
		join lid.Functie f on lf.FunctieID=f.FunctieID
		where f.IsNationaal=1
	) nf on nf.GapLidID = cl.GapLidID and nf.GapFunctieID = kipf.GapID
	where kipf.GapID is not null and nf.GapFunctieID is null
) prob 
join lid.Lid l on prob.GapLidID = l.LidID
join pers.GelieerdePersoon gp on l.GelieerdePersoonID = gp.GelieerdePersoonID
join pers.Persoon p on gp.PersoonID = p.PersoonID
join grp.Groep g on gp.GroepID = g.GroepID
