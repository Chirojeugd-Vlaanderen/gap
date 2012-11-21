use gap
go

declare @werkjaar as integer; set @werkjaar=2012
declare @ctype as integer; set @ctype=3;			-- e-mail

-- e-mailadressen in gap en niet in kipadmin

select x.adnummer, max(x.naam), max(x.voornaam), min(x.nummer), case when  max(y.adnummer) is null then 'nee' else 'ja' end from
(
		select adnummer, gp.naam, gp.voornaam, gcv.nummer, kci.info
		from lid.Lid gl
		join grp.GroepsWerkJaar ggwj on gl.GroepsWerkJaarID = ggwj.GroepsWerkJaarID
		join pers.GelieerdePersoon ggp on gl.GelieerdePersoonID = ggp.GelieerdePersoonID
		join pers.Persoon gp on gp.PersoonID = ggp.PersoonID
		join pers.CommunicatieVorm gcv on ggp.GelieerdePersoonID = gcv.GelieerdePersoonID and gcv.CommunicatieTypeID=@ctype
		join kipadmin.dbo.kipPersoon kp on kp.adnr = gp.adnummer -- join nodig; enkel mensen ook in KIP zijn relevant
		left outer join kipadmin.dbo.kipContactinfo kci on kci.AdNr=gp.Adnummer and kci.contacttypeid=gcv.CommunicatieTypeID and kci.info = gcv.nummer collate Latin1_General_CI_AI
		where ggwj.WerkJaar=@werkjaar and gl.UitSchrijfDatum is null
		and kci.info is null
) x
left outer join
(
		-- adnummers van personen met minstens 1 overeenkomstig e-mailadres in kip en gap

		select distinct adnummer
		from lid.Lid gl
		join grp.GroepsWerkJaar ggwj on gl.GroepsWerkJaarID = ggwj.GroepsWerkJaarID
		join pers.GelieerdePersoon ggp on gl.GelieerdePersoonID = ggp.GelieerdePersoonID
		join pers.Persoon gp on gp.PersoonID = ggp.PersoonID
		join pers.CommunicatieVorm gcv on ggp.GelieerdePersoonID = gcv.GelieerdePersoonID and gcv.CommunicatieTypeID=@ctype
		join kipadmin.dbo.kipContactinfo kci on kci.AdNr=gp.Adnummer and kci.contacttypeid=gcv.CommunicatieTypeID and kci.info = gcv.nummer collate Latin1_General_CI_AI
		where ggwj.WerkJaar=@werkjaar and gl.UitSchrijfDatum is null
) y on x.adnummer = y.adnummer
group by x.adnummer
order by max(y.adnummer) desc


