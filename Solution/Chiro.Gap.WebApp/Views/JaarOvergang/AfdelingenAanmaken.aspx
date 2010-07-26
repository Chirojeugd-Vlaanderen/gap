<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.JaarOvergangAfdelingsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
	$(document).ready(function() {
		$("#checkall").click(function() {
			$("INPUT[@name=GekozenAfdelingsIDs][type='checkbox']").attr('checked', $("#checkall").is(':checked'));
		});
	});
</script>

<h2>Afdelingen aanmaken</h2>

<%
    List<CheckBoxListInfo> info
	   = (from pa in Model.Afdelingen
          select new CheckBoxListInfo(
             pa.ID.ToString()
             , ""
             , false)).ToList();

    int j = 0;
%>

<%using (Html.BeginForm("AfdelingenGemaakt", "JaarOvergang"))
  { %>

<table>
<tr><th><%=Html.CheckBox("checkall") %></th><th>Afdeling</th><th>Afkorting</th><th>Officiële afdeling</th></tr>
<% foreach (var ai in Model.Afdelingen)
   { %>
    <tr>
		<td><%=Html.CheckBoxList("GekozenAfdelingsIDs", info[j])%><% j++; %></td>
        <td><%=ai.Naam %></td>
        <td><%=ai.Afkorting %></td>
        <td><%=ai.OfficieleAfdelingNaam %></td>
        <td><%=Html.ActionLink("Afdeling aanpassen", "AfdelingAanpassen", new {Controller = "JaarOvergang", id=ai.ID }) %></td>
    </tr>
<% } %>
</table>

<%=Html.ActionLink("Afdeling aanmaken", "AfdelingMaken", new {Controller = "JaarOvergang" }) %>

<input id="volgende" type="submit" value="Volgende"/>
<%} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
