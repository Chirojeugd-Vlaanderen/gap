use gap
go

declare @werkjaar as integer; set @werkjaar=2012
declare @ctype as integer; set @ctype=3;			-- e-mail

		-- mensen met e-mailadres

		select distinct adnummer
		from lid.Lid gl
		join grp.GroepsWerkJaar ggwj on gl.GroepsWerkJaarID = ggwj.GroepsWerkJaarID
		join pers.GelieerdePersoon ggp on gl.GelieerdePersoonID = ggp.GelieerdePersoonID
		join pers.Persoon gp on gp.PersoonID = ggp.PersoonID
		join pers.CommunicatieVorm gcv on ggp.GelieerdePersoonID = gcv.GelieerdePersoonID and gcv.CommunicatieTypeID=@ctype
		where ggwj.WerkJaar=@werkjaar and gl.UitSchrijfDatum is null