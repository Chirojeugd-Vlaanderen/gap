Chiro.Gap.Diagnostics.\*
========================

De projecten onder Chiro.Gap.Diagnostics vormen een administratieve
webapplicatie voor secretariaatsmedewerkers. De bedoeling is dat ze
daarmee tijdelijke logins kunnen aanmaken, en a-posteriori een aantal
zaken kunnen rechtzetten als er fouten optraden. Ik denk in eerste
instantie aan het opnieuw syncen van adressen die niet doorgekomen zijn
naar Kipadmin.

Architectuur
------------

De administratieve toepassing is een webapplicatie. Deze webapplicatie
(Chiro.Gap.Diagnostics.!WebApp) contacteert een backend via services
(Chiro.Gap.Diagnostics.Services). De services spreken businesslogica aan
uit Chiro.Gap.Workers en Chiro.Gap.Diagnostics.Workers.

Chiro.Gap.Diagnostics.Workers bevat alle logica die van toepassing is op
zowel Kipadmin als Gap. Achterliggend wordt een ander entity data model
gebruikt, om te vermijden dat het entity data model van GAP vervuild
wordt door views op de huidige (oude) Kipadmin.

Alle andere businesslogica komt uit Chiro.Gap.Workers, die door
Chiro.Gap.Diagnostics.Services rechtstreeks wordt aangesproken, en niet
via de GAP-webservices. Dit omdat de administratieve app een andere
security heeft dan de webservices van GAP ('superGAV' in plaats van GAV
met gekoppelde groepen).

Security
--------

De webservices van GAP mogen enkel aangeroepen worden door users uit een
bepaalde security group van active directory. (In de 'live' is dat
KIPDORP\\g-!GapSuper.) De frontend moet 'impersonation' doen om de
service aan te roepen. (Momenteel is de frontend altijd
'geimpersonated', wat waarschijnlijk niet ideaal is.) Achter de
webservices is de dependency injection zodanig geconfigureerd, dat
'IAutorisatieManager.!IsSuperGav' altijd true oplevert (waar dat
standaard false is). Op die manier kan de bestaande backend gebruikt
worden voor de businesslogica.
