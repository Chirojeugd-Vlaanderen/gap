use gap

declare @werkjaar as integer; set @werkjaar=2013
declare @nu as datetime; set @nu=getdate();

update r
set r.facturerenvanaf = tmp2.eindeperiode
from kipadmin.dbo.rekening r
join
(
select tmp.nr,max(eindeinstapperiode) as eindeperiode,max(facturerenvanaf) as factuurvanaf
from
(
select g.code, p.adnummer, kr.nr, p.naam, p.voornaam, kp.adnr, l.eindeinstapperiode, kr.facturerenvanaf, l.uitschrijfdatum
from lid.lid l 
join grp.groepswerkjaar gwj on l.groepswerkjaarid = gwj.groepswerkjaarid
join pers.gelieerdepersoon gp on l.gelieerdepersoonid = gp.gelieerdepersoonid
join pers.persoon p on gp.persoonid = p.persoonid
join grp.groep g on g.groepid = gwj.groepid
join kipadmin.lid.lid kl on kl.adnr = p.adnummer and kl.werkjaar=gwj.werkjaar and kl.aansl_nr > 0
join kipadmin.dbo.kippersoon kp on p.adnummer = kp.adnr
join kipadmin.grp.chirogroep kcg on kcg.Stamnr=g.code collate latin1_general_ci_ai and kcg.groepid=kl.groepid
join kipadmin.lid.aansluiting ka on ka.werkjaar=kl.werkjaar and ka.volgnummer=kl.aansl_nr and ka.groepid=kcg.groepid
join kipadmin.dbo.rekening kr on ka.rekeningid=kr.nr
where (
  l.uitschrijfdatum is not null and l.uitschrijfdatum < l.eindeinstapperiode or  -- al uitgeschreven
  l.eindeinstapperiode > @nu)           -- gaan mogelijk nog uitschrijven
and l.eindeinstapperiode > kr.facturerenvanaf -- DIT VEROORZAAKT HET PROBLEEM
and  gwj.werkjaar=@werkjaar
and kr.doorgeboe='n'
) tmp group by nr
)tmp2 on r.nr = tmp2.nr