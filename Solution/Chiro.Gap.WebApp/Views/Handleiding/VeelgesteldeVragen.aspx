<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master" Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
Handleiding: Veelgestelde vragen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
<h2>Veelgestelde vragen</h2>
<p>De website is nog niet gelanceerd, dus zijn er nog geen vragen gesteld. :)</p>
</asp:Content>