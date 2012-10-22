-- op zoek naar abonnementen die niet doorkwamen naar Kipadmin

-- OPGELET! Het zou goed kunnen dat Ingrid die manueel verwijderde, omdat het groepsleiding
-- is die het abonnement niet meer wil. Hier blijven we dan maar af.

use gap


select *
from abo.abonnement a
join grp.groepswerkjaar gwj on a.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on a.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid=p.persoonid
left outer join kipadmin.znd.abonnement ka on ka.adnr=p.adnummer and ka.werkjaar=gwj.werkjaar
where gwj.werkjaar=2012 and ka.adnr is null


