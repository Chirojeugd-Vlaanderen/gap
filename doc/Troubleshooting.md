Troubleshooting
===============

(work in progress)

Compilatie en debugging
-----------------------

### De toepassing compileert, maar start niet op

Mogelijk is het verkeerde project ingesteld als 'startup project'. Klik
in de solution explorer met de rechtermuisknop op 'Chiro.Gap.!WebApp',
en kies 'Set as startup project'.

### De toepassing heeft geen toegang tot de database

Als je de testdatabase op Kipdorp gebruikt (standaard is dat het geval),
en je bevindt je buiten het Chironetwerk, controleer dan of je
VPN-connectie open staat (bijv. ping 172.16.1.250 moet replies
opleveren). Zie [VpnGebruiken](VpnGebruiken.md).

Gebruik je je eigen kopie van de database, controleer dan de connection
string in \[source:trunk/Solution/Chiro.Gap.Services/Web.config\#L88\].

### Als ik debug met een andere browser dan Internet Explorer, worden mijn credentials niet aanvaard

Waarschijnlijk ondersteunt je browser geen NTLM-authenticatie.

Klik in de solution explorer met de rechtermuisknop op het project van
de te debuggen web applicatie, en kies 'Properties'. Open het flapje
'Web', en vink onder 'Servers' 'NTLM Authentication' uit.
