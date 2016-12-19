Volledige testomgeving
======================

Zie \#1234.

Zoals het wordt
---------------

Bedoeling is dat we op termijn onder andere nightliy builds kunnen
aanbieden op http://gap.chiro.be/test. Hiervoor moet enerzijds de
solution dagelijks gebuild en gedeployd kunnen worden, en anderzijds
moet de database dagelijks gemigreerd kunnen worden van live naar test.

### Databasemigratie

**** De livedatabase is momenteel 'gap' op kipsrv1\\sql2k5.

**** De testdatabase is 'gap\_test' op devsrv1\\sql2k8.

**** (De developmentdatabase is 'gap\_dev' op devsrv1\\sql2k8.)

Om live over te zetten naar test, moeten deze scripts uitgevoerd worden:

**** source:database/TestSetup/1\_restore\_db.sql - zet een backup van
de live terug. Momenteel is dat een backupbestand dat staat in
\\\\devsrv1\\c\$\\tmp, maar op termijn moet dat de laatste dagelijkse
backup worden. (Dagelijkse backups staan op dit moment in
\\\\kipsrv7\\sqlbackup\$)

**** source:database/TestSetup/2\_anonimize\_data.sql - verwijdert
persoonlijke gegevens uit de database

**** source:database/TestSetup/3\_verwijder\_gebruikers.sql - neemt
gebruikersrechten weg van niet-testers, om te vermijden dat mensen hun
groepsadministratie bijhouden in de testomgeving

**** source:database/TestSetup/4\_live\_naar\_dev.sql - sql scripts om
de live-database om te vormen naar de dev-database

(Er zijn ook nog 3 scripts voor de dev-omgeving:
source:database/TestSetup/X\_dbUsers.sql,
source:database/TestSetup/X\_rechtendevs.sql en
source:database/TestSetup/X\_unit\_tests.sql. De output van dat laatste
script moet overgenomen worden in
source:Solution/TestProjecten/Chiro.Gap.TestDbInfo/TestInfo.cs.)

**TODO**: scriptje voor users van 'de test', zoals bijv Merijn.
