use kipadmin
go

declare @werkjaar as integer; set @werkjaar=2013

--select r.factuurnr, r.stamnr,
--(ribbelsj+ribbelsm+speelclubj+speelclubm+rakwisj+rakwism+titosj+titosm+ketisj+ketism+aspisj+aspism+speciaalj+speciaalm+leidingj+leidingm+vb+proost+freelance) as aantal,
--r.bedrag_eur
--from lid.aansluiting a 
--join rekening r on a.rekeningid = r.nr
--where a.werkjaar=@werkjaar and r.doorgeboe='j' and r.bedrag_eur<>(ribbelsj+ribbelsm+speelclubj+speelclubm+rakwisj+rakwism+titosj+titosm+ketisj+ketism+aspisj+aspism+speciaalj+speciaalm+leidingj+leidingm+vb+proost+freelance)*10

select max(r.stamnr), max(r.factuurnr)
,max(ribbelsj+ribbelsm+speelclubj+speelclubm+rakwisj+rakwism+titosj+titosm+ketisj+ketism+aspisj+aspism+speciaalj+speciaalm+leidingj+leidingm+vb+proost+a.freelance) as factuuraantal
,count(*) as effectief_aantal
from lid.lid l
join lid.aansluiting a on l.groepid=a.groepid and l.aansl_nr=a.volgnummer and l.werkjaar=a.werkjaar
join rekening r on r.nr = a.rekeningid
where l.werkjaar=@werkjaar and r.doorgeboe='j'
group by a.aansluitingid
having max(ribbelsj+ribbelsm+speelclubj+speelclubm+rakwisj+rakwism+titosj+titosm+ketisj+ketism+aspisj+aspism+speciaalj+speciaalm+leidingj+leidingm+vb+proost+a.freelance)<>count(*)

