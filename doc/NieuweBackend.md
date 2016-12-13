Voorstel voor de nieuwe backend
===============================

**OPGELET:** gedeeltelijk verouderd. Zie [Backend](Backend.md) voor recentere
informatie.

Ik heb het internet afgeschuimd naar allerlei manieren om proper met
Entity Framework te werken. Uiteindelijk heb ik me vooral gebaseerd op
[A Generic Repository and Unit of Work Implementation for Entity
Framework](http://thedatafarm.com/blog/data-access/agile-entity-framework-4-repository-part-1-model-and-poco-classes/).
Hier en daar wat verfijnd. En waarschijnlijk sommige dingen dan toch
gedaan zoals het niet moet :-)

Algemeen
--------

Er komt een nieuwe backend voor het GAP. De service methods in
Chiro.Gap.Services blijven dezelfde, maar achterliggend wordt er
vanalles veranderd. Dit zijn de ambities:

-   We blijven entity framework gebruiken
-   Zo groot mogelijke loskoppeling van data access en businesslogica
-   Data access en businesslogica testable maken
-   Weg met `AttachObjectGraph` en property `TeVerwijderen` (die laaste
    was sowieso onmogelijk consequent te gebruiken.)
-   Dit zo inobtrusive mogelijk; de programmeur zou moeten kunnen werken
    zonder zich zorgen te moeten maken van data access

