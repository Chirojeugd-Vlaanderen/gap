use gap;

select gwj.werkjaar, p.adnummer, gwj.groepid, l.uitschrijfdatum  from lid.lid l join pers.gelieerdepersoon gp on l.GelieerdePersoonID = gp.GelieerdePersoonID
join pers.persoon p on p.persoonid=gp.persoonid
join grp.groepswerkjaar gwj on l.groepswerkjaarid=gwj.groepswerkjaarid
where p.adnummer=114390