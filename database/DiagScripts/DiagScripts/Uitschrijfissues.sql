select p.adnummer, p.naam, p.voornaam, gwj.werkjaar, l.EindeInstapPeriode, l.UitschrijfDatum
from pers.persoon p
join pers.gelieerdepersoon gp on p.PersoonID = gp.PersoonID
join lid.lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
join grp.groepswerkjaar gwj on l.GroepsWerkjaarID = gwj.GroepsWerkJaarID
where p.AdNummer in (193252, 193254, 313897, 193269, 193274)
order by gwj.werkjaar desc
