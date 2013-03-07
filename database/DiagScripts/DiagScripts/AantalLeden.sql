select distinct adnummer 
from grp.groepswerkjaar gwj 
join lid.lid l on l.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid=gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
where gwj.werkjaar=2012 and l.uitschrijfdatum is null


