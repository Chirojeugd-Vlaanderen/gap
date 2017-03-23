# Ontwikkelen met Linux

Het heeft bijna 10 jaar geduurd, maar vandaag kan gap-ontwikkeling
quasi helemaal met Linux en Mono gebeuren. En bijgevolg kunnen we ook gebruik maken
van de CI-functionaliteit van Gitlab.

## Praktische gevolgen

Ik heb hiervoor een paar kleine wijzigingen gedaan aan de dev-branch:

* CAS-authenticatie staat uit in de dev-branch, omdat dat ook nog niet goed werkt
  met mono. In staging en live staat dat aan. En in de publieke dev-omgeving staat
  CAS-authenticatie ook aan.
* De webservices in de dev-branch worden aangesproken met een BasicHttpBinding ipv
  WsHttpBinding, omdat die laatste niet beschikbaar is voor mono. Net zoals met CAS,
  wordt de WsHttpBinding ook geënabled in stagin, live, en de publieke dev-site.
* De unit tests gebeuren nu met NUnit, in plaats van MsTest. Uit praktische
  overweging is het nog NUnit2, omdat die runner het makkelijkst te installeren
  was in onze container voor CI. Om unit tests uit te voeren vanuit Visual Studio,
  moet je de extensie installeren voor Nunit 2. (Dat kan via Tools, Extensions.
  Zoek naar alle extensies, ook die op het internet.)

## Een werkende dev-omgeving onder Linux

### Software

In mijn setup gebruik ik Fedora 25, met deze packages:

    sudo dnf install docker docker-compose mono-devel
    # installeer ook de sql server client tools, om de database te maken:
    curl https://packages.microsoft.com/config/rhel/7/prod.repo > /etc/yum.repos.d/msprod.repo
    sudo dnf install -y --allowerasing mssql-toolsdnf install -y --allowerasing mssql-tools

Als IDE gebruik ik nu [Rider](https://www.jetbrains.com/rider/), want dat
is van Jetbrains, en dat gedraagt zich gelijkaardig aan Resharper. Maar met
[monodevelop](https://nl.wikipedia.org/wiki/MonoDevelop) gaat het ook.
Monodevelop is free software, en zit in de repository's. Maar om bijvoorbeeld
frontend en backend samen te debuggen, moet je twee instanties van Monodevelop
opstarten, terwijl dat met Rider in één instantie kan. Ook de unit tests
kreeg ik makkelijker aan de praat met Rider, maar als je wat zoekt, zal het
met Monodevelop ook wel lukken.

### SQL server

Voor SQL server gebruik ik docker. Setup van de databaseserver, gaat zo:
```
sudo docker-compose build
sudo docker-compose run mssql /opt/gap/mssqlsetup.sh
sudo docker-compose up
```

En zo maak je de database:
```
/opt/mssql-tools/bin/sqlcmd -U sa -P Db_Root_Pw -d gap_local < database/sql/create-db.sql
/opt/mssql-tools/bin/sqlcmd -U sa -P Db_Root_Pw -d gap_local < database/sql/gap-testdata.sql
/opt/mssql-tools/bin/sqlcmd -U sa -P Db_Root_Pw -d gap_local < database/sql/gap-procedures.sql
```

Je moet dan nog de connection string aanpassen, opdat je zou kunnen debuggen
op je eigen database:

    patch -p1 < setup/connectionstring-docker.patch

### Debuggen

Open Solution/Cg2Solution.sln in je IDE. De backend start je op
localhost:2734, met Rider kan dat als volgt:

* Klik 'Run', 'Edit configurations'
* Klik onder '.NET Project' 'Chiro.Gap.Services' aan, en maak een kopie
  van die configuratie. Vervang bij 'arguments' `--port=5001` door `--port=2734`.
  Je moet dat in een kopie van de configuratie doen, want anders 'vergeet'
  Rider die instelling iedere 5 voet.
* Klik 'Ok'.
* Klik 'Run', 'Debug....', en klik de nieuwe configuratie die je net maakte aan.
* De backend wordt nu gestart.

Als je naar je backend surft op http://localhost:2734, dan krijg je een 404.
Om te kijken of er iets draait, kun je surfen naar
http://localhost:2734/HelloService.svc

Dan start je de frontend. met Rider:

* Klik 'Run', 'Debug...'.
* Klik 'Chiro.Gap.WebApp' aan. Hiervan hoef je geen kopie te maken.

Rider start de toepassing standaard op http://localhost:5000, monodevelop op
http://localhost:8080.

Dat zou dan moeten werken.


## Troubleshooting

Krijg je een foutmelding over `/etc/mono/registry`, maak die directory dan aan,
en zorg ervoor dat de user waarmee je debugt schrijfrechten eheft in die folder.

Krijg je een exception met deze boodschap:
```
Column 'InvariantName' is constrained to be unique.  Value 'System.Data.Odbc' is already present.
```
Comment dan in `/etc/mono/4.5/machine.config` deze lijnen uit:
```
            <add name="Odbc Data Provider"         invariant="System.Data.Odbc"
                 description=".Net Framework Data Provider for Odbc"
                 type="System.Data.Odbc.OdbcFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/>
            <add name="OleDb Data Provider"        invariant="System.Data.OleDb"
                 description=".Net Framework Data Provider for OleDb"
                 type="System.Data.OleDb.OleDbFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/>
```
En deze ook:
```
            <add name="SqlClient Data Provider"    invariant="System.Data.SqlClient"
                 description=".Net Framework Data Provider for SqlServer"
                 type="System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/>
```
