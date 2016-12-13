PossibleMultipleEnumeration
===========================

Wanneer je met `IEnumerable` werkt, zal Resharper je af en toe
confronteren met de waarschuwing 'Possible multiple enumeration of
IEnumerable'.

Van een `IEnumerable` weet je niks, behalve dat je de elementen één voor
een kunt overlopen.

-   Om te tellen hoeveel elementen een `IEnumerable` bevat, wordt het
    eerste genomen, en telkens het volgende op gevraagd, tot je aan het
    laatste bent.
-   Als je bijvoorbeeld het vijfde element nodig hebt, dan wordt er
    achterliggend begonnen met het eerste, en 4 keer het volgende
    opgevraagd
-   ...

Afhankelijk van wat er achter die `IEnumerable` zit, zijn dat dure
operaties. Resharper raadt dan ook af om een `IEnumerable` meerdere
keren af te lopen.

Hoe vermijden we die foutmelding?
---------------------------------

### `Any()`

Als je wilt weten of een `IEnumerable x` al dan niet leeg is, gebruik
dan `if (x.Any())` in plaats van `if (x.Count()==0)`. `Count()` zal
namelijk alle elementen aflopen, om te bepalen hoeveel het er zijn,
terwijl je die informatie niet nodig hebt. `x.Any()` werkt veel sneller,
want is equivalent met `x.FirstOrDefault() != null`.

### Tel slechts één keer

Als je het aantal elementen van een `IEnumerable` meerdere keren nodig
hebt, tel ze dan één maal, en bewaar dat aantal in een variabele. Die
variabele kun je zo veel gebruiken als je wilt.

### Gebruik `IList` i.p.v. `IEnumerable`

Als je niet anders kunt dan de `IEnumerable` meerdere keren af te lopen,
gebruik dan geen `IEnumerable`, maar meteen een `IList` als method
parameter.

Wat doen we liever niet?
------------------------

Wat ik persoonlijk niet zo graag zie, is het volgende\
&lt;pre&gt;\
public void MijnMethod(IEnumerable&lt;Persoon&gt; personen)\
{\
var personenArray = personen.ToArray();

// werk nu verder met personenArray i.p.v. met personen\
}\
&lt;/pre&gt;

Hier hou ik niet van, omdat ik dat verwarrend vind: twee lijsten met
personen, die dezelfde personen bevatten, maar toch niet dezelfde
lijsten zijn. Ik zou dan liever hebben dat dit als volgt gerefactord
wordt:

&lt;pre&gt;\
public void MijnMethod(IList&lt;Persoon&gt; personen)\
{\
// lijsten zijn ook enumerable. maar met als voordeel dat het\
// niets kost om element x op te vragen.\
}\
&lt;/pre&gt;

Als je dan `MijnMethod` wilt gebruiken, dan heb je hier en daar
waarschijnlijk wel een `.ToList()` nodig. Zoals in:

&lt;pre&gt;\
MijnMethod(personenCollectie.ToList());\
&lt;/pre&gt;

Wat op zich nog niet zo slecht is - denk ik - omdat je dan op dat moment
duidelijk ziet dat je `IEnumerable` sowieso afgelopen wordt.
