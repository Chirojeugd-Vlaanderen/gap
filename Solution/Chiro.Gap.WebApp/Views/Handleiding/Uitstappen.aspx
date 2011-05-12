<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Handleiding: Uitstappen/bivak
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Uitstappen en bivakken</h2>
    <p>
        Je kunt de GAP-website gebruiken voor elke uitstap die je organiseert. Dat is voor
        je eigen gemak. Eén uitstap <em>moet</em> je in het systeem steken, en wel voor
        1 juni: het jaarlijkse bivak.</p>
    <p>
        We vragen je bivakgegevens in de eerste plaats om je bij noodgevallen te kunnen
        bereiken of efficiënt te kunnen ondersteunen - of het nu gaat om een ouder die dringend
        zijn of haar kind moet bereiken, een (natuur)ramp waarvan we je op de hoogte moeten
        brengen of een probleem waarvoor jij ons contacteert.
        <br />
        Daarnaast kunnen we met die gegevens het fenomeen bivak iets beter opvolgen en ondersteunen.</p>
    <h3>
        Wat kun je hier doen?</h3>
    <ul>
        <li>
            <%=Html.ActionLink("Een uitstap/bivak toevoegen", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapToevoegen" })%></li>
        <li>
            <%=Html.ActionLink("Een uitstap/bivak bewerken", "ViewTonen", new { controller = "Handleiding", helpBestand = "UitstapBewerken" })%></li>
        <li>
            <%=Html.ActionLink("Deelnemersadministratie", "ViewTonen", new { controller = "Handleiding", helpBestand = "Deelnemersadministratie" })%></li>
    </ul>
    <h3>Wat moet je op een andere pagina doen?</h3>
    <ul>
        <li>
            <%=Html.ActionLink("Deelnemers inschrijven", "ViewTonen", new { controller = "Handleiding", helpBestand = "DeelnemersInschrijven" })%></li>
            <li>
            <%=Html.ActionLink("Logistiek medewerkers (zoals koks) inschrijven", "ViewTonen", new { controller = "Handleiding", helpBestand = "MedewerkersInschrijven" })%></li>
    </ul>
</asp:Content>
