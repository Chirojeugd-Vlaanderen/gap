Implementatie gebruikersrechten
===============================

(Pas van toepassing in versie version\#78)

Permissies
----------

Permissies is een enum, met volgende members:

-   Geen (0)
-   Lezen (1)
-   Bewerken (3)

De tabel auth.GebruikersRechtV2
-------------------------------

Velden:

-   (GebruikersRechtV2ID)
-   PersoonID
-   GroepID
-   VervalDatum
-   PersoonsPermissies - permissies op eigen persoon
-   GroepsPermissies - permissies op algemene gegevens van je groep
-   AfdelingsPermissies - permissies op de mensen uit je afdeling
-   IedereenPermissies - permissies op alle mensen van je groep
-   Versie

In de backend is de corresponderende entity is GebruikersRechtV2.

De frontend kent het datacontract GebruikersRecht, met
PersoonsPermissies, Groepspermissies, AfdelingsPermissies en
IedereenPermissies. Zo'n GebruikersRecht zit vervat in
PersoonLidGebruikersInfo.

Stored procedures
-----------------

Beperkt, voornamelijk om compatibility met vroeger te blijven
garanderen.

-   auth.spGebruikersRechtToekennenAd stamnr, adnr - bewerken-permissies
    op groep en alle personen van die groep
-   auth.spAlleGebruikersrechtenOntnemenAd adnr - alle rechten van
    persoon met ad-nummer krijgen gisteren als vervaldatum
-   auth.spGebruikersrechtenOphalenAd adnr - groepid, code, naam, plaats
    en vervaldatum van personen met bewerkrechten op groep en alle
    personen van die groep
-   auth.spGebruikersRechtenPerGroepOphalenAd stamnr - ad-nr en
    vervaldatum van personen met bewerkrechten op groep en alle personen
    van die groep
-   auth.spGebruikersrechtOntnemenbAd stamnr, adnr - laat
    gebruikersrecht van persoon met adnr op groep met stamnr vervallen.
-   (dev only:) auth.spWillekeurigeGroepToekennenAd adnr, naam,
    voornaam, geboortedatum, geslacht

'security aspects'
------------------

De enum SecurityAspects geeft aan op welke zaken er permissies kunnen
staan:

-   PersoonlijkeGegevens
-   GroepsGegevens
-   PersonenInAfdeling
-   PersonenInGroep
-   AllesVanGroep

Voornamelijk om programmeerwerk te vereenvoudigen.

Rechten nakijken
----------------

`_autorisatieMgr.IsGav(entity)` blijft werken voor backward
compatibility. Geeft true als je groepsinfo en personen mag wijzigen van
de groep die overeenkomt met de entity.

We gaan dit vervangen door iets in de stijl

&lt;pre&gt;\
if (\_autorisatieMgr.PermissiesOphalen(entity) != Permissies.Bewerken)\
{\
throw FaultExceptionHelper.GeenGav();\
}\
&lt;/pre&gt;

Of

&lt;pre&gt;\
if (!\_autorisatieMgr.MagLezen(ik, entity))\
{\
throw FaultExceptionHelper.GeenGav();\
}\
&lt;/pre&gt;

Dat weet ik niet meer heel precies.

Zie `IAutorisatieManager` voor meer functionaliteit.

TODO:
-----

Openstaande issues voor version\#78
