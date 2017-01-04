<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GavOverzichtModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head" >
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2014, 2015 Chirojeugd-Vlaanderen vzw
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
</asp:Content>

<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
<table>
    <tr>
        <th>Login</th>
        <th>Naam</th>
        <th>Vervaldatum</th>
        <!-- Dit tonen we nog niet, zie #3645
        <th>Zichzelf</th>
        <th>Groepsinfo</th>
        <th>Afdeling</th>
        <th>Iedereen</th>
        -->
        <th>Actie</th>
    </tr>
    <%
        foreach (var gebr in Model.GebruikersDetails.Where(det => det.VervalDatum >= DateTime.Now))
        {
            %>
    <tr>
        <td><%:gebr.Login %></td>
        <td><%=gebr.PersoonID > 0 ? Html.ActionLink(String.Format("{0} {1}", gebr.VoorNaam, gebr.FamilieNaam), "Bewerken", new { Controller = "Personen", id = gebr.GelieerdePersoonID}).ToHtmlString() : "(onbekend)" %></td>
        <td><%:gebr.VervalDatum == null ? "nooit" : ((DateTime)gebr.VervalDatum).ToString("d") %></td>
        <!-- Dit tonene we nog niet, zie #3645
        <td><%=Html.Permissie(gebr.GebruikersRecht.PersoonsPermissies) %></td>
        <td><%=Html.Permissie(gebr.GebruikersRecht.GroepsPermissies) %></td>
        <td><%=Html.Permissie(gebr.GebruikersRecht.AfdelingsPermissies) %></td>
        <td><%=Html.Permissie(gebr.GebruikersRecht.IedereenPermissies) %></td>
        -->
        <td>
            <% if (gebr.IsVerlengbaar)
                { // gebruikersrecht toekennen/verlengen is onderliggend dezelfde controller action
            %>
              <%=Html.ActionLink("Verlengen", "AanmakenOfVerlengen", new { id = gebr.GelieerdePersoonID }) %>
            <%              
                }

                if (Model.GebruikersDetails.Count() > 1)
                {
%>
              <%= Html.ActionLink("Afnemen", "Intrekken", new { id = gebr.PersoonID })%>
              <!-- Bewerken is nog niet geimplementeerd, zie #3647
              <%= Html.ActionLink("Bewerken", "Bewerken", new { id = gebr.PersoonID })%>
              -->
            <% } %>
        </td>
    </tr>

            <%
        }
    %>
</table>

<p>TIP: Je kunt een gebruiker bijmaken door op een personenfiche op &lsquo;gebruikersrecht toekennen&rsquo; te klikken. Het e-mailadres van die persoon moet wel ingevuld zijn, anders kan het wachtwoord natuurlijk niet doorgemaild worden.</p>

</asp:Content>

