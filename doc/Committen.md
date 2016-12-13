Committen
=========

Let op! Als je een changeset commit, dan blijft die commit waar die is,
namelijk op jouw eigen machine. Hoe je je wijzigingen in de remote
repository krijgt, lees je in [branches](branches.md).

GUI
---

Als je de git source control provider gebruikt, kun je je wijzigingen
committen door rechts te klikken op de naam van je solution, en dan
'pending changes' te kiezen.

Je krijgt een overzichtje van toegevoegde en gewijzigde bestanden, de
bestanden die je mee wilt nemen met je commit, vink je aan.

In het vakje 'comments' geef je wat uitleg bij je commit. Vermeld 'refs
\#ticketnr' als je commit betrekking heeft op het ticket met dat gegeven
nummer, of 'closes \#ticketnr' als je changeset dat ticket sluit.

Tenslotte klik je op de knop 'commit'.

Command Line
------------

Als je in je source code wijzigingen doorvoert, moet je zowel de nieuwe
bestanden als de gewijzigde bestanden 'markeren' als 'to be committed'.
Let op, dat is dus anders dan bij subversion.

Als je op de command line het volgende ingeeft:\
&lt;pre&gt;\
git status\
&lt;/pre&gt;\
dan krijg je een overzichtje te zien van de gewijzigde en nieuwe
bestanden die nog niet gemarkeerd zijn voor de volgende commit.
Bestanden die meegenomen moeten worden kun je als volgt toevoegen:\
&lt;pre&gt;\
git add path/naar/bestand\
&lt;/pre&gt;\
Als er bestandsnamen of -types zijn die uitgesloten moeten worden van de
versiecontrole, kun je die toevoegen aan een relevant
`.gitignore`-bestand.\
Van zodra je alle nodige bestanden 'geadd' hebt, kun je je changeset
committen:\
&lt;pre&gt;\
git commit\
&lt;/pre&gt;\
Vermeld in de omschrijving 'refs \#ticketnr' als je commit betrekking
heeft op het ticket met dat gegeven nummer, of 'closes \#ticketnr' als
je changeset dat ticket sluit.

Het is mogelijk om in één commando alle gewijzigde bestanden te 'adden'
en te committen, en wel als volgt:\
&lt;pre&gt;\
git commit -a\
&lt;/pre&gt;\
Let er wel op dat dit alleen opgaat voor gewijzigde bestanden; de nieuwe
zul je sowieso manueel moeten 'adden'.
