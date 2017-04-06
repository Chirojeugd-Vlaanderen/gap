Message Queueing
================

Microsoft Message Queueing is een systeem dat toelaat om berichten te
sturen naar een queue op een windowsmachine. Clienttoepassingen kunnen
die berichten dan uitlezen en verwerken. Er zit daar een systeem achter
dat een retry queue implementeert. Berichten die niet correct
afgehandeld worden komen in een dead message queue terecht.

Hoe zet je dat aan voor Windows 7: [MessageQueueEnablen](MessageQueueEnablen.md)

Als je een queue wilt aanmaken om KipSync of CiviSync te testen, dan kan
dat via Computer Management, Services and applications, Message Queues.
Let er op dat je een transactional queue aanmaakt, en dat je zelf de
rechten hebt om berichten te verzenden en te lezen.
