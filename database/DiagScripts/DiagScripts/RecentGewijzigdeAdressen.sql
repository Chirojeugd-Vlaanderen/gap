use gap

select * from pers.persoonsadres pa
join pers.gelieerdepersoon gp on pa.persoonsadresid=gp.voorkeursadresid
join pers.persoon p on gp.persoonid=p.persoonid
where p.adnummer is not null
order by pa.versie desc