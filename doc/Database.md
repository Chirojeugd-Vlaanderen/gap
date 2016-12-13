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

[**Download
devdbdump.zip**](https://develop.chiro.be/johan/devdbdump.zip).

In dat zip-bestand zitten 3 files:

-   `gap_schema_en_testdata.sql`. Dit script genereert heel het
    databaseschema, en vult het op met testdata.(Dit zijn random
    gegevens, en niet de echte gegevens van Chiroleden.) Run het vanop
    je nieuwe GAP-database. Je krijgt misschien nog een paar
    foutmeldingen omdat er nog een aantal views worden gecreeerd die nog
    verwijzen naar Kipadmin. Maar misschien ook niet. Als je dit
    uitgetest hebt, pas je dat dan even aan in deze wiki aub? :-)
-   `gap.straten.sql`. De testdata bevat slechts een beperkt aantal
    straten, en dat is wat vervelend als je adressen wil invoeren bij
    het debuggen. Het script gap.straten.sql vult je stratenlijst aan
    met een (verouderde) stratenlijst. Je krijgt een paar honderden
    foutmeldingen over dubbels, maar dat is omdat het dumpbestand al een
    aantal van de straten bevat; de foutmeldingen mag je weer negeren.
-   `gap_schema.sql` bevat enkel het databaseschema, zonder data. Voor
    als je dat eens nodig zou hebben.

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
