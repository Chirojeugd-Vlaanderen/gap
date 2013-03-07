use gap

select p.naam,p.voornaam,g.code,g.naam from pers.persoon p
join pers.gelieerdePersoon gp on p.persoonid = gp.persoonid
join grp.groep g on gp.groepid=g.groepid
where p.adnummer=210357
