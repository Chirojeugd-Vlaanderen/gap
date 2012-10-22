use gap
go

select * from adr.vPersoonsAdresInfo gpa
join lid.Lid gl on gpa.GelieerdePersoonID = gl.GelieerdePersoonID
join grp.GroepsWerkJaar ggwj on gl.GroepsWerkJaarID = ggwj.GroepsWerkJaarID
join kipadmin.dbo.kipPersoon kp on gpa.adnummer=kp.adnr
join grp.Groep gg on ggwj.groepid=gg.groepid
left outer join kipadmin.dbo.kipWoont kw on kp.adnr=kw.adnr and kw.volgnr=1
left outer join kipadmin.dbo.kipAdres ka on kw.adresid = ka.adresid
left outer join kipadmin.dbo.kipInschrijving ki on ki.verantwoordelijke = kp.adnr and ki.evenement >= 76 and ki.evenement <= 83
where ggwj.WerkJaar=2012
and ki.id is null -- negeer inschrijvers aspitrant, startdag. Gaven hun adres in via ws.
and
(ka.AdresID is null and gpa.straatnaam is not null
or gpa.StraatNaam <> ka.Straat collate Latin1_General_CI_AI
	or gpa.WoonPlaats <> ka.Gemeente collate Latin1_General_CI_AI)
order by gpa.versie