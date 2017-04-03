Namespaces en projecten
=======================

Deze pagina geeft een beknopte omschrijving van iedere namespace en elk
project. Voor de grote lijnen, zie [BasisArchitectuur](BasisArchitectuur.md).

Namespaces
----------

Alwat we programmeren, komt in de namespace 'Chiro'. Deze namespace
heeft een aantal subnamespaces:

-   Chiro.Adf - de dingen die we overnamen van Tommy's collega's voor de
    aanroep van de services
-   Chiro.Cdf - 'Chiro Development Framework'. Wat in deze namespace
    zit, moet herbruikbaar zijn voor toekomstige andere projecten van
    de Chiro.
-   Chiro.Gap - Code specifiek voor het GAP-project

Projecten
---------

-   Chiro.Ad.ServiceContracts -- interface via dewelke we onze active
    directory aanspreken.
-   Chiro.Adf en Chiro.Adf.ServiceModel -- Dit namen we initieel over
    van een collega van Tommy. We gebruiken hieruit de 'ServiceHelper'.
    Ondertussen is deze code erg veranderd.
-   Chiro.Cdf.!DependencyInjectionBehavior -- Dit project werd door
    Tommy geschreven om de Dependency Injection (IOC) in de webservice
    aan de gang te krijgen.
-   Chiro.Cdf.Ioc -- Definieert onze IOC-container 'Factory'.
-   Chiro.Cdf.Intranet -- Hiermee roepen we webservices aan die op het intranet draaien, bv. mailadrescontrole.
-   Chiro.Cdf.Mailer -- Kan gebruikt worden om mails te sturen
-   Chiro.Cdf.Poco -- Bouwstenen om generieke 'repositories' te bouwen.
    Relevante concepten:
    -   `IContext` - interface voor data context
    -   `IRepository&lt;T&gt;` - interface voor repositories
    -   `Repository&lt;T&gt;` - implementatie van repository met entity
        framework
    -   `IRepositoryProvider`- klasse die repositories oplevert
    -   `RepositoryProvider` - implementatie van RepositoryProvider met
        entity framework
-   Chiro.Cdf.ServiceModel -- Hier zit iets in dat we gebruiken om
    Windows Services te maken. Dit is in een apart project
    terechtgekomen, wat waarschijnlijk overkill is.
-   Chiro.Cdf.UnityWcfExtensions -- Iets om de dependency injecion via
    unity in orde te krijgen voor de services
-   Chiro.Cdf.Validation -- Bevat onze eigen attributen
    voor validatieregels.
-   Chiro.Gap.Domain -- De naamgeving van dit project is wat vreemd,
    maar het bevat de zaken die zowel in frontend als in backend
    gebruikt worden.
-   Chiro.Gap.Poco.Context -- de data context voor GAP, die met Entity
    Framework werkt. Koppelt de entiteiten uit
    Chiro.Gap.Poco.Model aan de tabellen in de database
-   Chiro.Gap.Poco.Model -- definitie van de entiteiten die we in gap
    gebruiken
-   Chiro.Gap.ServiceContracts -- Servicecontracts voor de backend.
-   Chiro.Gap.ServiceContracts.Mappers -- Mappers die de entiteiten uit
    EF mappen naar de datacontracts van de backend.
-   Chiro.Gap.Services -- Implementatie van de backendservices.
    Zie [BasisArchitectuur](BasisArchitectuur.md)\#Chiro.Gap.Services.
-   Chiro.Gap.Sync -- Mapt entiteiten naar het datacontract van KipSync
    (synchronisatie GAP -&gt; kipadmin), en stuurt die gemapte
    entiteiten via een message queue naar KipSync. Chiro.Gap.Sync stuurt
    sowieso informatie naar KipSync. De controle of er al dan niet
    gesynct moet worden, gebeurt elders (vermoedelijk
    in Chiro.Gap.Services).
-   Chiro.Gap.SyncInterfaces -- Interfaces voor KipSync
-   Chiro.Gap.UpdateSvc.\* -- De service voor de communicatie Kipadmin
    -&gt; GAP
    -   Chiro.Gap.UpdateSvc.![](ConsoleServiceHost: een service host die je kan runnen vanuit de console
        ** Chiro.Gap.UpdateSvc.Contracts: de service contracts voor UpdateSvc
        ** Chiro.Gap.UpdateSvc.Service: de implementatie van UpdateSvc
        ** Chiro.Gap.UpdateSvc.)WindowsService: (nog te maken) de
        service host als windows service
-   Chiro.Gap.Validatie -- De bedoeling was om een soort van
    validatieframework te hebben, dat in de verschillende lagen gebruikt
    kon worden om data te valideren. Het is echter nog niet zeker hoe
    validatie zal verlopen. We experimenteren momenteel door dit voor de
    UI-kant door MVC2 te laten afhandelen.
-   Chiro.Gap.WebApp -- Asp.NET MVC webapplicatie voor de
    user interface.
-   Chiro.Gap.Workers -- Bevat de managerklasses die de niet-triviale
    businesslogica implementeren. De managers werken zo veel mogelijk
    met objecten, en zo weinig mogelijk met ID's. Zie
    ook [Backend](Backend.md).

