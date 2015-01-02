-- ontbrekende telefoonnummers
-- let op: werkjaar hardgecodeerd in where-clause

use gap
go


--insert into kipadmin.dbo.kipcontactinfo(adnr, ContactTypeId, info, volgnr, geenmailings)
--select g.code, p.adnummer,cv.Nummer, cv.IsVoorOptIn, p.naam, p.voornaam, kp.adnr
select p.AdNummer
	, cv.CommunicatieTypeID
	, cv.Nummer as info
	, case when x.offset is null then row_number() over (partition by p.adnummer order by cv.communicatievormid desc) 
	else row_number() over (partition by p.adnummer order by cv.communicatievormid desc) + x.offset end as volgnummer
	, 1 - cv.IsVoorOptIn as GeenMailings
from lid.lid l 
join grp.groepswerkjaar gwj on l.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
join pers.communicatievorm cv on cv.GelieerdePersoonID = gp.GelieerdePersoonID and cv.CommunicatieTypeID = 1 -- telefoonnrs
join grp.groep g on g.groepid = gwj.groepid
-- inner join met persoon, want we bekijken enkel de ad-nummers die we daadwerkelijk vinden in kipadmin :-)
join kipadmin.dbo.kippersoon kp on p.adnummer = kp.adnr
left outer join kipadmin.dbo.kipcontactinfo kci on kci.AdNr = kp.AdNr and core.ufnEnkelCijfers(kci.info  collate latin1_general_ci_ai) = core.ufnEnkelCijfers(cv.Nummer)
left outer join 
(
select adnr, max(volgnr) as offset from kipadmin.dbo.kipcontactinfo where contacttypeid=1 group by adnr
) x on p.AdNummer = x.AdNr
where l.NonActief=0 and  gwj.werkjaar=2014 and kci.AdNr is null
