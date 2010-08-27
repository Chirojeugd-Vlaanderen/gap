<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<JaarOvergangAfdelingsJaarModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
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
	<!--<%
		List<CheckBoxListInfo> info
		   = (from pa in Model.Afdelingen
			  select new CheckBoxListInfo(
				 pa.AfdelingID.ToString()
				 , ""
				 , true)).ToList();

		int j = 0;
	%>-->
	<%using (Html.BeginForm("VerdelingMaken", "JaarOvergang"))
   { %>
	<table>
		<tr>
			<th>Afdeling</th>
			<th>Officiële afdeling</th>
			<th>Vanaf geboortejaar </th>
			<th>Tot geboortejaar </th>
			<th>Geslacht </th>
			<!--<th><%=Html.CheckBox("checkall", true) %></th> 
				TODO dit zou niet logisch zijn: je wilt geen afdeling uitsluiten waar je nu mensen insteekt, je wilt
				een afdeling die je al had niet opnieuw verdelen, bvb akabe
			-->
		</tr>
		<% foreach (var ai in Model.Afdelingen)
	 { %>
		<tr>
			<!--<td>
				<%=Html.CheckBoxList("GekozenAfdelingsIDs", info[j])%><% j++; %>
			</td>-->
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
	<input id="volgende" type="submit" value="Verdeling bewaren en huidige leden herinschrijven" />
	<%} %>
	<p>
		Ter informatie de &lsquo;standaardafdelingen&rsquo; voor dit werkjaar:
	</p>
	<table>
		<!--TODO exentsion method die gegeven een werkjaar, het standaard geboortejaar berekend. Nu is het niet correct. -->
		<%  foreach (var oa in Model.OfficieleAfdelingen.OrderBy(ofaf => ofaf.LeefTijdTot))
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
		<%=Html.ActionLink("Meer weten over afdelingen die een speciaal geval zijn?", "ViewTonen", new { controller = "Handleiding", helpBestand = "SpecialeAfdelingen" })%></p>
</asp:Content>
