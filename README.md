# GAP

Het GAP is het GroepsAdministratiePortaal van de Chiro.

## Nieuws

* 2017-03-07 [GAPIP komt eraan](doc/GAPIP.md)
* 2017-02-15 [Een API In dev](doc/news/api.md)
* 2017-02-02 [CAS-authenticatie in beta](doc/news/cas.md)

## Docker

Experimenteel: GAP met Docker en Linux.

### Container bouwen voor SQL Server
```
sudo docker-compose build
sudo docker-compose run mssql /opt/gap/mssqlsetup.sh
sudo docker-compose up
```

### Tools voor SQL server lokaal installeren

Installeer de [SQL server tools](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-setup-tools).

### Database inlezen:
```
/opt/mssql-tools/bin/sqlcmd -U sa -P Db_Root_Pw -d gap_local < database/sql/gap-testdata.sql
/opt/mssql-tools/bin/sqlcmd -U sa -P Db_Root_Pw -d gap_local < database/sql/gap-procedures.sql
```

### Connection string

Voor Mono is de connection string van de backend (met die entity-framework-hocus-pocus)
anders. Pas setup/connectionstring-docker.patch toe.

    patch -p 1 < setup/connectionstring-docker.patch

### Troubleshooting

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

## Documentatie

De [documentatie voor ontwikkelaars](doc/README.md) was vroeger een
wiki, maar staat nu gewoon bij [in de source code](doc/README.md).
Voor gebruikersdocumentatie kun je eens kijken op de
[gapwiki](https://gapwiki.chiro.be).

## Source en issue tracker

De officiële source code en issue tracker zijn gehost op onze
[gitlabserver](https://gitlab.chiro.be/gap/gap).  Er staat een [kloon van de repository](https://github.com/Chirojeugd-Vlaanderen/gap)
op GitHub, voor diegenen die geen Chiro-account hebben, en zich toch
aan een pull request willen wagen.

Als je in de issue tracker wilt werken, moet je in gitlab aanloggen
met je Chiro-account. Hiervoor klik je op de
[aanmeldpagina](https://gitlab.chiro.be/users/sign_in) op de link 'CAS'.

### Submodules

GAP gebruikt 2 git-submodules, namelijk [tools/Chiro.CiviCrm.Wcf](tools/Chiro.CiviCrm.Wcf)
en [tools/DotNetCasClient](tools/DotNetCasClient). Als je de git-repostirory 'gewoon'
clonet met `git clone`, dan komt de code van die submodules niet mee, en krijg je
compilatieproblemen. Zit je in dat geval, probeer dan eens:

```
git submodule init
git submodule update
```

## Chiro-account?

Als je geen Chiro-account hebt, contacteer dan
[de helpdesk](https://chiro.be/eloket/feedback-gap), en vermeld dat
je graag een account hebt voor de issue tracker van het GAP. Als je
roots hebt in de Chiro, vermeld dan ook de groep waarin je actief
bent geweest.
