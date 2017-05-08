# GapMaintenance

GapMaintenance is een .NET-consoletoepassing die periodiek wordt uitgevoerd
door een scheduled task op de GAP-server.

Het doet de volgende zaken:

* Het controleert hoeveel berichten er staan te wachten in de message queue
  voor [CiviSync](SyncGapCivi.md). Als dat er 'te veel' zijn, dan ligt
  CiviSync waarschijnlijk uit, en dan wordt er een bericht gestuurd naar
  de Chirohelpdesk.
* Als er leden zijn zonder AD-nummer, dan wordt de lidrelatie opnieuw
  doorgestuurd naar CiviCRM. In theorie is deze controle niet nodig, maar
  als CiviSync foute AD-nummers vaststelt in GAP, worden die verwijderd.
  Deze controle zorgt er dan voor dat de persoonsgegevens opnieuw naar
  CiviCRM gaan. Hopelijk worden ze dan correct gematcht, en dan zal
  CiviCRM achteraf het juiste AD-nummer doorgeven aan GAP via
  UpdateApi.
* Er wordt nagekeken welke leden voorbij hun probeerperiode zijn, en
  nog niet aangesloten. Voor deze leden wordt er dan via
  [CiviSync](SyncGapCivi.md) een
  [aansluiting](https://gitlab.chiro.be/civi/chirocivi/blob/dev/doc/Aansluitingen.md)
  gemaakt in CiviCRM.
