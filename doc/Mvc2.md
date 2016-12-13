MVC 2
=====

Sinds eind 2009 gebruiken we de features van MVC2 in ons project.

Typesafe HTML-helpers
---------------------

Zie bijv:
\[source:trunk/Solution/Chiro.Gap.WebApp/Views/Personen/AdresBewerken.aspx\#L116\].

Door constructies te gebruiken zoals\
&lt;pre&gt;\
&lt;<span style="text-align:center;">Html.EditorFor(mdl =&gt;
mdl.Adres.Gemeente)</span>&gt;\
&lt;/pre&gt;\
en\
&lt;pre&gt;\
&lt;<span style="text-align:center;">Html.DropDownListFor(mdl =&gt;
mdl.AdresType, new [SelectList](SelectList.md)(values, "value",
"text"))</span>&gt;\
&lt;/pre&gt;\
worden tikfouten in namen van members van het model vermeden.

Een interessante helper is de deze:\
&lt;pre&gt;\
&lt;<span style="text-align:center;">Html.LabelFor(mdl =&gt;
mdl.Adres.Gemeente)</span>&gt;\
&lt;/pre&gt;\
Deze constructie zet een labeltje voor het veld
AdresModel.Adres.Gemeente op de webpagina. In dit geval is dat
gemakkelijk en standaard, namelijk gewoon de naam van de property,
Gemeente.

In sommige gevallen is het niet de bedoeling dat de propertynaam als
label gebruikt wordt. Neem bijvoorbeeld
'![](AdresType'.  De hoofdletter 'T' is een gevolg van onze coding standard, maar de gebruiker heeft daar geen boodschap aan.  Door AdresModel.)AdresType
te 'decoreren' met het attribuut \[!DisplayNaam\], kunnen we ervoor
zorgen dat er een alternatief label wordt gebruikt. Zie
\[source:trunk/Solution/Chiro.Gap.WebApp/Models/AdresModel.cs\#L28\].

&lt;pre&gt;\
\[DisplayName("Adrestype")\]\
public [AdresTypeEnum](AdresTypeEnum.md) AdresType { get; set; }\
&lt;/pre&gt;

Modelvalidatie in de UI
-----------------------

MVC2 bevat een eenvoudig inputvalidatiemechanisme. Zie bijv.
\[source:trunk/Solution/Chiro.Gap.WebApp/Controllers/PersonenController.cs\#L200\].
De controller actie die de gegevens van een bestaande persoon moet
wijzigen, begint op deze manier:

&lt;pre&gt;\
if (!ModelState.IsValid)\
{\
// gebruiker opnieuw laten proberen indien het model\
// ongeldig is.

return View("EditGegevens", model);\
}

// in het andere geval worden de gewijzigde gegevens bewaard.\
&lt;/pre&gt;

Als het model dat de gebruiker postte niet geldig is, wordt opnieuw de
view '!EditGegevens' getoond, met als model de geposte gegevens. In
\[source:trunk/Solution/Chiro.Gap.WebApp/Views/Personen/EditGegevens.aspx\#L11\]
zijn nu bijvoorbeeld deze lijnen relevant:\
&lt;pre&gt;\
&lt;<span style="text-align:center;">Html.ValidationSummary("Er zijn
enkele opmerkingen:")</span>&gt;\
&lt;/pre&gt;\
dewelke ervoor zorgt dat er - indien het model niet geldig is - een
overzichtje van alle problemen in het model getoond wordt. Verder vind
je ook dit soort constructie:\
&lt;pre&gt;\
&lt;<span
style="text-align:center;">Html.ValidationMessageFor(s=&gt;s.HuidigePersoon.Persoon.VoorNaam)</span>&gt;\
&lt;/pre&gt;\
Dit zorgt ervoor dat als er een probleem is met de ingegeven voornaam,
het foutbericht m.b.t. de voornaam afgebeeld wordt.

Hoe weet MVC nu wanneer het model geldig is, en wanneer niet? Hiervoor
kun je de attributen \[Verplicht\],
\[![](StringLengte] en [)StringMinimumLengte\] gebruiken. Dit zijn eigen
gedefinieerde attributen (Zie
\[source:trunk/Solution/Chiro.Cdf.Validation\]), omdat we op die manier
gebruik kunnen maken van eigen (en dus ook Nederlandstalige)
foutmeldingen. De foutmeldingen zijn in ons geval gedefinieerd in
\[source:trunk/Solution/Chiro.Cdf.Validation/Properties/Resources.nl.resx\#L120\].

Validatie- en 'displaynameattributen' op gegenereerde klasses
-------------------------------------------------------------

Een probleem met deze attributen, is dat we ze niet kunnen zetten op de
klasses die het entity framework automatisch genereert. Tot nader order
is hiervoor niets voorzien in de EF-designer. Maar we kunnen hier wel
rond werken.

Het entity framework genereert partial classes. We kunnen deze klasses
zelf uitbreiden, en door een '!MetaDatatype' te definiëren kunnen we
alsnog attributen toepassen op reeds gedefinieerde property's. Een
voorbeeld maakt wellicht veel duidelijk:
\[source:trunk/Solution/Chiro.Gap.Orm/Persoon.cs\#L27\].

&lt;pre&gt;\
\[MetadataType(typeof(Persoon\_Validatie))\]\
public partial class Persoon : IEfBasisEntiteit\
{\
/// &lt;summary&gt;\
/// Nested class die toelaat om validatie properties op te zetten, en
die gereferenced wordt door het [MetadataType](MetadataType.md) attribute\
/// Dit kan niet op de echte class, want die wordt gegenereerd door de
EF Designer\
/// &lt;/summary&gt;\
public class Persoon\_Validatie\
{\
\[Verplicht(), [StringLengte](StringLengte.md)(160),
[StringMinimumLengte](StringMinimumLengte.md)(2)\]\
\[DisplayName("Familienaam")\]\
public string Naam { get; set; }

\[Verplicht()\]\
\[DisplayName("Voornaam")\]\
\[StringLengte(60), [StringMinimumLengte](StringMinimumLengte.md)(2)\]\
public string [VoorNaam](VoorNaam.md) { get; set; }

\[Verplicht()\]\
public Chiro.Gap.Orm.GeslachtsType Geslacht { set; get; }

\[DisplayName("AD-nummer")\]\
public Nullable&lt;int&gt; [AdNummer](AdNummer.md) { set; get; }

\[DataType(DataType.Date)\]\
\[DisplayName("Geboortedatum")\]\
public [DateTime](DateTime.md)? [GeboorteDatum](GeboorteDatum.md) { get; set; }\
}\
// ...\
}\
&lt;/pre&gt;

Bedenkingen bij het attribuut \[!DisplayName\]
----------------------------------------------

Zie \#223.
