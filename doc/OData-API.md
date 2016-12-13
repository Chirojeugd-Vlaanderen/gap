OData-API
=========

De OData-API wordt afgevoerd, en vervangen door een REST-API. Zie
[API](API.md).

De informatie hier is nog van toepassing op de oude API.

Todo
====

mag later naar bugs/features

-   Authorisatie / Authenticatie
-   Gebruik van services in helperfuncties?
-   nakijken queries + optimaliseren indien mogelijk
-   Alles wat te maken heeft met schrijven (version, ...)

2013-06-18
==========

Als ik mezelf binnenkort weer afvraag (of iemand anders vraagt zich af).
Waarom het niet mogelijk is om 1 authorisatie (GAV, of een acceskey) te
gebruiken voor meerder groepen:

We geven nu als we een persoon opvragen mee of deze Lid, Leiding,
Ingeschreven of Uitgeschreven is voor het huidige groepswerkjaar. Als we
toestaan om meerder chiro's te beheren met 1 account, kunnen we dat niet
meer weten als er /api/Persoon(42) wordt gevraagd, want we weten dan
niet voor welke groep we dit vragen.

Als wi dit willen doen moeten we zelf iets implementeren waardoor er met
elke request ook de gewenste groep wordt meegegeven. Eisen dat elke
account/key/... maar bij 1 groep kan horen doet dit impliciet.

2013-06-15
==========

Te onthouden:
-------------

-   Om constructors met parameters te gebruiken in een Select moet je
    vertrekken vanuit een concreet object.
    -   Dit werkt: &lt;pre&gt;\_groepsWerkJaar.AfdelingsJaar.Select(aj
        =&gt; new AfdelingModel(aj)).AsQueryable();&lt;/pre&gt;
    -   Dit kan ook (als er een parameterloze constructor is):
        &lt;pre&gt;\_context.AfdelingsJaar.Where(aj =&gt;
        aj.GroepsWerkJaar.ID  \_groepsWerkJaar.ID)
                                .Select(aj =&gt; new AfdelingModel {Id = aj.ID, Naam = aj.Afdeling.Naam})
                                .AsQueryable();&lt;/pre&gt;
        \*\* Maar dit niet: &lt;pre&gt;\_context.AfdelingsJaar.Where(aj =&gt; aj.GroepsWerkJaar.ID 
        \_groepsWerkJaar.ID)\
        .Select(aj =&gt; new
        AfdelingModel(aj)).AsQueryable();&lt;/pre&gt;\
        h2. Opmerkingen

<!-- -->

-   Kan 1 afdeling meerdere afdelingsjaren hebben in hetzelfde werkjaar?
-   Hoe werken Postcode en Postnummer voor buitenlandse adressen
-   kunnen we via ??? (adres, communicatievorm) personen ophalen die
    niet in onze groep zitten?
-   waarvoor dient PersoonsAdres?
-   selectmany geeft dubbele resultaten (bv bij adres)

2013-06-10
==========

Om het eenvoudig te houden, dacht ik deze beperkingen te gebruiken:

-   Een api-call hoort altijd bij exact 1 groep (op te halen
    uit authenticatie)
-   Een api-call is altijd voor het huidige werkjaar

Op die manier kan ik een heel deel tussenklassen uit de db verbergen
voor de gebruiker.

De endpoints zouden dan zijn:

-   Groep
    -   Properties
        -   Id = Groep.ID
        -   {Naam, Stamnummer} = Groep.{Naam, Code}
    -   Navigation
        -   Personen (alle gelieerde personen)
        -   Afdelingen (alle afdelingen huidig groepswerkjaar)
-   Afdeling
    -   Properties
        -   Id = Afdelingsjaar. ID of Afdeling.ID (nog te bekijken)
        -   Naam = Afdelingsjaar.naam
    -   Navigation
        -   Groep
        -   Personen (leden + leiding)
-   Persoon
    -   Properties
        -   Id = GelieerdePersoon.ID
        -   {Naam, Voornaam, Geboortedatum} = Persoon.{Naam, VoorNaam,
            GeboorteDatum}
        -   Type = \[Lid, Leiding, Ingeschreven, Niet-Ingeschreven\]
            (eventueel op te splitsen een Bool Ingeschreven en Type
            \[Lid, Leiding, Ander\]
    -   Navigation
        -   Groep
        -   Contactgegevens
-   Contactgegeven
    -   Properties
        -   Id
        -   Type = \[E-mail, Fax, Telefoon, ...\]
        -   Waarde = ...
    -   Navigation
        -   Persoon of Personen

Enkele opmerkingen
------------------

-   zowel lid, leiding, kind als een gelieerde persoon die niet is
    ingeschreven in het huidig werkjaar worden als Persoon wergegeven.
-   Communicatievorm heb ik nog niet in detail bekeken, maar ik zou
    adressen als 1 string doorgeven (eventueel met een \\n op de
    juiste plaats).
-   Als ik communictaievorm in detail bekijk, moet ik ook eens nadenken
    over voorkeursadressen.
-   OData ondersteunt geen \$unique-attribuut, dus bij het maken van
    adreslijsten moet de client hier de nodige logica zelf voor
    implementeren (als we er voor kiezen om adressen meerder kere te
    tonen om zo het voorkeursattribuut mee te kunnen sturen).
-   Ik doe nog niets met Version. Als we PUT/PATCH gaan ondersteunen
    moeten we dit misschien wel doen.

2013-06-09
==========

Api heeft al een voorbeeld in de api-branch.\
Gebruikt MVC4

Nuttige urls om verder te bouwen:\
http://www.asp.net/web-api\
http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/working-with-entity-relations\
http://blogs.msdn.com/b/astoriateam/archive/2011/01/20/oauth-2-0-and-odata-protecting-an-odata-service-using-oauth-2-0.aspx

Oudere info\
h2. De publieke API
===================

(of toch een poging tot)

**** [IntroductieWcfDataServices](IntroductieWcfDataServices.md) - een eenvoudig voorbeeld van
WCF data services, een technologie die we zouden kunnen gebruiken voor
de API.

**** Op 13/2/2012 vergaderden Ben en Johan over de API. Het verslag zit
in de repository: \[source:"doc/verslagen/2012-02-13 api.pdf"\]

**** [AanzetApi](AanzetApi.md) - aanzet tot een GAP-API
