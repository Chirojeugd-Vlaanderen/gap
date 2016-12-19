GebruikersRechtenOverzicht
==========================

In eerste instantie zagen we twee soorten accounts. *Ik maakte er drie
van.*

De anonieme account
-------------------

**UPDATE:** In principe zal publieke data opgeleverd worden door een api
op www.chiro.be.

*Dit is anders dan besproken. Op de vergadering zeiden we dat toegang
tot publieke data via een app-account zou verlopen. Maar het lijkt me
handiger dat publieke data zonder wachtwoord raadpleegbaar is, en dat de
groepen kiezen welke data ze publiceren.*

Er is publieke data, die je uit de API kunt opvragen zonder credentials.
De groepen kiezen zelf welke data ze beschikbaar stellen. Dat kan zijn
(ik zeg maar iets; we beginnen in eerste instantie best minder
utigebreid)

-   groepsnaam en plaats
-   e-mailadres van de groep (zie ook \#2891)
-   adres van de lokalen
-   voornaam en afdeling van alle leiding
-   volledige naam van alle leiding
-   telefoonnummer van de contactpersoon
-   e-mailadres van de contactpersoon
-   voornamen en afdelingen van de leden

Standaard wordt niets als publiek aangeduid.

*Bijkomende opmerkingen:*

-   *Moet publieke informatie niet eerder uit Kipadmin/Chirocivi komen
    ipv uit GAP?*
-   *Als publieke informatie uit Kipadmin/Chirocivi moet komen, dan
    moeten Kipadmin/Chirocivi ook weten wat de groep wil publiceren en
    wat niet.*

App-accounts
------------

Accounts worden gebruikt door een app om aan te melden. Als GAV (met
alle rechten) kun je zo'n account aanmaken. Het GAP onthoudt wie welke
account heeft aangemaakt. Je kunt ook als GAV app-accounts terug
intrekken. App-accounts hebben bepaalde permissies.

Een app-account zou read-only-access kunnen hebben tot (een subset van)
gegevens van de groep.
Dat kan dan varieren van de telefoonnummers/e-mailadressen van de
leiding tot echt alles.
Of zelfs beperkte schrijfrechten. Maar de ontwikkelaar die de account
aanvraagt moet hoe dan ook op zijn blote knieen beloven dat hij geen
credentials zal exposen (bijvoorbeeld in javascript code op een
website), en hij moet de leden van de GAP-ploeg een pint betalen ;-)

*App-accounts moeten we zo veel mogelijk vermijden. Er is publieke data,
en als je toegang wilt tot niet-publieke data, dan gebruik je beter een
persoonlijke account, zie verder*

*Misschien is het zelfs beter dat je de helpdesk moet bellen om een
app-account te maken, ipv dat mogelijk te maken via het GAP. Dan heb je
meteen de uitleg erbij. Bovendien lijken app-accounts me niet iets om te
stimuleren*

Apps die persoonlijke accounts gebruiken
----------------------------------------

Deze methode krijgt de voorkeur als je niet-publieke data nodig hebt. De
app zorgt ervoor dat je rechtstreeks bij de API kunt aanloggen. De
backend geeft alleen toegang tot de informatie waar de gebruiker toegang
toe heeft.

Dit vereist wel dat we een beter permissie-systeem nodig hebben op
gebruikers dan het huidige alles of niets (\#844). Mogelijke rechten:

-   alles (GAV)
-   leesrechten op alles
-   leesrechten op mensen van eigen afdeling
-   lees- en schrijfrechten op mensen van eigen afdeling. (We moeten dan
    wel wat opletten bij het toevoegen van een nieuwe persoon aan
    je afdeling. Want die is op voorhand uiteraard nog geen lid van
    je afdeling.)
-   schrijfrechten op gegevens van de leidingsploeg
-   lees- en schrijfrechten op gegevens van jezelf

We zullen wel eens moeten uitzoeken hoe we dat juist kan werken: een
rest-service met authenticatie, en een Angular client. Dat is nog niet
duidelijk.

Aandachtspunt
-------------

We moeten vermijden dat mensen hun persoonlijke account als app-account
gaan gebruiken, want dit is een security issue. Een persoonlijke account
heeft per definitie meer rechten dan een app-account. Bovendien is het
erg makkelijk om credentials te lekken bij onzorgvuldig gebruik van een
app-account.
