De synchronisatie GAP-KIP
=========================

Algemeen
--------

Als GAP een wijziging naar Kipadmin moet sturen, dan gebeurt dat door
een object van Chiro.Gap.Sync. Deze objecten plaatsen berichten in een
[MessageQueueing|message queue](MessageQueueing|message queue.md), die dan door KipSync (service aan
de kant van Kipadmin) worden opgepikt.

Persoonsgegevens
----------------

### Persoon herkennen

Als GAP het AD-nummer van een persoon kent, dan wordt dat AD-nummer
gebruikt om aan KipSync te laten weten welke persoon bewerkt moet
worden. Is het AD-nummer niet gekend, dan stuurt GAP alle informatie mee
naar Kipsync, die dan op deze manier probeert te bepalen of de persoon
al bestaat:

1.  AD-NR
2.  GAP-ID
3.  Zoek personen met zelfde naam, voornaam en geslacht\
    \#\* Iemand met zelfde telefoonnr/e-mail -&gt; OK\
    \#\* Iemand met zelfde postnr -&gt; OK\
    \#\* Iemand met zelfde geboortedatum -&gt; OK
4.  Niemand gevonden? -&gt; Nieuwe persoon

### Persoon bewaren

#### Use cases

Deze synchronisatie treedt enkel op als de gebruiker een al gekende
persoon aanpast door de gegevens te wijzigen op zijn persoonsfiche.

Er zijn 3 mogelijkheden:

-   De persoon heeft geen AD-nummer, en zijn AD-nummer is niet
    in aanvraag. Dan gebeurt er niets.
-   De persoon heeft een AD-nummer. KipSync vindt hem op basis van
    dat AD-nummer.
-   De persoon heeft een AD-nummer in aanvraag. Bij KipSync vindt hem
    normaalgezien op basis van GAP-ID. Als dat GAP-ID niet gekend is, is
    er vroeger wellicht iets misgelopen met de synchronisatie, en wordt
    hij op de bovengeschreven manier gezocht (naam, voornaam,
    geboortedatum), en in het slechtste geval aangemaakt.

Als we een nieuwe persoon toevoegen in GAP, moet die pas naar Kipadmin
als hij daadwerkelijk ingeschreven wordt als lid. Maar dat gaat via 'Lid
Bewaren', en niet via 'Persoon Bewaren'.

#### Technisch

Chiro.Gap.Sync.PersonenSync.Bewaren stuurt de gegevens van een persoon
naar KipSync. De persoonsgebonden gegevens (naam, geslacht,
geboortedatum, datum overlijden) worden altijd meegestuurd. Optioneel
zijn:

