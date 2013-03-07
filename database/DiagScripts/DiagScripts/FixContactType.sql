use gap
go

declare @werkjaar as integer; set @werkjaar=2012
declare @ctype as integer; set @ctype=1;			-- e-mail

		insert into kipadmin.dbo.kipContactInfo(adnr,contacttypeid,info,geenmailings,volgnr)
		select 
			adnummer, @ctype as contacttypeid, gcv.nummer as info, 1 - gcv.IsVoorOptIn as geenmailings, 
			isnull(vn.volgnr,0) + row_number() over (partition by adnummer order by IsVoorOptIn desc) as volgnr
		from lid.Lid gl
		join grp.GroepsWerkJaar ggwj on gl.GroepsWerkJaarID = ggwj.GroepsWerkJaarID
		join pers.GelieerdePersoon ggp on gl.GelieerdePersoonID = ggp.GelieerdePersoonID
		join pers.Persoon gp on gp.PersoonID = ggp.PersoonID
		join pers.CommunicatieVorm gcv on ggp.GelieerdePersoonID = gcv.GelieerdePersoonID and gcv.CommunicatieTypeID=@ctype
		join kipadmin.dbo.kipPersoon kp on kp.adnr = gp.adnummer -- join nodig; enkel mensen ook in KIP zijn relevant
		left outer join kipadmin.dbo.kipContactinfo kci on kci.AdNr=gp.Adnummer and kci.contacttypeid=gcv.CommunicatieTypeID and kci.info = gcv.nummer collate Latin1_General_CI_AI
		left outer join
		(
			select adnr,contacttypeid,max(volgnr) as volgnr from kipadmin.dbo.kipContactInfo
			group by adnr,contacttypeid
		) vn on vn.adnr = adnummer and vn.contacttypeid=@ctype
		where ggwj.WerkJaar=@werkjaar and gl.UitSchrijfDatum is null
		and kci.info is null
		

