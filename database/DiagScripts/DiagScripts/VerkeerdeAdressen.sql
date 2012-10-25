use gap
go

select * from adr.vPersoonsAdresInfo gpa
join lid.Lid gl on gpa.GelieerdePersoonID = gl.GelieerdePersoonID
join grp.GroepsWerkJaar ggwj on gl.GroepsWerkJaarID = ggwj.GroepsWerkJaarID
join kipadmin.dbo.kipPersoon kp on gpa.adnummer=kp.adnr
join grp.Groep gg on ggwj.groepid=gg.groepid
left outer join kipadmin.dbo.kipWoont kw on kp.adnr=kw.adnr and kw.volgnr=1
left outer join kipadmin.dbo.kipAdres ka on kw.adresid = ka.adresid
---- join aan websiteevenementen
--left outer join kipadmin.dbo.kipInschrijving ki on ki.verantwoordelijke = kp.adnr and ki.evenement >= 76 and ki.evenement <= 83
---- join aan andere groep
--left outer join pers.gelieerdepersoon ggp2 on gpa.persoonid=ggp2.persoonid and gg.groepid<>ggp2.groepid
---- join aan ander adres
--left outer join pers.persoonsadres gpa2 on gpa2.persoonid=gpa.persoonid and gpa2.persoonsadresid<>gpa.persoonsadresid
where ggwj.WerkJaar=2012
--and ki.id is null -- negeer inschrijvers aspitrant, startdag. Gaven hun adres in via ws.
--and ggp2.gelieerdepersoonid is null -- negeer mensen gekoppeld aan meerdere groepen
--and gpa2.persoonsadresid is null -- negeer personen met meerdere adressen
and
(ka.AdresID is null and gpa.straatnaam is not null
or gpa.StraatNaam <> ka.Straat collate Latin1_General_CI_AI
	or ka.PostNr not like convert(varchar(10),gpa.Postnr) + '%' collate Latin1_General_CI_AI
)
order by gpa.versie desc