<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<JaarOvergangAfdelingsModel>" %>

<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #697) %>
        <script type="text/javascript">
            $(document).ready(function () {
                $("#checkall").click(function () {
                    $(this).closest('table').find(':checkbox').prop('checked', this.checked);
                });
            });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <strong>Let op!</strong> Als je de jaarovergang uitvoert, dan sluit je werkjaar <%: Model.HuidigWerkJaar %>-<%: Model.HuidigWerkJaar + 1 %> af. 
        Je mag de jaarovergang enkel uitvoeren als <strong>alle leden en leiding voor <%: Model.HuidigWerkJaar %>-<%: Model.HuidigWerkJaar + 1 %></strong>
        ingeschreven zijn. Na de jaarovergang zul je enkel nog inschrijvingen kunnen doen voor het nieuwe werkjaar.
    </p>
    <%
        var info = (from pa in Model.Afdelingen select new CheckBoxListInfo(pa.ID.ToString(), "", false)).ToList();
        var j = 0;
    %>
    Selecteer de afdelingen die je groep volgend werkjaar zal gebruiken.
    <br />
    <br />
    Als er afdelingen van naam veranderen of als er nieuwe bijkomen, kun je dat
    hier aanpassen.
    <br />
    <br />
    <%using (Html.BeginForm("Stap1AfdelingenSelecteren", "JaarOvergang"))
      { %>
    <table>
        <tr>
            <th>
                <%=Html.CheckBox("checkall") %></th>
            <th>Afdeling</th>
            <th>Afkorting</th>
        </tr>
        <% foreach (var ai in Model.Afdelingen)
           { %>
        <tr>
            <td>
                <%=Html.CheckBoxList("GekozenAfdelingsIDs", info[j])%><% j++; %>
            </td>
            <td>
                <%=ai.Naam %>
            </td>
            <td>
                <%=ai.Afkorting %>
            </td>
            <td>
                <%=Html.ActionLink("Afdeling aanpassen", "Bewerken", new { Controller = "JaarOvergang", afdelingID = ai.ID })%>
            </td>
        </tr>
        <% } %>
    </table>
    <%=Html.ValidationMessageFor(mdl => mdl.GekozenAfdelingsIDs) %>
    <br />
    <%=Html.ActionLink("Afdeling aanmaken", "NieuweAfdelingMaken", new { Controller = "JaarOvergang" })%>
    <br />
    <br />
    <input id="volgende" type="submit" value="Naar stap 2" />
    <%} %>
</asp:Content>
