<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GavTakenModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<p>
		Hier kun je zien welke administratieve taken er nog op jou liggen te wachten.</p>
	<% if (Model.Mededelingen != null && Model.Mededelingen.Count() > 0)
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
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
