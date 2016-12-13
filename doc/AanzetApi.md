Aanzet API
==========

(zie [IntroductieWcfDataServices](IntroductieWcfDataServices.md) voor een korte uitleg over de
gebruikte technologie)

Gebruiken
---------

(Ja hoor, hier vind je voorbeelden voor de api:)

Pak de solution uit, en zet Chiro.Gap.WebApi als startup project. Als je
de solution start, dan krijg je deze pagina te zien:

http://localhost:62225/

(het poortnummer kan verschillen) met daarop een foutmelding. Dat is
geen probleem, pas de url als volgt aan:

http://localhost:62225/api/Persoon

en je krijgt een overzicht van alle gelieerde personen van je groep.
Zoals gezegd kun je out of the box query'en via de url:

-   http://localhost:62225/api/Persoon(89183) (persoon met
    [GelieerdePersoonID](GelieerdePersoonID.md) 89183)
-   http://localhost:62225/api/Persoon?\$filter=Naam%20eq%20'Peeters' (filteren)
-   http://localhost:62225/api/Persoon?\$orderby=Voornaam (sorteren)
-   http://localhost:62225/api/Persoon?\$skip=20&amp;\$top=10 (paginering)

Andere interessante query's:

-   http://localhost:62225/api/Adres(81923)/Personen (alle personen die
    op een adres wonen)
-   http://localhost:62225/api/Afdeling(5222)/Personen (alle personen
    uit een afdeling)

Versies
-------

Personen, adressen en contactinfo krijgen een versie mee. Dat is een
soort van hexadecimale string die aangeeft hoe recent informatie is.
Informatie met een hogere versie is recenter dan informatie met een
lagere versie.

Architectuur
------------

De API werkt op IQueryables, dus we kunnen Chiro.Gap.Services niet
zomaar gebruiken. Op dit moment werkt de API rechtstreeks op de data
access in de backend. Maar niet zonder problemen, zie \#2774.

Datacontracts
-------------

De datacontracts zijn gedefinieerd in Chiro.Gap.WebApi/Models.

Opmerking
---------

Als je \$format=json toevoegt aan je query, dan zou je json moeten
krijgen. Dat werkt momenteel niet, vermoedelijk een configuratie issue.

Zie ook
-------

-   [API](API.md)

