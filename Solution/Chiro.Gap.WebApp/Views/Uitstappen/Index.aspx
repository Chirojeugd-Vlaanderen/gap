<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapOverzichtModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $('#BivakAangifte').click(function () {
                toonInfo("#BAINFO", "Bivakaangifte","#extraInfoDialog" );
            });
        });
    </script>

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
    <div class="kaderke">
        <div class="kadertitel">
            Overzicht uitstappen
            </div>
        <div id="extraInfoDialog" hidden>
            
        </div>
        <p>
            Informatie over uitstappen wordt niet doorgegeven aan Chirojeugd Vlaanderen. Van
            een kamp wordt alleen de informatie voor de bivakaangifte<%=Html.InfoLink("BivakAangifte") %>
           doorgestuurd. De deelnemerslijst is alleen voor je groep toegankelijk.</p>
        <p>
            [<%=Html.ActionLink("Uitstap/bivak toevoegen", "Nieuw", "Uitstappen") %>]
        </p>
        <table>
            <tr>
                <th>
                    Periode
                </th>
                <th>
                    Omschrijving
                </th>
                <th>
                    Bivak
                </th>
                <th>
                    Opmerking
                </th>
            </tr>
            <% foreach (var uitstap in Model.Uitstappen)
               {%>
            <tr>
                <td>
                    <%=String.Format("{0:d}", uitstap.DatumVan) %>
                    -
                    <%=String.Format("{0:d}", uitstap.DatumTot)%>
                </td>
                <td>
                    <%=Html.ActionLink(uitstap.Naam, "Bekijken", new {id = uitstap.ID})%>
                </td>
                <td>
                    <%=uitstap.IsBivak ? "&#10003;" : String.Empty %>
                </td>
                <td>
                    <%=uitstap.Opmerkingen %>
                </td>
            </tr>
            <%} %>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
