use gap
go

create view diag.vVerlorenAdressen as
select p.AdNr, gapg.Code as StamNr, p.VoorNaam, p.Naam, gapgp.GelieerdePersoonID, gapgp.VoorkeursAdresID
from pers.Persoon gapp
join kipadmin.dbo.kipPersoon p on gapp.AdNummer = p.Adnr
left outer join kipadmin.dbo.kipWoont w on p.Adnr=w.Adnr
join pers.GelieerdePersoon gapgp on gapp.PersoonID = gapgp.PersoonID 
-- we gaan via het voorkeursadres van de gelieerde persoon, want dat is het adres dat
-- naar Kipadmin komt.
join adr.Adres gapa on gapgp.VoorkeursAdresID = gapa.AdresID
-- inner join hierboven, zodat de gp's in gap zonder voorkeursadres niet meegenomen worden
join grp.Groep gapg on gapgp.GroepID = gapg.GroepID
where (w.WoontID is null)
