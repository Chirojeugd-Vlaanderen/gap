create procedure data.spGroepsWerkJaarVerwijderen(
@stamnr as varchar(10),
@werkjaar as int
)
as
begin

delete laj
from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
join lid.leidingInAfdelingsJaar laj on l.LidID = laj.LeidingID
where g.code=@stamnr and gwj.werkjaar=@werkjaar

delete lei
from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
join lid.leiding lei on l.lidID = lei.LeidingID
where g.code=@stamnr and gwj.werkjaar=@werkjaar

delete k
from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
join lid.kind k on l.lidID = k.kindID
where g.code=@stamnr and gwj.werkjaar=@werkjaar

delete lf
from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
join lid.lidFunctie lf on l.LidID = lf.LidID
where g.code=@stamnr and gwj.werkjaar=@werkjaar

delete l
from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
join lid.lid l on l.groepsWerkJaarID = gwj.GroepsWerkJaarID
where g.code=@stamnr and gwj.werkjaar=@werkjaar

delete aj
from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
join lid.Afdelingsjaar aj on gwj.GroepsWerkJaarID = aj.GroepsWerkJaarID
where g.code=@stamnr and gwj.werkjaar=@werkjaar

delete gwj
from grp.groep g join grp.groepsWerkJaar gwj on g.groepid = gwj.groepid
where g.code=@stamnr and gwj.werkjaar=@werkjaar
end