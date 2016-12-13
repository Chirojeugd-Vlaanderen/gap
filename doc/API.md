API
===

REST-API
--------

Zie \#3283.

### Moeilijkheden met authenticatie

Als de frontend de backend aanspreekt, dan authenticeert de user zich
via basic auth tegenover AD. 'Impersonation' zorgt ervoor dat de
frontend de backend aanspreekt alsof de frontend die aangelogde
gebruiker is.

Voor de API zou het leuk zijn moest er met tokens gewerkt kunnen worden
(zoals bijv. OAuth). Probleem is, dat als de api aangeroepen wordt met
een token, er geen voor de hand liggende manier bestaat om de
overeenkomstige user te 'impersonaten'.

Misschien kan dat wel, en moeten we nog uitvissen hoe.

Misschien kan het niet. In dat geval kan Chiro.Gap.Services opgesplitst
worden in Chiro.Gap.Services1 en Chiro.Gap.Services2 (maar dan met
betere namen).\
Chiro.Gap.Services1 zoekt dan voor elke call uit 'wie ben ik'?, en geeft
die informatie als dusdanig door aan dezelfde call in
Chiro.Gap.Services2, maar dan met 1 extra parameter (de user).

De API zoekt dan ook uit 'wie ben ik,' maar aan de hand van een token,
en kan dan ook Chiro.Gap.Services2 gebruiken.

OData-API
---------

De OData-API zal terug verdwijnen. Zie [OData-API](OData-API.md).
