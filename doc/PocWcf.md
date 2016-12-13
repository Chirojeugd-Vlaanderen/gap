Testapplicatie voor WCF en Entity Framework
===========================================

Source
------

-   Source code: source:trunk/poc/wcf
-   Subversion: https://develop.chiro.be/subversion/cg2/trunk/poc/wcf

Wat doet het
------------

De consoleapplicatie in het project doet het volgende:

Via de service wordt een persoon (p) opgevraagd. Diezelfde persoon wordt
opnieuw opgevraagd in een ander object (a).

De gebruiker kan een nieuwe voornaam ingeven, persoon p krijgt deze
nieuwe voornaam, en de wijzigingen worden via de service bewaard.

Daarna wordt de voornaam van persoon a gewijzigd, en wordt opnieuw aan
de service gevraagd om de wijzigingen te bewaren. Dit veroorzaakt een\
exceptie.

Opmerkingen
-----------

Iedere keer als we een object dat we terugkrijgen van de service willen
wijzigen, moeten we het expliciet klonen. Dat lijkt niet zo mooi.
Kunnen\
we dit eleganter doen?

De consoleapplicatie importeert [CgDal](CgDal.md), wat layergewijs niet
juist is. Ik vermoed dat het dit is wat Tommy bedoelde met 'pragmatic
SOA'. In deze\
applicatie is dat nodig, omdat de functie 'CloneSerializing'
gedefinieerd is in [CgDal](CgDal.md).EfExtensions. Mogelijk staat die daar
ook niet op zijn plaats.

Maar zelfs als we diezelfde functie zouden definiëren in het
clientproject, zou ze nog niet werken, omdat het persoonsobject waarop
ze werkt van het type 'EntityObject' moet zijn. Zonder 'pragmatic SOA'
krijgen we dat aan de clientkant niet goed, denk ik. Bedenkingen
hierbij?

Relevante links
---------------

-   http://blogs.msdn.com/cesardelatorre/archive/2008/09/04/updating-data-using-entity-framework-in-n-tier-and-n-layer-applications-short-lived-ef-contexts.aspx

