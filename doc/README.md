Welkom bij de GAP-dev-documentatie
==================================

Deze informatie is bedoeld voor de GAP-ontwikkelaars. Zoek je informatie
over het gebruik van GAP, dan kijk je beter eens op de
[gebruikerswiki](https://gapwiki.chiro.be).

**Dienstmededeling ;-)** Als je echt iets wil bijdragen aan het GAP, dan
meld je jezelf best aan op de [Chiro-Gitlab-server](https://gitlab.chiro.be).
Op die manier heb je rechten om
bijvoorbeeld fouten te rapporteren. Om aan te melden, klik je
op de knop 'Sign in with cas'. Je kunt dan aanmelden met je
GAP-account.

GAPIP
-----
[GAPIP](GAPIP.md) is een jaarlijks evenement voor en door de IT'ers in het
jeugdwerk, zowel voor vrijwilligers als voor beroepskrachten. De editie van
2017 vindt plaats in het weekend van 17-19 maart. Dit jaar is het in
De Hagaard te doen.

Mee programmeren aan het GAP
----------------------------

### Source Code

De source code is publiek toegankelijk; je kunt een clone maken van de
git-repository https://gitlab.chiro.be/gap/gap.git. Clone ze meteen met
submodules, want anders gaat de CAS-authenticatie niet werken:

    git clone --recursive https://gitlab.chiro.be/gap/gap.git

Moest je toch geclonet hebben zonder `--recursive`, dan kun je de code
van de submodules als volgt nog goed krijgen:

```
# dit is enkel nodig als je geclonet hebt zonder --recursive:
git submodule init
git submodule update
```

Je mag de source code downloaden en gebruiken
onder de voorwaarden van de [Apache License, Version
2](http://www.apache.org/licenses/LICENSE-2.0.html).

De solution file voor het gap is [Solution/Cg2Solution.sln](../Solution/Cg2Solution.sln).

Als je code bijdraagt, dan behoud je je auteursrechten. Zie
[Copyright](Copyright.md) voor meer informatie.

Vemeld in je commit messages telkens het nummer van het issue waaraan je werkt.
Als je tevreden bent over je werk, maak dan een merge request (gitlab) of
pull request (github).

### Database

GAP werkt met een database. In [Database](Database.md) staat uitgelegd hoe je
die opzet.

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

### Communicatie tussen GAP en Chirocivi

[Gegevens worden van GAP overgezet naar Chirocivi](SyncGapCivi.md) via berichten in een
message queue. Deze berichten worden uitgelezen en verwerkt door een
synchronisatietooltje: [CiviSync](doc/SyncGapCivi.md).
