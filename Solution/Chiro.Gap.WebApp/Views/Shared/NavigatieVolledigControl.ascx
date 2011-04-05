<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IMasterViewModel>" %>
	<ul>
		<li>
			<%= Html.ActionLink("Handleiding", "Index", "Handleiding") %></li>
		<li>
			<%= Html.ActionLink("Uitstappen/bivak", "Index", "Uitstappen") %></li>
		<li>
			<%= Html.ActionLink("Persoon toevoegen", "Nieuw", "Personen") %></li>
		<li>
			<%= Html.ActionLink("Groep", "Index", "Groep")%></li>
		<li>
			<%= Html.ActionLink("Iedereen", "Index", "Personen")%></li>
		<li>
			<%= Html.ActionLink("Ingeschreven", "Index", "Leden")%></li>
<% 
   if (Model.IsInOvergangsPeriode)
   {
%>
		<li><%=Html.ActionLink("Jaarovergang", "Index", "JaarOvergang")%></li>
<%
   }
%>
			
	</ul>