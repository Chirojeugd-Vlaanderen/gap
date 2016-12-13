Mappen van entiteiten naar DataContracts via [AutoMapper](AutoMapper.md)
===============================================================

Waarom
------

De GAP user interface communiceert met de businesslaag via services. Als
er voor de user interface een lijst met namen en afdelingen nodig is,
dan moet die informatie geserialiseerd over de lijn.

Onze business objects zijn entity's van het Entity Framework. Voor een
lijst met naam en afdeling informatie heb je bijgevolg informatie nodig
uit een graaf, bestaande uit entiteiten Persoon, GelieerdePersoon, Lid
en Afdeling. Zo'n geserialiseerde graaf wordt al gauw vrij groot, wat
niet interessant is.

Vandaar dat we voor dit soort lijsten speciale Data Contracts
definieerden, zoals bijvoorbeeld LidInfo. (Zie
\[source:trunk/Solution/Chiro.Gap.ServiceContracts/DataContracts/LidInfo.cs\].)

Om zo'n graaf van Entiteiten (bijv. gekoppeld aan lid) zonder te veel
programmeerwerk te kunnen mappen naar een compacter datacontract (bijv.
LidInfo), gebruiken we [AutoMapper](AutoMapper.md).

Gebruik
-------

Stel dat je een rij gelieerde personen wil mappen naar een lijst met
PersoonInfo. Leg dan een reference naar
\[source:trunk/Solution/References/AutoMapper.dll\] in je project, en
voorzie deze using:

&lt;pre&gt;\
using [AutoMapper](AutoMapper.md);\
&lt;/pre&gt;

Het mappen zelf, gebeurt dan als volgt:

&lt;pre&gt;\
IEnumerable&lt;GelieerdePersoon&gt; gelieerdePersonen;

// zorg ervoor dat gelieerdePersonen opgevuld geraakt

IList&lt;PersoonInfo&gt; infoVanPersonen =
Mapper.Map&lt;IEnumerable&lt;GelieerdePersoon&gt;,
IList&lt;PersoonInfo&gt;&gt;(gelieerdePersonen);\
&lt;/pre&gt;

De eerste generieke parameter van Mapper.Map is het 'Brontype', de
tweede is het 'Doeltype', en Mapper.Map zet dus een object van het
brontype om in het doeltype.

Je vindt hiervan allerlei voorbeelden in
\[source:trunk/Solution/Chiro.Gap.Services/GelieerdePersonenService.svc.cs\]

Configuratie
------------

De configuratie van de mapping gebeurt in de statische functie
Chiro.Gap.ServiceContracts.Mappers.MappingHelper.MappingsDefinieren.
(zie
\[source:trunk/Solution/Chiro.Gap.ServiceContracts.Mappers/MappingHelper.cs@630\#L17\].)

Als bron- en doeltype dezelfde veldnamen hebben, kan die configuratie zo
eenvoudig zijn als

&lt;pre&gt;\
Mapper.CreateMap&lt;Bron,Doel&gt;();\
&lt;/pre&gt;

Maar omdat dat bij ons niet het geval is, is het allemaal wat
ingewikkelder. Mogelijk kunnen sommige zaken ook eenvoudiger dan hoe ze
in de MappingHelper gebeuren, maar ik gebruik [AutoMapper](AutoMapper.md) ook nog
niet zo lang ;-)

De method MappingHelper.!MappingsDefinieren moet nog wel ergens
opgeroepen worden, en voor ons project is dat in
\[source:trunk/Solution/Chiro.Gap.Services/Global.asax.cs@630\#L18\].

Meer documentatie over [AutoMapper](AutoMapper.md)
-----------------------------------------

... vind je op http://automapper.codeplex.com/
