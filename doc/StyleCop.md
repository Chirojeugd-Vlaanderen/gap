Code review met StyleCop
========================

Wat is StyleCop?
----------------

StyleCop is een add-in voor Visual Studio (en MSBuild) waarmee je
C\#-code kunt reviewen.

Je vindt de installer, een discussieforum en een issue tracker op
http://code.msdn.microsoft.com/sourceanalysis.

StyleCop werkt met een vaste set regels, maar je kunt er ook zelf
toevoegen. Op
http://www.lovethedot.net/2008/05/creating-custom-rules-for-microsoft.html
en http://www.codeproject.com/KB/cs/StyleCop.aspx vind je daarvoor een
stappenplan.

De code voor het Chirogroepprogramma wordt nagekeken aan de hand van die
add-in. De instellingenbestandjes (Settings.!StyleCop) zijn mee
ingecheckt per project, maar ze zijn niet geïncludet. Als je Visual
Studio dus alleen de bestanden laat tonen die deel uitmaken van de
solution, zie je ze niet staan.

Instellingen
------------

Er is een globaal instellingenbestand toegevoegd op het niveau van de
solution (source:Solution/Settings.StyleCop). Elk project neemt die
automatisch over. Globale wijzigingen aan die instellingen moeten dus
ook op het niveau van de solution gebeuren.

Je kunt die globale instellingen per project overriden door ze op
projectniveau aan te passen. Voor de projecten in de namespace Chiro.Adf
(die code bevatten die we niet zelf geschreven hebben) is ingesteld dat
die globale instellingen niet overgenomen worden.

Als je defaultinstellingen op projectniveau verandert, maakt StyleCop
een nieuw instellingenbestand aan, op projectniveau. Denk er dan wel aan
dat je dat nog aan de source repository moet toevoegen. Een voorbeeld:
source:Solution/Chiro.Adf/Settings.StyleCop.

Meer uitleg over hoe instellingen werken en geërfd dan wel 'overridden'
worden:
http://blogs.msdn.com/sourceanalysis/pages/sharing-source-analysis-settings-across-projects.aspx.
