De dev-database
===============

SQL server
----------

GAP werkt met een Microsoft SQL server database. Als er geen SQL server
instantie draait op je PC, dan kun je de [express edition van SQL
Server](https://www.microsoft.com/en-us/server-cloud/products/sql-server-editions/sql-server-express.aspx)
downloaden en installeren.

Als je die installatie nog moet doen, dan noem je je database-instantie
best `SQLEXPRESS` (ik denk dat dat standaard is voor de express
edition). Als je de GAP source code downloadt, dan gaat die namelijk
naar zijn database op zoek in de instantie `.\SQLEXPRESS`, en als jouw
instantie dus ook zo heet, hoef je dat al niet te herconfigureren.

Database
--------

Je moet een database aanmaken voor het GAP. Het is het makkelijkst als
je die `gap_local` noemt, weer om de reden dat je dan niets moet
herconfigureren in de source code.

Nu moet je je database nog opvullen.

Tegenwoordig zitten de dumps bij in de [source code](database/sql).
Je vindt in [database/sql](database/sql) deze bestanden:

In dat zip-bestand zitten 3 files:

-   [gap-tabellen.sql](database/sql/gap-tabellen.sql). Dit maakt de tabellen voor het GAP.
-   [gap-testdata.sql](database/sql/gap-testdata.sql). Gerandomizede testadata.
-   [gap-straten.sql](database/sql/gap-straten.sql). Namen van straten, gemeentes, landen.
    Als je deze uitvoert, krijg je wat foutmeldingen omdat er al wat straten bij in de
    testdata zitten. Die fouten mag je dan negeren.
-   [gap-keys-indexes.sql](database/sql/gap-keys-indexes.sql). Deze maakt keys en indexes.

Je voert de bestanden best in die volgorde uit. Dan zou je database
in orde moeten zijn om het GAP (frontend en backend) te draaien.

Voor de API heb je nog een extra database nodig: `gap_api_auth`. Die mag
leeg zijn, de tabellen daarin worden automatisch aangemaakt.

Zorg ervoor dat je met de user waaronder je werkt toegang hebt tot die
database met windows authenticatie, dat maakt je leven straks wat
makkelijker.

Changes
-------

Als je nog wijzigingen moet toepassen aan je database, dan zijn die
wijzigingen gescript in het changescript:
source:database/changes/CHANGES.sql.

Connection string
-----------------

Als je database-instantie niet `.\SQLEXPRESS` is, of je database heet
niet `gap_local`, of je gebruikt geen Windowsauthenticatie, dan moet je
de connection string in de backend aanpassen. Die staat ergens in
source:Solution/Chiro.Gap.Services/Web.config.
