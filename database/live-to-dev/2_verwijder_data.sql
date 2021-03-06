-- Copyright 2012-2014, 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
-- top-level directory of this distribution, and at
-- https://gapwiki.chiro.be/copyright
-- 
-- Licensed under the Apache License, Version 2.0 (the "License");
-- you may not use this file except in compliance with the License.
-- You may obtain a copy of the License at
-- 
--     http://www.apache.org/licenses/LICENSE-2.0
-- 
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS,
-- WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
-- See the License for the specific language governing permissions and
-- limitations under the License.

-- verwijder heel wat data, om de dump kleiner te krijgen
-- we gaan dat doen na het shuffelen, omdat shuffelen beter zal werken op veel data

declare @bijhoudenvanaf as integer; set @bijhoudenvanaf=(select YEAR(getdate())-3);

-- een willekeurig gewest om alle behouden groepen aan te koppelen

declare @idVanHetGewest as integer; set @idVanHetGewest=1028;

-- behoud alleen gegevens van deze groepen:

declare @groepids table(GroepID int);

-- random groepids

insert into @groepids values(@idVanHetGewest);
insert into @groepids values(1060);
insert into @groepids values(970);
insert into @groepids values(77);
insert into @groepids values(702);
insert into @groepids values(518);
insert into @groepids values(608);
insert into @groepids values(892);
insert into @groepids values(312);
insert into @groepids values(302);

truncate table logging.bericht;

delete lf
from lid.Lid l 
join lid.LidFunctie lf on l.LidID = lf.LidID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;


delete lia
from lid.Lid l 
join lid.LeidingInAfdelingsJaar lia on l.LidID = lia.LeidingID
join lid.Leiding le on l.LidID = le.leidingID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;


delete le
from lid.Lid l 
join lid.Leiding le on l.LidID = le.leidingID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;


delete k 
from lid.Lid l 
join lid.Kind k on l.LidID = k.kindID
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;


delete l 
from lid.Lid l 
join grp.GroepsWerkJaar gwj on l.GroepsWerkjaarID=gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;

update u
set u.ContactDeelnemerID = null
from biv.Uitstap u
join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID = gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;

delete d
from biv.Uitstap u
join biv.Deelnemer d on u.UitstapID = d.UitstapID
join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID = gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;

delete u
from biv.Uitstap u
join grp.GroepsWerkJaar gwj on u.GroepsWerkJaarID = gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;

delete aj
from lid.AfdelingsJaar aj
join grp.GroepsWerkJaar gwj on aj.GroepsWerkJaarID = gwj.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;

delete ab
from grp.GroepsWerkJaar gwj
join abo.Abonnement ab on gwj.GroepsWerkJaarID = ab.GroepsWerkJaarID
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;


delete gwj
from grp.GroepsWerkJaar gwj
left outer join @groepids ids on gwj.GroepID = ids.GroepID
where gwj.WerkJaar < @bijhoudenvanaf
or ids.GroepID is null;

delete pc
from pers.GelieerdePersoon gp
join pers.PersoonsCategorie pc on pc.GelieerdePersoonID = gp.GelieerdePersoonID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
left outer join @groepids ids on gp.GroepID = ids.GroepID
where l.LidID is null
or ids.GroepID is null;

delete ab
from pers.GelieerdePersoon gp
join abo.Abonnement ab on gp.GelieerdePersoonID = ab.GelieerdePersoonID
left outer join @groepids ids on gp.GroepID = ids.GroepID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null
or ids.GroepID is null;


delete cm
from pers.GelieerdePersoon gp
join pers.CommunicatieVorm cm on gp.GelieerdePersoonID=cm.GelieerdePersoonID
left outer join @groepids ids on gp.GroepID = ids.GroepID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null
or ids.GroepID is null;

update u
set u.ContactDeelnemerID=null
from pers.GelieerdePersoon gp
join biv.Deelnemer dln on gp.GelieerdePersoonID=dln.GelieerdePersoonID
join biv.Uitstap u on dln.UitstapID=u.UitstapID
left outer join @groepids ids on gp.GroepID = ids.GroepID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null
or ids.GroepID is null;


delete dln
from pers.GelieerdePersoon gp
join biv.Deelnemer dln on gp.GelieerdePersoonID=dln.GelieerdePersoonID
left outer join @groepids ids on gp.GroepID = ids.GroepID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null
or ids.GroepID is null;

delete gp
from pers.GelieerdePersoon gp
left outer join @groepids ids on gp.GroepID = ids.GroepID
left outer join lid.Lid l on gp.GelieerdePersoonID=l.GelieerdePersoonID
where l.LidID is null
or ids.GroepID is null;

delete c
from core.Categorie c 
left outer join pers.PersoonsCategorie pc on c.CategorieID = pc.CategorieID
where pc.GelieerdePersoonID is null

-- Alle gebruikersrechten weg.
delete gr
from auth.GebruikersRechtV2 gr

delete pa
from pers.Persoon p
join pers.PersoonsAdres pa on p.PersoonID=pa.PersoonID
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

delete a
from grp.ChiroGroep cg
join lid.Afdeling a on cg.ChiroGroepID=a.ChiroGroepID
left outer join @groepids ids on cg.ChiroGroepID=ids.groepID
where ids.GroepID is null;

update cg 
set cg.KaderGroepID = @IdVanHetGewest
from grp.ChiroGroep cg

-- in testdata geen hierearchie kadergroepen; we hebben toch geen verbonden
update kg
set kg.ParentID = null
from grp.KaderGroep kg

delete cg
from grp.ChiroGroep cg
left outer join @groepids ids on cg.ChiroGroepID=ids.groepID
where ids.GroepID is null;

delete kg
from grp.KaderGroep kg
left outer join @groepids ids on kg.KaderGroepID=ids.groepID
where ids.GroepID is null;

delete p
from grp.Groep g
join biv.plaats p on g.groepid=p.groepid
left outer join @groepids ids on g.GroepID=ids.groepID
where ids.GroepID is null;

delete f
from grp.Groep g
join lid.functie f on g.groepid=f.groepid
left outer join @groepids ids on g.GroepID=ids.groepID
where ids.GroepID is null;

delete g
from grp.Groep g
left outer join @groepids ids on g.GroepID=ids.groepID
where ids.GroepID is null;


-- verwijder overbodige adresgegevens

update grp.Groep set adresID = null;

delete a
from adr.BelgischAdres a
left outer join pers.PersoonsAdres pa on a.BelgischAdresID = pa.AdresID
left outer join biv.Plaats p on a.BelgischAdresID = p.AdresID
where pa.PersoonsAdresID is null and p.PlaatsID is null


delete a
from adr.BuitenlandsAdres a
left outer join pers.PersoonsAdres pa on a.BuitenlandsAdresID = pa.AdresID
left outer join biv.Plaats p on a.BuitenlandsAdresID = p.AdresID
where pa.PersoonsAdresID is null and p.PlaatsID is null

delete a
from adr.Adres a
left outer join pers.PersoonsAdres pa on a.AdresID = pa.AdresID
left outer join biv.Plaats p on a.AdresID = p.AdresID
where pa.PersoonsAdresID is null and p.PlaatsID is null

-- we halen ook irrelevante straten weg, om de dump kleiner te maken.

delete s
from adr.StraatNaam s
left outer join adr.BelgischAdres a on s.StraatNaamID = a.StraatNaamID
where a.BelgischAdresID is null


-- shrink data file en transaction log

dbcc shrinkfile(gap1rc2_log, 128, NOTRUNCATE)
dbcc SHRINKFILE ('gap1rc2' , 240, NOTRUNCATE)
