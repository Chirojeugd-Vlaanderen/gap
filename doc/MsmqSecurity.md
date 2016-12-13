Security van de message queues
==============================

De GAP-backend stuurt updates naar Kipadmin via een message queue, die
uitgelezen wordt door KipSync.\
Kipadmin stuurt updates naar GAP via een message queue, die uitgelezen
wordt door GapUpdate.

Kipadmin draait in het domein chiro.lokaal (KIPDORP). Gap in
chiro.wereld (CHIROPUBLIC). Chiro.wereld vertrouwt chiro.lokaal, maar
niet omgekeerd. En om die reden krijg ik de security van de message
queues niet goed.

Op dit moment is er geen enkele security op de queues; iedereen kan
berichten in de queue posten. **Dat is een probleem.**

Zie \#927.
