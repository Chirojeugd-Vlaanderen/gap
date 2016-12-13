Eager Loading versus Lazy Loading
=================================

Eager loading
-------------

Om in de backend de database aan te spreken, gebruiken we implementaties
van `IRepository&lt;T&gt;`. Zo is bijvoorbeeld in
`Chiro.Gap.Services.GroepenService` als volgt een 'ledenrepository'
gedefinieerd:\
&lt;pre&gt;\
private readonly IRepository&lt;Lid&gt; \_ledenRepo;\
&lt;/pre&gt;\
Via dependency injection in de constructor wordt er dan een geschikte
implementatie aan `_ledenRepo` toegekend. (Concreet een
`Chiro.Cdf.Poco.Repository&lt;Lid&gt;`).

Wanneer de backend nu een lid moet opzoeken, dan gebeurt dat typisch via
de `Select`-method van `IRepository&lt;Lid&gt;`. Bijvoorbeeld:\
&lt;pre&gt;\
var ledenQuery = (from ld in \_ledenRepo.Select() where ld.ID ==
LIDID);\
&lt;/pre&gt;

Bovenstaande method levert een `IQueryable&lt;Lid&gt;`, en pas wanneer
die geevalueerd wordt door bijvoorbeeld `.First()`, zal de database
gequery'd worden. Uit de database zal enkel opgehaald worden wat Entity
Framework op dat moment nodig vindt. Concreet haalt\
&lt;pre&gt;\
var lid = ledenQuery.First();\
&lt;/pre&gt;\
enkel het betreffende lid op. Wat goed is. Maar als je dan achteraf iets
in deze stijl doet:\
&lt;pre&gt;\
Console.WriteLine(lid.GelieerdePersoon.Persoon.NaamEnVoornaam);\
&lt;/pre&gt;\
dan zullen er nieuwe query's naar de database gestuurd worden om aan de
gevraagde naam te komen.

Probleem
--------

Meestal is dit een goede manier van werken. Maar soms niet. Concreet,
als de groepenservice wil weten hoeveel leden er geen voorkeursadres
hebben, dan gebeurde er vroeger dit:\
&lt;pre&gt;\
var aantalLedenZonderAdres = (from ld in \_ledenRepo.Select()\
where ld.GroepsWerkJaar.ID  groepsWerkJaar.ID
                                                &amp;&amp; ld.UitschrijfDatum 
null &&\
ld.GelieerdePersoon.PersoonsAdres == null\
select ld).Count();\
&lt;/pre&gt;\
Toen Tommy de database monitorde, dan bleek dat er dan eerst een query
werd uitgevoerd naar leden waarvan de uitschrijfdatum null was. Die
leverde dan bijvoorbeeld 80 records op. Daarna kreeg de database dan 80
verschillende query's die opzochten of de bijhorende gelieerde personen
wel adressen hadden. En dat is uiteraard niet zo performant.

### Lazy loading

Als oplossing hiervoor kunnen we **eager loading** gebruiken: we maken
aan de `Select`-method van de ledenrepository duidelijk dat we sowieso
van elk lid het persoonsadres van bijhorende gelieerde persoon nodig
hebben. Om dit mogelijk te maken, hebben we de repository wat
herschreven. Op dit moment gebeurt er in de plaats:\
&lt;pre&gt;\
var aantalLedenZonderAdres = (from ld in
\_ledenRepo.Select("GelieerdePersoon.PersoonsAdres")\
where ld.GroepsWerkJaar.ID  groepsWerkJaar.ID
                                                &amp;&amp; ld.UitschrijfDatum 
null &&\
ld.GelieerdePersoon.PersoonsAdres == null\
select ld).Count();\
&lt;/pre&gt;\
Als nu de database gemonitord wordt, krijgt die gewoon 1 query met een
select count(\*). Een pak efficienter dus.

### Conclusie

De moraal van het verhaal: als je (in de backend) de database wilt
query'en via een repository, weeg dan goed af welke entiteiten je eager
wilt laden, en waarvoor je lazy loading wilt gebruiken.
