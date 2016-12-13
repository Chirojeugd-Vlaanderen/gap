Introductie WCF Data Services
=============================

Bedoeling
---------

De bedoeling van de publieke API is om de gebruiker via REST-calls
toegang te geven tot de gegevens van zijn groep in GAP. In eerste
instantie zal er enkel read-only access zijn. De gegevens moeten
beschikbaar zijn als JSON, zodanig dat ze gemakkelijk bruikbaar zijn
voor frameworks zoals JQuery.

Statische gegevens
------------------

In een eerste proof of concept wil ik gewoon een service opzetten die
gegevens oplevert. Hiervoor maken we gewoon een lege ASP.NET-applicatie.
In eerste instantie zullen de opgeleverde gegevens niet direct uit GAP
komen, maar gewoon uit een 'in memory' lijst.\
&lt;pre&gt;\
public class [LidDetail](LidDetail.md)\
{\
public int ID { get; set; }\
public string Naam { get; set; }\
public [DateTime](DateTime.md) GeboorteDatum { get; set; }\
public string Afdeling { get; set; }\
}

public static class Business\
{\
public static List&lt;LidDetail&gt; [AlleLedenOphalen](AlleLedenOphalen.md)()\
{\
return new List&lt;LidDetail&gt;\
{\
new [LidDetail](LidDetail.md)\
{\
ID = 1,\
Naam = "Nielske",\
Afdeling = "Speelclub",\
[GeboorteDatum](GeboorteDatum.md) = new [DateTime](DateTime.md)(2003, 07, 22)\
},\
new [LidDetail](LidDetail.md)\
{\
ID = 2,\
Naam = "Warke",\
Afdeling = "Sloebers",\
[GeboorteDatum](GeboorteDatum.md) = new [DateTime](DateTime.md)(2004, 07, 29)\
},\
new [LidDetail](LidDetail.md)\
{\
ID = 4,\
Naam = "Stanneke",\
Afdeling = "Rakwi's",\
[GeboorteDatum](GeboorteDatum.md) = new [DateTime](DateTime.md)(2001, 04, 07)\
}\
};\
}\
}\
&lt;/pre&gt;

De data context
---------------

Een WCF data service werkt steeds op een context. Die context maken we
zelf, en bevat een aantal 'IQueryables', die je kunt bekijken als data
sets waaruit de service zijn gegevens zal halen. Voor deze POC bevat de
context enkel een IQueryable van leden.\
&lt;pre&gt;\
public class [LedenContext](LedenContext.md)\
{\
public IQueryable&lt;LidDetail&gt; Leden { get { return
Business.AlleLedenOphalen().AsQueryable(); } }\
}\
&lt;/pre&gt;

De data service
---------------

Een data service kun je gewoon toevoegen aan een ASP.NET document via
'Add new item'. Als je in de search box 'WCF' tikt, dan vind je
gemakkelijk de WCF data service.

![](24.png)

Er wordt een .svc.cs-bestand gegenereerd, waar je de te gebruiken
context moet aanvullen, en moet definiëren welke sets uit de context
toegankelijk zullen zijn via de dataservice.\
&lt;pre&gt;\
public class [DataService](DataService.md) :
[DataService](DataService.md)&lt;LedenContext&gt;\
{\
// This method is called only once to initialize service-wide policies.\
public static void [InitializeService](InitializeService.md)(DataServiceConfiguration
config)\
{\
config.SetEntitySetAccessRule("Leden",
[EntitySetRights](EntitySetRights.md).AllRead);\
config.DataServiceBehavior.MaxProtocolVersion =
[DataServiceProtocolVersion](DataServiceProtocolVersion.md).V2;\
}\
}\
&lt;/pre&gt;

Als je nu je webapplicatie start, dan is je dataset toegankelijk op
volgende url (poortnummer zal waarschijnlijk verschillen):
http://localhost:50567/DataService.svc/Leden.

Als je naar die URL surft, dan krijg je een atom feed. Als je
JSON-output wilt, dan moet je dat specifiëren via een
HTTP-accept-header, bijv. met wget:

&lt;pre&gt;\
wget -q ~~O~~ --header "accept: application/json"
http://localhost:50567/DataService.svc/Leden

1.  als je met Windows werkt, moet je dubbele quotes (") gebruiken rond
    de header.
2.  voor Linux zijn single quotes (') ook OK\
    &lt;/pre&gt;

Interessant is dat je via de URL kunt query'en:

-   <http://localhost:50567/DataService.svc/Leden(2)>
-   http://localhost:50567/DataService.svc/Leden?\$orderby=Naam
-   http://localhost:50567/DataService.svc/Leden?\$orderby=Naam&amp;\$skip=1&amp;\$top=2
-   http://localhost:50567/DataService.svc/Leden?\$filter=Naam%20eq%20%27Warke%27

JSONP
-----

Dit soort van data service kan je niet rechtstreeks aanspreken met
JQuery vanop een remote host, omdat een browser dat meestal blokkeert
(op basis van cross domain scripting policies). Hier kun je rond werken
door [JSONP](http://en.wikipedia.org/wiki/JSONP) te gebruiken, wat er
eigenlijk op neer komt dat je resultaat wordt afgeleverd aan een
callback-functie, die JQuery zelf definieert. JSONP wordt niet standaard
ondersteund door WCF Data Services, maar gelukkig is er de [WCF Data
Services toolkit](http://wcfdstoolkit.codeplex.com/). Leg een referentie
naar deze DLL, en laat je data service erven van ODataService in plaats
van DataService.\
&lt;pre&gt;\
public class [DataService](DataService.md) : ODataService&lt;LedenContext&gt;\
&lt;/pre&gt;\
Je kunt nu ook via de URL duidelijk maken dat je een JSON-resultaat wilt
(wat vaak makkelijker is dan via de HTTP-header)\
&lt;pre&gt;\
http://localhost:50567/DataService.svc/Leden?\$format=json\
&lt;/pre&gt;

Voorbeeldclients
----------------

In \[source:trunk/poc/Chiro.Poc.OData/VoorbeeldClients\] vind je een
voorbeeldje van hoe je de service kunt gebruiken m.b.v. PHP of JQuery.

Source
------

De source van deze POC is te vinden in de repository:
source:poc/Chiro.Poc.OData

Zie ook
-------

[API](API.md) - pagina over de GAP-API
