Branches
========

**OPMERKING:** Je kunt enkel pushen naar onze repository als je
schrijfrechten hebt. Heb je die niet, dan mag je ook een patch aan een
issue hangen, en de issue status op 'needs review' zetten.

**TODO:** Deze tekst bijwerken, meer ingespeeld op hoe het werkt voor
GAP. De GAP-workflow wordt wel min of meer uitgelegd in deze video over
git extensions: http://youtu.be/n-WbItytu0U

(Als je nog geen 'git-bash-venster' open hebt staan, klik dan in je
solution explorer rechts op een bestand, en kies 'git', 'git bash').

Lokale branches
---------------

Als je git gebruikt, dan is het de gewoonte dat je voor elke feature die
je implementeert, een lokale feature branch maakt. Om te kijken welke
branches je lokaal hebt, typ je in je bashvenster

```
git branch
```

De output ziet er dan ongeveer als volgt uit

```
eenbranch
nogeenbranch

-   dev
    ```

Het sterretje voor 'dev' geeft aan dat de dev-branch op dit moment is
uitgepakt.

Een nieuwe feature branch maak je door het volgende in te tikken:

```
git branch mijnfeaturebranch
```

Dit commando maakt een branch ('mijnfeaturebranch') vertrekkende van de
momenteel uitgepakte branch (in dit geval 'dev'). Om in die branch te
beginnen werken, moet je hem nog 'uitchecken':

```
git checkout mijnfeaturebranch
```

Alle bewerkingen die je nu [committen| commit](committen| commit.md), komen in de nieuwe
branch terecht.
Interessant om weten: het maken van een nieuwe branch en het uitchecken
kan ook gecombineerd worden in één opdracht. Hiervoor geef je het
volgende in:

```
git checkout -b mijnfeaturebranch
```

Stel dat je nu een tijd in mijnfeaturebranch gewerkt hebt, misschien al
wel een paar commits gedaan, en er is plots een dringend probleem dat je
eerst moet aanpakken. Dan kun je erg gemakkelijk terug naar de dev
branch. Van daaruit maak je dan een nieuwe branch voor de dringende fix:

```
git checkout dev
git checkout -b dringendefixbranch
```

(Eerst checken we de dev-branch opnieuw uit, zodat de nieuwe branch op
de dev-branch gebaseerd wordt. De constructie `checkout -b` combineert
het maken en uitchecken van de nieuwe branch.)

Je hebt nu terug de toestand zoals die was in de dev branch, en je kunt
de dringende fix beginnen implementeren. Je kan heen en weer switchen
tussen de verschillende branches met `git checkout`. (Let op:
ongecommitte wijzigingen worden meegenomen, maar niet gecommit, naar de
branch waar je naartoe switcht)

Stel dat je klaar bent met de wijzigingen in 'dringendefixbranch', dan
kun je de changes uit die branch makkelijk terug mergen naar de dev
branch:

```
git checkout dev
git merge dringendefixbranch
```

Als dat allemaal goed is gelukt, kun je de 'dringendefixbranch' met een
gerust geweten opnieuw verwijderen:

```
git branch -d dringendefixbranch
```

(TODO: [ConflictResolution](ConflictResolution.md) documenteren.) Hierna kun je verder
werken in 'mijnfeaturebranch'. Optioneel kun je die branch 'opschuiven'
naar de kop van 'dev', zodat de fixes die je in dev gemerged hebt,
meteen ook in je feature branch opgenomen zijn:

```
git checkout mijnfeaturebranch
git rebase dev
```

Remote branches
---------------

Alwat hierboven beschreven staat, heeft betrekking op je eigen
repository, dat zich op je eigen harde schijf bevindt. Maar git is ook
in staat te synchroniseren met remote repositories. Gelukkig maar.

Als je

```
git branch -r
```

intikt, dan krijg je alle remote branches te zien.

Origin/dev is de branch voor ontwikkeling, daar zit de meest recente
gereviewde code.

Stel dat je de recentste wijzigingen van origin/dev wil binnenhalen voor
je eigen dev-branch, dan doe je dat als volgt:

```
git checkout dev
git pull origin dev
```

(Dit werkt enkel als je lokale wijzigingen [committen| gecommit](committen| gecommit.md)
of [stashen| gestasht](stashen| gestasht.md) zijn.)

Als je tevreden bent over de wijzigingen in een van je branches, dan kun je
ze pushen. Maar meestal gaat dat niet rechtstreeks naar de originele
GAP-repository. In dat geval maak je een fork van de GAP-repository op
gitlab of github, en push je je branch naar daar. Vanuit de user interface
van gitlab of github kun je dan een merge request/pull request maken.
