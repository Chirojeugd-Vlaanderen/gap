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

&lt;pre&gt;\
git branch\
&lt;/pre&gt;

De output ziet er dan ongeveer als volgt uit

&lt;pre&gt;\
eenbranch\
nogeenbranch

-   dev\
    &lt;/pre&gt;

Het sterretje voor 'dev' geeft aan dat de dev-branch op dit moment is
uitgepakt.

Een nieuwe feature branch maak je door het volgende in te tikken:

&lt;pre&gt;\
git branch mijnfeaturebranch\
&lt;/pre&gt;

Dit commando maakt een branch ('mijnfeaturebranch') vertrekkende van de
momenteel uitgepakte branch (in dit geval 'dev'). Om in die branch te
beginnen werken, moet je hem nog 'uitchecken':

&lt;pre&gt;\
git checkout mijnfeaturebranch\
&lt;/pre&gt;

Alle bewerkingen die je nu [committen| commit](committen| commit.md), komen in de nieuwe
branch terecht.\
Interessant om weten: het maken van een nieuwe branch en het uitchecken
kan ook gecombineerd worden in één opdracht. Hiervoor geef je het
volgende in:

&lt;pre&gt;\
git checkout -b mijnfeaturebranch\
&lt;/pre&gt;

Stel dat je nu een tijd in mijnfeaturebranch gewerkt hebt, misschien al
wel een paar commits gedaan, en er is plots een dringend probleem dat je
eerst moet aanpakken. Dan kun je erg gemakkelijk terug naar de dev
branch. Van daaruit maak je dan een nieuwe branch voor de dringende fix:

&lt;pre&gt;\
git checkout dev\
git checkout -b dringendefixbranch\
&lt;/pre&gt;

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

&lt;pre&gt;\
git checkout dev\
git merge dringendefixbranch\
&lt;/pre&gt;

Als dat allemaal goed is gelukt, kun je de 'dringendefixbranch' met een
gerust geweten opnieuw verwijderen:

&lt;pre&gt;\
git branch -d dringendefixbranch\
&lt;/pre&gt;

(TODO: [ConflictResolution](ConflictResolution.md) documenteren.) Hierna kun je verder
werken in 'mijnfeaturebranch'. Optioneel kun je die branch 'opschuiven'
naar de kop van 'dev', zodat de fixes die je in dev gemerged hebt,
meteen ook in je feature branch opgenomen zijn:

&lt;pre&gt;\
git checkout mijnfeaturebranch\
git rebase dev\
&lt;/pre&gt;

Remote branches
---------------

Alwat hierboven beschreven staat, heeft betrekking op je eigen
repository, dat zich op je eigen harde schijf bevindt. Maar git is ook
in staat te synchroniseren met remote repositories. Gelukkig maar.

Als je

&lt;pre&gt;\
git branch -r\
&lt;/pre&gt;

intikt, dan krijg je alle remote branches te zien.

Origin/dev is de branch voor ontwikkeling, daar zit de meest recente
gereviewde code. Origin/gap-1.4 is de branch voor de release van deze
zomer. Hier zullen enkel fixes in terechtkomen van de overeenkomstige
milestone (version\#17).

Bugfixes die nog niet gemerged zijn in dev, zitten in feature branches.
Als alles goed is, begint de naam van die branches met
/personal/&lt;username&gt;/,

Stel dat je de recentste wijzigingen van origin/dev wil binnenhalen voor
je eigen dev-branch, dan doe je dat als volgt:

&lt;pre&gt;\
git checkout dev\
git pull origin dev\
&lt;/pre&gt;

(Dit werkt enkel als je lokale wijzigingen [committen| gecommit](committen| gecommit.md)
of [stashen| gestasht](stashen| gestasht.md) zijn.)\
Je kunt ook wijzigingen uploaden naar de remote repository. Op die
manier kun je met meerdere personen aan dezelfde feature werken.
Bovendien heb je dan minder problemen als je harde schijf crasht :-)

Je mag enkel pushen naar persoonlijke branches! Ik ga er even vanuit dat
je je feature hebt gemaakt in een lokale branch
'personal/username/mijnfeaturebranch':

&lt;pre&gt;\
git checkout personal/username/mijnfeaturebranch \# pakt lokaal
'mijnfeaturebranch' uit\
git push origin personal/username/mijnfeaturebranch \# uploadt naar
remote 'mijnfeaturebranch'\
&lt;/pre&gt;

(Die eerste lijn is niet nodig als 'mijnfeaturebranch' al 'uitgecheckt'
is.)

Als je tevreden bent over de wijzigingen in je branch, zorg dan dat alle
wijzigingen gepusht zijn naar de remote repository, en vraag aan Johan
om je feature te mergen naar de remote dev branch. Op dat moment zal je
code ook gereviewd worden.
