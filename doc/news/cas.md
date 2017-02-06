# CAS-ondersteuning voor het GAP (#3771)

*2017-02-02*

Bij de Chiro hebben we een [CAS-server](https://login.chiro.be), die we
gebruiken voor single sign on. Je kunt al via CAS aanmelden op de
[chirosite](https://www.chiro.be), onze wiki's, de
[issue trackers](https://gitlab.chiro.be), de Chirocivi, en hier en daar
nog een obscure service. En vanaf nu kan dat ook voor het GAP.

't Is te zeggen: weldra kan dat ook voor het GAP. Nu moet er getest worden.

De wijzigingen aan het GAP zijn nogal ingrijpend. Niet dat de CAS-authenticatie
zoveel refactoring vroeg (#5621), maar wel omdat ik meteen de branch
met de herwerkte gebruikersrechten (#844) gemerged heb.

Normaal ga je daar niet veel van zien. De rechten in het GAP die gebruikt
worden, zijn nog altijd 'alles op een groep' of niets. Maar de database
en de entity's voor fijnere gebruikersrechten zijn er al wel, en dus
kan die functionaliteit stapsgewijze ingevoerd worden. Het mogelijk maken
om je eigen gegevens te wijzigen, kan bijvoorbeeld een eerste stap zijn.

## dev en staging

We hebben twee omgevingen om het GAP uit te proberen: dev en staging.

Dev zit op https://develop.chiro.be:2000/gap. Van zodra je aangemeld bent,
kun je toegang krijgen tot gegevens van een groep. De gegevens komen wel
uit een beperkte database, en ze zijn gerandomized. Dat wil zeggen dat het
GAP je niet gaat herkennen (bij aangemeld als zal er waarschijnlijk enkel je
AD-nummer staan), en ook de stratenlijst is niet volledig, wat het moeilijk
maakt om adressen in te geven.

De staging-site https://develop.chiro.be:2000/staging bevat wel een kopie
van de echte data, en synchroniseert met de staging-site van Chirocivi.
Maar omdat het om echte data gaat, heb je op de GAP-staging enkel toegang
tot de groepen waarvoor je in het echt ook toegang hebt. Als je in staging
wilt testen, dan kunnen we dat wel regelen, maar enkel als er een Chirogroep is
die jou toestemming geeft om hun gegevens te bekijken.
