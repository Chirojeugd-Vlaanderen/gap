Versiebeheer met git
====================

We gebruiken git voor het versiebeheer.

Voor GAP
--------

Wat specifieke informatie voor GAP. Heb je hulp nodig, stel je vraag dan
in het
[developersforum](http://websites.chiro.be/projects/gap/boards/2). Als
je kunt aanmelden, tenminste. Voor aanmeldproblemen richt je je tot de
[Chiro-helpdesk](https://chiro.be/eloket/feedback-gap).

### Commits

Vermeld in elke commit message het issue waarop je commit betrekking
heeft, zoals in dit voorbeeld:

Refs \#1786 - unit test lid zonder adres.

'Refs \#1788' wil zeggen 'heeft betrekking op issue \#1788'. Als je
denkt dat je commit een issue sluit, gebruik dan 'Closes'. Je kunt
meerdere issues referencen door ze comma-seperated mee te geven.
Bijvoorbeeld:

&lt;pre&gt;\
Closes \#530, \#1544 - meer bruikbare feedback als leden maken mis loopt

Je krijgt nu een lijstje van de leden die niet ingeschreven zijn, met
een\
specifieke foutboodschap, en links naar de leden in kwestie.\
&lt;/pre&gt;

Dit voorbeeld illustreert nog een git best practice: In de eerste lijn
geef je een korte samenvatting van wat je commit wijzigt. Daarna volgt
een lege lijn, en daarna wat meer details.

### Patches

Als je een issue fixt, en je hebt geen push-rechten op de centrale
repository, dan kun je een patch aan het issue attachen, en de issue
status veranderen naar 'Needs Review'. Maak je patch bij voorkeur op de
recentste versie van de dev branch.

### Pushen

Als je push-rechten hebt, push je wijzigingen dan in een **persoonlijke
branch**. Alvorens je branch voor het eerst te pushen, rebase je hem op
origin/dev. Eens je branch bestaat op de remote server, rebase je hem
liever niet meer.

&lt;pre&gt;\
git fetch origin\
git rebase origin/dev\
&lt;/pre&gt;

Als je op die manier conflicten tegenkomt:

git mergetool

Loopt het mergen helemaal mis, dan kun je je rebase-operatie aborten.

git rebase --abort

[Vraag gerust om hulp](http://websites.chiro.be/projects/gap/boards/2)
als je vast zit.

Pushen doe je naar een persoonlijke branch. Bijvoorbeeld

git push origin personal/vervljo/1786-verplichte\_velden

**Push niet naar dev**, of naar staging, of master. Dat is niet de
bedoeling. We kunnen die beperking op dit moment niet meer technisch
opleggen. [Zo lang dit probleem niet opgelost
is](https://github.com/jbox-web/redmine_git_hosting/issues/86),
verwachten we dat je verantwoordelijk omspringt met je push-rechten.

### Push-rechten aanvragen

Wil je graag rechtstreeks kunnen pushen naar de centrale repository?
Contacteer [de Chiro-helpdesk](https://chiro.be/eloket/feedback-gap).

Algemene git-info (wat verouderd)
---------------------------------

Deze informatie moet wellicht wat bijgewerkt worden. Ik stelde ze op
toen ik zelf nog een git-beginner was.

-   https://github.com/johanv/randomtexts/blob/master/gitstart.md\#git-is-gedistribueerd -
    de concepten en de werking van git, in grote lijnen
-   http://youtu.be/n-WbItytu0U - video over het gebruik van git
    extensions
-   [branches](branches.md) - Hoe werk je met branches?
-   [pull](pull.md) - Hoe haal je de recentste code op?
-   http://stackoverflow.com/questions/13541615/how-to-remove-files-that-are-listed-in-the-gitignore-but-still-on-the-repositor -
    foutjes rechtzetten als je te veel bestanden (lees: binaries) in git
    gestopt hebt

Aanvulling: voor de echte beginners is er een online cursusje
(https://www.codecademy.com/learn/learn-git) dat stap voor stap toont
hoe je met git werkt.
