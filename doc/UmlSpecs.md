**Deze informatie is niet meer correct.**

De specificaties voor het project zijn gemaakt met Enterprise Architect.
De UML zit in Subversion (\[source:trunk/Analyse\]), en het EA-project
zit in de SQL server database ChiroGroepSpecs op
database.chiro.lokaal\\SQL2K5.

De database is enkel toegankelijk vanop het ChiroLAN. Om dit thuis te
doen, heb je een VPN-connectie nodig. (Zie [VpnGebruiken](VpnGebruiken.md).)

Enterprise Architect is geinstalleerd op devserver.

Wat je vooraf moet doen
=======================

**** De source code uitpakken. Zie [EersteCheckOut](EersteCheckOut.md).

**** [VisualSVN](VisualSVN.md) installeren. Zie [VisualSVN](VisualSVN.md)

De specificaties voor de eerste keer openen
===========================================

Start Enterprise Architect, en klik 'File', 'Open project...'.

[Image(openproject.png)](Image(openproject.png).md)

Vink 'Connect to Server' aan, en klik op de knop met de drie puntjes
(...).

[Image(connecttoserver.png)](Image(connecttoserver.png).md)

Als 'Data Provider' kies je 'Microsoft OLE DB Provider for SQL Server'.
Klik op 'Next &gt;&gt;'.

[Image(dataprovider.png)](Image(dataprovider.png).md)

Vul deze gegevens in:

**** server name: database.chiro.lokaal\\SQL2K5

**** Use Windows NT Integrated security

**** database: [ChiroGroepSpecs](ChiroGroepSpecs.md)

[Image(connection.png)](Image(connection.png).md)

Een klik op 'Test Connection' zou dan 'Test connection succeeded.'
moeten tonen.

Klik tenslotte op Ok. Je krijgt dit venster:

[Image(connectionnametype.png)](Image(connectionnametype.png).md)

Klik nogmaals op Ok. Je krijgt opnieuw het venster 'Open Project', waar
bij 'Project to Open' een soort van connection string ingevuld is. Klik
tenslotte op 'Open' om het project daadwerkelijk te openen.

[Image(openproject2.png)](Image(openproject2.png).md)

Enterprise Architect zal merken dat het project in versiecontrole zit.
Op de vraag of je de configuratie wil vervolledigen, antwoord je 'Yes'.

[Image(completeconfiguration.png)](Image(completeconfiguration.png).md)

Achter 'Working Copy Path' zet je het pad van je lokale versie van de
map \[source:trunk/Analyse\]. Achter 'Subversion Exe Path' zet je de
volledige locatie van svn.exe (command-line subversion executable). Deze
wordt meegeleverd met VisualSvn, en is meestal
```
C:\\Program Files\\VisualSvn\\bin\\svn.exe
```

[Image(versioncontrolsettings.png)](Image(versioncontrolsettings.png).md)

Klik op 'Save', en vervolgens op 'Close'.

(Als je de boodschap krijgt dat het certificaat niet geldig is, lees dan
eerst \[\[UmlSpecs\#Watjevoorafmoetdoen\]\]. Je kan eventueel even
switchen naar Windows Explorer om het probleem op te lossen, en daarna
op OK klikken in EA.)

De specificaties een volgende keer openen
=========================================

Als je Enterprise Architect opnieuw start, dan kan je het project
onmiddellijk openen door op de link 'chirogroepspecs' te klikken in het
lijstje met recente projecten.
