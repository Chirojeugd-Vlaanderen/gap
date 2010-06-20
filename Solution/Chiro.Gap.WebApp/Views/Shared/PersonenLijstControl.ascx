<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.PersoonInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>

<script type="text/javascript">
    $(document).ready(function() {
        $("#checkall").click(function() {
            $("INPUT[@name=GekozenGelieerdePersoonIDs][type='checkbox']").attr('checked', $("#checkall").is(':checked'));
        });

        $('#kiesActie').hide();
        $("#GekozenActie").change(function() {
            $('#kiesActie').click();
        });
    });
</script>

<%
    List<CheckBoxListInfo> info
       = (from pa in Model.PersoonInfos
          select new CheckBoxListInfo(
             pa.GelieerdePersoonID.ToString()
             , ""
             , false)).ToList();

    int j = 0;
%>

<%using (Html.BeginForm("ToepassenOpSelectie", "Personen")){ %>

<div class="pager">
Pagina's <%= Html.PagerLinks(ViewData.Model.HuidigePagina, ViewData.Model.AantalPaginas, i => Url.Action("List", new { Controller="Personen", page = i })) %>
<br/>
Totaal aantal personen: <%= Model.Totaal %>  |  Maak een selectie en 

<select id="GekozenActie" name="GekozenActie">
<option value="0">kies een actie</option>
<option value="1">Lid maken</option>
<option value="2">Leiding maken</option>
<option value="3">In dezelfde categorie stoppen</option>
</select>
<input id="kiesActie" type="submit" value="Uitvoeren"/>
</div>

<table class="overzicht">
<tr>
<th><%=Html.CheckBox("checkall") %></th><th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th><th>Categorie&euml;n</th>
</tr>
<% foreach (PersoonDetail p in ViewData.Model.PersoonInfos) {  %>
<tr>
    <td><%=Html.CheckBoxList("GekozenGelieerdePersoonIDs", info[j]) %><% j++; %></td>
    <td><%=p.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", p); %></td>
    <td align="right"><%=p.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)p.GeboorteDatum).ToString("d") %></td>
    <td><%=p.Geslacht.ToString() %></td>
    <td>
        <% if(p.IsLid){ %>
			Is lid
        <%} if (p.IsLeiding){ %>
			Is Leiding
        <%} %>
        <% if (!p.IsLid && p.KanLidWorden) { %>
				<%=Html.ActionLink("Lid maken", "LidMaken", new { Controller = "Personen", gelieerdepersoonID = p.GelieerdePersoonID })%>
		<% } %>
		<% if (!p.IsLeiding && p.KanLeidingWorden){ %>
				<%=Html.ActionLink("Leiding maken", "LeidingMaken", new { Controller = "Personen", gelieerdepersoonID = p.GelieerdePersoonID })%>
		<% } %>
        <%=Html.ActionLink("Bro/sis maken", "Kloon", new { Controller = "Personen", gelieerdepersoonID = p.GelieerdePersoonID })%>
    </td>
    <td><% foreach (var c in p.CategorieLijst) 
           { %>
               <%=Html.ActionLink(Html.Encode(c.Code), "List", new { Controller = "Personen", id = c.ID }, new { title = c.Naam } )%>
        <% } %>
    </td>
</tr>
<% } %>

</table>
<%
  }
%>