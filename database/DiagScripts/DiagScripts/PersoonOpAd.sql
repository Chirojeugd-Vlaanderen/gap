-- persoonsinfo opzoeken op basis van AD-nr

declare @AdNr as integer; set @AdNr=251388;

select g.code, p.adnummer, p.naam, p.voornaam, gp.gelieerdepersoonid
from pers.persoon p 
join pers.gelieerdepersoon gp on p.persoonid=gp.persoonid
join grp.groep g on gp.groepid=g.groepid
where adnummer=@AdNr
