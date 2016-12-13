Welkom bij de GAP-dev-wiki
==========================

Deze wiki bevat informatie voor de GAP-ontwikkelaars. Zoek je informatie
over het gebruik van GAP, dan kijk je beter eens op de
[gebruikerswiki](https://gapwiki.chiro.be).

**Dienstmededeling ;-)** Als je echt iets wil bijdragen aan het GAP, dan
meld je jezelf best aan op de redmine. Op die manier heb je rechten om
bijvoorbeeld fouten te rapporteren. Om aan te melden, klik
achtereenvolgens op de links [Sign in](https://websites.chiro.be/login),
en vervolgens op 'Login with CAS'. Je kunt dan aanmelden met je
GAP-account. Als je aangemeld bent, staat je gebruikersnaam in de
rechterbovenhoek van deze pagina.

GAPIP16
-------

[GAPIP16](GAPIP16.md), de jaarlijkse hoogmis van de jeugdwerk-IT, komt er weer
aan. Lees er alles over: [GAPIP16](GAPIP16.md).
(Checklist voor de organisatie van gapip: [GapipCheckList](GapipCheckList.md).)

Mee programmeren aan het GAP
----------------------------

### Source Code

De source code is publiek toegankelijk; je kunt een clone maken van de
git-repository http://websites.chiro.be/gap-git.git. (Als je op die link
klikt, dan krijg je een 404. Dat is normaal. Maar je kunt de url wel
gebruiken om te clonen.) Je mag de source code downloaden en gebruiken
onder de voorwaarden van de [Apache License, Version
2](http://www.apache.org/licenses/LICENSE-2.0.html).

Als je code bijdraagt, dan behou je je auteursrechten. Zie
\[\[GAPIP14\#Copyrights\]\] voor meer informatie.

Wil je ook push-rechten, dan kan dat geregeld worden. Neem contact op
met de [Chiro-helpdesk](https://chiro.be/eloket/feedback-gap). Push
steeds naar eigen branches, en laat je branchnaam beginnen met
'personal/username/' (waarbij je username uiteraard vervangt door je
username.)

#### Compilatie-errors

Als je dit soort foutmeldingen krijgt:

&gt; Error 1 The type 'System.IDisposable' is defined in an assembly
that is not referenced. You must add a reference to assembly
'System.Runtime, Version=4.0.0.0, Culture=neutral,
PublicKeyToken=b03f5f7f11d50a3a'.

Dan kun je ofwel:

-   Windows updaten
-   Deze KB installeren: https://support.microsoft.com/en-us/kb/2971005.
    (Zie
    ook https://stackoverflow.com/questions/12349189/the-type-system-idisposable-is-defined-in-an-assembly-that-is-not-referenced)

Als de referentie naar System.ServiceModel en/of
System.Runtime.Serialization stuk zijn, moet je ook nog .Net Framework
3.5 installeren (dit kan langs Programs & Features &gt; Windows
Components).

Verder ligt de referentie naar Chiro.Cdf.ServiceHelper ook nog niet
overal goed.

### Database

GAP werkt met een database. In [Database](Database.md) staat uitgelegd hoe je
die opzet.

### Access denied?

Als je bij het starten van de toepassing een access denied krijgt, en
iets i.v.m. anonymous users en IUSR\_MACHINENAME, dan kun je 2 dingen
proberen:

-   voeg je user toe aan de lokale groep IIS\_IUSRS, en log uit en
    weer aan.
-   probeer het een keertje met Internet Explorer. (I know...) Die doet
    soms meer hocus-pocus met authenticatie, en daarna is alles plots
    blijkbaar geregeld.

### Algemene documentatie GAP-development:

We deden een poging om te documenteren hoe je je systeem klaarmaakt voor
GAP-development. Die documentatie is niet compleet. Doe me een plezier,
en werk de documentatie bij als je merkt dat zaken onduidelijk zijn.

-   Opzetten van de ontwikkelomgeving
    -   [SoftwareInstallatie](SoftwareInstallatie.md) - welke software heb je nodig?
    -   [EersteCheckOut](EersteCheckOut.md) - source code ophalen
    -   Stel **Chiro.Gap.WebApp** én **Chiro.Gap.Services** in als
        **startup projects** (rechtsklik op solution, kies 'startup
        projects' en kies voor 'multiple'). Je kunt de toepassing nu
        runnen in de debugger door op F5 te duwen.
-   [GitHowTo](GitHowTo.md) - Werken met git, het versiebeheersysteem.
-   De issue tracker
    -   Klik op 'Issues' voor de lijst met alle issues
    -   [EasyHacks](EasyHacks.md) - tickets voor beginnende programmeurs
    -   [IssueStatuses](IssueStatuses.md) - issue statuses en workflow
    -   [IssueCategorieen](IssueCategorieen.md) - issue categorieën
-   [Uitdagingen](Uitdagingen.md) - grote uitdagingen voor het GAP
-   [DomainModel](DomainModel.md) - concepten aan de basis van de databasetabellen
-   [BasisArchitectuur](BasisArchitectuur.md) - de grote lijnen van de architectuur, en
    de belangrijkste projecten in de solution
-   [ExceptionHandling](ExceptionHandling.md) - het propageren van exceptions doorheen
    de verschillende programma-lagen
-   [UnitTests](UnitTests.md) - gebruik en aanmaken van automatische tests
-   [CodingStandards](CodingStandards.md) - algemene standaarden voor de source code
-   [UnityIoc](UnityIoc.md) - implementatie van Dependency Injection met Untiy
-   [ServiceHelper](ServiceHelper.md) - ons framework voor het aanspreken van
    WCF-services
-   [Troubleshooting](Troubleshooting.md) - een aantal veel voorkomende problemen met
    hopelijk een oplossing
-   [UseCases](UseCases.md) - een lijst met use cases voor het programma

### Synchronisatie GAP-&gt;Kipadmin

Gegevens worden van GAP overgezet naar Kipadmin via berichten in een
message queue. Deze berichten worden uitgelezen en verwerkt door een
vrij lelijk synchronisatietooltje: KipSync. Meer info over die
synchronisatie: [SyncGapKip](SyncGapKip.md)
