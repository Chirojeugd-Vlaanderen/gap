use gap
go

declare @werkjaar as integer; set @werkjaar=2012
select xx.*, isnull(y.stamnr,''), z.evenement from
(
	select adnummer, min(stamnr) as stamnr,
		min(naam) as naam,min(voornaam) as voornaam, 
		min(straatnaam) as gapstraat , min(kipstraat) as kipstraat, 
		min(huisnr) as gapnr, min(bus) as gapbus, min(kiphuisnr) as kipnr, 
		min(postnr) as gappostnr, min(postcode) as gappostcode, min(kippostnr) as kippostnr, 
		min(woonplaats) as gapgemeente, min(kipgemeente) as kipgemeente, 
		min(land) as gapland, min(kipland) as kipland, max(versie) as versie 
	from
	(
		select adnummer,code as stamnr,kp.naam, kp.voornaam, 
			straatnaam, straat as kipstraat, 
			huisnr, bus, nr as kiphuisnr, 
			gpa.postnr, ka.postnr as kippostnr, postcode, 
			woonplaats, gemeente as kipgemeente, 
			gpa.land, ka.land as kipland, gpa.versie
		from adr.vPersoonsAdresInfo gpa
		join lid.Lid gl on gpa.GelieerdePersoonID = gl.GelieerdePersoonID and gl.uitschrijfdatum is null
		join grp.GroepsWerkJaar ggwj on gl.GroepsWerkJaarID = ggwj.GroepsWerkJaarID
		join kipadmin.dbo.kipPersoon kp on gpa.adnummer=kp.adnr
		join grp.groep gg on gg.groepid=ggwj.groepid
		left outer join kipadmin.dbo.kipWoont kw on kp.adnr=kw.adnr and kw.volgnr=1
		left outer join kipadmin.dbo.kipAdres ka on kw.adresid = ka.adresid
		where ggwj.WerkJaar=@werkjaar
		and
		(
			ka.AdresID is null and gpa.straatnaam is not null
			or gpa.StraatNaam <> ka.Straat collate Latin1_General_CI_AI
			or ka.PostNr not like convert(varchar(10),gpa.Postnr) + '%' collate Latin1_General_CI_AI
		)
	) x
	group by adnummer
) xx
-- adres in andere groep?
left outer join (

		select adnummer,g.code as stamnr
		from adr.vPersoonsAdresInfo gpa
		join pers.gelieerdepersoon gp on gpa.gelieerdepersoonid=gp.gelieerdepersoonid
		join grp.groep g on gp.groepid=g.groepid
		join kipadmin.dbo.kipPersoon kp on gpa.adnummer=kp.adnr
		left outer join kipadmin.dbo.kipWoont kw on kp.adnr=kw.adnr and kw.volgnr=1
		left outer join kipadmin.dbo.kipAdres ka on kw.adresid = ka.adresid
		where
		(
			ka.AdresID is null and gpa.straatnaam is null
			or gpa.StraatNaam = ka.Straat collate Latin1_General_CI_AI
			and ka.PostNr like convert(varchar(10),gpa.Postnr) + '%' collate Latin1_General_CI_AI
		)
) y on xx.adnummer = y.adnummer
-- join aan websiteevenementen
left outer join 
(
select verantwoordelijke, max(evenement) as evenement
from kipadmin.dbo.kipinschrijving
group by verantwoordelijke
) z on z.verantwoordelijke = xx.adnummer and z.evenement >= 76 and z.evenement <= 83


--order by x.adnummer