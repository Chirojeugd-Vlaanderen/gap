De backend van GAP
==================

De backend werd vernieuwd begin 2013. Een overzicht.

Concepten
---------

### Entiteiten

-   POCO (wel dependency's HashSet en ICollection)
-   geen exotische attributen
-   niet meer automatisch gegenereerd

Voorbeeld: Land

&lt;pre&gt;\
public partial class Land\
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
&lt;/pre&gt;

### Repository's

-   (vroegere DAO's)
-   entity sets query'en via Linq
-   volledig generiek
-   lazy loading (voorlopig?)
-   private members van de service
-   geinstantieerd bij in constructor van service-implementatie
-   delen context

Voorbeeld:

&lt;pre&gt;\
// Haal alle groepen op waarvoor ik gebruikersrecht heb\
var groepen = (from g in \_groepenRepo.Select()\
where g.GebruikersRecht.Any(gr =&gt; gr.Gav.Login == mijnLogin)\
select g).ToList();

&lt;/pre&gt;

### Data context

-   private member van de repository's: `_context`
-   de context wordt gedeeld door alle repository's gedurende 1
    service call. De dependency-injection-container zorgt hiervoor.
-   voorziet link met Entity Framework
-   (implementatie is overal al OK)
-   SaveChanges van eender welke repository roept SaveChanges van
    de (gedeelde) context op

Hier is iets dat niet helemaal klopt: De context is disposable. Een
repository ook. De dispose van een repository disposet de context, en
maakt dus alle andere repositories onbruikbaar. De repository's worden
nu allemaal samen gedisposed, en wel in de Dispose-method van de
services.

### Workers

-   niet-triviale businesslogica
-   doen geen expliciete data-access
-   doen geen autorisatie (dat is nu voor de services)
-   werken met objecten, en niet met ID's

### Data contracts

-   ongewijzigd
-   voor communicatie backend-frontend
-   entiteiten worden op contracts gemapt

Entity Framework 4
------------------

Voor je de nieuwe backend kunt gebruiken, moet je [Entity Framework
4](http://www.microsoft.com/en-us/download/details.aspx?id=8363)
installeren.

Voorbeeldimplementaties
-----------------------

### Details van een groep ophalen: `GroepenService.DetailOphalen`

&lt;pre&gt;\
public GroepDetail DetailOphalen(int groepID)\
{\
var resultaat = new GroepDetail\
{\
Afdelingen = new List&lt;AfdelingDetail&gt;()\
};

var groepsWerkJaar =\
\_groepsWerkJarenRepo.Select()\
.Where(gwj =&gt; gwj.Groep.ID == groepID)\
.OrderByDescending(gwj =&gt; gwj.WerkJaar)\
.FirstOrDefault();

if (!\_autorisatieMgr.IsGav(groepsWerkJaar))\
{\
throw FaultExceptionHelper.GeenGav();\
}

Debug.Assert(groepsWerkJaar != null);

// Lazy loading zorgt ervoor dat alle zeken die nodig zijn\
// voor het mappen uit de database worden opgehaald.

Mapper.Map(groepsWerkJaar.Groep, resultaat);\
Mapper.Map(groepsWerkJaar.AfdelingsJaar, resultaat.Afdelingen);

return resultaat;\
}\
&lt;/pre&gt;

### Naam van een groep wijzigen: `GroepenService.Bewaren`

&lt;pre&gt;\
public void Bewaren(GroepInfo groepInfo)\
{\
var groep = (from g in \_groepenRepo.Select()\
where g.ID == groepInfo.ID\
select g).FirstOrDefault();

// Momenteel ondersteunen we enkel het wijzigen van groepsnaam\
// en stamnummer. (En dat stamnummer wijzigen, mag dan nog enkel\
// als we super-gav zijn.)

if (!\_autorisatieMgr.IsGav(groep))\
{\
throw FaultExceptionHelper.GeenGav();\
}

Debug.Assert(groep != null);

if (String.Compare(groepInfo.StamNummer, groep.Code,
StringComparison.OrdinalIgnoreCase)
![]( 0 &amp;&amp; )\_autorisatieMgr.IsSuperGav())\
{\
throw FaultExceptionHelper.GeenGav();\
}

groep.Naam = groepInfo.Naam;\
groep.Code = groepInfo.StamNummer;

// Er zijn wijzigingen die bewaard moeten worden.

\_groepenRepo.SaveChanges();\
}\
&lt;/pre&gt;

### Nieuwe categorie toevoegen aan groep: `GroepenService.CategorieToevoegen`

&lt;pre&gt;\
public int CategorieToevoegen(int groepID, string naam, string code)\
{\
var groep = (from g in \_groepenRepo.Select()\
where g.ID == groepID\
select g).FirstOrDefault();

if (!\_autorisatieMgr.IsGav(groep))\
{\
throw FaultExceptionHelper.GeenGav();\
}

Debug.Assert(groep != null);

// Check of de categorie al bestaat\
// Merk op dat we de categorieen niet expliciet hebben\
// opgevraagd. Entity framework doet dat voor ons, met\
// lazy loading

var bestaande = (from c in groep.Categorie\
where String.Compare(c.Code, code, StringComparison.OrdinalIgnoreCase)
== 0\
select c).FirstOrDefault();

if (bestaande != null)\
{\
var info = Mapper.Map&lt;Categorie, CategorieInfo&gt;(bestaande);\
throw FaultExceptionHelper.BestaatAl(info);\
}

var nieuwe = new Categorie {Code = code, Naam = naam};\
groep.Categorie.Add(nieuwe);\
\_groepenRepo.SaveChanges();

return nieuwe.ID;\
}\
&lt;/pre&gt;

Let op:
-------

### Data-access

-   Met Linq op `_XXXrepo.Select()`
-   Gegevens bewaren: `_XXXrepo.SaveChanges()`

### Gebruikersrechten controleren

-   In service-implementatie (ipv workers)
-   Via overloads van `_autorisatieMgr.IsGav(entiteit)`
    -   (ipv `isGavGroep(groepID)`, `isGavPersoon(persoonID)`,...)
    -   -&gt; minder databasecalls
    -   minder foutgevoelig dan ID's

**OPGELET:** Er wordt gewerkt aan een nieuw systeem voor
gebruikersrechten.

-   De functies `AutorisatieManager.IsGav` leveren `true` als de
    entiteit gekoppeld is aan een groep waarop je GAV-permissies hebt.
-   Op termijn zou ik dit willen vervangen door functies
    -   `AutorisatieManager.MagLezen(Persoon, Entity)`
    -   `AutorisatieManager.MagSchrijven(Persoon, Entity)`
-   Zie ook [GebruikersRechten](GebruikersRechten.md).

### Exceptions

-   `FaultExceptions` gegenereerd door `FaultExceptionHelper`.
-   Bijv.: `throw new FaultExceptionHelper.GeenGav()`
-   -&gt; debugger kent stack trace
-   -&gt; code tamelijk leesbaar

