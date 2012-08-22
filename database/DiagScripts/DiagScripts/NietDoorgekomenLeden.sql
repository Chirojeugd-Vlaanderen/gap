-- op zoek naar leden die niet doorkwamen naar Kipadmin

use gap

select g.code, p.adnummer, p.naam, p.voornaam
from lid.lid l 
join grp.groepswerkjaar gwj on l.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
join grp.groep g on g.groepid = gwj.groepid
left outer join kipadmin.lid.lid kl on kl.adnr = p.adnummer and kl.werkjaar=gwj.werkjaar and kl.aansl_nr > 0
where l.NonActief=0 and  gwj.werkjaar=2012 and kl.adnr is null
