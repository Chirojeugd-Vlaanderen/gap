Uitzonderingen in de businesslaag opvangen in de UI
===================================================

In praktijk gebeurt het hier en daar wat verschillend. Ik probeer een
min of meer standaardmanier van werken te omschrijven.

Als de backend een exception wil doorgeven aan de frontend, dan gaat dat
via een `FaultException`. Om die `FaultException` te genereren,
gebruiken we de `FaultExceptionHelper`. Zie \[\[Backend\#Exceptions\]\].

Mogelijke FaultExceptions opgeven in service contract
-----------------------------------------------------

Als een service method een FaultException kan veroorzaken, dan moet dit
vermeld worden in het service contract. Zie bijvoorbeeld:
\[source:trunk/Solution/Chiro.Gap.ServiceContracts/IGelieerdePersonenService.cs@1955\#L295\]

&lt;pre&gt;\
\[OperationContract\]\
\[FaultContract(typeof(GapFault))\]\
\[FaultContract(typeof(FoutNummerFault))\]\
void [CommunicatieVormToevoegen](CommunicatieVormToevoegen.md)(int gelieerdePersoonID,
[CommunicatieInfo](CommunicatieInfo.md) commInfo);\
&lt;/pre&gt;

Dit wil zeggen dat de method CommunicatieVormToevoegen een
FaultException&lt;![](GapFault&gt; of een FaultException&lt;)FoutNummerFault&gt;
kan opleveren.

Fault contracts
---------------

FoutNummerFault is een voorbeeld van een faultcontract, en bevat
informatie over het probleem. Zie
\[source:trunk/Solution/Chiro.Gap.ServiceContracts/FaultContracts/FoutNummerFault.cs\].
De FoutNummerFault is vrij eenvoudig:

&lt;pre&gt;\
\[DataContract\]\
public class [FoutNummerFault](FoutNummerFault.md) : [GapFault](GapFault.md)\
{\
\[DataMember\]\
public [FoutNummer](FoutNummer.md) FoutNummer { get; set; }\
\[DataMember\]\
public string Bericht { get; set; }\
}\
&lt;/pre&gt;

Dit faultcontract bevat enkel een foutnummer en een bericht.

De FaultException opwerpen
--------------------------

Op het moment dat de service method CommunicatieVormToevoegen
bijvoorbeeld merkt dat het formaat van bijv. een e-mailadres ongeldig
is, werpt ze een FaultException&lt;!FoutNummerFault&gt; met het juiste
foutnummer op.

&lt;pre&gt;\
throw
FaultExceptionHelper.FoutNummer(FoutNummer.GroepsWerkJaarNietBeschikbaar,
Properties.Resources.MeerInfo);\
&lt;/pre&gt;

Het foutnummer is FoutNummer.!ValidatieFout, en de foutboodschap wordt
overgenomen uit de exception. In dit geval kan dat geen kwaad. Je moet
er wel op letten dat je op die manier geen gevoelige informatie over de
lijn stuurt.

Afhandelen van de fout aan de UI-kant
-------------------------------------

Dan is het uiteraard de bedoeling dat de geworpen FaultException door de
UI opgevangen wordt. Helaas is dat voor dit voorbeeld niet het geval
:-(. En dat is op zich niet zo erg, omdat de validatie van het formaat
van een communicatievorm ook aan de client-kant gebeurt, zodat deze
exception zich in praktijk niet zal voordoen.

Maar dit is hoe het zou kunnen gebeuren:

&lt;pre&gt;\
try\
{\
[ServiceHelper](ServiceHelper.md).CallService&lt;IGelieerdePersonenService&gt;(l
=&gt; l.CommunicatieVormToevoegen(gelieerdePersoonID, comminfo));

}\
catch (FaultException&lt;FoutNummerFault&gt; ex)\
{\
if (ex.FoutNummer == [FoutNummer](FoutNummer.md).ValidatieFout)\
{\
ModelState.AddModelError(\
"Model.NieuweCommVorm.Nummer",\
string.Format(\
Properties.Resources.FormatValidatieFout,\
communicatieType.Omschrijving,\
communicatieType.Voorbeeld));\
}\
else\
{\
// onverwachte exception gewoon opnieuw throwen;\
// dat debugt het gemakkelijkst.

throw;\
}\
}

&lt;/pre&gt;

Let op de truuk met ModelState, die ervoor zorgt dat het foutbericht op
deze manier in de view getoond kan worden:\
&lt;pre&gt;\
&lt;<span style="text-align:center;">Html.ValidationMessageFor(mdl =&gt;
mdl.NieuweCommVorm.Nummer)</span>&gt;\
&lt;/pre&gt;
