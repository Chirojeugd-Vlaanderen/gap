-- verwijder oude dingen

use gap_dev;

declare @bijhoudenvanaf as integer; set @bijhoudenvanaf=2011;


delete lf
from lid.Lid l 
join lid.LidFunctie lf on l.LidID = lf.LidID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;


delete lia
from lid.Lid l 
join lid.LeidingInAfdelingsJaar lia on l.LidID = lia.LeidingID
join lid.Leiding le on l.LidID = le.leidingID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;

delete le
from lid.Lid l 
join lid.Leiding le on l.LidID = le.leidingID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;


delete k 
from lid.Lid l 
join lid.Kind k on l.LidID = k.kindID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;


delete l 
from lid.Lid l 
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;

update u
set u.ContactDeelnemerID = null
from biv.Uitstap u
join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID = gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;

delete d
from biv.Uitstap u
join biv.Deelnemer d on u.UitstapID = d.UitstapID
join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID = gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;

delete u
from biv.Uitstap u
join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID = gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;

delete aj
from lid.AfdelingsJaar aj
join grp.GroepsWerkJaar gwj on aj.GroepsWerkJaarID = gwj.GroepsWerkJaarID
where gwj.WerkJaar < @bijhoudenvanaf;

delete gwj
from grp.GroepsWerkJaar gwj
where gwj.WerkJaar < @bijhoudenvanaf;

delete pc
from pers.GelieerdePersoon gp
join pers.PersoonsCategorie pc on pc.GelieerdePersoonID = gp.GelieerdePersoonID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null

delete ab
from pers.GelieerdePersoon gp
join abo.Abonnement ab on gp.GelieerdePersoonID = ab.GelieerdePersoonID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null


delete cm
from pers.GelieerdePersoon gp
join pers.CommunicatieVorm cm on gp.GelieerdePersoonID=cm.GelieerdePersoonID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null

update u
set u.ContactDeelnemerID=null
from pers.GelieerdePersoon gp
join biv.Deelnemer dln on gp.GelieerdePersoonID=dln.GelieerdePersoonID
join biv.Uitstap u on dln.UitstapID=u.UitstapID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null


delete dln
from pers.GelieerdePersoon gp
join biv.Deelnemer dln on gp.GelieerdePersoonID=dln.GelieerdePersoonID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null

delete gp
from pers.GelieerdePersoon gp
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null

delete c
from core.Categorie c 
left outer join pers.PersoonsCategorie pc on c.CategorieID = pc.CategorieID
where pc.GelieerdePersoonID is null

delete pa
from pers.Persoon p
join pers.PersoonsAdres pa on p.PersoonID=pa.PersoonID
left outer join pers.GelieerdePersoon gp on p.PersoonID=gp.PersoonID
where gp.GelieerdePersoonID is null

delete gs
from pers.Persoon p
join auth.GavSchap gs on p.PersoonID=gs.PersoonID
left outer join pers.GelieerdePersoon gp on p.PersoonID=gp.PersoonID
where gp.GelieerdePersoonID is null

delete pv
from pers.Persoon p
join verz.PersoonsVerzekering pv on p.PersoonID=pv.PersoonID
left outer join pers.GelieerdePersoon gp on p.PersoonID=gp.PersoonID
where gp.GelieerdePersoonID is null


delete p
from pers.Persoon p
left outer join pers.GelieerdePersoon gp on p.PersoonID=gp.PersoonID
where gp.GelieerdePersoonID is null

delete gr
from auth.Gav gv
join auth.GebruikersRecht gr on gv.GavID = gr.gavid
left outer join auth.GavSchap gs on gv.GavID=gs.gavid
where gs.PersoonID is null


delete gv
from auth.Gav gv
left outer join auth.GavSchap gs on gv.GavID=gs.gavid
where gs.PersoonID is null


-- shrink data file en transaction log

dbcc shrinkfile(gap1rc2_log, 128, NOTRUNCATE)
dbcc SHRINKFILE ('gap1rc2' , 240, NOTRUNCATE)
