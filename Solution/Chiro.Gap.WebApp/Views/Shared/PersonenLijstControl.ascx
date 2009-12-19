<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<div class="pager">
Pagina: <%= Html.PagerLinks(ViewData.Model.PageHuidig, ViewData.Model.PageTotaal, i => Url.Action("List", new { page = i })) %>
</div>

<table>
<tr>
<th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th><th>Categorie&euml;n</th>
</tr>

<% foreach (PersoonInfo p in ViewData.Model.PersoonInfoLijst) {  %>
<tr>
    <td><%=p.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLinkControl", p); %></td>
    <td class="right"><%=p.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)p.GeboorteDatum).ToString("d") %></td>
    <td><%=p.Geslacht.ToString() %></td>
    <td>
        <% if (!p.IsLid)
           { %>
        <%=Html.ActionLink("Lid maken", "LidMaken", new { Controller = "Personen", id = p.GelieerdePersoonID })%>
        <% } %>
    </td>
    <td><% foreach (Categorie c in p.CategorieLijst) 
           { %>
               <%=Html.ActionLink(c.Code.ToString(), "List", new { Controller = "Personen", id = c.ID }, new { title = c.Naam.ToString() } )%>
        <% } %>
    </td>
</tr>
<% } %>

</table>