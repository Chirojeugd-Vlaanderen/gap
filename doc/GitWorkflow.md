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

```
git clone gitolite@develop.chiro.be:gap
```

![](gapipgit00.png)

### De branch voor dit weekend is ''gapip13'':

**Tenminste** als je werkt aan de [open tickets voor de
zomerrelease](https://develop.chiro.be:3000/projects/gap/issues?fixed_version_id=18&amp;set_filter=1&amp;status_id=o).
**In het andere geval maak je een eigen branch!**

```
git fetch origin
git checkout gapip13
```

![](gapipgit01.png)
![](gapipgit02.png)
![](gapipgit03.png)
![](gapipgit04.png)

### Committen: beter te vaak dan te weinig

```
git status \# toont nieuwe en gewijzigde files
git add nieuwefiles \# staget nieuwe files
git add gewijzigdefiles \# staget gewijzigde files
git commit \# commit gestagede files
```

![](gapipgit05.png)
![](gapipgit06.png)

-   De wijzigingen in gestagede files worden gecommit. (stagen met
    paarse pijlen)
-   Commitbericht:
    -   Eerste lijn: korte omschrijving van wat je doet. **Vermeld een
        ticketnummer.!**
    -   Optioneel: lege lijn, en daarna wat meer uitleg

### Pushen naar de centrale repository

```
git fetch origin
git rebase origin/gapip13 \# past remote wijzigingen toe
git push origin gapip13
```

![](gapipgit01.png)
![](gapipgit07.png)
![](gapipgit08.png)
![](gapipgit09.png)
![](gapipgit10.png)

### Conflicten bij rebasen?

(Git extensions heef hier ook iets voor; nog geen screenshots atm)

```
git mergetool
```

Als conflicten opgelost:

```
git rebase --continue
```

Conflictoplossing liep in het honderd:

```
git rebase --abort

1.  en hulp roepen (git-experts genoeg hier)
    ```

