<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<script type="text/javascript" src="/Scripts/jquery-1.2.6.min.js"></script>
<script type="text/javascript">
    $(document).ready(function() {
        $('input:submit').hide();

        $("#checkall").click(function() {
            $("input[@name=GekozenGelieerdePersoonIDs]").each(function() {
                this.checked = !this.checked;
            });
        });

        $("#GekozenActie").change(function() {
            $('input:submit').click();
        });
    });
</script>

<div class="pager">
Pagina: <%= Html.PagerLinks(ViewData.Model.PageHuidig, ViewData.Model.PageTotaal, i => Url.Action("List", new { page = i })) %>
</div>

<%
    List<CheckBoxListInfo> info
       = (from pa in Model.PersoonInfoLijst
          select new CheckBoxListInfo(
             pa.GelieerdePersoonID.ToString()
             , ""
             , false)).ToList<CheckBoxListInfo>();

    int j = 0;
%>

<form id="mactie" action="" method="post"> 
<input type="submit" value="List" />
<select id="GekozenActie" name="GekozenActie">
<option value=0>Kies een actie ...</option>
<option value=1>Lid maken</option>
<option value=2>In (dezelfde) categorie stoppen</option>
</select>

<table>
<tr>
<th><%=Html.CheckBox("checkall") %></th><th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th><th>Acties</th><th>Categorie&euml;n</th>
</tr>
<% foreach (PersoonInfo p in ViewData.Model.PersoonInfoLijst) {  %>
<tr>
    <td><%=Html.CheckBoxList("GekozenGelieerdePersoonIDs", info[j]) %><% j++; %></td>
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
               <%=Html.ActionLink(Html.Encode(c.Code.ToString()), "List", new { Controller = "Personen", id = c.ID }, new { title = c.Naam.ToString() } )%>
        <% } %>
    </td>
</tr>
<% } %>

</table>
</form>