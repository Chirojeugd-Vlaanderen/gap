<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GroepsInstellingenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Cleanup en refactoring met module pattern: Copyright 2015 Sam Segers
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
    <script src="<%= ResolveUrl("~/Scripts/Modules/GroepModule.js") %>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/Modules/AdresModule.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            GroepModule.InitVoorAlgemeen();
            //Adres Bewerken
            $('.bewerkAdres').click(function () {
                url = link("Groep", "AdresBewerken") + "/" + <%= Model.Detail.ID %> + " #main";
                AdresModule.OpenDialog(url, "Adres wijzigen");
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
    <div id="extraInfoDialog" hidden>
        <img src="<%= ResolveUrl("~/Content/images/loading.gif")%>" />
    </div>

    <ul id="groep_Menu">
        <li><%=Html.ActionLink("Algemene groepsinfo", "Index")%></li>
        <% 
		if ((Model.Detail.Niveau & Niveau.Groep) != 0)
		{ // Afdelingen enkel tonen voor Chirogroepen (niet voor kadergroepen of satellieten) 
        %>
        <li><%=Html.ActionLink("Afdelingen dit werkjaar", "Afdelingen")%></li>
        <%} %>
        <li><%=Html.ActionLink("Categorieën", "Categorieen")%></li>
        <li><%=Html.ActionLink("Functies", "Functies")%></li>
    </ul>
    
    <fieldset id="groep_algemeen">
        <legend>Algemene groepsinfo</legend>
		<table>
		    <tbody>
			    <tr>
				    <td><%=Html.LabelFor(mdl => mdl.Detail.Naam)%></td>
				    <td><span id="groepsNaam"><%=Html.DisplayFor(mdl => mdl.Detail.Naam)%></span></td>
                    <td><div class="ui-icon ui-icon-pencil" title="Bewerken" id="bewerkGroepsNaam" style="cursor:pointer"></div></td>
			    </tr>
			    <tr>
				    <td><%=Html.LabelFor(mdl => mdl.Detail.Plaats)%></td>
				    <td><%=Html.DisplayFor(mdl => mdl.Detail.Plaats)%></td>
                    <td></td>
			    </tr>
			    <tr>
				    <td><%=Html.LabelFor(mdl => mdl.Detail.StamNummer)%></td>
				    <td><%=Html.DisplayFor(mdl => mdl.Detail.StamNummer)%></td>
                    <td></td>
			    </tr>
                <tr>
                    <td><%:Html.LabelFor(mdl => mdl.WerkJaarWeergave)%></td>
                    <td><%:Html.DisplayFor(mdl => mdl.WerkJaarWeergave)%></td>
                </tr>
                <tr>
                    <td>Adres lokalen</td>
                    <td>
                        <% if (Model.Detail.Adres != null)
                           { %>
                            <%= Model.Detail.Adres.StraatNaamNaam %>
			                <%= Model.Detail.Adres.HuisNr %>
			                <%= Model.Detail.Adres.Bus %>,
			                <%= Model.Detail.Adres.PostNr %>
			                <%= Model.Detail.Adres.PostCode %>
			                <%= Model.Detail.Adres.WoonPlaatsNaam %>
			                (<%= Model.Detail.Adres.LandNaam %>)
                        <% } %>
                    </td>
                    <td>
                        <div class="bewerkAdres ui-icon ui-icon-pencil" title="Bewerken" style="cursor: pointer"></div>
                    </td>
                </tr>
            </tbody>
		</table>
    </fieldset>
</asp:Content>
