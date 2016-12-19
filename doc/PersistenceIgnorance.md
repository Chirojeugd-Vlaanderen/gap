Persistence Ignorance
=====================

Met 'Persistence Ignorance' (PI) wordt bedoeld dat de business objects
losgekoppeld moeten zijn van de persistentielaag (data access layer).

Als je iets van het onderstaande tegenkomt, dan weet je dat je software
**niet** PI is:

**** Alle klasses moeten erven van een provider-specifieke basisklasse.
(uitzondering: Object)

**** Alle klasses moeten geinstantieerd worden via een
provider-specifieke factory.

**** Providerspecifieke datatypes voor bijvoorbeeld collecties.

**** Alle klasses moeten providerspecifieke interfaces implementeren.

**** Alle klasses moeten een specifieke niet-standaardconstructor
aanbieden.

(Bron: Pro LINQ Object Relational Mapping with C\# 2008, Vijay P. Mehta)
