Communicatie tussen GAP en Chirocivi
====================================

Algemeen
--------

Als GAP een wijziging naar CiviCRM wil sturen, dan gebeurt dat door
een object in Chiro.Gap.Sync. Deze objecten plaatsen berichten in een
[message queue](MessageQueueing.md), die dan door
CiviSync (een Windows-service) worden opgepikt.

Persoonsgegevens
----------------

### Persoon herkennen

Als GAP het AD-nummer van een persoon kent, dan wordt dat AD-nummer
gebruikt om aan CiviSync te laten weten welke persoon bewerkt moet
worden. Is het AD-nummer niet gekend, dan stuurt GAP alle informatie mee
naar CiviSync, die dan op deze manier probeert te bepalen of de persoon
al bestaat:

1.  AD-NR
2.  GAP-ID
3.  Zoek personen met zelfde naam, voornaam en geslacht
    * Iemand met zelfde telefoonnr/e-mail -&gt; OK
    * Iemand met zelfde postnr -&gt; OK
    * Iemand met zelfde geboortedatum -&gt; OK
4.  Niemand gevonden? -&gt; Nieuwe persoon

### Persoon bewaren

#### Use cases

Deze synchronisatie treedt enkel op als de gebruiker een al gekende
persoon aanpast door de gegevens te wijzigen op zijn persoonsfiche.

Er zijn 3 mogelijkheden:

-   De persoon heeft geen AD-nummer, en zijn AD-nummer is niet
    in aanvraag. Dan gebeurt er niets.
-   De persoon heeft een AD-nummer. CiviSync vindt hem op basis van
    dat AD-nummer.
-   De persoon heeft een AD-nummer in aanvraag. Bij CiviSync vindt hem
    normaalgezien op basis van GAP-ID. Als dat GAP-ID niet gekend is, is
    er vroeger wellicht iets misgelopen met de synchronisatie, en wordt
    hij op de bovengeschreven manier gezocht (naam, voornaam,
    geboortedatum), en in het slechtste geval aangemaakt.

Als we een nieuwe persoon toevoegen in GAP, moet die pas naar CiviCRM
als hij daadwerkelijk ingeschreven wordt als lid. Maar dat gaat via 'Lid
Bewaren', en niet via 'Persoon Bewaren'.

#### Technisch

Chiro.Gap.Sync.PersonenSync.Bewaren stuurt de gegevens van een persoon
naar CiviSync. De persoonsgebonden gegevens (naam, geslacht,
geboortedatum, datum overlijden) worden altijd meegestuurd. Optioneel
zijn:

