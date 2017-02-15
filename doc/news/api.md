# Een HTTP-API in test (#3283)

*2017-02-15*

Niet veel na de initiele release van het GAP als webtoepassing, was er de
vraag naar een API (#845). We hebben intussen ook al wel wat pogingen 
achter de rug.

Een eerste poging was OData (dc1df503). OData is een standaard die
via rare querystrings toelaat om gegevens gefilterd op te halen.
Helaas is de syntax tamelijk ingewikkeld, en daarenboven was onze
implementatie niet efficient, omdat hij eerst heel veel informatie 
ophaalde uit de backend, om dan in-memory te filteren.

Een tweede poging was die van @bridtbe. Hij gebruikte ASP.NET Webapi, en
werkte via Entity-Framework rechtstreeks op de GAP-database, zodat het
ophalen van gegevens veel efficienter verliep (8be29567). Nadeel was
dat de autorisatielogica dan opnieuw geschreven moest worden. De
structuur van de tabellen is ook niet ideaal voor een 
API-eindgebruiker, en het was niet voor de hand liggend om de 
authenticatie goed te regelen.

Maar derde keer, goeie keer, laat ons hopen. Ik heb nu een API in dev (#3283),
die werkt met OAuth, die achterliggend onze webservices aanspreekt, en
dan mapt naar eenvoudige json. Filtering zal wat beperkter zijn, maar we
kunnen dat zelf vrij eenvoudig implementeren, en alle filters die de
frontend kan gebruiken in de bestaande backend (dat zijn er wel wat) kunnen
we aanbieden in onze API.

De huidige functionaliteit is erg basic. Maar potentiele ontwikkelaars
van clients kunnen er al mee aan de slag. En het uitbreiden van de
API-functionaliteit is niet zo moeilijk.

## Dev!

De API is gereleaset op de dev-versie van het GAP. Dat wil zeggen dat je
toegang hebt tot een beperkte set van dummy-data, en dat die database
dagelijks herbouwd wordt. In de loop van volgende week zou ik de API op
de staging-omgeving willen hebben, dat lijkt me handiger voor developers.

Opdat je gegevens zou kunnen ophalen via de API, is het belangrijk dat
je toegang hebt tot een groep in dev. Omdat de dev-db dummy-data bevat,
kun je die toegang zelf geven: surf naar https://develop.chiro.be:2000/gap,
en klik een aantal keer op de link 'testgroep toevoegen'.

## Aan de slag

Als je de API wil gebruiken, heb je een user name en een key nodig voor
authenticatie. De API authenticeert voorlopig met eigen user names en
wachtwoorden, de autorisatie gebeurt via het AD-nummer. Op termijn is
het waarschijnlijk beter dat authenticatie voor de API ook via de
loginserver verloopt, maar dat is nog toekomstmuziek.

Chiro-IT'ers kunnen [zelf een key aanmaken](https://adminwiki.chiro.be/gap:api),
wil je als vrijwilliger een key, vraag er dan een aan Johan.

Met je username en je key, kun je een token aanvragen:

```
curl -X POST  -d 'username=MIJN_USERNAME&password=MIJN_KEY&grant_type=password' https://develop.chiro.be:2000/gapi_tst/token
```

(Als je je resultat 'doorpipet' naar `jq .`, dan wordt je json netjes
geformatteerd.) `MIJN_USERNAME` en `MIJN_KEY` vervang je uiteraard door
de juiste username en key.

Het kan zijn dat je lang moet wachten op een resultaat. Als de api of de
backend lang niets gedaan hebben, vallen ze in slaap, en het duurt een tijdje
vooraleer ze er weer zijn. Bovendien draait de dev-omgeving op een server met
beperkte resources, waar wel heel wat toepassingen op draaien.

Je krijgt als resultaat een token, dat 24 uur geldig is:

```
{
  "access_token": "JE_TOKEN",
  "token_type": "bearer",
  "expires_in": 86399
}
```

Met dat token kun je je authenticeren. Je zou dat als volgt kunnen testen:

```
curl -H 'Authorization: Bearer JE_TOKEN' https://develop.chiro.be:2000/gapi_tst/api/account/hello
```

Hier vervang je `JE_TOKEN` door het token dat je kreeg via de eerste call.

## Ondersteunde calls

Voorlopig kun je maar een beperkt aantal calls uitvoeren. Dit zijn ze:

```
curl -H 'Authorization: Bearer JE_TOKEN' https://develop.chiro.be:2000/gapi_tst/api/groepen
```

Dit levert de groepen op waarvoor je toegang hebt, bijvoorbeeld:

```
[
  {
    "GroepId": 892,
    "Naam": "DON BOSCO",
    "Plaats": "Torhout",
    "StamNummer": "WJ /1709"
  },
  {
    "GroepId": 1028,
    "Naam": "GEWEST SCHELDELAND",
    "Plaats": "n.v.t.",
    "StamNummer": "OG /2300"
  }
]
```

Je kunt dan ook alle lidgegevens ophalen uit een bepaalde groep, bijvoorbeeld:

```
curl -H 'Authorization: Bearer JE_TOKEN' https://develop.chiro.be:2000/gapi_tst/api/leden/892
```

Merk op dat het Groep-ID (in dit geval 892) meegegeven moet worden met de call.
Dit is relevant voor personen die toegang hebben tot meerdere groepen.

## Work in progress

Zoals je ziet, is het allemaal nog erg basic. Maar het begin is er. Als je
zin hebt om de API mee verder uit te werken,
[neem dan zeker contact op](http://gapwiki.chiro.be/gap:vrijwilligers).
