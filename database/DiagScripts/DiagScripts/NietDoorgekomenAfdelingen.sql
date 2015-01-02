-- zoek leiding met 1 of meer afdelingen in gap
-- die geen afdeling heeft in kipadmin.
-- neem dan 1 afdeling over, voor de vorm :-P

select g.code, p.adnummer,l.LidID, p.naam, p.voornaam, oa.OfficieleAfdelingID, ka.afd_naam
--update kl set kl.afdeling1=ka.afd_naam
from lid.lid l 
join lid.leiding lei on l.lidid=lei.leidingid
join lid.leidinginafdelingsjaar laj on lei.leidingid=laj.leidingid
join lid.afdelingsjaar aj on aj.afdelingsjaarid = laj.AfdelingsJaarID
join lid.OfficieleAfdeling oa on aj.OfficieleAfdelingID = oa.OfficieleAfdelingID
join kipadmin.dbo.afdeling ka on ka.afd_id = oa.OfficieleAfdelingID
join grp.groepswerkjaar gwj on l.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
join grp.groep g on g.groepid = gwj.groepid
join kipadmin.grp.chirogroep kcg2 on kcg2.Stamnr=g.code collate latin1_general_ci_ai
join kipadmin.grp.Groep kg2 on kg2.GroepID=kcg2.groepid
left outer join kipadmin.lid.lid kl on kl.adnr = p.adnummer and kl.werkjaar=gwj.werkjaar and kl.aansl_nr > 0 and kl.GroepID=kcg2.groepid
where l.NonActief=0 and  gwj.werkjaar=2014 
--and kl.adnr is null 
and kg2.StopDatum is null
and oa.OfficieleAfdelingId <= 6		-- die speciale gevallen nemen we niet mee
and (kl.AFDELING1 is null or kl.AFDELING1 = '')
