-- STAP 2: anonimize data

USE gap_tst;

update cv
set nummer = cast(communicatievormid as varchar(50)) + '@example.org'
from pers.communicatievorm cv where cv.communicatietypeid in (3,5,6,8)

update cv
set nummer = left(right('000000000' + cast (20000000 +
cv.communicatievormid as varchar(50)),9),2) + '-' + 
left(right('000000000' + cast (20000000 + cv.communicatievormid as
varchar(50)),7),3) + ' ' +
left(right('000000000' + cast (20000000 + cv.communicatievormid as
varchar(50)),4),2) + ' ' +
right('000000000' + cast (20000000 + cv.communicatievormid as
varchar(50)),2)
from pers.communicatievorm cv where cv.communicatietypeid < 3

-- dit is ook niet écht willekeurig, maar hopelijk wel goed genoeg om gauw
-- te anonymiseren
--

update a1
set a1.straatnaamid = a2.straatnaamid, a1.woonplaatsid = a2.woonplaatsid
from
(select *, row_number() over (order by newid()) as volgorde
 from adr.belgischadres) a1 join
(select *, row_number() over (order by newid()) as volgorde
 from adr.belgischadres) a2 on a1.volgorde=a2.volgorde

update a1
set a1.postnummer=15500,a1.straat='Kípdorpskà',a1.woonplaats='Praha 5',
a1.landid=14
from adr.buitenlandsAdres a1

update p1 set p1.naam = p2.naam
from
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon) p1 join
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon) p2 on p1.volgorde=p2.volgorde

-- randomize jongensnamen
-- (jongens en meisjes apart, zodat afdelingen met geslacht blijven kloppen)
update p1 set p1.voornaam = p2.voornaam
from
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon where Geslacht = 1) p1 join
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon where Geslacht = 1) p2 on p1.volgorde=p2.volgorde

-- randomize meisjesnamen
-- (jongens en meisjes apart, zodat afdelingen met geslacht blijven kloppen)
update p1 set p1.voornaam = p2.voornaam
from
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon where Geslacht = 2) p1 join
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon where Geslacht = 2) p2 on p1.volgorde=p2.volgorde

-- van de geboortedata blijven we af, dan blijven de afdelingen kloppen
-- ad-nummers gooien we ook door elkaar

update p1 set p1.adnummer = p2.adnummer
from
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon where AdNummer is not null) p1 join
(select *, row_number() over (order by newid()) as volgorde from
pers.persoon where AdNummer is not null) p2 on p1.volgorde=p2.volgorde
