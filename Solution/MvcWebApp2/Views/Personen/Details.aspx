<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Details: <%=Model.HuidigePersoon.Persoon.VolledigeNaam%></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Details: <%=Model.HuidigePersoon.Persoon.VolledigeNaam%></h2>
<%=Html.ActionLink("Bewerken", "Edit", new { Controller = "Personen", id = Model.HuidigePersoon.ID }) %>

<%Html.RenderPartial("PersoonsDetailsControl"); %>

</asp:Content>
