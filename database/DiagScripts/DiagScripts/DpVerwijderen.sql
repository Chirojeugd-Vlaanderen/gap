use gap

declare @adnr as integer; set @adnr= 164751;
declare @datumVanaf as datetime; set @datumVanaf='2012-09-01'


select *
--delete a 
from pers.persoon p 
join pers.gelieerdePersoon gp on p.persoonid=gp.persoonid
join abo.Abonnement a on a.gelieerdepersoonid=gp.gelieerdepersoonid
where p.adnummer=@adnr and a.aanvraagdatum > @datumvanaf

