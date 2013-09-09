<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LidInfoModel>" %>
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
<% // TODO: onderstaand script moet naar 'head'. %>
<script src="<%= ResolveUrl("~/Scripts/TableTools.js")%>" type="text/javascript"></script>
<script src="<%= ResolveUrl("~/Scripts/ZeroClipboard.js")%>" type="text/javascript"></script>
<link href="<%= ResolveUrl("~/Content/TableTools.css")%>" rel="stylesheet" type="text/css" />

<table style="width: 100%" id="ledenOverzichtsTabel" class="overzicht">
    <thead>
            <tr>
                <th><%=Html.CheckBox("checkall") %></th>
                <% if (!Model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep)) { %>
                <th class="center">Type</th>
                <% } %>
                <th>Naam</th>
                <th>Geb.</th>
                <th>Vrjrdg</th>
                <th>
                    <%=Html.Geslacht(GeslachtsType.Man) %>
                    <%=Html.Geslacht(GeslachtsType.Vrouw) %>
                </th>
                <th>Betaald</th>
                <% // Afdelingen enkel relevant voor plaatselijke groepen		
                  if ((Model.GroepsNiveau & Niveau.Groep) != 0) {
                %>
                    <th>Afd.</th>
                <% } else { %>
                        <th style="overflow: hidden" hidden></th>
                <% } %> 
                <th>Func.</th>
                <th>Instap tot</th>
                <th>Telefoon</th>
                <th>E-mail</th>
                <th>Straat</th>
                <th>Nr</th>
                <th>Postnr.</th>
                <th>Woonplaats</th>
                <th>Land</th>
        </tr>
    </thead>
    <tbody>
    <% foreach (LidOverzicht lidOverzicht in ViewData.Model.LidInfoLijst) { %>
        <tr >
            <td>
                <input type="checkbox" name="SelectieGelieerdePersoonIDs" value="<%=lidOverzicht.GelieerdePersoonID %>"
                    <%=Model.SelectieGelieerdePersoonIDs != null && Model.SelectieGelieerdePersoonIDs.Contains(lidOverzicht.GelieerdePersoonID) ? "checked=\"checked\"" : String.Empty%> />
            </td>
            <% if (!Model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep)) { %>
            <td>
                <%= lidOverzicht.Type == LidType.Kind ? "Lid" : "Leiding" %>
            </td>
            <% } %>
            <td>
                <%=Html.PersoonsLink(lidOverzicht.GelieerdePersoonID, lidOverzicht.VoorNaam, lidOverzicht.Naam)%>
                <%=lidOverzicht.SterfDatum.HasValue? "&nbsp;(&dagger;)" : string.Empty %>
            </td>
            <td class="right">
                <%=lidOverzicht.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)lidOverzicht.GeboorteDatum).ToString("d")%>
            </td>
            <td>
                <%=lidOverzicht.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" :  ((DateTime)lidOverzicht.GeboorteDatum).ToString("MM-dd") %>
            </td>
            <td class="center">
                <%= Html.Geslacht(lidOverzicht.Geslacht)%>
            </td>
            <td>
                <%= lidOverzicht.LidgeldBetaald?"Ja":"Nee"%>
            </td>
            
            <%  // Afdelingen enkel relevant voor plaatselijke groepen
                if ((Model.GroepsNiveau & Niveau.Groep) != 0){     %>
                <td>
                <%=Html.AfdelingsLinks(lidOverzicht.Afdelingen, Model.IDGetoondGroepsWerkJaar, Model.GroepID)%>
                </td>
            <%}else { %> 
                <td style="overflow: hidden" hidden>
                </td>
            <%}%>
            <td>
                <% foreach (var functieInfo in lidOverzicht.Functies)
                   { %>
                <%=Html.ActionLink(
					Html.Encode(functieInfo.Code), 
					"Functie", 
					new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, groepID = Model.GroepID, id = functieInfo.ID }, 
					new { title = "Toon alleen mensen met functie " + functieInfo.Naam })%>
                <% } %>
            </td>
            <td>
                <%=lidOverzicht.EindeInstapPeriode == null ? String.Empty : ((DateTime)lidOverzicht.EindeInstapPeriode).ToString("d") %>
            </td>
            <td>
                <%if (!lidOverzicht.SterfDatum.HasValue)
                  {%>
                <%=Html.Telefoon(lidOverzicht.TelefoonNummer)%>
                <%
                    }%>
            </td>
            <td>
                <%if (!lidOverzicht.SterfDatum.HasValue)
                  {%>
                <a href='mailto:<%=lidOverzicht.Email %>'>
                    <%=lidOverzicht.Email %></a>
                <%
                    }%>
            </td>
            <td>
                <%: lidOverzicht.StraatNaam %>
            </td>
            <td>
                <%: String.Format( "{0}{1}", lidOverzicht.HuisNummer, 
                    String.IsNullOrEmpty(lidOverzicht.Bus) ? String.Empty: "/" + lidOverzicht.Bus) %>
            </td>
            <td>
                    <%= String.Format( "{0} {1}", lidOverzicht.PostNummer, lidOverzicht.PostCode) %>
            </td>
            <td>
                <%: lidOverzicht.WoonPlaats %>
            </td>
            <td>
                    <%: (lidOverzicht.Land ?? String.Empty).StartsWith("Belg") ? String.Empty : lidOverzicht.Land%>
            </td>
        </tr>
        <% } %>
        </tbody>
</table>
