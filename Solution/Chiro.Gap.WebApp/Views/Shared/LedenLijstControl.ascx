<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
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
<% // TODO: onderstaand script moet naar 'head'. %>

<table style="width: 100%" id="ledenOverzichtsTabel" class="overzicht">
    <thead>
        <tr>
            <th><%=Html.CheckBox("checkall") %></th>
            <th class="center">Type</th>
            <th>Naam</th>
            <th>Geb.</th>
            <th>Vrjrdg</th>
            <th>
                <%=Html.Geslacht(GeslachtsType.Man) %>
                <%=Html.Geslacht(GeslachtsType.Vrouw) %>
            </th>
            <th>Betaald</th>
            <th>Afd.</th>
            <th>Func.</th>
            <th>Instap tot</th>
            <th>Telefoon</th>
            <th>E-mail</th>
        </tr>
    </thead>
    <tbody>
        <% foreach (LidOverzicht lidOverzicht in ViewData.Model.LidInfoLijst)
            { %>
        <tr>
            <td>
                <input type="checkbox" name="SelectieGelieerdePersoonIDs" value="<%=lidOverzicht.GelieerdePersoonID %>"
                    <%=Model.SelectieGelieerdePersoonIDs != null && Model.SelectieGelieerdePersoonIDs.Contains(lidOverzicht.GelieerdePersoonID) ? "checked=\"checked\"" : String.Empty%> />
            </td>
            <td>
                <%= lidOverzicht.Type == LidType.Kind ? "Lid" : "Leiding" %>
            </td>
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
            <td>
                <%=Html.AfdelingsLinks(lidOverzicht.Afdelingen, Model.IDGetoondGroepsWerkJaar, Model.GroepID)%>
            </td>
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

                <% if (lidOverzicht.VoorkeurmailadresIsVerdacht)
                    {  %>
                <div class="uitlegIsVerdacht ui-icon ui-icon-alert" title="Mailadres ziet er verdacht uit" style="cursor: pointer"></div>
                &nbsp;
                <% } %>

                <a href='mailto:<%=lidOverzicht.Email %>'>
                    <%=lidOverzicht.Email %></a>
                <%
                    }%>
            </td>
        </tr>
        <% } %>
    </tbody>
</table>
