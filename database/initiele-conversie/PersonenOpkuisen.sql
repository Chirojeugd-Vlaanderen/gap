create procedure data.spPersonenOpkuisen as
-- Doel: anomalieen in personendatabase zo veel mogelijk opkuisen

---- Verwijder personen zonder naam en voornaam.

delete cv
from pers.persoon p join pers.gelieerdepersoon gp on p.persoonid=gp.persoonid
join pers.communicatievorm cv on cv.gelieerdepersoonid = gp.gelieerdepersoonid
where p.naam='' and p.voornaam=''

update gp
set gp.voorkeursadresid=null
from pers.persoon p
join pers.persoonsadres pa on pa.persoonid = p.persoonid
join pers.gelieerdepersoon gp on gp.voorkeursadresid = pa.persoonsadresid
where p.naam='' and p.voornaam=''

delete pa
from pers.persoon p
join pers.persoonsadres pa on pa.persoonid = p.persoonid
where p.naam='' and p.voornaam=''

delete gp
from pers.persoon p join pers.gelieerdepersoon gp on p.persoonid=gp.persoonid
where p.naam='' and p.voornaam=''

delete from pers.persoon where naam='' and voornaam=''

-- verwijder ad-nummers '0'
-- komen normaal niet voor, maar zijn er in gesukkeld bij initiele import.

update pers.persoon set adnummer = null where adnummer=0

-- Koppel personen met eenzelfde ad-nummer aan elkaar.

-- Opzoeken dubbels en bewaren in temp table

create table #DubbelePersoon(oudID int not null, nieuwID int not null, primary key(nieuwID, oudID));

-- Dubbele personen zijn personen met hetzelfde ad-nummer

insert into #DubbelePersoon(oudID, nieuwID)
select distinct max(p.PersoonID), min(p.PersoonID) 
from pers.persoon p 
where adnummer is not null
group by adnummer
having count(*) > 1

select * from #DubbelePersoon

-- verleg koppelingen en verwijder

update gp
set gp.persoonid = dp.nieuwid
from pers.GelieerdePersoon gp join #DubbelePersoon dp on gp.persoonid = dp.oudid

update gp
set gp.voorkeursadresid = pb.persoonsadresid
from pers.PersoonsAdres pa join #dubbelepersoon dp on pa.persoonid = dp.oudid
join pers.PersoonsAdres pb on pb.persoonid = dp.nieuwid and pa.adresid = pb.adresid
join pers.GelieerdePersoon gp on gp.VoorKeursAdresid = pa.persoonsAdresID

delete pa
from pers.PersoonsAdres pa join #dubbelepersoon dp on pa.persoonid = dp.oudid
join pers.PersoonsAdres pb on pb.persoonid = dp.nieuwid and pa.adresid = pb.adresid

update pa
set pa.persoonid = dp.nieuwid
from pers.PersoonsAdres pa join #dubbelepersoon dp on pa.persoonid = dp.oudid

delete p
from pers.persoon p join #dubbelepersoon dp on p.persoonid = dp.oudid

-- drop temp table

drop table #dubbelepersoon
