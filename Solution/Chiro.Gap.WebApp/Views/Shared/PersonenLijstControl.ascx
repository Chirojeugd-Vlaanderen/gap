<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.min.js")%>"></script>
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
             , false)).ToList<CheckBoxListInfo>();

    int j = 0;
%>

<%using (Html.BeginForm("ToepassenOpSelectie", "Personen")){ %>
<select id="GekozenActie" name="GekozenActie">
<option value="0">Kies een actie ...</option>
<option value="1">Lid maken</option>
<option value="2">In (dezelfde) categorie stoppen</option>
</select>
<input id="kiesActie" type="submit" value="Uitvoeren"/>

<div class="pager">
Pagina: <%= Html.PagerLinks(ViewData.Model.HuidigePagina, ViewData.Model.AantalPaginas, i => Url.Action("List", new { page = i })) %>
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
        <% if (!p.IsLid)
		   { %>
			<% if (p.GeboorteDatum.HasValue && p.GeboorteDatum.Value.Year > DateTime.Today.Year - 21)
	  { %>
				<%=Html.ActionLink("Lid maken", "LidMaken", new { Controller = "Personen", id = p.GelieerdePersoonID })%>
			<% } %>
			<% if (p.GeboorteDatum.HasValue && p.GeboorteDatum.Value.Year < DateTime.Today.Year - 14)
	  { %>
				<%=Html.ActionLink("Leiding maken", "LeidingMaken", new { Controller = "Personen", id = p.GelieerdePersoonID })%>
			<% } %>
        <% }
		   else
		   { %>
		   Is lid
        <%} %>
    </td>
    <td><% foreach (Categorie c in p.CategorieLijst) 
           { %>
               <%=Html.ActionLink(Html.Encode(c.Code.ToString()), "List", new { Controller = "Personen", id = c.ID }, new { title = c.Naam.ToString() } )%>
        <% } %>
    </td>
</tr>
<% } %>

</table>
<%
  }
%>