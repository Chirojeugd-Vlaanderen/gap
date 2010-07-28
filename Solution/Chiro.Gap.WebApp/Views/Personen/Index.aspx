<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.PersoonInfoModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>

	<script type="text/javascript">
		$(document).ready(function() {
			$('#kiesCategorie').hide();
			$("#GekozenCategorieID").change(function() {
				$('#kiesCategorie').click();
			});
		});
	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<%using (Html.BeginForm("List", "Personen"))
   { %>
	<div id="acties">
		<h1>
			Acties</h1>
		<ul>
			<li>
				<%= Html.ActionLink("Nieuwe persoon", "Nieuw") %></li>
			<li>
				<%= Html.ActionLink("Lijst downloaden", "Download", new { id = Model.GekozenCategorieID })%></li>
		</ul>
		<h1>
			Filteren</h1>
		<ul>
			<li>
				<select id="GekozenCategorieID" name="GekozenCategorieID">
					<option value="">Op categorie</option>
					<% foreach (var s in Model.GroepsCategorieen)
		{%>
					<option value="<%=s.ID%>">
						<%=s.Naam%></option>
					<%}%>
				</select>
				<input id="kiesCategorie" type="submit" value="Uitvoeren" />
				<%=Html.HiddenFor(e => e.Sortering) %>
			</li>
		</ul>
	</div>
<%} %>
	<% Html.RenderPartial("PersonenLijstControl", Model); %>
</asp:Content>
