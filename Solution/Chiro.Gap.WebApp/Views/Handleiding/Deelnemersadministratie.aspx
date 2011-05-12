<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Handleiding: Uitstappen/bivak
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Deelnemersadministratie voor uitstappen/bivakken</h2>
    <p>
        Van de mensen die meegaan, kun je bijhouden of ze al betaald hebben en of hun medische
        fiche al binnen is. Je kunt ook een opmerking toevoegen.</p>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Ga naar het tabblad 'Uitstappen/bivak' en klik op de juiste uitstap om de deelnemerslijst
            te bekijken.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Fiche_bivakdetails_met_leden.png") %>"
        alt="Detailsfiche van je uitstap, met deelnemerslijst" />
    <ul>
        <li>Klik in de deelnemerslijst op een naam om naar de deelnemersfiche te gaan. Daar
            kun je aanpassen of het inschrijvingsgeld betaald is en of de medische fiche binnen
            is.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Deelnemersinfo_bewerken.png") %>"
        alt="Deelnemersinfo bewerken" />
</asp:Content>
