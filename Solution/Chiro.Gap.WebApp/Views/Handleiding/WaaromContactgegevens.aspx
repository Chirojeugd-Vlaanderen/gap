<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Waarom moeten contactgegevens volledig ingevuld zijn?</h2>
    <p>
        De contactpersoon van je groep moeten we kennen om post te kunnen sturen, de
        financieel verantwoordelijke om facturen op te sturen. Uiteraard moeten hun
        contactgegevens daarom zo snel mogelijk en juist ingevuld zijn.</p>
    <p>
        Van de leiding hebben we het mailadres nodig voor de Snelleberichtenlijsten.
        Zo kunnen we jullie snel op de hoogte brengen van dringend nieuws. Gewest- en
        verbondsmedewerkers kunnen dat ook opvragen om makkelijker contact op te kunnen
        nemen. Postadressen hebben we bijvoorbeeld nodig om Dubbelpunt op te sturen
        naar de mensen met een abonnement, en om de afdelingsspelen bij de juiste mensen
        te kunnen bezorgen.</p>
    <p>
        Van keti's en aspi's hebben we het postadres nodig om Kramp op te sturen.</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Handleiding: Waarom contactgegevens vervolledigen?
</asp:Content>
