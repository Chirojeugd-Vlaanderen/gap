<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Handleiding: Uitstappen/bivak
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Medewerk(st)ers inschrijven voor je uitstap/bivak</h2>
    <p>
        Eerst moet je zorgen dat je uitstap
        <%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Uitstap", new { helpBestand = "Trefwoorden" }, new { title = "Wat wordt hier beschouwd als een uitstap?" } ) %>
        of bivak
        <%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Bivak", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een bivak?" } ) %>
        geregistreerd is. Lees eventueel eerst
        <%=Html.ActionLink("hoe je dat doet", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" })%>.</p>
    <p>
        Medewerkers zijn mensen die niet als lid of als leiding meegaan. Zij nemen dus niet
        deel aan het groepsleven, maar voeren andere taken uit die noodzakelijk zijn voor
        het verloop van je kamp. De kookploeg is daar het duidelijkste voorbeeld van.</p>
    <p>
        <b>Opgelet</b>: logistiek medewerkers op bivak zijn typisch mensen die niet aangesloten
        zijn bij je groep. <b>Zij zijn dus nog niet verzekerd!</b> Voor hen kun je wel een
        verzekering voor een beperkte periode afsluiten. Kijk voor alle info daarover op
        <a href="http://www.chiro.be/bivak">www.chiro.be/bivak</a> of <a href="http://www.chiro.be/verzekeringen">
            www.chiro.be/verzekeringen</a>.</p>
    <p>
        De kookploeg op een afdelingsweekend bestaat geregeld uit leiding van andere afdelingen.
        Je kunt hen dan wel inschrijven als logistiek medewerk(st)ers, maar voor hen is
        de verzekering uiteraard wél al in orde.</p>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Ga naar het tabblad 'Iedereen'.</li>
        <li>Vink aan wie je wilt inschrijven, en kies in het selectielijstje onder 'Acties'
            voor 'Inschrijven voor uitstap/bivak'.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Deelnemers_inschrijven.png") %>" alt="Deelnemers selecteren en inschrijven" />
    <ul>
        <li>Je krijgt nu een nieuwe pagina te zien. Daarop staan al de mensen die je aanvinkte,
            en een selectielijstje met je uitstappen en bivakken.</li></ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Inschrijving_uitstap_bevestigen.png") %>"
        alt="De inschrijving bevestigen" />
    <ul>
        <li>Kies de juiste uitstap of het juiste kamp, vink aan dat het om logistiek medewerk(st)ers
            gaat en klik op Bewaren. Je keert dan terug naar het tabblad 'Iedereen', waar je
            de melding ziet dat die mensen ingeschreven zijn.</li>
        <li>Ga naar het tabblad 'Uitstappen/bivak' en klik op de juiste uitstap om de deelnemerslijst
            te bekijken.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Fiche_bivakdetails_met_leden.png") %>"
        alt="Detailsfiche van je uitstap, met deelnemerslijst" />
</asp:Content>
