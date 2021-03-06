-- Copyright 2012-2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

-- Maak GAP-data anoniem.

-- telefoonnummers

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

-- verwijder privacygevoelige info

delete from pers.communicatieVorm where communicatietypeid >= 4
update pers.communicatieVorm set nota = null;
update biv.uitstap set opmerkingen = null;

-- dit is ook niet �cht willekeurig, maar hopelijk wel goed genoeg om gauw
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
set a1.postcode='15500',a1.straat='K�pdorpsk�',a1.woonplaats='Praha 5',
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

-- log shrinken :-)

dbcc shrinkfile(gap1rc2_log, 128, NOTRUNCATE)