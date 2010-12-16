<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Controllers" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #713) %>
	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>
	<script type="text/javascript">
		$(document).ready(function() {
			$('#filter').hide();
			$("#AfdelingID").change(function() {
				$('#filter').click();
			});
			$("#FunctieID").change(function() {
				$('#filter').click();
			});
			$("#SpecialeLijst").change(function() {
				$('#filter').click();
			});		
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="actiesmini">
		<span><%= Html.ActionLink(
		      	"Lijst downloaden", 
		      	"Download", 
		      	new { id = Model.IDGetoondGroepsWerkJaar, afdelingID = Model.AfdelingID, functieID = Model.FunctieID, sortering = Model.GekozenSortering, ledenLijst = Model.SpecialeLijst }, 
		      	new { title = "Download de geselecteerde gegevens in een Excel-bestand" })%></span>
		<span>
				<%
				if ((Model.GroepsNiveau & (Niveau.Gewest|Niveau.Verbond)) == 0)
				{
					using (Html.BeginForm("Lijst", "Leden"))
					{
						var dummyAfdeling = new SelectListItem[] {new SelectListItem {Text = "Filter op afdeling", Value = "0"}};
						var dummyFunctie = new SelectListItem[] {new SelectListItem {Text = "Filter op functie", Value = "0"}};
						var dummySpeciaal = new SelectListItem[] {new SelectListItem {Text = "Speciale Lijst", Value = "0"}};%>
						
  						<%=Html.DropDownListFor(
							mdl => mdl.AfdelingID,
							dummyAfdeling.Union(
								Model.AfdelingsInfoDictionary.Select(
									src => new SelectListItem {Text = src.Value.AfdelingNaam, Value = src.Value.AfdelingID.ToString()})))%>

						<%=Html.DropDownListFor(
							mdl=>mdl.FunctieID, 
								dummyFunctie.Union(
							Model.FunctieInfoDictionary.Select(src => new SelectListItem {Text = src.Value.Naam, Value = src.Value.ID.ToString()}))) %>

						<%=Html.DropDownListFor(
							mdl=>mdl.SpecialeLijst, 
								dummySpeciaal.Union(
							Model.SpecialeLijsten.Select(src => new SelectListItem {Text = src.Value, Value = src.Key.ToString()}))) %>									

						<input id="filter" type="submit" value="Filter toepassen" />
						
						
						<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
						<%=Html.HiddenFor(s => s.GekozenSortering)%>
				<%
					}
				}%>
		</span>
	</div>
	
	<div class="pager">
	Pagina:
	<%= Html.WerkJaarLinks(
                ViewData.Model.IDGetoondGroepsWerkJaar, 
                ViewData.Model.WerkJaarInfos,
				wj => Url.Action("Lijst", new { Controller = "Leden", id = wj.ID, sortering = Model.GekozenSortering, afdelingID = Model.AfdelingID, functieID = Model.FunctieID, ledenLijst = Model.SpecialeLijst }))%>
	</div>
	
	<% Html.RenderPartial("LedenLijstControl"); %>
</asp:Content>
