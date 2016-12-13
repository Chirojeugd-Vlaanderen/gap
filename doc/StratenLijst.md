We gebruiken de
[CRAB-stratenlijst](http://www.agiv.be/gis/nieuws/?artid=463) om ons
adressenbestand te standaardiseren.

Het originele bestand (komende van Agiv) is opgenomen in subversion:
source:trunk/3rd-party/crab.

Een aantal opmerkingen:

**** We opteren er voor om straat- en gemeentenamen waarvan er
verschillende namen bestaan (bv in het Nederlands en het Frans)\
de verschillende versies op te nemen in ons bestand.

**** Bij het converteren van CRAB naar CG2 formaat, moeten we er voor
zorgen dat we mogelijke updates kunnen doorvoeren,\
hoe zo'n update er uitziet kunnen we nog niet zeggen, maar we nemen van
elke CRAB tabel die we gebruiken de ID mee naar\
onze CG2 tabel. Door dit te doen hopen we dat we alle updates kunnen
uitvoeren.

------------------------------------------------------------------------

Onderstaand is niet meer geldig.

Het importeren van de stratenlijst in de testdatabase gebeurde via het
projectje 'CrabConverter'. Dit is niet meer up to date.
