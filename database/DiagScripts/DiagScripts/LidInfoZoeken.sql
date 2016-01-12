use gap_local;

select p.naam,p.voornaam,p.adnummer,l.lidid,g.code,gwj.werkjaar,l.UitschrijfDatum,l.IsAangesloten
from lid.lid l
join grp.groepswerkjaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
join grp.groep g on gwj.groepid = g.groepid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
where p.adnummer=172157
order by gwj.WerkJaar desc
