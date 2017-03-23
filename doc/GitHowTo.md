Versiebeheer met git
====================

We gebruiken git voor het versiebeheer.

### Commits

Vermeld in elke commit message het issue waarop je commit betrekking
heeft, zoals in dit voorbeeld:

Refs \#1786 - unit test lid zonder adres.

'Refs \#1788' wil zeggen 'heeft betrekking op issue \#1788'. Als je
denkt dat je commit een issue sluit, gebruik dan 'Closes'. Je kunt
meerdere issues referencen door ze comma-seperated mee te geven.
Bijvoorbeeld:

```
Closes #530, #1544 - meer bruikbare feedback als leden maken mis loopt

Je krijgt nu een lijstje van de leden die niet ingeschreven zijn, met
een specifieke foutboodschap, en links naar de leden in kwestie.
```

Dit voorbeeld illustreert nog een git best practice: In de eerste lijn
geef je een korte samenvatting van wat je commit wijzigt. Daarna volgt
een lege lijn, en daarna wat meer details.

Je commit beter een keer te veel, dan een keer te weinig. Beschouw commits
als een soort van 'save game' in van de 1st-person-shooters. Hoe meer je
commit, hoe makkelijker je terug kunt. Kleine commits kun je achteraf
veranderen van volgorde, en waar nodig samen voegen. Een grote commit
opnieuw splitsen, is een pak lastiger.

### Merge requests

Als je al developerrechten hebt op de GAP-repository, dan kun je in de
officiële repository feature branches maken. Ik raad aan om dat vanuit de
issue tracker zelf te doen. Bij een issue waar nog niemand aan gewerkt heeft,
staat een knop 'create feature branch'. Als je daarop klikt, dan wordt er
een branch gemaakt die direct aan het issue gekoppeld wordt. Wanneer je dan
zelf `git fetch` uitvoert, dan komt die branch over, en kun je hem
'checkouten'.

Nieuwe commits push je naar je feature branch. Ben je tevreden over je fix, dan 
kun je (weer vanuit gitlab) een merge request maken voor die feature branch. 
In dat geval kan iemand van de Chirodevelopers je wijzigingen mergen in de 
dev-branch.

Als je nog geen developerrechten hebt, dan kun je volgens mij een persoonlijke 
fork maken van de gap-repository. Dan maak je daar zelf een branch, en ik
vermoed dat je dan ook merge requests kunt maken, gelijkaardig aan hoe je
pull requests maakt op github. Dat zal wat experimenteren zijn, contacteer
[de helpdesk](https://chiro.be/eloket/feedback-gap) als je er niet uitgeraakt.

### Dev-rechten aanvragen

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
-   [Hoe kuis je je branch op](BranchOpkuisen.md) als het een rommeltje is geworden?
-   http://stackoverflow.com/questions/13541615/how-to-remove-files-that-are-listed-in-the-gitignore-but-still-on-the-repositor -
    foutjes rechtzetten als je te veel bestanden (lees: binaries) in git
    gestopt hebt

Aanvulling: voor de echte beginners is er een online cursusje
(https://www.codecademy.com/learn/learn-git) dat stap voor stap toont
hoe je met git werkt.
