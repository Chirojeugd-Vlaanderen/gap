Wat moet je weten over de unit tests?
=====================================

**OPMERKING:** met de overgang naar de nieuwe [backend](backend.md), zijn veel
unit tests gebroken. Dat moet nog gefixt worden.

-   Sommige zaken zijn gewoon nog niet geimplementeerd
-   Sommige tests moeten opnieuw geschreven worden
-   Sommige testcases waren vroeger van toepassing op de workers, en
    zullen nu van toepassing zijn op de services

Dat staat op mijn TODO.

Waar zitten de unit tests?
--------------------------

In source:Solution/TestProjecten.

Hoe run ik de unit tests?
-------------------------

Als je in Visual Studio Ctrl-R tikt, gevolgd door T, dan worden alle
tests uitgevoerd. Normaalgezien lukken ze allemaal.

Wanneer run ik de unit tests?
-----------------------------

Alvorens je een changeset commit in de trunk, moet je de unit tests
laten lopen.

Wat te doen als een unit test failt?
------------------------------------

Als een unit test mislukt, dan zijn er drie mogelijkheden (met dalende
waarschijnlijkheid):

-   Je hebt iets veranderd waardoor de software zich niet meer gedraagt
    zoals verwacht. Zorg ervoor dat het gewenste gedrag hersteld wordt.
-   Een unit test maakt een fout bij het testen van de
    verwachte situatie. Omdat jij iets veranderde, wordt die
    fout duidelijk. Pas de unit test aan, zodat de verwachte situatie op
    een juiste manier geverifieerd wordt.
-   Er wordt getest op iets dat niet meer verwacht wordt of niet meer
    relevant is. In dit zeldzame geval mag de test verwijderd worden.

Wanneer maak ik een unit test?
------------------------------

Als je aan test driven development doet, dan gaat het als volgt:

-   Je wilt iets implementeren
-   Je creeert een unit test voor hetgeen je wilt implementeren
-   Omdat je nog niets geimplementeerd hebt, failt de test
-   Implementeer wat je wilt implementeren
-   Run de unit tests. Als je geen fouten hebt gemaakt, lukken
    ze allemaal.

Op voorhand tests schrijven voor alwat je wilt implementeren, is
misschien overkill. Dit kan ook:

-   Je stelt een bug vast
-   Je maakt een unit test die de bug reproduceert, en die eindigt met
    een assertion van het correcte gedrag
-   De test failt, want er is een bug
-   Fix de bug
-   Alle tests slagen.

Inversion of control en unit tests
----------------------------------

### Data access e.d. vermijden

Als je een method van een worker wilt testen, dan is het niet de
bedoeling dat die test de database gaat raadplegen. Dat wordt op twee
manieren vermeden.

Een eerste manier bestaat erin om enkel methods te testen die geen data
access gebruiken. Deze tests zitten in het project
Chiro.Gap.Workers.Test (zie
source:Solution/TestProjecten/Chiro.Gap.Workers.Test).

De configuratie van dat project vervangt alle klasses uit de data access
door dummy's, die geen enkele method effectief implementeren. Moest er
toch iets van de data access aangeroepen worden, dan volgt een
NotImplementedException, waardoor de test failt. De betreffende
configuratie vind je in
source:Solution/TestProjecten/Chiro.Gap.Workers.Test/App.config\#L40.

Een andere mogelijkheid bestaat erin om de calls naar de data access te
mocken. Dit gebeurt bijvoorbeeld in de tests van het project
Chiro.Gap.Workers.Test en Chiro.Gap.Services.Test. Een call naar de data
access zal dan een vooraf gedefinieerd resultaat opleveren, zoals
bijvoorbeeld in
source:Solution/Chiro.Gap.Services.Test/GelieerdePersonenServiceTest.cs\#L121.

In deze test worden 'Mocks' gemaakt van IDao&lt;CommunicatieType&gt;,
ICommunicatieVormDao en ICommunicatieSync (die laatste om te vermijden
dat een test een bericht in een message queue zal duwen om te syncen met
Kipadmin). Van deze mocks wordt het gewenste gedrag gedefinieerd, en ze
worden geregistreerd bij de IOC-container. Iedere keer er dus bijv. een
ICommunicatieVormDao nodig is, zal de IOC-container onze mock opleveren.

### GeenGavException vermijden

Als we een unit test willen uitvoeren die bijvoorbeeld een persoon lid
maakt van een groep, zou het belachelijk zijn moest die failen omdat we
geen GAV zijn van die groep. Om dit te vermijden zijn de unit tests van
de backend in de meeste gevallen zodanig geconfigureerd dat je altijd de
rechten krijgt die je nodig hebt. Dit wordt gerealiseerd via de
configuratie van de Dependency Injection:

Zie
source:Solution/TestProjecten/Chiro.Gap.Workers.Test/App.config\#L29:

&lt;pre&gt;\
&lt;!-- altijd GAV voor de tests --&gt;\
&lt;type type="Chiro.Gap.Orm.DataInterfaces.IAutorisatieDao,
Chiro.Gap.Orm"\
mapTo="Chiro.Gap.Dummies.AutDaoAltijdGav, Chiro.Gap.Dummies" /&gt;\
&lt;type type="Chiro.Gap.Workers.IAutorisatieManager,
Chiro.Gap.Workers"\
mapTo="Chiro.Gap.Dummies.AutMgrAltijdGav, Chiro.Gap.Dummies" /&gt;\
&lt;/pre&gt;

SynchronisationLockExceptions bij het debuggen
----------------------------------------------

Wanneer de IOC-Container tijdens een test geconfigureerd wordt
(bijvoorbeeld om een mock te registreren), dan krijg je bij het debuggen
SynchronisationLockExceptions. Dat komt omdat er maar 1 statische
IOC-container is, en omdat de tests in theorie in parallel kunnen lopen.
Als test A de configuratie van de IOC-container wijzigt terwijl test B
de IOC-container gebruikt, is het niet ondenkbaar dat er iets mis loopt.

Ik heb hier nog geen mooie oplossing voor gevonden. Als lelijke
workaround catch ik de betreffende exception in
source:Solution/Chiro.Cdf.Ioc/Factory.cs\#L102, en kun je je debugger
instellen om gecatchte exceptions te negeren. Maar dat is dus geweldig
lelijk.

Andere exceptions bij het debuggen
----------------------------------

In sommige gevallen wordt er getest of een bepaalde actie een exception
opwerpt. Als je debugger geconfigureerd is dat hij moet stoppen bij een
exception, dan zal die debugger dat ook doen. Klik gewoon op continue
(of F5) om het testen verder te zetten.