-   standaardadres
-   alle communicatie (e-mailadressen, telefoonnr's, enz.)

Die optionele zaken worden in praktijk nergens meegestuurd, en dat is
niet onlogisch, omdat de gebruiker die zaken 1 voor 1 wijzigt, en elke
wijziging apart naar CiviCRM gaat. Maar de mogelijkheid is wel nuttig
voor wanneer er eens een heel aantal personen via een script opnieuw
gesynct moeten worden.

### Voorkeursadres bewaren

#### Use cases

-   Een aantal (&gt;0) personen verhuizen van een gezamenlijk adres A
    naar een nieuw (gezamenlijk) adres B. De in CiviCRM gekende
    personen voor wie adres A het voorkeursadres was, krijgen nu adres B
    als voorkeursadres.
-   Een persoon (en eventueel enkele van zijn adresgenoten) krijgt een
    nieuw adres, dat als voorkeursadres moet dienen. Zij die door
    CiviCRM gekend zijn, moeten ook in CiviCRM het nieuwe
    adres krijgen.
-   Een adres wordt verwijderd van een persoon (en eventueel ook van een
    aantal adresgenoten). De personen waarvan het adres het
    voorkeursadres was, krijgen een ander voorkeursadres. Voor diegenen
    die gekend zijn door CiviCRM, wordt dat nieuwe adres gesynct.
-   Als het laatste adres van een persoon wordt verwijderd, dan moet het
    voorkeursadres in CiviCRM ook verwijderd worden. Dat is nu nog niet
    het geval. (En dat valt ook niet onder voorkeursadres
    bewaren, eigenlijk)
-   Een persoon, gekend door CiviCRM, en met meerdere adressen, krijgt
    een ander van zijn bestaande adressen als voorkeursadres. Dat nieuwe
    adres gaat naar CiviCRM.

'Gekend door CiviCRM', wil zeggen:

-   heeft een AD-nummer
-   of: AD-nummer is in aanvraag

#### Technisch

Chiro.Gap.Sync.AdressenSync mapt een Belgisch adres of buitenlands adres
naar Kip.ServiceContracts.DataContracts.Adres, dat alle adresgegevens
als string bevat. (Dus: volledige straatnaam, landnaam,... en geen
Crab-ID's of dergelijke.)

Het adres wordt samen met alle persoonsgegevens (ongeacht of het
AD-nummer gekend is of niet; hier is nog ruimte voor optimalisatie) naar
CiviSync gestuurd (StandaardAdresBewaren).

### Communicatie

#### Use cases

##### Nieuwe communicatie

-   Geef een nieuwe communicatievorm aan een door CiviCRM gekende
    persoon via de personenfiche.
-   Maak een nieuwe persoon, die op hetzelfde adres woont als een
    persoon die gekend is door CiviCRM, en geef die nieuwe persoon bij
    het maken een gezinsgebonden communicatievorm. (Dit werkt nog niet,
    zie #1070)
-   Geef een nieuwe gezinsgebonden communicatievorm aan een persoon
    onbekend door CiviCRM, die wel op hetzelfde adres woont als een
    persoon wel gekend door CiviCRM. (Werkt ook niet, zie #1070)

Als we een onbekend persoon inschrijven als lid of leiding, dan wordt
zijn communicatie ook gesynct. Maar dat verloopt via NieuwLidBewaren.

##### Communicatie wijzigen

-   De gebruiker wijzigt een communicatievorm van een persoon vanop
    de personenfiche.
-   De gebruiker togglet snelleberichtenlijst bij een communicatievorm
    van een gekende persoon.

##### Communcatie verwijderen

-   De gebruiker klikt op het 'verwijderen'-knopje van een
    communicatievorm van een gekende persoon op de personenfiche.

#### Technisch

##### Nieuwe Communicatie

CommunicatieToevoegen van CiviSync wordt aangeroepen door
Chiro.Gap.Sync.CommunicatieSync.Toevoegen. Er wordt steeds maar één
communicatievorm tegelijk toegevoegd. In GAP hangt die t.g.v. een
designfout aan slechts 1 persoon, waardoor CommunicatieSync.Toevoegen
voldoende informatie heeft aan de communicatievorm alleen.

CiviSync behoudt de voorkeur van de communicatievormen niet. Dat is een
bug (\#1406). Als de communicatievorm al gekend is, dan blijft die zijn
voorkeur houden. Een nieuwe communicatievorm wordt achteraan de lijst
toegevoegd.

##### Communicatie wijzigen

CommunicatieBijwerken van CiviSync wordt aangeroepen door
CommunicatieSync.Bijwerken.

In het overgrote deel van de gevallen zijn alle communicatievormen van
een gekende persoon ook gekend in CiviCRM. Dan is het aanpassen geen
probleem.

Het kan zijn dat een groep een communicatievorm van een persoon heeft
verwijderd. Als een andere groep die communicatievorm nog wel heeft, en
aanpast, dan is de te wijzigen communicatievorm niet meer te vinden in
CiviCRM. In dat geval wordt er gewoon een nieuwe aangemaakt.

Om in CiviCRM de bestaande communicatievorm te vinden, worden de
strings letterlijk vergeleken. Op zich niet erg dramatisch, omdat het
invoerformaat min of meer vast ligt. Maar het zou nog beter kunnen.
(Bijv. enkel de numerieke karakters bekijken, en +32 gelijk stellen aan
een voorloop-0).

##### Communicatie verwijderen

De te verwijderen communicatievorm wordt opgezocht op basis van de
persoon, het communicatietype en het nummer. Als er niets gevonden
wordt, wordt er ook niets verwijderd.

Net zoals bij het wijzigen, is er een probleem als de communicatievorm
in een ander formaat in CiviCRM zou zitten als in GAP.

Lidgegevens
-----------

### Leden bewaren

#### Use cases

-   Er wordt een nieuwe persoon toegevoegd, en deze persoon wordt vanuit
    het formulier van 'nieuwe persoon' meteen mee ingeschreven.
-   Vanop de persoonsfiche van een bestaande persoon, wordt op
    'inschrijven' geklikt.
-   In de personenlijst wordt op de link 'inschrijven' geklikt achter de
    gegevens van een bepaalde persoon.
-   In de personenlijst worden een aantal personen aangevinkt, en dan
    wordt er 'inschrijven' geselecteerd uit de lijst met acties.
-   In een ledenlijst van een vorig werkjaar worden personen aangevinkt,
    en opnieuw wordt er 'inschrijven' geselecteerd uit de lijst
    met acties.
-   Bij de jaarovergang worden meteen een aantal leden overgezet.

Opgelet:

-   De nieuwe leden hebben soms al wel een AD-nummer (makkelijk geval),
    soms ook niet (moeilijker geval)
-   Het zou kunnen dat een inactief lid terug ingeschreven wordt.
    Alnaargelang dat lid in CiviCRM zit (hangt van moment van
    uitschrijven af), moet het lid in CiviCRM opnieuw worden
    aangemaakt, of anders moet lidtype en afdeling mogelijk
    worden bijgewerkt. **LET OP:** als je dit manueel nakijkt, let er
    dan op dat je in CiviCRM en in GAP in hetzelfde werkjaar bezig
    bent!

#### Technisch

Alles wordt geregeld door LedenSync.Bewaren, dat een lid meekrijgt als
parameter, en dan LidBewaren of NieuwLidBewaren aanroept van CiviSync
(alnaargelang een AD-nummer is gekend of niet)

NieuwLidBewaren wordt normaalgezien aangeroepen inclusief adressen en
communicatie.

CiviSync heeft wel wat werk om alles in de structuur van CiviCRM te
krijgen:

-   Als er geen AD-nummer gegeven is, wordt de persoon voor zover het
    kan opgezocht of aangemaakt. (Hier is nog wel ruimte
    voor verbetering)
-   Als de persoon al lid is in de gegeven groep, dan blijft dat
    lidobject bestaan, maar worden afdelingen, functies en soort
    juist gezet.
-   Iemand die lid is van een ploeg, moet een membership (voorlopig uniek per
    persoon en werkjaar) krijgen. Als dat nog niet bestaat moet dat aangemaakt
    worden.

### Leden verwijderen

#### Use cases

-   In de persoonsfiche van een lid wordt er op de link
    'uitschrijven' geklikt.
-   In de ledenlijst van dit werkjaar worden een aantal personen
    geselecteerd, en vervolgens wordt de actie 'uitschrijven' gekozen.

#### Technisch

Leden worden enkel verwijderd uit CiviCRM als hun probeerperiode nog
niet voorbij is. Die controle gebeurt door het GAP zelf. Enkel als de
probeerperiode voorbij is, wordt LedenSync.Verwijderen aangeroepen, die
dan op zijn beurt (Nieuw)LidVerwijderen aanroept van CiviSync.

### Functies updaten

#### Use cases

-   Op de fiche van een lid klik je op het wijzigen-icoontje om de
    functies aan te passen. Die pas je dan aan in het popupvenstertje.

#### Technisch

LedenSync.Functiesupdaten geeft alle functies van een lid door aan
FunctiesUpdaten van CiviSync. Er wordt geen onderscheid gemaakt tussen
leden met en zonder ad-nummer. (Wat niet heel erg is, want slechts een
beperkt aantal leden zal functies krijgen.)

CiviSync verwijdert eerst de te verwijderen functies, en voegt daarn de
toe te voegen functies toe.

Als de functie 'financieel verantwoordelijke' of 'contactpersoon' wordt
toegevoegd of afgenomen, dan zou dat best meteen aangepast worden in het
ploegrecord. Maar dat doen we niet. In CiviCRM loopt er ieder uur een job
die dit soort aanpassingen doet.

### LidType updaten

#### Use case

-   Op een personenfiche staat 'ingeschreven als lid' of 'ingeschreven
    als leiding', met daarachter een wijzigen-icoontje. Als de gebruiker
    op dat icoontje klikt, dan wisselt het om.

#### Technisch

Telkens de user het lidtype wijzigt in de personenfiche, wordt dat door
LedenSync.TypeToggle doorgestuurd naar LidTypeInstellen van CiviSync. Er
wordt geen onderscheid gemaakt tussen leden met en zonder ad-nummer. Ook
hier is dat niet erg, want de use case is zeldzaam.

CiviCRM kent niet echt lidtypes. Als je leiding of kadermedewerker bent in GAP,
dan krijg je in CiviCRM de afdeling 'leiding'. De afdelingen waarover je
leiding bent, zitten in CiviCRM in een ander veld.

In principe kan het wisselen van type enkel gebeuren bij plaatselijke
groepen. Moest CiviSync zo'n verzoek krijgen voor een kaderploeg, dan
blijft de afdeling 'Leiding'.

### Afdelingen updaten

#### Use cases

-   De gebruiker verandert het lidtype van een lid. Wordt een lid
    leider, dan verdwijnt de afdeling. Wordt hij/zij lid, dan krijgt
    hij/zij de 'voor de hand liggende afdeling'. Die afdeling wordt
    bepaald door GAP, en niet door CiviSync.
-   De gebruiker verandert een afdeling vanuit de persoonsfiche
-   De gebruiker selecteert X personen in de ledenlijst, en kiest de
    actie 'afdeling aanpassen'
    -   Als dat allemaal leiding is, kun je meerdere afdelingen kiezen,
        of geen.
    -   Als er een lid bij zit, moet er precies 1 afdeling gekozen
        worden

### Technisch

Alles gaat via LedenSync.AfdelingenUpdaten. Die vertaalt de
afdelingsjaren van een lid naar afdelingen voor CiviCRM, en stuurt die
met persoonsgegevens, stamnummer en werkjaar naar CiviSync. Er wordt geen
onderscheid gemaakt tussen leden met en zonder ad-nummer. Weer geen
probleem, want bij het initieel lid maken gaat de afdeling/de afdelingen
mee.

Verzekering loonverlies
-----------------------

### Use case

Als iemand aangesloten is, en 16 jaar of ouder, dan staat er op de
personenfiche aangegeven of de persoon verzekerd is voor loonverlies. Is
dat niet het geval, dan kan de user aanklikken dat hij die verzekering
wenst voor de gegeven persoon. Na een bevestiging gaat de info door naar
CiviCRM.

### Technisch

**TODO!**

Bivakaangifte
-------------

**TODO!**

Dubbelpuntabonnement
--------------------

Niet meer via het GAP, wel via de website.
