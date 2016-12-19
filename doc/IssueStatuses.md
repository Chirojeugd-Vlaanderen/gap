Issue Statuses
==============

Overzicht statussen
-------------------

-   Nieuw: standaard - het isssue is gerapporteerd.
-   Needs review: je hebt een wijziging gepusht, die nagekeken
    moet worden.
-   Needs work: er is nog geen fix.
-   RTBC: ('reviewed and tested by the community') wijziging is
    nagekeken, en is goed bevonden.
-   Needs deployment: wijziging is gemerged in dev
-   Gesloten: fix is opgenomen in live-omgeving. (Gesloten)
-   Afgewezen: wordt niet gefixt.
-   Duplicate: dit ticket zit dubbel in het systeem.

Typische workflow
-----------------

-   Iemand rapporteert een issue. De status wordt **Nieuw**.
-   Als je aan die issue wilt werken, assign je die aan jezelf. Je
    creeert een **issue branch** voor je wijziging (vanuit dev).
-   Bij iedere commit die je doet voor die issue, zet je in de commit
    message 'refs \#issuenr'.
-   Als je branch pusht, dan verandert de status in **needs review**.
    Opgelet: push je eigen branch. **Push niet direct naar dev, staging
    of master** tenzij je de toorn des Johans wilt oproepen ;-)
-   De bedoeling is dat iemand anders dan jijzelf de code bekijkt.
    -   Vindt die het ok, dan verandert die de status in **RTBC**
        (reviewed and tested by the community).
    -   Vindt die het niet ok, dan verandert die de status in **Needs
        work**, met dan in de comment waarom er nog werk nodig is.
-   Een projectbeheerder (op dit moment enkel Johan en Bart) merget
    issue branches in dev, en verandert de status in **Needs
    deployment**. Issues met status 'RTBC' kunnen waarschijnlijk vrij
    snel gemerged worden.
-   Van zodra de fix in de live-omgeving zit, dan wordt de status
    **Closed**.

