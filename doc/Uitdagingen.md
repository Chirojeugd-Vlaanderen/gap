De toekomst van het GAP
=======================

Op 20 oktober 2014 hadden we een vergadering over wat de plannen van het
GAP zijn op langere termijn. Op basis van een [presentatie met heel wat
vragen](https://develop.chiro.be/johan/toekomst-gap) bespraken we een
nieuwe lange-termijnvisie.

Ik maakte een roundup van het gesprek. Sommige dingen heb ik nog
bijbedacht na gisterenavond; ik vermeld dat dan ook dat ik ze bijbedacht
heb. Ben je ergens niet akkoord, laat iets weten. Vul zaken die ik
vergat ook gerust aan.

AngularJS op restful backend
----------------------------

We zouden op termijn de frontend van het GAP graag willen vervangen door
een AngularJS-toepassing (of gelijkaardig), die werkt op een REST-API.
Als we die REST-API goed aanpakken, dan kunnen we die ook (al dan niet
gedeeltelijk) toegankelijk maken voor de groepen.

Permissies (gebruikersrechten)
------------------------------

Zie [GebruikersRechten](GebruikersRechten.md).

Een restful API
---------------

Momenteel haalt de API zijn gegevens op via Entity Framework op de
database. Beter zou zijn dat de API de backend zou gebruiken, dat
vermijdt dubbel werk, en dat vermijdt ook dat dezelfde fouten 2 keer
worden gemaakt. Er zijn een aantal frameworks die hierbij mogelijk
kunnen helpen.

-   [ASP.NET Web API](http://www.asp.net/web-api)
-   [Nancy](Nancy.md), zie http://nancyfx.org/
-   [ServiceStack](https://servicestack.net/)

In principe kan Nancy de WCF vervangen. In eerste instantie is het
misschien wel beter om Nancy bovenop WCF te zetten.

Aandachtspunt voor de API: hou de terminologie zo eenvoudig mogelijk.
Gebruik bijv. Persoon i.p.v. GelieerdePersoon. Aan de frontend is er
toch geen verschil.

Stappenplan
-----------

1.  Opzetten van beperkt permissiesysteem
2.  Opzetten van een restful API, in eerste instantie voor app-accounts
    die read-only access hebben
3.  Uitvissen hoe we persoonlijke accounts kunnen laten werken met de
    API
4.  Het GAP stapsgewijze herschrijven zodat het meer en meer gebruik
    maakt van de nieuwe API
5.  API documenteren voor de gebruikers

Extra documentatie
------------------

*Met dank aan Tommy*

-   [Visug-presentatie over Servicestack en
    Nancy](http://www.slideshare.net/hanneslowette1/visug-session-servicestack-nancy)
-   We kwamen gisteren OWIN tegen, dat is een interface voor webservers
    die .NET webapps kunnen hosten. Denk ik. Die laat dan toe om je
    webapps te runnen op iets anders dan IIS. Denk ik.
    [Katana](http://www.asp.net/aspnet/overview/owin-and-katana/an-overview-of-project-katana)
    is een implementatie van die interface.

