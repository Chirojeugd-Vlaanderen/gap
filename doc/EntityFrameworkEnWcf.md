De combinatie Entity Framework - Wcf
====================================

Updaten van gedetachte entities
-------------------------------

Om een entity van de Data Access layer naar de User Interface layer te
krijgen, moet deze entity 'gedetacht' worden van zijn 'objectcontext'.
Als je deze gedetachte entity nu updatet, en terug wil sturen naar de
data access layer, dan kunnen we dit entity wel terug 'attachen', maar
de object context weet dan niet welke velden gewijzigd zijn.

We kunnen dit probleem omzeilen door een object dat gewijzigd moet
worden, eerst te klonen. Nadat het object gewijzigd is, geven we het
samen met de kloon van het origineel terug aan de data access layer.

De data access layer zal dan het origineel 'attachen' aan de
objectcontext, en daarna de gegevens van de nieuwe entity kopiëren naar
het origineel. Op die manier is de objectcontext op de hoogte van de
wijzigingen, op het moment dat de wijzigingen bewaard moeten worden.

Concurrency
-----------

Stel dat gebruiker A en B eenzelfde object opvragen, en dat zowel A als
B het object willen wijzigen. Als A eerst is om zijn wijzigingen te
submitten, dan mag B zijn wijziging niet zomaar doorvoeren; hij weet
immers niet wat A ondertussen gedaan heeft.

Om dit probleem op te lossen, moeten we weten met welke 'versie' van het
object we te maken hebben. Hiervoor gebruiken we SQL Server timestamps.
Een timestamp heeft niets te maken met tijd, datum, of unix-timestamp,
maar is gewoon een nummer dat aangeeft in welke volgorde records door
SQL server gewijzigd zijn. Een timestamp is altijd uniek, en als een
record aangepast wordt, zal SQL server automatisch de timestamp updaten.

Door in het entitymodel de timestamp van een object 'concurrency mode'
fixed te geven, zal SQL server een exceptie opwerpen wanneer iemand
probeert een record te updaten, als de huidige timestamp van dat record
verschilt van de timestamp van de update.

Meer informatie
---------------

http://blogs.msdn.com/cesardelatorre/archive/2008/09/04/updating-data-using-entity-framework-in-n-tier-and-n-layer-applications-short-lived-ef-contexts.aspx.
