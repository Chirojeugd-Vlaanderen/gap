GAPIP14
=======

Praktische info voor de GAP-ploeg.

Vind ons:
---------

-   irc: \#\#gapip op freenode
-   twitter:
    [\#gapip](https://twitter.com/hashtag/gapip?f=realtime&amp;src=hash)

Wat kun je doen?
----------------

-   Programmeerwerk
    -   Nieuw als GAP-programmeur? Neem een kijkje in
        de [EasyHacks](EasyHacks.md).
        -   Stel gerust vragen als je vast zit.
    -   Werken aan de open issues voor version\#19.
        -   Het is niet de bedoeling dat al deze issues gefixt zijn
            voor release.
        -   Dit lijken me gewoon de meest dringende zaken.
    -   Werken aan eender welk issue dat je interessant of
        belangrijk vindt.
    -   Ongebruikte functies verwijderen.
    -   Naamgeving van de functies wat opkuisen (maak eerst een voorstel
        en vraag feedback).
    -   Opkuisen HTML/javascript van de frontend.
    -   Ben helpen met de API.
-   Het GAP uittesten en issues filen.
-   Documentatie in deze wiki bijwerken.
-   Open issues in het algemeen bekijken, en een nieuwe
    roadmap voorstellen.

Git-info
--------

-   Is git nieuw voor jou? Installeer [Git
    Extensions](http://code.google.com/p/gitextensions/). Vraag hulp aan
    je collega's wanneer je wilt committen of pushen.
-   De centrale repository: gitolite@develop.chiro.be:gap

**Warning! :)** Bij het stagen krijg je warnings over line endings. Je
mag die negeren. Als iemand met kennis van zaken hiervoor een propere
oplossing heeft, zie \#1778.

### De branch voor dit weekend is ''gapip14'':

**Tenminste** als je werkt aan de zomerrelease (version\#19) of aan de
[EasyHacks](EasyHacks.md). Voor andere projecten maak je een eigen branch.

&lt;pre&gt;\
git fetch origin\
git checkout gapip14\
&lt;/pre&gt;

### Committen: beter een keer te veel dan te weinig

Na ieder blokje wijzigingen dat bij elkaar hoort, commit je. Dit stuurt
niets naar de server, dit bewaart je wijzigingsgeschiedenis lokaal. Op
die manier kun je makkelijk terug naar een eerdere toestand.

&lt;pre&gt;\
git add nieuwefiles\
git commit -a\
&lt;/pre&gt;

### Rebasen: jouw wijzigingen op recentste code in branch gapip14

&lt;pre&gt;

1.  commit je wijzigingen:\
    git commit
2.  haal de recentste commits op van de server\
    git fetch origin
3.  pas jouw wijzigingen toe op de recentste code\
    git rebase origin/gapip14\
    &lt;/pre&gt;

#### Conflicten bij rebasen?

&lt;pre&gt;\
git mergetool\
&lt;/pre&gt;

Als conflicten opgelost:

&lt;pre&gt;\
git rebase --continue\
&lt;/pre&gt;

Conflictoplossing liep in het honderd:

&lt;pre&gt;\
git rebase --abort

1.  en hulp roepen (git-experts genoeg hier)\
    &lt;/pre&gt;

### Jouw wijzigingen pushen naar de gapip14-branch in de centrale repository

Als je een issue hebt gefixt, dan push je je fix naar de centrale
repository.

&lt;pre&gt;\
git commit

1.  eerst gaan we rebasen\
    git fetch origin\
    git rebase origin/gapip14
2.  en dan pas pushen\
    git push origin gapip14\
    &lt;/pre&gt;

### Foutje gemaakt?

(vraag gerust om hulp)

#### Alle wijzigingen ongedaan maken, en terug naar de toestand van de laatste commit:

&lt;pre&gt;\
git reset --hard\
&lt;/pre&gt;

#### Commit ongedaan maken

&lt;pre&gt;\
git reset HEAD\~1\
&lt;/pre&gt;

(Doe dit niet met een commit die je al hebt gepusht.)

### Ander probleem?

Er is erg veel mogelijk met git. Laat je helpen.

Copyrights
----------

Op de code die je bijdraagt, behoud je je copyrights, maar je geeft je
broncode vrij onder de [Apache License, Version
2.0](http://www.apache.org/licenses/LICENSE-2.0).\
Als je nieuwe source files toevoegt aan het project, dan mag je daar
deze header boven plakken:

&lt;pre&gt;\
Copyright 2014 Jouw Naam

Licensed under the Apache License, Version 2.0 (the "License");\
you may not use this file except in compliance with the License.\
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software\
distributed under the License is distributed on an "AS IS" BASIS,\
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.\
See the License for the specific language governing permissions and\
limitations under the License.\
&lt;/pre&gt;

Je kunt ook een meer algemene header gebruiken:

&lt;pre&gt;\
/\*\
\* Copyright 2014 the GAP developers. See the NOTICE file at the\
\* top-level directory of this distribution, and at\
\* https://develop.chiro.be/gap/wiki/copyright\
\*\
\* Licensed under the Apache License, Version 2.0 (the "License");\
\* you may not use this file except in compliance with the License.\
\* You may obtain a copy of the License at\
\*\
\* http://www.apache.org/licenses/LICENSE-2.0\
\*\
\* Unless required by applicable law or agreed to in writing, software\
\* distributed under the License is distributed on an "AS IS" BASIS,\
\* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.\
\* See the License for the specific language governing permissions and\
\* limitations under the License.\
\*/\
&lt;/pre&gt;

Pas je een bestaande file aan, waar bijvoorbeeld staat
`Copyright 2008-2013 the GAP developers`, dan kun je er
`Copyright 2008-2014 the GAP developers` van maken. Staat er in een
bestaande file `Copyright xxxx iemand-anders`, dan kun je daaronder
gewoon een lijntje toevoegen `Copyright 2014 Jouw Naam`, met eventueel
wat verduidelijking over wat je wijzigt.

Als je die auteursrechtenadministratie niet doet, is dat zo geen ramp:
we hebben sowieso de hele geschiedenis van de code in git. Maar wees je
ervan bewust dat alle broncode die je voor het GAP schrijft, onder
Apache-2-licentie valt. Het goede nieuws is bijgevolg dat je alle code
uit GAP mag hergebruiken voor je eigen projecten.
