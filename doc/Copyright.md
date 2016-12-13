Stagiairs, vrijwilligers en auteursrecht
========================================

(Dit is een suggestie. En een kladje. Zie ook [CopyRightDoc](CopyRightDoc.md))

Aanleiding: De stage van Arno Soontjens
---------------------------------------

Arno Soontjens komt naar alle waarschijnlijkheid een stage bij ons doen,
met als bedoeling de GAP gebruiksvriendelijker te maken. Een
interessante alinea uit de stagebundel die we kregen, heeft te maken met
auteursrecht:

&gt; De student is schrijver van zijn eindwerk. Aangezien hij als
student geen werknemer van de stageplaats is heeft de student dus ook
automatisch het auteursrecht op “alles” wat hij schrijft. Dit kan de
eindwerktekst zijn, maar ook alle andere teksten zoals handleidingen,
software (softwarecode = tekst) en dergelijke. Wens je als stageplaats
het volledige recht te behouden over de geschreven teksten (denk aan
software of handleidingen die later eventueel gecommercialiseerd worden)
dan kan je best vooraf een verklaring opstellen waarbij de student
afstand doet van zijn auteursrecht.

Interessant. Want dit is ook van toepassing op onze vrijwilligers. De
vrijwillige programmeurs hebben auteursrecht op alle code die zij
schrijven (zie de [conventie van
Bern](http://nl.wikipedia.org/wiki/Conventie_van_Bern_%281886%29)). In
principe zouden de vrijwilligers op een mooie dag kunnen zeggen: ´Dit is
mijn code, en jullie mogen ze niet meer gebruiken.´ We hebben het hier
al eerder over gehad, maar misschien is dit een aanleiding om het (voor
gap alvast) officieel te regelen.

Afstand doen van auteursrecht is inderdaad een optie, maar ik zou het
sympathiek vinden moesten we het zodanig organiseren dat een
vrijwilliger/stagiair zijn eigen auteursrechten behoudt. Zoals blijkbaar
ook de hogeschool dat doet. Dan kan een stagiair of een vrijwilliger
zijn eigen code sowieso ook elders gebruiken, zonder bijkomende
voorwaarden. Maar natuurlijk wel zodanig dat wij (de Chiro) het werk dat
hij bij zijn stage/vrijwilligersperiode afleverde wel kunnen blijven
gebruiken en aanpassen.

Mijn concreet voorstel: we vragen een vrijwilliger/stagiair om de code
die hij voor ons schrijft, vrij te geven onder een open-sourcelicentie.
Hij behoudt zijn auteursrecht (als hij dat zou willen mag hij achteraf
zijn code ook onder een andere licentie voor andere projecten gebruiken,
want hij is auteur), en wij (de Chiro en algemener: iedereen) mogen
verder bouwen op wat hij (onder open-sourcelicentie) afleverde tijdens
zijn stage/vrijwilligersperiode.

Er bestaan een heel aantal
[open-sourclicenties](http://opensource.org/licenses/category). Mijn
favoriete licentie voor webapplicaties is de [GNU Affero General Public
License v3](http://www.gnu.org/licenses/agpl-3.0.html), die wordt onder
andere gebruikt door civicrm, launchpad en statusnet. Maar omwille van
een paar praktische problemen, kunnen we die niet direct gebruiken
(hierover later meer). Mijn voorstel is om GPLv3 te gebruiken.

GPLv3 (GNU General Public License, versie 3)
--------------------------------------------

**UPDATE:** We kunnen voorlopig geen GPL-licenties gebruiken voor gap,
omdat die in conflict is met de softwarelicentie van Unity (Ms-PL).
Unity is een .NET-project van Codeplex, dat we gebruiken voor dependency
injection.

Dus stel ik voor om de
[GPLv3](http://www.gnu.org/licenses/quick-guide-gplv3.html) te
gebruiken. In grote lijnen komt het erop neer dat iedereen de software
en de code mag gebruiken voor eender welk doel. Zonder enige garantie,
maar ook zonder kost. Op voorwaarde dat je niet moeilijk doet over
patenten.

Er zijn bijkomende voorwaarden voor als je de software wilt verkopen of
uitdelen. Die zijn voor GAP niet van toepassing, want wij
verkopen/verdelen de software niet, we draaien ze alleen op onze
webserver. (Zoals we bijvoorbeeld ook Drupal, onder GPL-licentie,
gebruiken voor onze website.) Maar voor de volledigheid vermeld ik die
distributievoorwaarden wel, want ze vormen de essentie van de GPL:

Als je de software opnieuw verdeelt (al dan niet aangepast):

-   dan moet de software (inclusief eventuele aanpassingen
    of uitbreidingen) opnieuw verspreid worden onder GPLv3 of een hogere
    versie
-   je moet de source code van de software, inclusief die van
    aanpassingen of uitbreidingen, mee verdelen, zodat de ontvanger
    desgewenst jouw software opnieuw kan aanpassen.

(Aanpassingen en uitbreidingen zijn vrij ruime begrippen. Stel dat je
een softwarelibrary gebruikt met een GPLv3-licentie in jouw
softwareproject, en je wilt dat project verspreiden/verkopen, dan moet
je dat ook onder GPL-licentie doen. Maar nogmaals, voor GAP is dat
allemaal niet van toepassing, omdat we geen software verspreiden of
verkopen. We bieden enkel een webtoepassing aan.)

Wat zijn de voordelen:

-   Iedereen mag de source code gebruiken. Op die manier helpen we
    misschien bevriende organisaties, en hopelijk worden ze op die
    manier geinspireerd om gelijkaardige initiatieven te nemen.
-   Stel dat er iemand ooit de source oppikt, en begint te verkopen. Dan
    zal die persoon waarschijnlijk geneigd zijn om verbeteringen aan de
    software aan te brengen, op vraag van zijn klanten. Goed nieuws,
    want die verbeteringen moeten opnieuw onder GPLv3-licentie zijn, en
    dan kunnen wij die ook gebruiken als we ze interessant vinden.
-   We kunnen achteraf makkelijk naar Affero General Public
    License migreren.

Andere mogelijkheden
--------------------

### Apachelicentie (versie 2)

De [apachelicentie](http://www.apache.org/licenses/LICENSE-2.0.html)
(versie 2) is minder strict dan de GPL. Als je software onder een
apachelicentie hebt, en je verspreidt een aangepaste of uitgebreide
versie, dan ben je niet verplicht om jouw wijzigingen/uitbreidingen
onder dezelfde licentie vrij te geven.

Op die manier vergroot je het publiek dat jouw software mogelijk wil
gebruiken, omdat ze bijvoorbeeld een verbeterde versie kunnen
distribueren, zonder dat ze de verbeteringen publiek moeten maken. Dat
geldt dan trouwers ook voor ons.

### GNU Affero General Public License (versie 3)

De [GNU Affero General Public License
v3](http://www.gnu.org/licenses/agpl-3.0.html) (aGPL) is stricter dan de
GPLv3. Dit is een licentie voor webapplicaties/webservices, en ze
bepaalt dat je de broncode van je project (al dan niet
aangepast/uitgebreid) beschikbaar moet maken voor je gebruikers, ook al
verspreid je de software niet.

Concreet wil het zeggen dat je webapplicatie ergens een linkje bevat
´source code verkrijgen´.

Verder zijn dezelfde bepalingen van kracht als bij de ´gewone´ GPLv3.

Dit kunnen we echter niet zomaar doen. Want stel dat de code van de
stagiair aGPL is, dan zouden we heel de code van het GAP aGPL moeten
maken, en beschikbaar moeten stellen.

Om een licentie toe te passen op heel de code van het GAP, hebben we de
toestemming nodig van alle auteurs. Ik vermoed dat de vrijwilligers van
het GAP daar niet te moeilijk over zullen doen, maar hier en daar hebben
we ook source code waarvan we niet de auteurs kennen. (I.h.b. is de
source code van ´ServiceHelper´ gebaseerd op code van onduidelijke bron.
Dat stuk zou dus herschreven moeten worden. Wat ook niet onoverkomelijk
is, want over zo veel code gaat het niet.)

Dus - tenzij we bovenstaande problemen aanpakken - is aGPL niet direct
mogelijk. Nochtans zijn er voordelen:

Als er andere organisaties GAP zouden gebruiken, en wijzigingen
aanbrengen, dan kunnen wij die wijzigingen overnemen (op voorwaarde dat
we ze interessant vinden)

Bovendien maakt dit de drempel voor de werkgroep GAP misschien lager,
omdat iedereen direct de code kan downloaden, en ermee experimenteren.

Suggestie voor teksten die geen source code zijn
------------------------------------------------

Ik denk dat we ook iets analoogs kunnen doen voor teksten die geen
source code zijn (dubbelpuntartikels, liedjes, publicaties,...). Dat is
minder mijn winkel (tenzij voor documentatie), maar ik heb wel een
suggestie: Mijn favoriete licentie op dit vlak is de [CC-BY-SA
3](http://creativecommons.org/licenses/by-sa/3.0/deed.nl), die wordt
gebruikt door o.a. wikipedia.
