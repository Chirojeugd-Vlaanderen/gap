Infrastructuur van de live-omgeving
===================================

Web server
----------

Frontend (MVC webapp) en backend (WCF service) draaien momenteel beide
op gapsrv1.chiro.wereld (IIS7). Voor beide toepassingen is er een aparte
application pool.

Windows server 2008R2 voorziet zoiets als 'Application Pool Identities',
waardoor er automatisch een virtuele user wordt gemaakt voor iedere
application pool. Voor de backend kunnen we dit niet gebruiken, omdat
deze users in chiro.wereld bestaan, en bijgevolg niet via Windows
authenticatie naar de database kunnen (die draait in chiro.lokaal). Voor
de frontend wel.

De backend draait onder de service-account KIPDORP\\GapService, de
frontend onder de application pool account.

Message queue
-------------

De backend van GAP communiceert met Kipadmin door messages naar een
queue te sturen. Deze queue staat bij voorkeur niet op een
Windows-2003-server, want daar zijn de mogelijkheden om probleemmessages
af te handelen te beperkt. Vandaar dat de queue momenteel een private
queue is op gapsrv1: msmq://gapsrv1/private/KipSyncLive

Kipsync
-------

Kipsync is de service die de messages uit de queue leest, en de
wijzigingen doorvoert in Kipadmin. Kipsync is een Windows service, en
wordt uitgevoerd met de service-account KIPDORP\\kipsync.

Lidsyncer
---------

Lidsyncer stuurt de info van alle leden met een probeerperiode die
voorbij is, naar de queue. Lidsyncer wordt via een periodieke taak
iedere nacht aangeroepen, met de credentials van KIPDORP\\!GapService.

Mailscript
----------

Het mailscript stuurt 4 dagen vooraleer leden uit hun probeerperiode
gaan, een mailtje naar de GAV's van die groepen. De manier waarop dit
gebeurt is niet proper, dat was 'van de rap rap'.

Het script loopt op kiplin2, de linuxserver die ook gebruikt wordt voor
trac/svn/redmine. Met de credentials van een database-user haalt dit de
logins van de betreffende GAV's op uit de GAP-database. Dan zoekt het
script in AD de AD-nummers van die users op. Daarna haalt het de
e-mailadressen van die GAV's op uit Kipadmin (ook
databaseauthenticatie), en stuurt het een gepersonaliseerde mail met een
link naar de leden in probeerperiode.
