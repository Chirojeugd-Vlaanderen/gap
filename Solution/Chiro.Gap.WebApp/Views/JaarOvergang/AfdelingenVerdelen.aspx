<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.JaarOvergangAfdelingsJaarModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<h2>AfdelingenVerdelen</h2>

<!-- TODO Kan dit met dezelfde functionaliteit als de browserknop vorige? (nu zullen geselecteerde afdelingen dat niet meer zijn! -->
<%=Html.ActionLink("Vorige", "AfdelingenMaken", new { Controller = "JaarOvergang" }) %>

<%using (Html.BeginForm("VerdelingMaken", "JaarOvergang"))
  { %>

<table>
<tr><th>Afdeling</th><th>Officiële afdeling</th><td>Leeftijd van</td><td>Leeftijd tot</td><td>Geslacht</td></tr>
<% foreach (var ai in Model.Afdelingen)
   { %>
    <tr>
        <td><%=Html.LabelFieldList("AfdelingsIDs", new TextFieldListInfo(ai.AfdelingID.ToString(), ai.AfdelingNaam))%></td>
        <td><%=Html.OffAfdelingsDropDownList("OfficieleAfdelingsIDs", Model.OfficieleAfdelingen, ai.OfficieleAfdelingNaam)%></td>
		<td><%=Html.TextFieldList("VanLijst", new TextFieldListInfo(ai.GeboorteJaarVan==0?"":ai.GeboorteJaarVan.ToString(), ""))%></td>
		<td><%=Html.TextFieldList("TotLijst", new TextFieldListInfo(ai.GeboorteJaarTot == 0 ? "" : ai.GeboorteJaarTot.ToString(), ""))%></td>
		<td><%=Html.GeslachtsDropDownList("GeslLijst", ai.Geslacht)%></td>
    </tr>
<% } %>
</table>

<input id="volgende" type="submit" value="Verdeling bewaren en vorige leden herinschrijven."/>
<%} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
