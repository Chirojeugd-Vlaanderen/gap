use gap;

-- welke groepen hebben in kip werkjaar 2013-2014 en in gap niet\
-- (ie wj in gap wss ongedaan gemaakt)

select * from
(
	select max(l.werkjaar) as werkjaar, cg.StamNr
	from kipadmin.lid.lid l join kipadmin.grp.ChiroGroep cg on l.GroepID = cg.GroepID
	group by cg.StamNr
) kipwerkjaar
left outer join
(
	select max(gwj.werkjaar) as werkjaar, g.Code
	from grp.GroepsWerkJaar gwj join grp.Groep g on gwj.groepid = g.groepid
	group by g.Code
) gapwerkjaar
on kipwerkjaar.werkjaar = gapwerkjaar.werkjaar and kipwerkjaar.stamnr = gapwerkjaar.code collate Latin1_General_CI_AI
where kipwerkjaar.werkjaar=2013 and gapwerkjaar.werkjaar is null

