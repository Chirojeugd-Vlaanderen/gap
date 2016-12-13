Veel voorkomende waarschuwingen
===============================

ReSharper geeft waarschuwingen over code die niet voldoet aan allerlei
standaarden. Sommige van die waarschuwingen kunnen nogal obscuur lijken,
zeker als de code zonder problemen buildt en de applicatie werkt zoals
het hoort. Hier kunnen we daar meer uitleg over verzamelen.

Possible Multiple Enumeration of IEnumerable
--------------------------------------------

Zie [PossibleMultipleEnumeration](PossibleMultipleEnumeration.md).

Access to modified closure
--------------------------

Bronnen:

-   http://stackoverflow.com/questions/2951037/modified-closure-warning-in-resharper
-   http://weblogs.asp.net/fbouma/archive/2009/06/25/linq-beware-of-the-access-to-modified-closure-demon.aspx

Op die eerste pagina snappen ze ook niet zo goed waarom ze de
waarschuwing krijgen, maar ze geven wel aan dat ReSharper het 'probleem'
oplost door een 'overbodige' extra variabele te creëren. Op de tweede
pagina leggen ze uit waarom je die waarschuwing krijgt: het heeft met
scope te maken. Onze code kan zonder problemen werken als we geen
bewerkingen doen op de variabele die de waarschuwing genereert. Als we
wél een bewerking uitvoeren, loopt de loop in het honderd. Vandaar dat
ReSharper een extra variabele introduceert: die bestaat enkel binnen de
scope van 1 iteratie, en zelfs al wordt de waarde veranderd in de loop
van de iteratie, dan kan de loop toch verder zijn werk doen.

Possible InvalidOperation (in Linq-statement
--------------------------------------------

Het probleem is hetzelfde als bij "Access to modified closure", alleen
is het niet in een loop. Je krijgt ze bijvoorbeeld op
gp.Persoon.!GeboorteDatum in het volgende statement.

&lt;pre&gt;\
var afdeling = (from a in gwj.AfdelingsJaar\
where\
(gp.Persoon.GeboorteDatum.Value.Year - gp.ChiroLeefTijd &lt;=
a.GeboorteJaarTot\
&& a.GeboorteJaarVan &lt;= gp.Persoon.GeboorteDatum.Value.Year)\
select a).FirstOrDefault();\
&lt;/pre&gt;

Oplossing:

&lt;pre&gt;\
var gebdat = gp.Persoon.GeboorteDatum;\
var afdeling = (from a in gwj.AfdelingsJaar\
where\
(gebdat.Value.Year - gp.ChiroLeefTijd &lt;= a.GeboorteJaarTot\
&& a.GeboorteJaarVan &lt;= gebdat.Value.Year)\
select a).FirstOrDefault();\
&lt;/pre&gt;
