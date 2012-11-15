use gap

declare @stamnr as varchar(10); set @stamnr='bg /0609';
declare @werkjaar as int; set @werkjaar=2012
declare @aanslnr as int; set @aanslnr=1


select gp.adnummer,gp.naam, gp.voornaam,gl.uitschrijfdatum,gl.lidid
from kipadmin.dbo.rekening kr
join kipadmin.grp.chirogroep kcg on kr.stamnr=kcg.stamnr
join kipadmin.lid.lid kl on kl.groepid=kcg.groepid and kl.werkjaar=kr.werkjaar and kl.aansl_nr=kr.verwijsnr
join grp.groep gg on gg.code=@stamnr
join grp.groepswerkjaar ggwj on gg.groepid=ggwj.groepid and ggwj.werkjaar=@werkjaar
join pers.persoon gp on kl.adnr=gp.adnummer
join pers.gelieerdepersoon ggp on ggp.persoonid=gp.persoonid and ggp.groepid=gg.groepid
left outer join lid.lid gl on gl.gelieerdepersoonid=ggp.gelieerdepersoonid and gl.groepswerkjaarid=ggwj.groepswerkjaarid
where kr.stamnr=@stamnr and kr.werkjaar=@werkjaar and kr.rek_bron='AANSLUIT' and kr.verwijsnr=@aanslnr
order by uitschrijfdatum desc, lidid
 
 