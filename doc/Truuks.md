Truuks
======

Groepen fusioneren
------------------

Om 2 groepen te fusioneren in GAP, is het het gemakkelijkst als de
nieuwe groep al bestaat in Kipadmin, zodat de gegevens op de
'traditionele wijze' geïmporteerd kunnen worden.

Maak dus eerst de nieuwe groep aan in Kipadmin. Hiervoor kun je de
volgende stored procedure (in Kipadmin) gebruiken:

```
exec grp.spFusieGroepMaken 'oudstamnr1', 'oudstamnr2', 'nieuwstamnr',
'nieuwegroepsnaam', 'soort'
```

(soort is vermoedelijk 'g', een gemengde groep. 'j' voor jongens, 'm'
voor meisjes.)

Zorg er zeker voor dat de volgende gegevens in orde zijn:

**** heeft de groep een zesde afdeling?

**** zo ja, de naam van die zesde afdeling

Vervolgens gebruik je deze stored procedure (in GAP) om de personen van
de groepen te fusioneren, en een standaardafdelingsverdeling te maken
voor het huidige groepswerkjaar:

```
exec data.spChiroGroepenFusioneren
werkjaar,'oudstamnr1','oudstamnr2','nieuwstamnr', 'nieuwegroepsnaam'
```

Je krijgt als output een aantal mogelijk dubbele personen.

Dubbele personen verwijderen
----------------------------

```
exec data.spDubbelePersoonVerwijderen
[PersoonIdTeVerwijderenPersoon](PersoonIdTeVerwijderenPersoon.md),
[PersoonIdTeBehoudenPersoon](PersoonIdTeBehoudenPersoon.md)
```

(Als te behouden persoon kies je uiteraard een persoon waarvan de
informatie allemaal juist is!)

Nieuwe groep maken.
-------------------

Er is in gap ook een stored procedure om een nieuwe groep te maken.
Voorbeeld:

```
exec grp.spNieuweGroep 'om /1218','Flo','Gent',2015
```

Jaarovergang ongedaan maken
---------------------------

Om een jaarovergang ongedaan te maken, kun je de stored procedure
data.spGroepsWerkJaarVerwijderen gebruiken. **Doe dit enkel in een
testdatabase!** (Anders moet je in Kipadmin de jaarovergang ook ongedaan
maken)

Een voorbeeld:

```
exec data.spGroepsWerkJaarVerwijderen 'mm /0813',2011
```
