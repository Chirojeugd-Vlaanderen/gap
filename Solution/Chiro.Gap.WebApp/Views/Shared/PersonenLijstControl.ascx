<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonInfoModel>" %>
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
%>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<% // TODO: onderstaand script moet naar 'head'. %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#checkall").click(function () {
            $("INPUT[@name=GekozenGelieerdePersoonIDs][type='checkbox']").attr('checked', $("#checkall").is(':checked'));
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


<div class="pager">
    Pagina's:
    <%= Html.PagerLinksLetters(ViewData.Model.HuidigePagina, ViewData.Model.Paginas, letter => Url.Action("List", new { Controller = "Personen", page = letter, sortering = Model.Sortering }), ViewData.Model.GekozenCategorieID > 0)%>
    (Totaal:
    <%= Model.Totaal %>
    personen)
</div>
<table class="overzicht">
    <tr>
        <th>
            <%=Html.CheckBox("checkall") %></th>
        <th>Ad-nr.</th>
        <th>
            <%= Html.ActionLink("Naam", "List", new { Controller = "Personen", page = Model.HuidigePagina, id = Model.GekozenCategorieID, sortering = PersoonSorteringsEnum.Naam }, new { title = "Sorteren op naam" })%>
        </th>
        <th>
            <%= Html.ActionLink("Geboortedatum", "List", new { Controller = "Personen", page = Model.HuidigePagina, id = Model.GekozenCategorieID, sortering = PersoonSorteringsEnum.Leeftijd }, new { title = "Sorteren op geboortedatum" })%>
        </th>
        <th>
            <%=Html.Geslacht(GeslachtsType.Man) %>
            <%=Html.Geslacht(GeslachtsType.Vrouw) %></th>
        <th>
            <%= Html.ActionLink("Cat.", "List", new { Controller = "Personen", page = Model.HuidigePagina, id = Model.GekozenCategorieID, sortering = PersoonSorteringsEnum.Categorie }, new { title = "Sorteren op categorie" })%>
        </th>
        <th>Ingeschr.</th>
        <th>Acties</th>
    </tr>
    <% foreach (PersoonDetail p in ViewData.Model.PersoonInfos)
       {  %>
    <tr>
        <td>
            <%=Html.CheckBoxList("GekozenGelieerdePersoonIDs", info[j]) %><% j++; %>
        </td>
        <td>
            <%=p.AdNummer %>
        </td>
        <td>
            <%=Html.PersoonsLink(p.GelieerdePersoonID, p.VoorNaam, p.Naam)%>
            <%=p.SterfDatum.HasValue? "&nbsp;(&dagger;)" : string.Empty %>
        </td>
        <td class="right">
            <%=p.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)p.GeboorteDatum).ToString("d") %>
        </td>
        <td class="center">
            <%=p.Geslacht == GeslachtsType.Onbekend? "<span class=\"error\">??</span>" : Html.Geslacht(p.Geslacht) %>
        </td>
        <td>
            <% foreach (var c in p.CategorieLijst)
               { %>
            <%=Html.ActionLink(Html.Encode(c.Code), "List", new { Controller = "Personen", id = c.ID, page = 1, sortering = "Naam"}, new { title = "Toon alleen mensen uit de categorie " + c.Naam })%>
            <% } %>
        </td>
        <td>
            <%=p.IsLid ? "lid" : p.IsLeiding ? "leiding" : "--" %>
        </td>
        <td>
            <% if (!p.IsLid && !p.IsLeiding && (p.KanLidWorden || p.KanLeidingWorden))
               { %>
            <%=Html.ActionLink(
				String.Format("inschrijven als {0}", p.KanLidWorden ? "lid": "leiding"), 
				"Inschrijven", 
				new { Controller = "Personen", gelieerdepersoonID = p.GelieerdePersoonID })%>
            <% } %>
            <%=Html.ActionLink("zus/broer maken", "Kloon", new { Controller = "Personen", gelieerdepersoonID = p.GelieerdePersoonID })%>
        </td>
    </tr>
    <% } %>
</table>
