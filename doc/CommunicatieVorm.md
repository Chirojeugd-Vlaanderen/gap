[CommunicatieVorm](CommunicatieVorm.md)
========================

In het domain model uit onze specificaties, staat het als volgt:

![](commvormtypespec.png)

Een gelieerde persoon heeft communicatievormen. Die communicatievormen
zijn van een bepaald type. De associatie gelieerde persoon -
communicatievorm heet 'communicatie'.

Per communicatietype per persoon heeft 1 communicatievorm de voorkeur.
De voorkeur wordt aangegeven in de entiteit 'communicatie', net zoals
eventuele opmerkingen die voor die communicatievorm en die persoon
gelden.

Op deze manier kunnen communicatievormen gedeeld worden door
verschillende gelieerde personen. De gedeelde communicatievorm kan dan
voor persoon A de voorkeur hebben, en voor persoon B niet. Ook de
opmerkingen kunnen verschillen.

In onze database-implementatie, zit het echter zo:

![](commvormtypeimpl.png)

De entiteiten 'communicatie' en 'communicatievorm' zijn blijkbaar
gemerged. Omdat 'voorkeur' persoonsgebonden is, kunnen
communicatievormen niet meer gedeeld worden, maar moeten ze worden
gekopieerd.

Noot: De velden [CommunicatieVorm](CommunicatieVorm.md).IsVoorOptIn en
[CommunicatieType](CommunicatieType.md).IsOptIn hebben een erg onduidelijke naam.
Misschien zijn de namen in den tijd wel per ongeluk omgewisseld.
[CommunicatieType](CommunicatieType.md).IsOptIn is true als de mogelijkheid van opt-in
moet bestaan voor communicatievormen van het gegeven type. Als
[CommunicatieType](CommunicatieType.md).IsVoorOptIn gezet is, dan wil dat zeggen dat er
'ingeopt' is voor die communicatievorm. (In praktijk: dat het
e-mailadres mee mag naar de snelleberichtenlijst.)
