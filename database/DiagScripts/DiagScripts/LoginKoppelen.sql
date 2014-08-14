-- koppel login aan persoon

declare @username as varchar(max); set @username='CHIROPUBLIC\vermeti';
declare @adnr as int; set @adnr=61553;

insert into auth.GavSchap(GavID, PersoonID)
select gav.GavID, p.PersoonID
from auth.gav gav, pers.persoon p
where gav.login=@username and p.adnummer=@adnr