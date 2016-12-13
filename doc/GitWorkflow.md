Git workflow (GAPIP-versie)
===========================

Is git nieuw voor jou? Installeer [Git
Extensions](http://code.google.com/p/gitextensions/). Moest je nog
tortoise git hebben staan, en je bent daar niet bijzonder aan gehecht,
verwijder het dan maar. Met git extensions kunnen we je helpen; we weten
hoe dat werkt. (Demo'tje op http://youtu.be/n-WbItytu0U)

Ik geef op deze pagina eerst de commando's, en vervolgens de screenshots
voor Git Extensions (zij het momenteel nog wat rommelig)

### Moest je de code nog niet hebben

&lt;pre&gt;\
git clone gitolite@develop.chiro.be:gap\
&lt;/pre&gt;

![](gapipgit00.png)

### De branch voor dit weekend is ''gapip13'':

**Tenminste** als je werkt aan de [open tickets voor de
zomerrelease](https://develop.chiro.be:3000/projects/gap/issues?fixed_version_id=18&amp;set_filter=1&amp;status_id=o).
**In het andere geval maak je een eigen branch!**

&lt;pre&gt;\
git fetch origin\
git checkout gapip13\
&lt;/pre&gt;

![](gapipgit01.png)\
![](gapipgit02.png)\
![](gapipgit03.png)\
![](gapipgit04.png)

### Committen: beter te vaak dan te weinig

&lt;pre&gt;\
git status \# toont nieuwe en gewijzigde files\
git add nieuwefiles \# staget nieuwe files\
git add gewijzigdefiles \# staget gewijzigde files\
git commit \# commit gestagede files\
&lt;/pre&gt;

![](gapipgit05.png)\
![](gapipgit06.png)

-   De wijzigingen in gestagede files worden gecommit. (stagen met
    paarse pijlen)
-   Commitbericht:
    -   Eerste lijn: korte omschrijving van wat je doet. **Vermeld een
        ticketnummer.!**
    -   Optioneel: lege lijn, en daarna wat meer uitleg

### Pushen naar de centrale repository

&lt;pre&gt;\
git fetch origin\
git rebase origin/gapip13 \# past remote wijzigingen toe\
git push origin gapip13\
&lt;/pre&gt;

![](gapipgit01.png)\
![](gapipgit07.png)\
![](gapipgit08.png)\
![](gapipgit09.png)\
![](gapipgit10.png)

### Conflicten bij rebasen?

(Git extensions heef hier ook iets voor; nog geen screenshots atm)

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

