De architectuur van GAP in grote lijnen
=======================================

Hieronder een oplijsting van de belangrijkste projecten uit de
GAP-solution (Voor een overzicht van alle projecten, zie
[NamespacesEnProjecten](NamespacesEnProjecten.md)).

Backend
-------

Zie [Backend](Backend.md).

Frontend
========

Chiro.Gap.WebApp
----------------

Een Asp.NET MVC2 frontend. De communicatie met de backend gebeurt via
service calls.

In het kort werkt het zo:

-   Er wordt een http-request verstuurd dat er ongeveer zo
    uitziet: gap-url/GroepID/ControllerNaam/ActieNaam/ID?parameter1=waarde1&amp;parameter2=waarde2...
-   De method ActieNaam van de klasse ControllerNaam
    (in Chiro.Gap.WebApp/Controllers/ControllerNaam.cs) wordt uitgevoerd
    met de gegeven parameters: ActieNaam(GroepID, ID,
    waarde1, waarde2,...)
    -   Mogelijkheid 1: De method redirect naar een andere actie.

We zijn stilaan bezig om de app te migreren naar HTML5. Maar we zitten
nog in een experimentele fase. Het gebruik van Javascript moet nog wat
gestroomlijnd worden.
