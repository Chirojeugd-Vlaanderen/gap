<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Handleiding: Verjaardagslijsten in Excel
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Een verjaardagslijst maken in Excel</h2>
    <p>
        De ledenlijst kun je sorteren op verjaardag - ook als je al filters toepaste, zoals
        'Enkel afdeling X'. Kies daarvoor bij 'speciale lijst' de optie 'Verjaardagslijst'.
        Wanneer je die downloadt, zijn de gegevens automatisch al juist gesorteerd.
    </p>
    <p>
        Voor de personenlijst is die optie niet voorzien. Wil je een verjaardagslijst van
        je kookploeg, dan zul je die dus zelf moeten maken. Download de gegevens die je
        nodig hebt en volg de stappen hieronder om ze zelf juist te sorteren. Sorteren op
        geboortedatum gaat namelijk wel, maar dan staan de mensen in volgorde van leeftijd,
        niet in volgorde van verjaardag. Om dat laatste te kunnen, moet je twee kolommen
        toevoegen: één voor de dag en één voor de maand. Met formules kunnen we die uit
        de geboortedatum halen. Als we dan eerst op maand en dan op dag sorteren, heb je
        een proper overzicht.</p>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Download de ledenlijst en open ze.</li>
        <li>Voeg twee kolommen toe, bijvoorbeeld vlak achter de geboortedatum. Je kunt ook lege
            kolommen achteraan gebruiken. Zet in de eerste het titeltje 'Dag' en in de tweede
            'Maand'.</li>
        <li>Selecteer de kolommen en roep de celeigenschappen op. De waardenotatie moet op 'standaard'
            ('general') of 'getal' ('number') staan - niet op datum. De celeigenschappen oproepen,
            doe je door met je rechtermuisknop te klikken op één van de geselecteerde cellen
            en dan in het menu 'Celeigenschappen' ('Format Cells') te kiezen. Zorg dat de hele
            kolom geselecteerd is, anders geldt je aanpassing alleen voor de cel waar je op
            klikte.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/vjd_celeigenschappen_oproepen.png") %>"
        alt="De celeigenschappen bekijken" />
    <br />
    <img src="<%=ResolveUrl("~/Content/Screenshots/vjd_standaardcelopmaak.png") %>" alt="De waardenotatie instellen op Standaard" />
    <ul>
        <li>Vul in de eerste rij onder de titeltjes de formules in. Wat tussen haakjes staat,
            is een verwijzing naar de cel met de geboortedatum: in dit geval kolom I, rij 2.
            Terwijl je typt, krijg je normaal gezien een kadertje rond de cel die je nodig hebt.</li></ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/vjd_dagformule.png") %>" alt="De formule om de dag uit de geboortedatum te halen" /><br />
    <img src="<%=ResolveUrl("~/Content/Screenshots/vjd_maandformule.png") %>" alt="De formule om de maand uit de geboortedatum te halen" />
    <ul>
        <li>Als je Excel in het Engels is, moet je ook je formules in het Engels typen: 'day'
            in plaats van 'dag' en 'month' in plaats van 'maand'.</li>
        <li>Diezelfde formules moeten in alle volgende rijen komen, maar met de juiste verwijzing
            naar de geboortedatum. Dat kan via knippen-en-plakken. Selecteer de cel met de formule
            en druk op Ctrl-C. Selecteer daarna alle cellen waar de formule in moet komen en
            druk op Ctrl-V. Excel past automatisch de verwijzingen aan, zodat ze overal juist
            staan.</li>
        <li>Om te sorteren, selecteer je alle gegevens. Klik daarna op Gegevens &gt; Sorteren.
            Vink daar zeker aan dat de tabel 'kopteksten' (Excel 2007) of een 'veldnamenrij'
            (Excel 2003) heeft. Laat eerst op maand sorteren en daarna op dag. Dan krijg je
            iedereen in volgorde van verjaardag. In Excel 2007 moet je op de knop 'Niveau toevoegen'
            klikken om op meer dan één kolom te selecteren, in Excel 2003 kun je direct de twee
            kolommen instellen.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/vjd_sorteren.png") %>" alt="In Excel 2007 sorteren op maand en op dag" /><br />
    <img src="<%=ResolveUrl("~/Content/Screenshots/vjd_sorteren2003.png") %>" alt="In Excel 2003 sorteren op maand en op dag" />
</asp:Content>
