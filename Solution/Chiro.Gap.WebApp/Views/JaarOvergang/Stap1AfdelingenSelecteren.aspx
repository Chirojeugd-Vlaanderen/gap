<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<JaarOvergangAfdelingsModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>
<script type="text/javascript">
	$(document).ready(function() {
		$("#checkall").click(function() {
			$("INPUT[@name=GekozenAfdelingsIDs][type='checkbox']").attr('checked', $("#checkall").is(':checked'));
		});
	});
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<%
		List<CheckBoxListInfo> info
		   = (from pa in Model.Afdelingen
			  select new CheckBoxListInfo(
				 pa.ID.ToString()
				 , ""
				 , false)).ToList();

		int j = 0;
	%>
	Selecteer de afdelingen die je groep volgend werkjaar zult gebruiken. <br/><br/>
	Als er afdelingen van naam veranderen of als er nieuwe bijkomen, kun je dat hier aanpassen. <br/><br/>
	
	<%using (Html.BeginForm("Stap1AfdelingenSelecteren", "JaarOvergang"))
   { %>
	<table>
		<tr>
			<th><%=Html.CheckBox("checkall") %></th>
			<th>Afdeling</th>
			<th>Afkorting</th>
		</tr>
		<% foreach (var ai in Model.Afdelingen)
	 { %>
		<tr>
			<td>
				<%=Html.CheckBoxList("GekozenAfdelingsIDs", info[j])%><% j++; %>
			</td>
			<td>
				<%=ai.Naam %>
			</td>
			<td>
				<%=ai.Afkorting %>
			</td>
			<td>
				<%=Html.ActionLink("Afdeling aanpassen", "Bewerken", new { Controller = "JaarOvergang", afdelingID = ai.ID })%>
			</td>
		</tr>
		<% } %>
	</table>
	<br />
	<%=Html.ActionLink("Afdeling aanmaken", "NieuweAfdelingMaken", new { Controller = "JaarOvergang" })%> <br /><br />
	<input id="volgende" type="submit" value="Naar stap 2" />
	<%} %>
</asp:Content>
