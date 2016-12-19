Dependency injection (Inversion of control) met Chiro.Cdf.Ioc
=============================================================

Waarom
------

Wanneer een stuk programmacode gebruik moet maken van 'iets waarvan de
functionaliteit belangrijker is dan hoe het werkt', dan gebruiken we
hiervoor graag een interface.

De GelieerdePersonenService wil bijvoorbeeld weten of de gebruiker wel
de GAV-rechten heeft om een operatie op een gelieerde persoon uit te
voeren, en heeft daarom een member \_autorisatieMgr van het type
IAutorisatieManager. (Zie bijv.
source:Solution/Chiro.Gap.Services/GelieerdePersonenService.svc.cs@861bb8\#L319.)

Op het moment dat GAV-schap over een gelieerde persoon gecontroleerd
moet worden, volstaat een check

```
if (!\_autorisatieManager.IsGavGelieerdePersoon(gelieerdePersoon))
{
// foutafhandeling
}
```

Voor de GelieerdePersonenService speelt het geen rol hoe
\_autorisatieManager werkt, als het IAutorisatieManager maar
implementeert.

Bij normaal gebruik van het programma, zal de implementatie van
IAutorisatieManager deze zijn uit
source:Solution/Chiro.Gap.Workers/AutorisatieManager.cs. Maar wanneer we
de GelieerdePersonenManager willen testen, en we voor de test in kwestie
niet geïnteresseerd zijn in het autorisatiegedeelte, is het wel
interessant om
source:Solution/TestProjecten/Chiro.Gap.Dummies/AutMgrAltijdGav.cs te
gebruiken, zodat het lijkt alsof de gebruiker altijd GAV is.

Ook de \[\[NieuweBackend\#Context-en-repository|repository provider\]\]
(verantwoordelijke voor data access) wordt door de services aangesproken
via een interface. Op die manier kan kunnen we voor testsituaties direct
geprefabriceerde objecten uit het geheugen opleveren, wat sneller is dan
eerst een query te moeten uitvoeren op de database.

Het moeilijke aan heel dit verhaal, is het koppelen van de
implementaties (in bovenstaande voorbeelden AutorisatieManager en
RepositoryProvider) aan de interfaces (IAutorisatieManager en
IRepositoryProvider). En hier komt Chiro.Gap.Ioc op de proppen.

Hoe werkt het
-------------

We bekijken het aan de hand van een voorbeeld.

### Dependency injection

De klasse AutorisatieManager heeft een implementatie nodig van
IAutorisatieDao en IAuthenticatieManager. Voor deze implementaties zijn
private members voorzien in AutorisatieManager:

```
private IAutorisatieDao \_dao;
private IAuthenticatieManager \_am;
```

AutorisatieManager heeft één constructor, en die ziet er als volgt uit:

```
public [AutorisatieManager](AutorisatieManager.md)(IAutorisatieDao dao,
IAuthenticatieManager am)
{
\_dao = dao;
\_am = am;
}
```

De constructor heeft een parameter voor iedere interface waarvoor een
implementatie nodig is.

De AutorisatieManager wordt op zijn beurt gebruikt in de
GelieerdePersonenManager, die deze constructor heeft:

```
public [GelieerdePersonenManager](GelieerdePersonenManager.md)(
IGelieerdePersonenDao dao,
IGroepenDao groepenDao,
ICategorieenDao categorieenDao,
IAutorisatieManager autorisatieMgr)
{
\_dao = dao;
\_groepenDao = groepenDao;
\_categorieenDao = categorieenDao;
\_autorisatieMgr = autorisatieMgr;
}
```

Om een GelieerdePersonenManager te maken, is dus onder andere een
IAutorisatieManager nodig, en voor deze IAutorisatieManager zal dan weer
een IAutorisatieDao nodig zijn.

De bedoeling is nu niet dat je om een GelieerdePersonenManager te maken
eerst (recursief) alle parameters voor de constructors manueel moet
instantiëren. De constructie van deze objecten is voor de rekening van
de 'IOC-container'.

### IOC-container

De eenvoudigste manier om een object te creëren waarbij automatisch alle
'dependency's' geïnjecteerd worden, is via de statische functie
Factory.Maak&lt;T&gt;. Op die manier gebeurt het bijv. in
\[source:trunk/Solution/TestProjecten/Chiro.Gap.Workers.Test/CategorieTest.cs@653\#L83\]:

```
var gpMgr = Factory.Maak&lt;GelieerdePersonenManager&gt;();
```

Alles wat nodig is voor de constructie van een GelieerdePersonenManager
zal door deze aanroep automatisch gegenereerd worden.

Op zich is dit niet de mooiste oplossing; voor de services is het iets
eleganter aangepakt.

Bekijk bijvoorbeeld de GelieerdePersonenService. Hier verwacht de
constructor de managers als parameters:

```
public [GelieerdePersonenService](GelieerdePersonenService.md)(
GelieerdePersonenManager gpm,
PersonenManager pm,
AdressenManager adm,
GroepenManager gm,
CommVormManager cvm,
CategorieenManager cm,
IAutorisatieManager aum)
{
\_gpMgr = gpm;
\_pMgr = pm;
\_auMgr = aum;
\_adrMgr = adm;
\_grMgr = gm;
\_cvMgr = cvm;
\_catMgr = cm;
}
```

Wanneer de GelieerdePersonenService nu als webservice wordt aangeroepen,
komt magischerwijze de dependency injection in orde. 't Is te zeggen;
uiteraard is dat niet magischerwijze, maar dankzij de
DependecyInjectionBehavior die Tommy implementeerde
(\[source:trunk/Solution/Chiro.Cdf.DependencyInjectionBehavior\]), en
die van toepassing is op onze services. (Zie
\[source:trunk/Solution/Chiro.Gap.Services/Web.config@665\#L172\].) Zie
ook [DIServiceBehaviorExtension](DIServiceBehaviorExtension.md).

Misschien zou het interessant zijn moest voor de unit tests de
constructie met 'Factory.Maak' ook vervangen kunnen worden door
parameters in de constructor. Maar misschien ook niet, omdat je voor
tests vaak andere dependency's dan gewoonlijk wil. Hier ben ik nog niet
uit.

Configuratie
------------

### Via configuratiebestand

Onderliggend wordt Unity gebruikt door Chiro.Cdf.Ioc, en in het algemeen
wordt Unity via Web.Config of App.Config geconfigureerd. Zie
bijvoorbeeld
\[source:trunk/Solution/Chiro.Gap.Services/Web.config@665\#L29\]. Een
typische lijn ziet er zo uit:

```
&lt;type type="Chiro.Gap.Orm.DataInterfaces.IAutorisatieDao,
Chiro.Gap.Orm"
mapTo="Chiro.Gap.Data.Ef.AutorisatieDao, Chiro.Gap.Data" /&gt;
```

Wat wil zeggen: Als je een implementatie nodig hebt van de interface
Chiro.Gap.Orm.!DataInterfaces.IAutorisatieDao (die gedefinieerd is in de
assembly Chiro.Gap.Orm), neem dan Chiro.Gap.Data.Ef.!AutorisatieDao uit
de assembly Chiro.Gap.Data.

Unity moet wel expliciet opgedragen worden om het configuratiebestand te
gebruiken, en dat kan door de statische method Factory.ContainerInit()
op te roepen. Voor de services gebeurt dit in
\[source:trunk/Solution/Chiro.Gap.Services/Global.asax.cs@618\#L17).
(Maar misschien zou dit ook niet misstaan in de statische constructor
van Factory?)

### At runtime

Je kan de configuratie van Unity ook at runtime wijzigen, zoals dat
bijvoorbeeld gebeurt in
\[source:trunk/Solution/TestProjecten/Chiro.Gap.Workers.Test/AutorisatieTest.cs@638\#L134\]:

```
Factory.InstantieRegistreren&lt;IAutorisatieManager&gt;(new
[AutMgrNooitGav](AutMgrNooitGav.md)());
```

Meer documentatie over Unity
----------------------------

... vind je op http://unity.codeplex.com/
