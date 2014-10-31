-- op zoek naar uitschrijvingen die niet doorkwamen naar Kipadmin

use gap
go
--alter view diag.vVerlorenUitschrijvingen as
select g.code, p.adnummer, p.naam, p.voornaam, kp.adnr, kl.id as klid, l.UitschrijfDatum
from lid.lid l 
join grp.groepswerkjaar gwj on l.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
join grp.groep g on g.groepid = gwj.groepid
join kipadmin.lid.lid kl on kl.adnr = p.adnummer and kl.werkjaar=gwj.werkjaar and kl.aansl_nr > 0
join kipadmin.dbo.kippersoon kp on p.adnummer = kp.adnr
join kipadmin.grp.ChiroGroep kcg on kl.GroepID = kcg.GroepID and kcg.STAMNR=g.code collate Latin1_General_CI_AI
where (l.uitschrijfdatum < l.eindeinstapperiode) and  gwj.werkjaar=2014
