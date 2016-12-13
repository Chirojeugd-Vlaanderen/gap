Message Queue opzetten
======================

Het is niet nodig dat je dit doet op je development PC; sinds changeset
r1988 is het versturen van de messages weggemockt in de trunk.

Maar voor het geval je toch een queue wilt opzetten (bijv. om met
kipsync te experimenteren), vind je hier hoe.

Als message queueing niet geactiveerd is, krijg je deze foutboodschap
bij het wijzigen van personen:

&lt;pre&gt;\
[TypeInitializatonException](TypeInitializatonException.md) occured.\
The type initializer for 'System.ServiceModel.Channels.Msmq' threw an
exception.\
&lt;/pre&gt;

Heb je message queuing wel geactiveerd, maar bestaat de queue niet, dan
is dit de foutmelding:

&lt;pre&gt;\
[EndpointNotFoundException](EndpointNotFoundException.md) occured.\
An error occurred while opening the queue:Unrecognized error -1072824317
(0xc00e0003). The message cannot be sent or received from the queue.
Ensure that MSMQ is installed and running. Also ensure that the queue is
available to open with the required access mode and authorization.\
&lt;/pre&gt;

Message Queuing enablen op Win XP
---------------------------------

### Op Win XP

**** Log aan als domain admin.

**** Start via 'Control Panel' 'Add or Remove Programs'.

**** Klik 'Add/Remove Windows Components'

**** Vink 'Message Queueing' aan, en klik 'Details'

**** Vink 'Active Directory Integration', 'Common' en 'Triggers' aan.
(Dat laatste zullen we niet gebruiken, maar we installeren het alvast
voor het geval dat :-)

**** 'Next' om te installeren

### Op Win 7

**** Klik op Start &gt; Control Panel

**** Klik in het nieuwe venstertje op de link 'Programs'

**** Klik in het nieuwe venstertje op de link 'Turn Windows features on
or off'

**** Zoek in het nieuwe venstertje 'Microsoft Message Queue (MSMQ)
Server Core' en klik daar alles aan

(Op [MessageQueueEnablen](MessageQueueEnablen.md) zie je dat met screenshots)

De Queue toevoegen
------------------

**** Via 'Administrative Tools' start je 'Computer Management'

**** Klik 'Services and Applications' en 'Message Queueing' open

**** Rechtsklik op 'Private Queues', klik 'New' en 'Private Queue'

**** De Queue naam is '!KipSyncPersoonServiceQueue', en vink
'Transactional' aan. (De queue naam is gedefinieerd in de source; zie
\[source:trunk/Solution/Chiro.Gap.Services/Web.config@1147\#L178\].)
