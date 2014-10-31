-- op zoek naar leden die niet doorkwamen naar Kipadmin

use gap

select g.code, p.adnummer,l.LidID, p.naam, p.voornaam, kp.adnr
from lid.lid l 
join grp.groepswerkjaar gwj on l.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
join grp.groep g on g.groepid = gwj.groepid
join kipadmin.grp.chirogroep kcg2 on kcg2.Stamnr=g.code collate latin1_general_ci_ai
join kipadmin.grp.Groep kg2 on kg2.GroepID=kcg2.groepid
left outer join kipadmin.lid.lid kl on kl.adnr = p.adnummer and kl.werkjaar=gwj.werkjaar and kl.aansl_nr > 0 and kl.GroepID=kcg2.groepid
left outer join kipadmin.dbo.kippersoon kp on p.adnummer = kp.adnr
where l.NonActief=0 and  gwj.werkjaar=2014 and kl.adnr is null and kg2.StopDatum is null
order by g.Code 

----alles
--select * from kipadmin.lid.Lid l join grp.Groep g on l.GroepID=g.GroepID where g.Code<>'nat/0000'
--and l.AANSL_NR>0 and l.werkjaar=2014

-- verwijder vis noch vlees (kind noch leiding)

--delete l
----select g.GroepID,g.code, gwj.WerkJaar, k.kindID, k.afdelingsJaarID, le.leidingID 
--from lid.Lid l 
--left outer join lid.Kind k on l.LidID=k.kindid
--left outer join lid.Leiding le on l.LidID=le.leidingID
--join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
--join grp.Groep g on gwj.GroepID=g.groepid
--where gwj.WerkJaar=2014 and k.kindID is null and le.leidingID is null
----where l.LidID>=714964 and l.LidID<=714971
