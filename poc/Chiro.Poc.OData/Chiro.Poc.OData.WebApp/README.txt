PROOF OF CONCEPT VOOR EEN ODATA-API

Source: https://develop.chiro.be/subversion/cg2/trunk/poc/Chiro.Poc.OData

De bedoeling is dat dit een dummy API wordt, waarbij we onze gegevens als WCF 
Data Service aanbieden.  De gegevens moeten gemakkelijk als JSON opgehaald
kunnen worden d.m.v. een PHP-script.

(De opgeleverde gegevens zijn gewoon hardgecodeerd, zie Business.cs)


TESTEN IN VISUAL STUDIO:

Als je dit project start, krijg je een ATOM-bestand te zien, waaruit blijkt dat 
je leden kunt opvragen via deze service.  Dat kan door te surfen naar deze url:

http://localhost:50567/DataService.svc/Leden

De mogelijkheid bestaat om individuele leden op te halen op basis van hun ID

http://localhost:50567/DataService.svc/Leden(2)

Je kunt ook met een soort van query's werken:

http://localhost:50567/DataService.svc/Leden?$orderby=Naam
http://localhost:50567/DataService.svc/Leden?$orderby=Naam&$skip=1&$top=2
http://localhost:50567/DataService.svc/Leden?$filter=Naam%20eq%20%27Warke%27

Voor meer info over de query strings:
http://www.odata.org/developers/protocols/uri-conventions#QueryStringOptions


DE DATA SERVICE DEPLOYEN

Let op: als je de data service wilt aanspreken vanop een andere machine
dan diegene waarop Visual Studio draait, moet je je service deployen op
een IIS server.  Je kunt hiervoor IIS configureren op je eigen PC, maar 
als je dat doet nadat .NET 4 is geïnstalleerd, zul je nog 
aspnet_regiis -ri
moeten uitvoeren, om .NET 4 applicaties te kunnen starten.

Zorg ervoor dat je toepassing draait in een application pool met het
.NET 4-framework.


JSON OPHALEN

De OData-specificatie schrijft voor dat je de gegevens ook kunt ophalen als
JSON, maar daarvoor moet je een aangepaste http-header meegeven met je
request, namelijk 'accept: application/json'.  Aangenomen dat je de service
deployt op http://lap-jve2/ODataPoc, dan kan dat als volgt met wget:

wget -q -O - --header 'accept: application/json' http://lap-jve2/ODataPoc/DataService.svc/Leden

Als je de header weglaat, zul je opnieuw een ATOM-resultaat verkrijgen.

wget -q -O - http://lap-jve2/ODataPoc/DataService.svc/Leden


CLIENTS

In het mapje 'VoorbeeldClients' vind je wat voorbeelden van client code.
Om met JQuery cross-domain-requests te kunnen doen, moet de service JSONP ondersteunen.
Hiervoor heb ik Microsoft.Data.Services.Toolkit gebruikt, wat je kunt downloaden van
http://wcfdstoolkit.codeplex.com/