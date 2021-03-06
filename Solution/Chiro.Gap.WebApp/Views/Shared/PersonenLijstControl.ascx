﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonInfoModel>" %>
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<%
    List<CheckBoxListInfo> info
       = (from pa in Model.PersoonInfos
          select new CheckBoxListInfo(pa.GelieerdePersoonID.ToString(), "", false)).ToList();
    int j = 0;
%>

<table id="overzichtsTabel" class="overzicht">
    <thead>
        <tr>
            <th>
                <%=Html.CheckBox("checkall") %></th>
            <th>AD-nummer</th>
            <th>
                Naam
            </th>
            <th>
                Geb.dat.
            </th>
            <th>
                Vrjrdg
            </th>
            <th>
                <%=Html.Geslacht(GeslachtsType.Man) %>
                <%=Html.Geslacht(GeslachtsType.Vrouw) %></th>
            <th>
                Cat.
            </th>
            <th>Ingeschr.</th>
            <th>Acties</th>
        </tr>
    </thead>
    <tbody>
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
        <td>
            <%=p.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" :  ((DateTime)p.GeboorteDatum).ToString("MM-dd") %>
        </td>
        <td class="center">
            <%=p.Geslacht == GeslachtsType.Onbekend? "<span class=\"error\">??</span>" : Html.Geslacht(p.Geslacht) %>
        </td>
        <td>
            <% foreach (var c in p.CategorieLijst)
               { %>
            <%=Html.ActionLink(Html.Encode(c.Code), "List", new { Controller = "Personen", id = c.ID }, new { title = "Toon alleen mensen uit de categorie " + c.Naam })%>
            <% } %>
        </td>
        <td>
            <%=p.IsLid ? "lid" : p.IsLeiding ? "leiding" : "--" %>
        </td>
        <td>
            <% if (!p.IsLid && !p.IsLeiding && (p.KanLidWorden || p.KanLeidingWorden)) { %>
                <%=Html.ActionLink(
                    String.Format("inschrijven{0}", Model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep) ? "" : (p.KanLidWorden ? " als lid" : " als leiding")), 
				    "Inschrijven", 
				    new { Controller = "Personen", gelieerdePersoonIDs = p.GelieerdePersoonID })
                    %><%
                   
                    // tamelijk lelijke hack hierboven. Door een int door te geven aan gelieerdePersoonIDs, zal
                    // personen/inschrijven die int interpreteren als een lijst.
                    
                    %>
                &nbsp;
            <% } %>
            <%=Html.ActionLink("zus/broer maken", "Kloon", new { Controller = "Personen", gelieerdepersoonID = p.GelieerdePersoonID })%>
        </td>
    </tr>
    <% } %>
    </tbody>
    
</table>
