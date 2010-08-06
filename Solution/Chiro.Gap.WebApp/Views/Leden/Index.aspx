<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" />
	<script type="text/javascript">
		$(document).ready(function() {
			$('#kiesAfdeling').hide();
			$("#GekozenAfdeling").change(function() {
				$('#kiesAfdeling').click();
			});
			$('#kiesFunctie').hide();
			$("#GekozenFunctie").change(function() {
				$('#kiesFunctie').click();
			});
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="acties">
		<h1>
			Filteren</h1>
		<ul>
			<li>
				<%=Html.ActionLink("Alle leden bekijken", "Lijst", new { groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, sortering=Model.GekozenSortering, lijst=LijstEnum.Alles })%>
			</li>
			<li>
				<%using (Html.BeginForm("AfdelingsLijst", "Leden"))
	  { %>
				<select id="GekozenAfdeling" name="GekozenAfdeling">
					<option value="">Op afdeling</option>
					<% foreach (var s in Model.AfdelingsInfoDictionary)
		{%>
					<option value="<%=s.Value.AfdelingID%>">
						<%=s.Value.AfdelingNaam%></option>
					<%}%>
				</select>
				<input id="kiesAfdeling" type="submit" value="Afdeling bekijken" />
				<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
				<%=Html.HiddenFor(s => s.GekozenSortering)%>
				<%} %>
			</li>
			<li>
				<%using (Html.BeginForm("FunctieLijst", "Leden"))
	  { %>
				<select id="GekozenFunctie" name="GekozenFunctie">
					<option value="">Op functie</option>
					<% foreach (var s in Model.FunctieInfoDictionary)
		{%>
					<option value="<%=s.Value.ID%>">
						<%=s.Value.Naam%></option>
					<%}%>
				</select>
				<input id="kiesFunctie" type="submit" value="Functie bekijken" />
				<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
				<%=Html.HiddenFor(s => s.GekozenSortering)%>
				<%} %>
			</li>
		</ul>
	</div>
	<% Html.RenderPartial("LedenLijstControl"); %>
</asp:Content>
