use gap;

declare @adnr as integer; set @adnr=147015;
declare @datumVanaf as datetime; set @datumVanaf='2012-09-01'


--delete a 
select * from pers.persoon p 
join verz.persoonsverzekering pv on p.persoonid=pv.persoonid
where p.adnummer=@adnr and pv.van > @datumvanaf
