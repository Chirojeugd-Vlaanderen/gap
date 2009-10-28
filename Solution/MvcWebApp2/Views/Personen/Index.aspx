<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PersoonInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="acties">
<li><%= Html.ActionLink("Nieuwe persoon", "Nieuw") %></li>
</ul>
<ul id="info">
<li>Totaal aantal personen: <%= Model.Totaal %></li>
</ul>
<% Html.RenderPartial("PersonenLijstControl"); %>

</asp:Content>
