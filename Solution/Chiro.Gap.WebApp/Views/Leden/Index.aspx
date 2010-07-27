<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>
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
<ul id="acties">
<li>
	<%=Html.ActionLink("Alle leden bekijken", "Lijst", new { groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, sortering=Model.GekozenSortering, lijst = LijstEnum.Alles, ID = Model.GekozenID })%>
</li>
<li><%= Html.ActionLink("Lijst downloaden", "Download", new { id = Model.IDGetoondGroepsWerkJaar, afdelingID = Model.GekozenAfdeling, functieID = Model.GekozenFunctie })%></li>
<li>
	<%using (Html.BeginForm("AfdelingsLijst", "Leden")){ %>
		<select id="GekozenAfdeling" name="GekozenAfdeling">
		<option value="">Afdeling</option>
		<% foreach (var s in Model.AfdelingsInfoDictionary){%>
		<option value="<%=s.Value.AfdelingID%>"> <%=s.Value.AfdelingNaam%></option>
		<%}%>
		</select>
		<input id="kiesAfdeling" type="submit" value="Afdeling bekijken"/>
		<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
		<%=Html.HiddenFor(s => s.GekozenSortering)%>
	<%} %>
</li>
<li>
	<%using (Html.BeginForm("FunctieLijst", "Leden")){ %>
		<select id="GekozenFunctie" name="GekozenFunctie">
		<option value="">Functie</option>
		<% foreach (var s in Model.FunctieInfoDictionary){%>
		<option value="<%=s.Value.ID%>"> <%=s.Value.Naam%></option>
		<%}%>
		</select>
		<input id="kiesFunctie" type="submit" value="Functie bekijken"/>
		<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
		<%=Html.HiddenFor(s => s.GekozenSortering)%>
	<%} %>
</li>
</ul>

<% Html.RenderPartial("LedenLijstControl"); %>
</asp:Content>
