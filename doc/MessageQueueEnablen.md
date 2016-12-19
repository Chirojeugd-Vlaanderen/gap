De message queue enablen
========================

Op Windows 7/8
--------------

Je kunt message queueing enablen via 'Programs' (software). Klik op
'Turn Windows features on or off'.

In het venster dag verschijnt, vink je onder 'Microsoft Message Queue
(MSMQ) Server' Microsoft Message Queue (MSMQ) Server Core' aan. Dat is
voldoende. ('t Is te zeggen, je moet wel rebooten :()

Queues kun je dan aanmaken via 'Administrative Tools', 'Computer
Management', 'Services and Applications', 'Message Queues'. Zorg ervoor
dat je queues transactional zijn.

Zie ook
-------

[MessageQueueing](MessageQueueing.md)
