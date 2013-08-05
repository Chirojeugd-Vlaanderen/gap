<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GroepsInstellingenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <script src="<%= ResolveUrl("~/Scripts/jquery-groep.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#groep_Menu').menu();
            $('#algInfo').click(function () {
                verberg();
                $('#groep_algemeen').show();
            });
            $('#actAfd').click(function () {
                verberg();
                $('#groep_actieveAfdelingen').show();
            });
            $('#cat').click(function () {
                verberg();
                $('#groep_categorieën').show();
            });
            $('#funct').click(function () {
                verberg();
                $('#groep_functies').show();
            });

            function verberg() {
                $('#groep_algemeen,#groep_actieveAfdelingen,#groep_functies, #groep_categorieën').hide();
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <input id='MGID' value="<%=Model.Info.ID %>" hidden readonly/>
    <div id="extraInfoDialog" hidden>
        <img src="<%= ResolveUrl("~/Content/images/loading.gif")%>" />
    </div>

    <ul id="groep_Menu">
        <li><a href="#" id="algInfo">Algemene groepsinfo</a></li>
        <li><a href="#" id="actAfd">Actieve afdelingen dit werkjaar</a></li>
        <li><a href="#" id="cat">Categorieën</a></li>
        <li><a href="#" id="funct">Functies</a></li>
    </ul>
    
        <fieldset id="groep_algemeen">
            <legend>Algemene groepsinfo</legend>
		    <table>
		        <tbody>
			        <tr>
				        <td><%=Html.LabelFor(mdl => mdl.Detail.Naam)%></td>
				        <td id="groepsNaam"><%=Html.DisplayFor(mdl => mdl.Detail.Naam)%></td>
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
                </tbody>
		    </table>
        </fieldset>
<% 
		if ((Model.Detail.Niveau & Niveau.Groep) != 0)
		{
  			// Afdelingen enkel tonen voor Chirogroepen
  			// (niet voor kadergroepen of satellieten)  	
%>
			<fieldset id="groep_actieveAfdelingen" hidden>
				<legend>Actieve afdelingen dit werkjaar</legend>
                <table>
                    <thead>
                        <th><strong>Afdeling</strong></th>
                        <th><strong>Afk.</strong></th>
                        <th><strong>Officiële benaming</strong></th>
                        <th></th>
                    </thead>
                    <tbody>
  				    <% foreach (var afd in Model.Detail.Afdelingen.OrderByDescending(afd => afd.GeboorteJaarVan))
  				    { %>
						    <tr>
							    <td><%=afd.AfdelingNaam%><input value="<%=afd.AfdelingsJaarID %>" readonly hidden/></td>
                                <td><%=afd.AfdelingAfkorting %></td>
                                <td><%=afd.OfficieleAfdelingNaam.ToLower() %></td>
                                <td><div class="groep_bewerkAfdeling ui-icon ui-icon-pencil" title="Bewerken" style="cursor:pointer"></div></td>
                            </tr>
                    <% } %>   
                    </tbody>           
                </table>
                <button id="groep_afdelingen_aanpassen_knop">Afdelingsverdeling aanpassen</button>
				<%//=Html.ActionLink("Afdelingsverdeling aanpassen", "Index", "Afdelingen")%>
			</fieldset>
    <% } %>
	<fieldset id="groep_categorieën" hidden>
		<legend>Categorieën voor ingeschreven en niet-ingeschreven personen</legend>
		<table>
		    <thead>
		        <th>Categorie</th>
                <th>Code</th>
                <th></th>
		    </thead>
		    <tbody>
			<% foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code)) { %>
			<tr>
				<td><%=cat.Naam%><input value="<%=cat.ID %>" hidden/></td>
                <td><%=cat.Code %></td>
                <td><div class="categorieVerwijderen ui-icon ui-icon-circle-minus" title="Verwijderen" style="cursor: pointer"></div></td>
			</tr>
			<% } %>
            </tbody>
		</table>
        <button id="groep_categorieën_Toevoegen">Categorie toevoegen</button>
		<%//=Html.ActionLink("Categorieën toevoegen/verwijderen", "Index", "Categorieen") %>
	</fieldset>

	<fieldset id="groep_functies" hidden>
		<legend>Eigen functies voor ingeschreven leden en leiding</legend>
		<table>
		    <thead>
		        <th>Naam</th>
                <th>Code</th>
                <th>Type</th>
                <th></th>
                <th></th>
		    </thead>
		    <tbody>
			    <% foreach (var fie in Model.Detail.Functies.Where(fie=>fie.WerkJaarTot == null || fie.WerkJaarTot < Model.HuidigWerkJaar).OrderBy(fie => fie.Type)) { %>
			    <tr>
			        <td hidden><input value="<%=fie.ID %>"/></td>
			        <td><%=fie.Naam %></td>
                    <td><%=fie.Code %></td>
                    <td><%=fie.Type == LidType.Kind ? "leden" : fie.Type == LidType.Leiding ? "leiding" : "leden en leiding"%></td>
                    <td><div class="functieBewerken ui-icon ui-icon-pencil" title="Bewerken" style="cursor:pointer"></div></td>
                    <td><div class="functieVerwijderen ui-icon ui-icon-circle-minus" title="Verwijderen" style="cursor: pointer"></div></td>
			    </tr>
			    <% } %>
            </tbody>
		</table>
        <button id="groep_functies_toev_verw">Functie toevoegen</button>
		<%//=Html.ActionLink("Functie toevoegen", "Index", "Functies") %>
	</fieldset>
</asp:Content>
