Nancy
=====

Nancy ([nancyfx.org](http://nancyfx.org)) is een .NET-framework dat
toelaat om HTTP-API's te bouwen. In principe kun je Nancy-toepassingen
op verschillende manieren hosten, ik omschrijf hier gauw hoe het kan
werken met Asp.Net.

Hello World
-----------

-   Creëer een lege asp.net-toepassing (Best .net 4.5, anders is de
    toepassing niet leeg :-))
-   Open de package manager console in Visual Studio (Tools, NuGet
    Package Manger, Packet Manager Console).
-   Zorg ervoor dat bij 'Default Project' het lege
    asp.net-project staat.
-   Tik in:

Install-Package Nancy\
Install-Package Nancy.Hosting.Aspnet\
Install-Package Nancy.Bootstrappers.Unity

-   Dat installeert Nancy, de ASP.NET-hosting voor Nancy, en de
    Unity-integratie, die we nodig zullen hebben voor onze
    dependency injection.
-   Deze Hello-World-toepassing geeft 'Hello world!' als je naar /
    surft:

&lt;pre&gt;\
public class TestModule:NancyModule\
{\
public TestModule()\
{\
// Toont 'Hello world![](' op /.
            Get["/"] = _ =&gt; "Hello world)";\
}\
}\
&lt;/pre&gt;

-   Om dependency injection te gebruiken, maak je een bootstrapper aan:

&lt;pre&gt;\
/// &lt;summary&gt;\
/// Deze NancyBootstrapper zorgt ervoor dat de Dependency Injection
container gebruikt wordt voor\
/// het creëren van modules.\
/// &lt;/summary&gt;\
public class DependencyInjectionBootstrapper : UnityNancyBootstrapper\
{\
protected override void ApplicationStartup(IUnityContainer container,
IPipelines pipelines)\
{\
// No registrations should be performed in here, however you may\
// resolve things that are needed during application startup.

// Zoals bijvoorbeeld automapper initialiseren?\
MappingHelper.MappingsDefinieren();\
}

protected override void ConfigureApplicationContainer(IUnityContainer
existingContainer)\
{\
// Perform registation that should have an application lifetime

var section =
(UnityConfigurationSection)ConfigurationManager.GetSection("unity");

// Als er geen unity-configuratie in app.config of web.config zit, dan
is dat\
// waarschijnlijk een fout.

Debug.Assert(section != null);

// Als je hier een IoException of zoiets krijgt, dan mis je
waarschijnlijk referenties\
// naar assemblies die in je untiy-configuratie voorkomen.\
// Typisch voor services: Chiro.Cdf.DependencyInjectionBehavior.\
// Kijk ook eens na of alle assembly's in de types van de
unity-configuratie\
// bij de 'References' van je project staan.\
// Ook een mogelijke bron van problemen, is als interfaces van assembly
zijn veranderd,\
// maar als dat niet is aangepast in de configuratiefile :)

section.Configure(existingContainer);\
}

protected override void ConfigureRequestContainer(IUnityContainer
container, NancyContext context)\
{\
// Perform registrations that should have a request lifetime\
}

protected override void RequestStartup(IUnityContainer container,
IPipelines pipelines, NancyContext context)\
{\
// No registrations should be performed in here, however you may\
// resolve things that are needed during request startup.\
}\
}\
&lt;/pre&gt;

Testproject
-----------

Ik maakte een testproject `Chiro.Gap.Api2`, waarbij je door naar
http://localhost:poortnr/persoon/12345 te surfen, de gegevens van de
gelieerde persoon met GelieerdePersoonID 12345 opgeleverd krijgt.
Tenminste als je rechten hebt op die persoon. (Geen toegang tot gegevens
van eender welke groep? Zie [DatabaseConnectie](DatabaseConnectie.md) voor tips om dat
op te lossen.)

Dit project zit in de branch 'personal/vervljo/nancy'; zie
source:Solution/Chiro.Gap.Api2@d226ebda

&lt;pre&gt;\
git fetch origin\
git checkout personal/vervljo/nancy\
&lt;/pre&gt;

Aandachtspunten
---------------

### Verbs

Voor een api is de url typisch de entity waarop je wilt werken. De actie
wordt in het algemeen bepaald door een verb.

-   GET - ophalen
-   PUT - wijzigen
-   POST - nieuw
-   DELETE - verwijderen

### Response codes

Lever een goede response code op:

-   200 OK (bijv na succesvolle put. Url van nieuwe entry in
    location header)
-   201 Created (na succesvolle post)
-   202 Async Ops (request ontvangen, we zijn er nog mee bezig. Polling
    url in location header)
-   403 Forbidden (als je geen rechten hebt)
-   409 Conflict (bij concurrency problemen)
-   ...

### Zie zeker ook

http://restcookbook.com/
