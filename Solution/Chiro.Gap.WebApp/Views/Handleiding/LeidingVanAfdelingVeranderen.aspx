<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Handleiding: Leiding van afdeling veranderen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Leiding van afdeling veranderen</h2>
    <p>
        Het is belangrijk dat je bij de leiding de afdeling invult. Anders weet het
        nationaal secretariaat niet naar wie we de afdelingsspelen moeten opsturen:
        we willen die namelijk ineens bij de juiste afdeling laten terechtkomen.</p>
    <p>
        Heb je je leiding al ingeschreven maar ben je de afdelingen vergeten of heb
        je een foutje gemaakt? Ga naar het tabblad Ingeschreven. Vink daar alle leiding
        aan van dezelfde afdeling (voor wie dat nog niet ingevuld is). Kies onder 'Selectie'
        (boven of rechts van de tabel) voor 'Afdeling aanpassen'. Op een andere pagina
        kun je voor alle geselecteerde personen dan aanvinken bij welke afdeling ze
        staan.</p>
        <img src="<%=ResolveUrl("~/Content/Screenshots/Mensen_van_afdeling_veranderen.png.png") %>"
        alt="Mensen in een andere afdeling stoppen" />
    <p>
        Je wijzigingen worden normaal gezien binnen de 24 uur gesynchroniseerd met de
        gegevens van het nationaal secretariaat, daar moet je niets extra's voor doen.</p>
</asp:Content>
