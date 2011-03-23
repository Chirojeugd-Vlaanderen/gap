<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<JaarOvergangAfdelingsJaarModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" />
	<script type="text/javascript">
		$(document).ready(function() {
			$("#checkall").click(function() {
				$("INPUT[@name=GekozenAfdelingsIDs][type='checkbox']").attr('checked', $("#checkall").is(':checked'));
			});
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<%=Html.ActionLink("Terug naar stap 1", "AfdelingenMaken", new { Controller = "JaarOvergang" }) %>
	<br />
	<br />

	<%using (Html.BeginForm("VerdelingMaken", "JaarOvergang"))
   { %>
	<table>
		<tr>
			<th>Afdeling</th>
			<th>Officiële afdeling</th>
			<th>Vanaf geboortejaar </th>
			<th>Tot geboortejaar </th>
			<th>Geslacht </th>

		</tr>
		<% foreach (var ai in Model.Afdelingen)
	 { %>
		<tr>
			<td>
				<%=Html.LabelFieldList("AfdelingsIDs", new TextFieldListInfo(ai.AfdelingID.ToString(), ai.AfdelingNaam))%>
			</td>
			<td>
				<%=Html.OffAfdelingsDropDownList("OfficieleAfdelingsIDs", Model.OfficieleAfdelingen, ai.OfficieleAfdelingNaam)%>
			</td>
			<td>
				<%=Html.TextFieldList("VanLijst", new TextFieldListInfo(ai.GeboorteJaarVan==0?"":ai.GeboorteJaarVan.ToString(), ""))%>
			</td>
			<td>
				<%=Html.TextFieldList("TotLijst", new TextFieldListInfo(ai.GeboorteJaarTot == 0 ? "" : ai.GeboorteJaarTot.ToString(), ""))%>
			</td>
			<td>
				<%=Html.GeslachtsDropDownList("GeslLijst", ai.Geslacht)%>
			</td>
		</tr>
		<% } %>
	</table>
	<br />
	<br />
	<input id="volgende" type="submit" value="Verdeling bewaren en huidige leden herinschrijven" /> <strong>Dit kan een paar minuutjes duren!</strong>
	<%} %>
    <p>De leden en de leiding van vorig jaar wordt automatisch opnieuw ingeschreven. Ze krijgen daarbij opnieuw een instapperiode
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is die instapperiode?" } ) %>. Je hebt dus drie weken (of tot 15 oktober) de tijd om de nodige mensen uit te schrijven, zodat je geen factuur krijgt voor hun aansluiting. Achteraf kun je nog inschrijven wie je wilt, het hele jaar door.</p>
	<p>
		Ter informatie de &lsquo;standaardafdelingen&rsquo; voor dit werkjaar:
	</p>
	<table>
		<!--TODO exentsion method die gegeven een werkjaar, het standaardgeboortejaar berekend. Nu is het niet correct. -->
		<%  foreach (var oa in Model.OfficieleAfdelingen.Where(ofaf => ofaf.ID != (int)NationaleAfdeling.Speciaal).OrderBy(ofaf => ofaf.LeefTijdTot))
	  {%>
		<tr>
			<td>
				<%=oa.Naam %>
			</td>
			<td>
				<%=oa.StandaardGeboorteJaarVan(Model.NieuwWerkjaar) %>-<%=oa.StandaardGeboorteJaarTot(Model.NieuwWerkjaar)%>
			</td>
		</tr>
		<%}%>
	</table>
	<p>
		<%=Html.ActionLink("Meer weten over afdelingen die een speciaal geval zijn?", "ViewTonen", new { Controller = "Handleiding", helpBestand = "SpecialeAfdelingen" })%></p>
</asp:Content>