Ik denk dat ik iets heb dat hier vrij goed in slaagt. Hoezee. Ik gebruik
wel [Entity Framework
4.1](http://www.microsoft.com/en-us/download/details.aspx?id=8363), dus
dat zal je moeten installeren. Als het dat maar is.

Uiteraard is nog niet alles helemaal uitgewerkt; ik heb op dit moment
nog maar 1 klein voorbeeldje. Maar ik denk dat de basis goed is. Ik
probeer te omschrijven hoe het werkt.

De service-implementatie
------------------------

### Een service method als voorbeeld

Zo ziet `GroepenService.MijnGroepenOphalen` er uit:

&lt;pre&gt;&lt;code&gt;string mijnLogin =
\_authenticatieMgr.GebruikersNaamGet();\
var groepen = from g in \_groepenRepo.Select()\
where g.GebruikersRecht.Any(gr =&gt; gr.Gav.Login == mijnLogin)\
select g;\
return Mapper.Map&lt;IEnumerable&lt;Groep&gt;,
IEnumerable&lt;GroepInfo&gt;&gt;(groepen);\
&lt;/code&gt;&lt;/pre&gt;

Je ziet dat we nog managers zullen gebruiken, namelijk voor
niet-triviale businesslogica. Niet meer voor ophalen of bewaren van
zaken. En ook niet meer voor eenvoudige koppelingen. Daar komen we later
op terug.

`_groepenRepo` is iets dat de vroegere `GroepenDao` vervangt. De method
`_groepenRepo.Select()` levert een `IQueryable` op, die je toelaat met
linq-query's te werken.

Dat laatste satement mapt de gevonden groepen op het
`GroepInfo`-datacontract. dat is niet anders dan vroeger.

Een uitgebreider voorbeeld van een service method, is
GroepenService.FunctieToevoegen (zie
source:Solution/Chiro.Gap.Services/GroepenService.svc.cs@77930\#L341).
Hier is autorisatie van belang, en worden meer worker methods gebruikt.

### Context en repository

Een service-implementatie heeft als private members een 'context' van
het type `IContext`, en een aantal 'repositories' van het type
`IRepository&lt;T&gt;`.

Bijvoorbeeld, voor GroepenService (ATM)

&lt;pre&gt;&lt;code&gt; private IContext \_context;\
private IRepository&lt;Groep&gt; \_groepenRepo;\
&lt;/code&gt;&lt;/pre&gt;

`_context` is verantwoordelijk voor het tracken van wijzigingen van
entiteiten. Als er wijzigingen bewaard moeten worden, dan kan dat door
`_context.SaveChanges()` aan te roepen.

`_groepenRepo` is wat we vroeger een 'Dao' genoemd zouden hebben,
verantwoordelijk voor het ophalen van entiteiten uit de database en het
aanmaken van nieuwe entiteiten. De `_groepenRepo` werkt op groepen. Het
is niet onwaarschijnlijk dat er services zullen zijn die gebruik maken
van verschillende repository's.

In één service call gebruiken alle repository's dezelfde (gedeelde)
context. Op die manier kan bijvoorbeeld een groep ophalen uit een
`IRepository&lt;Groep&gt;` en een adres kunnen bijmaken via
`IRepository&lt;Adres&gt;`. Die groep zou je aan dat adres kunnen
koppelen. Omdat de context gedeeld is, weet die perfect wat er moet
gebeuren bij een `SaveChanges`.

De werking van een service call is dus in grote lijnen:

-   De service haalt entiteiten op of maakt ze aan via de repository's.
-   Triviale bewerkingen doet de service zelf (bijv. het koppelen van
    2 entiteiten)
-   Voor niet-triviale bewerkingen en waarschijnlijk ook voor
    authenticatie behouden we het concept van workers.
-   Eventuele wijzigingen kunnen dan bewaard worden met
    `_context.SaveChanges()`.

### Dependency injection

De context en de repositories voor een serviceïmplementatie worden door
de constructor geïnitialiseerd. De constructor voor de groepenservice
vraagt hiervoor een `IRepositoryProvider`.

&lt;pre&gt;&lt;code&gt;public GroepenService(IRepositoryProvider
repositoryProvider /\*, enz\*/)\
&lt;/code&gt;&lt;/pre&gt;

In eerste instantie had ik geprobeerd om een `IContext` en verschillende
`IRepository&lt;T&gt;`'s mee te geven aan de constructor, en alles te
laten regelen door de Unity-IOC-container. Maar op de manier waarop wij
Unity gebruiken (via configuratiefiles), kan niet geregeld worden dat de
context door alle repository's gedeeld wordt (denk ik). Vandaar dus een
IRepositoryProvider. Dit is de declaratie:

&lt;pre&gt;&lt;code&gt; public interface IRepositoryProvider\
{\
IContext ContextGet();\
IRepository&lt;TEntity&gt; RepositoryGet&lt;TEntity&gt;() where TEntity
: class;\
}\
&lt;/code&gt;&lt;/pre&gt;

De repository provider heeft een method die een context aflevert, en een
generieke method die een repository creëert voor entiteiten van type
`TEntity`. Alle repository's gebruiken dezelfde context.

De `IContext` zal geïnjecteerd worden in de repository provider, en
wordt bepaald door het configuratiebestand. Wil je andere repository's
gebruiken, dat kan ook, maar niet puur via de configuratiefile. Je zult
in dat geval een andere RepositoryProvider moeten schrijven, ik kom
hierop terug bij de testability.

### Disposable services

Iedere keer dat er een service call gebeurt naar bijvoorbeeld
`GroepenService`, dan wordt er een object van het type `GroepenService`
geïnstantieerd. Via dependency injection wordt er een context gemaakt.
Voor onze implementatie is die context een `DbContext` van entity
framework. Probleem: die `DbContext` is disposable, en moet dus
vrijgegeven worden als de service call eindigt.

Dit is opgevangen door de service-implementatie `IDisposable` te laten
implementeren. De `Dispose`-method van `GroepenService` wordt
aangeroepen op het moment dat de aangemaakte `GroepenService`-instantie
verdwijnt, met name na de service call. Die `Dispose`-method roept op
zijn beurt `_context.Dispose()` op, en zo wordt alles proper opgeruimd.

Schoonheidsfoutje: De context wordt nu aangemaakt door de IOC-container,
en gedisposed door de service. Dat is niet proper. Maar ik zie niet
direct een manier om daarrond te werken.

De workers
----------

In de workers implementeren we niet-triviale businesslogica. Dat is
zowat alwat niet ophalen, bewaren of eenvoudig koppelen is. De workers
implementeren een interface, zodat we ze waar nodig kunnen wegmocken.

In sommige gevallen hebben workers toegang nodig tot de data-access.
Bijvoorbeeld \`GroepenManager.FunctieZoeken\` (zie
source:solution/Chiro.Gap.WorkerInterfaces/IGroepenManager.cs@77930\#L28),
die zoekt in groepseigen en nationale functies. Voor die nationale
functies is data access nodig.

In eerste instantie had ik de neiging om een \`IRepository\` of een
\`IRepositoryProvider\` te injecteren via de constructor, maar dat mag
ik niet doen, omdat de IOC-container dan een nieuwe data context maakt;
te vermijden. Workers mogen geen repository's vragen bij constructie, en
ook geen repository's als member variables hebben. Iedere method die
data access nodig heeft, moet een repository als parameter hebben, en
het is aan de service om ervoor te zorgen dat de juiste repository
meegegeven wordt.

De definitie van \`GroepenManager.FunctieZoeken\` is nu als volgt:

Functie FunctieZoeken(Groep groep, string code,
IRepository&lt;Functie&gt; functieRepo);

En dit is hoe de \`GroepenService\` het aanroept:

var bestaande = \_groepenMgr.FunctieZoeken(groep, code, \_functiesRepo);

\`\_functiesRepo\` is een repository van \`GroepenService\` zelf,
aangemaakt door de \`RepositoryProvider\` uit \`GroepenService\`s
constructor. We zijn dus zeker dat de juiste context aan die repository
gekoppeld is.

De entiteiten
-------------

De entiteiten (Groep, Persoon, GelieerdePersoon,...) erven nu niet meer
van een klasse entiteit. Zijn het POCO's (plain old c\# objects)? Plain
old is veel gezegd, want ze gebruiken HashSets en ICollections. Het komt
alleszins wel in de buurt. Ze zijn gedefinieerd in
`Chiro.Gap.Poco.Model`; ter illustratie de definitie van `Land`:

&lt;pre&gt;&lt;code&gt; public partial class Land\
{\
public Land()\
{\
this.BuitenLandsAdres = new HashSet&lt;BuitenLandsAdres&gt;();\
}

public int ID { get; set; }\
public string Naam { get; set; }

public virtual ICollection&lt;BuitenLandsAdres&gt; BuitenLandsAdres {
get; set; }\
}\
&lt;/code&gt;&lt;/pre&gt;

Deze klasses zijn veel cleaner dan wat we vroeger hadden (automatisch
gegenereerd door Entity Framework 2). Ze worden wel niet meer
automatisch gegenereerd als je iets bijtekent op de edmx. Al is het niet
onmogelijk om ze automatisch te genereren, want ik heb niet alle
bestanden voor alle entiteiten manueel aangemaakt :-)

De context
----------

Waar komt entity framework (v.4) er nu aan te pas? Die magie zit 'em in
Chiro.Gap.Poco.Context.ChiroGroepEntities:

&lt;pre&gt;&lt;code&gt; public partial class ChiroGroepEntities :
DbContext, IContext\
{\
public ChiroGroepEntities()\
: base("name=ChiroGroepEntities")\
{\
}

protected override void OnModelCreating(DbModelBuilder modelBuilder)\
{\
throw new UnintentionalCodeFirstException();\
}

public DbSet&lt;Groep&gt; Groep { get; set; }\
public DbSet&lt;Persoon&gt; Persoon { get; set; }

// enz...\
}\
&lt;/code&gt;&lt;/pre&gt;

Aan de hand van de connection string met de naam `ChiroGroepEntities`
koppelt de DbContext alle entiteiten aan de correcte databasetabellen.
Hoe dat nu juist gaat werken met inheritance en many-to-many, daar ben
ik nog niet helemaal uit. De edmx-file staat er trouwens nog wel, maar
ik denk dat die niet meer gebruikt wordt.

In de `Web.config` van `Chiro.Gap.Services` staat volgende relevante
lijn in de IOC-configuratie:

&lt;pre&gt;&lt;code&gt;&lt;type type="Chiro.Cdf.Poco.IContext,
Chiro.Cdf.Poco"\
mapTo="Chiro.Gap.Poco.Context.ChiroGroepEntities,
Chiro.Gap.Poco.Context" /&gt;\
&lt;/code&gt;&lt;/pre&gt;

Van zodra de service een `IContext` nodig heeft, zal de IOC-container
een instantie van `ChiroGroepEntities` aanmaken, en doet entity
framework de rest.

AttachObjectGraph, TeVerwijderen,...
------------------------------------

Vroeger hadden we het probleem dat de datacontext enkel bestond binnen
de data access, en niet bij de workers of bij de services. Als we dan
een simpele wijziging wilden doen, dan liep dat als volgt:

-   Haal op. Opgehaald object is gedetacht, context is kwijt
-   Wijzig
-   Om te bewaren moest eerst terug geattacht worden, wat wil zeggen:
    haal eerst opnieuw op.

Omdat nu de context nu bestaat gedurende de hele life time van een
service-object, moet er niet meer gedetacht en geattacht worden. Dat
bespaart heel wat database calls, en heel wat miserie met entiteiten die
mogelijk `TeVerwijderen` zijn.

Testability
-----------

Context, repository, workers, services: allemaal hebben ze interfaces,
en allemaal worden ze via die interfaces aangesproken. (Zelfs de
services aan de frontend, dankzij de `ServiceHelper`, die we blijven
gebruiken.) Testability: **+. :)

Voorlopig nog niet duidelijk
----------------------------

De context is op serviceniveau enkel nodig om te kunnen disposen en
changes te bewaren. Misschien kan dat ergens weggeabstraheerd worden. Al
is zoiets niet zo vanzelfsprekend als dat lijkt.

Nog na te kijken
----------------

Er is nog vanalles uit te zoeken:

-   Hoe goed werkt het bewaren van wijzigingen?
-   Hoe doen we de autorizatie op een goede manier?
-   Na te kijken: is het de moeite waard om lazy loading af te zetten,
    om zo aan performantie te winnen?
-   En uiteraard zullen er nog heel wat lijken uit de kast vallen. Ik
    heb op dit moment slechts één eenvoudige method geïmplementeerd. Ik
    verwacht dat er nog heel wat miserie aan zal komen.

Maar ik vind dat het er veelbelovend uitziet. Ik werk er hopelijk de
komende dagen nog wat aan verder, zodat het wat meer vorm krijgt. Alle
opmerkingen en suggesties welkom.

Waar is de code?
----------------

Ik maakte een branch gap-nieuwe-repo in de publieke git-repository. Hoe
pak je die branch nu uit:\
&lt;pre&gt;&lt;code&gt;git fetch origin\
git checkout -t origin/gap-nieuwe-backend\
&lt;/code&gt;&lt;/pre&gt;\
Ik ben trouwens nog altijd aan het werken aan een hopelijk duidelijke
tekst 'hoe werkt git nu eigenlijk echt' :-) De '-t' switch staat voor
'track', en mag je eigenlijk weglaten omdat dat standaard is. Meer info
hierover in de in-progress-zijnde tekst waarvan sprake.

Ik heb op de redmine ook een nieuwe milestone aangemaakt. 1.9, voor de
release van GAP met zijn nieuwe backend. Al is het nummer 1.9 gewoon
arbitrair.
