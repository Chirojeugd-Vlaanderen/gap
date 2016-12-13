De recentste wijzigingen downloaden en toepassen
================================================

In principe kun je 'git pull' gebruiken om de remote wijzigingen toe te
passen op je eigen branch.\
Maar ik ben zelf eerder voorstander van rebasen, dan kun je conflicten
commit per commit oplossen.

De manier van werken voor rebase, is de volgende:

&lt;pre&gt;

1.  haal wijzigingen in remote branches over\
    git fetch origin

<!-- -->

1.  kijk na of je lokale wijzigingen zijn\
    git status

<!-- -->

1.  als het wijzigingen zijn die je wil bewaren, commit ze eerst\
    git commit

<!-- -->

1.  wil je ze niet bewaren, reset dan\
    git reset --hard

<!-- -->

1.  als er na het resetten nog steeds gewijzigde files zijn, dan zijn
    dat
2.  waarschijnlijk line ending issues. In dat geval commit je ze maar:\
    git commit ~~am "Refs \#1778~~ Line endings."

<!-- -->

1.  rebase op origin/dev. Als je dan lokale commits hebt die niet
2.  gepusht waren, dan worden die verschoven naar de nieuwe HEAD van
    dev.\
    git rebase origin/dev

<!-- -->

1.  Als je conflicten hebt, kun je ze als volgt resolven:\
    git mergetool

<!-- -->

1.  Als de rebase nog niet was afgewerkt, ga je als volgt verder\
    git rebase --continue\
    &lt;/pre&gt;

Als mergetool niets wil oplossen, en 'git rebase -~~continue' toch
blijft zeuren over conflicten, schakel een hulplijn in :~~)
