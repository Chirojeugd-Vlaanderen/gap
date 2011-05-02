<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Uitstap/bivak toevoegen</h2>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Ga naar het tabblad 'Uitstappen/bivak'.</li>
        <li>Klik op 'Uitstap/bivak toevoegen'.</li>
        <li>Vul de nodige gegevens in en klik op de knop Bewaren.</li>
    </ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    Handleiding: Uitstap/bivak toevoegen
</asp:Content>
