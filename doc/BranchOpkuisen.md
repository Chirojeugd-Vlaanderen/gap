# Een branch opkuisen

Stel: je hebt een feature branch gemaakt, en je bent tevreden over het
resultaat. Je hebt vaak gecommit (dat is overigens erg verstandig), maar
hierdoor is je git-log wat rommelig geworden. Hij ziet er bijvoorbeeld zo uit
(recentste wijziging bovenaan):

```
41ed59d Kleine aanpassing aan de unit test.
1aaf7ae Fix van een bug in de eerste wijziging.
88cb2ae Een unit test.
2863121 Wat lokale wijzigingen die ik niet wil bijhouden maar die ik toch committe.
224bc98 Een tweede wijziging voor mijn nieuwe feature.
b1c937e Een wijziging voor mijn nieuwe feature.
fce8c1a Hier zat dev toen ik mijn feature branch maakte.
```

Je zou er dan voor kunnen kiezen om die history wat op te kuisen. Door
bijvoorbeeld de wijzigingen aan de unit test bijeen te zetten in 1 commit. En
die fix in 1aaf7ae had je misschien ook graag bij in 224bc98 gehad.

Om dit te doen, gaan we met de geschiedenis van je code foefelen. Omdat dat wel
eens mis kan gaan, maken we voor de zekerheid een branch bij, zodat we de
huidige toestand van de repository nog hebben voor als het helemaal mis gaat:

    git branch mijnfeaturebranch-backup

Nu kunnen we aan de slag. We willen de geschiedenis na commit fce81ca
herschrijven, en we doen dit door interactief te rebasen. Dat kan met dit
commando:

    git rebase -i fce8c1a

Er wordt een text-editor geopend, met deze lijnen:

```
pick b1c937e Een wijziging voor mijn nieuwe feature.
pick 224bc98 Een tweede wijziging voor mijn nieuwe feature.
pick 2863121 Wat lokale wijzigingen die ik niet wil bijhouden maar die ik toch committe.
pick 88cb2ae Een unit test.
pick 1aaf7ae Fix van een bug in de eerste wijziging.
pick 41ed59d Kleine aanpassing aan de unit test.
```

Hier staat de oudste commit bovenaan. We passen de volgorde van de lijnen aan,
zodat de commits in de volgorde staan die we graag willen. Commits die we niet
meer nodig hebben, laten we weg.

```
pick 88cb2ae Een unit test.
pick 41ed59d Kleine aanpassing aan de unit test.
pick b1c937e Een wijziging voor mijn nieuwe feature.
pick 1aaf7ae Fix van een bug in de eerste wijziging.
pick 224bc98 Een tweede wijziging voor mijn nieuwe feature.
```

We willen ook graag dat commit 41ed59d wordt samengenomen met 88cb2ae, en commit
1aaf7ae met b1c937e. Om dat te bereiken, vervangen we het woord 'pick' door
'squash'. Stel dat we ook niet tevreden waren over de commit message van die
laaste commit, dan kunnen we die ook nog aanpassen, door 'pick' te veranderen
in 'reword'.

```
pick 88cb2ae Een unit test.
squash 41ed59d Kleine aanpassing aan de unit test.
pick b1c937e Een wijziging voor mijn nieuwe feature.
squash 1aaf7ae Fix van een bug in de eerste wijziging.
reword 224bc98 Een tweede wijziging voor mijn nieuwe feature.
```

Als je dan bewaart en afsluit, begint git met zijn magie.

Als je alles erg door elkaar gooide, kan het zijn dat je merge conflicts krijgt.
Gebruik je merge-tool om die op te lossen. Een goeie merge-tool is daarbij goud
waard, want die kan heel wat problemen automatisch oplossen. Git Extensions komt
met kdiff3, dat is een van de betere in zijn soort. Maar er zijn er nog.

Als je het conflict hebt opgelost, dan save je je files, je hoeft de wijzigingen
niet te committen, maar je hervat het rebaseproces met

    git rebase --continue

Het kan zijn dat je dan weer op een conflict stuit. Je zult moeten blijven
conflicten oplossen en 'rebase --continue'en tot alle conflicten eruit zijn.

Voor gesquashte en gereworde commits moet je een nieuwe commit-message ingeven,
daarvoor wordt ook een text editor geopend.

Als alles dan uiteindelijk gelukt is, heb je de mooie git history die je wou:

```
174afaf Wat meer uitleg bij mijn tweede wijziging.
26546b9 Een wijziging voor mijn nieuwe feature.
295abc2 Een unit test.
fce8c1a Hier zat dev toen ik mijn feature branch maakte.
```

Je pusht je branch nog eens met `--force`, want veranderde geschiedenis pushen
doe je natuurlijk niet zomaar. En dan kun je je merge request maken.

Moest het tijdens het rebasen ergens helemaal mislopen, dan kun je je rebase
afbreken met

    git rebase --abort
