<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GavTakenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<p>
		Hier kun je zien welke administratieve taken er nog op jou liggen te wachten.
		Gaat het over functies die niet toegekend zijn, dan vind je meer info in 
		<%=Html.ActionLink("de handleiding", "ViewTonen", new { controller = "Handleiding", helpBestand = "IemandsFunctiesAanpassen" })%>.
	</p>
	<% if (Model.Mededelingen != null && Model.Mededelingen.Any())
	{
	%>
	<ul>
		<%
			foreach (var m in Model.Mededelingen)
			{
		%>
		<li><strong>
			<%=m.Type %>:</strong>
			<%=m.Info %></li>
		<%
			}
		%>
	</ul>
	<%
		}
	%>
</asp:Content>
