De source repository downloaden
===============================

De repository klonen
--------------------

### GUI

De GUI-tool die we aanbevelen voor Git onder windows, is gitextensions.
Voor installatiehulp, zie [GitExtensions](GitExtensions.md).

-   In Windows Explorer klik je met de rechtermuisknop op een folder
    waarin je de code wilt uitpakken.
-   Selecteer 'GitEx Clone'.
    -   (Als dat niets doet, start dan eerst 'Git Extensions'. Je krijgt
        een configuratiescherm. Meestal is alles in orde. Klik OK, sluit
        Git Extensions, en dan zal het wel gaan.)
-   Bij 'repository to clone' vul je het volgende in:
    ```
    https://gitlab.chiro.be/gap/gap.git
    ```
-   De rest van de instellingen zal wel in orde zijn. Je kunt ze nog
    tweaken als je dat nodig vindt.

### Command line

-   In Windows Explorer klik je met de rechtermuisknop op een folder
    waarin je de code wilt uitpakken.
-   Klik 'Git bash here'.
-   Tik het volgende:
    ```
    git clone https://gitlab.chiro.be/gap/gap.git
    ```
-   Het is de eerste keer dat je op die server connecteert, je zult
    moeten bevestigen dat je de server vertrouwt.

Na enige tijd is de repository gedownload, en de dev-branch uitgepakt.
In je git-bash-venster tik je nu best eens `cd gap`, zodat je command
line-omgeving zich binnen je source code bevindt, wat handiger is voor
straks.

Connection string
-----------------

Als je nog geen [Database](Database.md) het opgezet, doe het dan nu. Vervolgens
moet je een connection string instellen. Dat moet vooral gebeuren in de
file `Web.Config` van het project `Chiro.Gap.Services`. Om met een
testgroep te werken, moet je dat ook aanpassen in
`Chiro.Gap.Poco.Context`.

In die app.config-bestanden zoek je naar de lijn die begint met
`<add name="ChiroGroepEntities"`, daar staat normaal:

```
<add name="ChiroGroepEntities"
connectionString="metadata=res://\*/ChiroGroepModel.csdl|res://\*/ChiroGroepModel.ssdl|res://\*/ChiroGroepModel.msl;provider=System.Data.SqlClient;provider
connection string='Data Source=.\\SQLEXPRESS;Initial
Catalog=gap\_local;Integrated
Security=SSPI;MultipleActiveResultSets=True;Application
Name=gap'" providerName="System.Data.EntityClient" />
```

Deze connection string werkt als je sql-server-instantie `SQLEXPRESS`
heet, je database `gap_local`, en dat je met Windows-authenticatie
voldoende rechten op de GAP-database hebt. Dat wil zeggen, minstens lid
zijn van de rol `GapRole`. Is je configuratie anders, dan pas je de
connection string aan.

Er staat ook een connection string in
Chiro.Gap.TestHacks.Properties.Settings. Zoeken op "Data
Source=.\\SQLEXPRESS" zal niet helpen, want in dat settingsbestand staat
er een dubbele backslash (.\\\\SQLEXPRESS).

Opgelet: die gewijzigde connection strings moet je niet mee committen.
Hoe regel je dat:
http://source.kohlerville.com/2009/02/untrack-files-in-git/

En verder...
------------

-   De solution file GAP is `gap/Solution/Cg2Solution.sln`, deze kun je
    openen in Visual Studio.
-   Er zijn twee startup projects: Chiro.Gap.Services
    en Chiro.Gap.WebApp. Klik in de solution explorer (rechterpaneel)
    rechts op de solution om die startup projects in te stellen ('Set
    Startup Projects'). Kies voor 'multiple startup project' en
    selecteer bij Services en WebApp de actie 'Start'.

Let op: lees het stukje over [branches](branches.md) voor je begint te hacken!

Als je de solution opent voor de eerste keer opent, kan het zijn dat er
projecten niet geladen worden en dat je de volgende foutmelding krijgt.

`error  : The imported project "C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0\WebApplications\Microsoft.WebApplication.targets" was not found. Confirm that the path in the <Import> declaration is correct, and that the file exists on disk.`

In die foutmelding staat een verwijzing naar het projectbestand waar het
probleem zich voordoet, bv. \[pad\]\\Chiro.Gap.WebApp.csproj. Open dat
bestand dan in een editor en verwijder de volgende regel.

`<Import Project="$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v14.0\WebApplications\Microsoft.WebApplication.targets" />`

Daarna kun je het project normaal gezien reloaden in Visual Studio.

Een SSH-key
-----------

Als je wijzigingen wilt kunnen pushen, heb je een ssh keypair nodig:

-   Duw op de Windows-key, tik 'git gui', en klik 'Git GUI'.
-   Klik 'Help', 'SSH key'.
-   Vervolgens klik je op 'Generate key'. Je mag op die key een
    wachtwoord zetten, maar dat is niet verplicht.
-   Na het genereren van de key, krijg je je public key te zien. Die koppel
    je aan je gitlab- of github-account (https://gitlab.chiro.be/profile/keys of
    https://github.com/settings/keys).
    Kopieer hem. Klik bovenaan deze pagina op 'my account', en plak hem
    in het venstertje 'public key'. Als identifier kun je je
    e-mailadres gebruiken.
-   Nu kun je de Git-GUI afsluiten.
