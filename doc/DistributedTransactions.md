Distributed transactions
========================

**Dit is niet nodig voor de dev-omgeving, maar WEL relevant voor live.**

Dit is enkel relevant voor mensen die aanloggen op Kipdorp. Distributed
transactions werken sowieso niet als je niet aanmeldt op het domein.

Om de niet-kipdorpers toch mee te kunnen laten ontwikkelen, mogen de
distributed-transation-scopes enkel tussen '\#if KIPDORP' en '\#endif'
statements gebruikt worden.

(Als bij het runnen de toepassing crasht op distributed transactions,
kijk dan in de project properties of er onder 'BUILD' nergens nog
'KIPDORP' voorkomt.)

Omdat het updaten van de database van GAP en het verzenden van de
messages naar de queue in 1 transactie moeten gebeuren, en de database
en de queue meestal op een verschillende server staan, moeten
distributed transactions geactiveerd worden.

**Let op!** Database en frontend draaien wel eens in een verschillend
domein. Denk daaraan als je DTC door de firewall moet doorlaten.

Zeer kort:

Windows XP
----------

**** Administrative tools

**** Component Services

**** Computers

**** My Computer

**** flapje MSDTC

**** Security configuration

**** Vink aan: Network DTC

**** Vink aan: Allow inbound

**** Vink aan: Allow outbound

**** Vink aan: No authentication

Zet de windows-firewall uit.

Windows 7/Windows server
------------------------

**** Administrative tools

**** Component Services

**** Computers

**** My Computer

****\* Rechts klikken op 'Local DTC' onder 'Distributed Transaction
Coordinator'. Dan 'Properties'

****\* Flapje 'Security'

**** Vink aan: Network DTC

**** Vink aan: Allow inbound

**** Vink aan: Allow outbound

**** Vink aan: No authentication

**** Contgrol Panel

**** System and Security

**** Onder 'Windows Firewall: allow a program through Windows Firewall'

**** Achter 'Distributed Transaction Coordinator' 'Domain' aanvinken.

Als het nog niet werkt:

-   vink ook 'Allow Remote Clients' aan
-   zet bij in de firewall ook een dtc-uitzondering voor
    publieke netwerken.

Ik kreeg het voor GAP niet in orde zonder de firewall binnen het
domeiein uit te zetten.
TODO: verfijnen
