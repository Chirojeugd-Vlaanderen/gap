<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cg2.Orm.GelieerdePersoon>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Details: <%=Model.Persoon.VolledigeNaam %></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Details: <%=Model.Persoon.VolledigeNaam %></h2>
<%=Html.ActionLink("Bewerken", "Edit", new { Controller = "Personen", id = Model.ID }) %>

<%Html.RenderPartial("PersoonsDetailsControl"); %>

</asp:Content>
