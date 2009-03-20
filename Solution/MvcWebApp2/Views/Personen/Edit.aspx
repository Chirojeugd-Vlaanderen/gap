<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cg2.Orm.GelieerdePersoon>" %>
<%@ Import Namespace="Cg2.Orm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Gegevens bewerken: <%=ViewData.Model.Persoon.VolledigeNaam %></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Gegevens bewerken: <%=ViewData.Model.Persoon.VolledigeNaam %></h2>

    <% Html.RenderPartial("PersoonsUpdateControl"); %>


    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