-   standaardadres
-   alle communicatie (e-mailadressen, telefoonnr's, enz.)

Die optionele zaken worden in praktijk nergens meegestuurd, en dat is
niet onlogisch, omdat de gebruiker die zaken 1 voor 1 wijzigt, en elke
wijziging apart naar Kipadmin gaat. Maar de mogelijkheid is wel nuttig
voor wanneer er eens een heel aantal personen via een script opnieuw
gesynct moeten worden.

### Voorkeursadres bewaren

#### Use cases

-   Een aantal (&gt;0) personen verhuizen van een gezamenlijk adres A
    naar een nieuw (gezamenlijk) adres B. De door Kipadmin gekende
    personen voor wie adres A het voorkeursadres was, krijgen nu adres B
    als voorkeursadres, en dat moet naar Kipadmin.
-   Een persoon (en eventueel enkele van zijn adresgenoten) krijgt een
    nieuw adres, dat als voorkeursadres moet dienen. Zij die door
    Kipadmin gekend zijn, moeten ook in Kipadmin het nieuwe
    adres krijgen.
-   Een adres wordt verwijderd van een persoon (en eventueel ook van een
    aantal adresgenoten). De personen waarvan het adres het
    voorkeursadres was, krijgen een ander voorkeursadres. Voor diegenen
    die gekend zijn door Kipadmin, wordt dat nieuwe adres gesynct.
-   Als het laatste adres van een persoon wordt verwijderd, dan moet het
    voorkeursadres in Kipadmin ook verwijderd worden. Dat is nu nog niet
    het geval. (En dat valt ook niet onder voorkeursadres
    bewaren, eigenlijk)
-   Een persoon, gekend door Kipadmin, en met meerdere adressen, krijgt
    een ander van zijn bestaande adressen als voorkeursadres. Dat nieuwe
    adres gaat naar Kipadmin.

'Gekend door Kipadmin', wil zeggen:

-   heeft een AD-nummer
-   of: AD-nummer is in aanvraag

#### Technisch

Chiro.Gap.Sync.AdressenSync mapt een Belgisch adres of buitenlands adres
naar Kip.ServiceContracts.DataContracts.Adres, dat alle adresgegevens
als string bevat. (Dus: volledige straatnaam, landnaam,... en geen
Crab-ID's of dergelijke.)

Het adres wordt samen met alle persoonsgegevens (ongeacht of het
AD-nummer gekend is of niet; hier is nog ruimte voor optimalisatie) naar
KipSync gestuurd (StandaardAdresBewaren).

### Communicatie

#### Use cases

##### Nieuwe communicatie

-   Geef een nieuwe communicatievorm aan een door kipadmin gekende
    persoon via de personenfiche.
-   Maak een nieuwe persoon, die op hetzelfde adres woont als een
    persoon die gekend is door kipadmin, en geef die nieuwe persoon bij
    het maken een gezinsgebonden communicatievorm. (Dit werkt nog niet,
    zie \#1070)
-   Geef een nieuwe gezinsgebonden communicatievorm aan een persoon
    onbekend door kipadmin, die wel op hetzelfde adres woont als een
    persoon wel gekend door kipadmin. (Werkt ook niet, zie \#1070)

Een nieuwe communicatievorm kan bij Kipadmin al gekend zijn,
bijvoorbeeld als een andere groep of het secretariaat die al had
toegevoegd. In dat geval wordt de instelling 'inschrijven
snelleberichtenlijst' wel overgenomen. (We moeten iets doen.)

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

CommunicatieToevoegen van KipSync wordt aangeroepen door
Chiro.Gap.Sync.CommunicatieSync.Toevoegen. Er wordt steeds maar één
communicatievorm tegelijk toegevoegd. In GAP hangt die t.g.v. een
designfout aan slechts 1 persoon, waardoor CommunicatieSync.Toevoegen
voldoende informatie heeft aan de communicatievorm alleen.

Kipsync behoudt de voorkeur van de communicatievormen niet. Dat is een
bug (\#1406). Als de communicatievorm al gekend is, dan blijft die zijn
voorkeur houden. Een nieuwe communicatievorm wordt achteraan de lijst
toegevoegd.

##### Communicatie wijzigen

CommunicatieBijwerken van Kipsync wordt aangeroepen door
CommunicatieSync.Bijwerken.

In het overgrote deel van de gevallen zijn alle communicatievormen van
een gekende persoon ook gekend in Kipadmin. Dan is het aanpassen geen
probleem.

Het kan zijn dat een groep een communicatievorm van een persoon heeft
verwijderd. Als een andere groep die communicatievorm nog wel heeft, en
aanpast, dan is de te wijzigen communicatievorm niet meer te vinden in
Kipadmin. In dat geval wordt er gewoon een nieuwe aangemaakt.

Om in Kipadmin de bestaande communicatievorm te vinden, worden de
strings letterlijk vergeleken. Op zich niet erg dramatisch, omdat het
invoerformaat min of meer vast ligt. Maar het zou nog beter kunnen.
(Bijv. enkel de numerieke karakters bekijken, en +32 gelijk stellen aan
een voorloop-0).

##### Communicatie verwijderen

De te verwijderen communicatievorm wordt opgezocht op basis van de
persoon, het communicatietype en het nummer. Als er niets gevonden
wordt, wordt er ook niets verwijderd.

Net zoals bij het wijzigen, is er een probleem als de communicatievorm
in een ander formaat in kipadmin zou zitten als in GAP.

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
    Alnaargelang dat lid in Kipadmni zit (hangt van moment van
    uitschrijven af), moet het lid in Kipadmin opnieuw worden
    aangemaakt, of anders moet lidtype en afdeling mogelijk
    worden bijgewerkt. **LET OP:** als je dit manueel nakijkt, let er
    dan op dat je in Kipadmin en in GAP in hetzelfde werkjaar bezig
    bent!

#### Technisch

Alles wordt geregeld door LedenSync.Bewaren, dat een lid meekrijgt als
parameter, en dan LidBewaren of NieuwLidBewaren aanroept van KipSync
(alnaargelang een AD-nummer is gekend of niet)

NieuwLidBewaren wordt normaalgezien aangeroepen inclusief adressen en
communicatie.

Kipsync heeft wel wat werk om alles in de rare structuur van Kipadmin te
krijgen:

-   Als er geen AD-nummer gegeven is, wordt de persoon voor zover het
    kan opgezocht of aangemaakt. (Hier is nog wel ruimte
    voor verbetering)
-   Als de persoon al lid is in de gegeven groep, dan blijft dat
    lidobject bestaan, maar worden afdelingen, functies en soort
    juist gezet.
-   Als er dit jaar nog geen aansluitingen gebeurden, verwijder dan alle
    leden van de groep (want bij de Kipadmin-jaarovergang worden alle
    oude leden gewoon opnieuw lid in het nieuwe werkjaar. Dat dateert
    nog uit de tijd dat jaarlijks alle leden verdwenen.)
-   Er moet een nieuwe aansluiting (die krijgt meteen een
    niet-doorgeboekte factuur) gemaakt worden in deze gevallen:
    -   er was nog geen aansluiting
    -   de recentste aansluiting heeft een doorgeboekte factuur
    -   de recentste aansluiting is te lang geleden
-   Het nieuwe lid wordt nu toegevoegd aan de recentste aansluiting
    -   bevat het aantal jaar lid of leiding. Dat wordt gewoon geteld.
    -   bereken einde instapperiode
    -   koppel lidtype, afdelingen en functies

### Leden verwijderen

#### Use cases

-   In de persoonsfiche van een lid wordt er op de link
    'uitschrijven' geklikt.
-   In de ledenlijst van dit werkjaar worden een aantal personen
    geselecteerd, en vervolgens wordt de actie 'uitschrijven' gekozen.

#### Technisch

Leden worden enkel verwijderd uit Kipadmin als hun probeerperiode nog
niet voorbij is. Die controle gebeurt door het GAP zelf. Enkel als de
probeerperiode voorbij is, wordt LedenSync.Verwijderen aangeroepen, die
dan op zijn beurt (Nieuw)LidVerwijderen aanroept van KipSync.

### Functies updaten

#### Use cases

-   Op de fiche van een lid klik je op het wijzigen-icoontje om de
    functies aan te passen. Die pas je dan aan in het popupvenstertje.

#### Technisch

LedenSync.Functiesupdaten geeft alle functies van een lid door aan
FunctiesUpdaten van KipSync. Er wordt geen onderscheid gemaakt tussen
leden met en zonder ad-nummer. (Wat niet heel erg is, want slechts een
beperkt aantal leden zal functies krijgen.)

KipSync verwijdert eerst de te verwijderen functies, en voegt daarn de
toe te voegen functies toe.

Als de functie 'financieel verantwoordelijke' wordt toegevoegd, dan
wordt dat ook aangepast in het groepsrecord.

Als de functie 'financieel verantwoordelijke' wordt afgenomen, dan wordt
die van het groepsrecord ook aangepast: een andere financieel
verantwoordelijke (if any), en anders de contactpersoon. Als dat ook
niet gaat, dan blijft het tóch dezelfde.

### LidType updaten

#### Use case

-   Op een personenfiche staat 'ingeschreven als lid' of 'ingeschreven
    als leiding', met daarachter een wijzigen-icoontje. Als de gebruiker
    op dat icoontje klikt, dan wisselt het om.

#### Technisch

Telkens de user het lidtype wijzigt in de personenfiche, wordt dat door
LedenSync.TypeToggle doorgestuurd naar LidTypeInstellen van KipSync. Er
wordt geen onderscheid gemaakt tussen leden met en zonder ad-nummer. Ook
hier is dat niet erg, want de use case is zeldzaam.

Kipadmin kent 3 lidtypes: lid, leiding, kader. Dat is anders dan bij het
GAP.

In principe kan het wisselen van type enkel gebeuren bij plaatselijke
groepen. Moest KipSync zo'n verzoek krijgen voor een kaderploeg, dan
wordt het lidtype gewoon 'kader'.

### Afdelingen updaten

#### Use cases

-   De gebruiker verandert het lidtype van een lid. Wordt een lid
    leider, dan verdwijnt de afdeling. Wordt hij/zij lid, dan krijgt
    hij/zij de 'voor de hand liggende afdeling'. Die afdeling wordt
    bepaald door GAP, en niet door Kipsync.
-   De gebruiker verandert een afdeling vanuit de persoonsfiche
-   De gebruiker selecteert X personen in de ledenlijst, en kiest de
    actie 'afdeling aanpassen'
    -   Als dat allemaal leiding is, kun je meerdere afdelingen kiezen,
        of geen.
    -   Als er een lid bij zit, moet er precies 1 afdeling gekozen
        worden

### Technisch

Alles gaat via LedenSync.AfdelingenUpdaten. Die vertaalt de
afdelingsjaren van een lid naar afdelingen voor Kipadmin, en stuurt die
met persoonsgegevens, stamnummer en werkjaar naar KipSync. Er wordt geen
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
Kipadmin.

### Technisch

**TODO!**

Bivakaangifte
-------------

**TODO!**

Dubbelpuntabonnement
--------------------

Niet meer via het GAP, wel via de website.
