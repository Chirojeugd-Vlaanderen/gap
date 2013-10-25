use kipadmin;

declare @stamnr as varchar(10); set @stamnr='og /3306' 
declare @werkjaar as int; set @werkjaar=2013;
declare @aansl_nr as int; set @aansl_nr=2;
declare @nieuwedatum as datetime; set @nieuwedatum='2013-11-10'

update r
set r.facturerenvanaf=@nieuwedatum
from grp.Chirogroep cg 
join lid.aansluiting a on cg.GroepID=a.GroepID
join dbo.rekening r on r.nr=a.rekeningid
where cg.stamnr=@stamnr and a.werkjaar=@werkjaar and a.volgnummer=@aansl_nr