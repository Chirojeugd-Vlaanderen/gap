<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidInfoModel>" %>

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
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #713) %>
	<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>"></script>
	<script type="text/javascript">
	    $(document).ready(function () {
	        $('#filter').hide();
	        $('#kiesActie').hide();
	        $("#AfdelingID").change(function () {
	            $('#filter').click();
	        });
	        $("#FunctieID").change(function () {
	            $('#filter').click();
	        });
	        $("#SpecialeLijst").change(function () {
	            $('#filter').click();
	        });
	        $("#GekozenActie").change(function () {
	            $('#kiesActie').click();
	        });            
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
				<%
                    // Eén groot form, want voor elk lid staat een checkbox
                    
					using (Html.BeginForm("Lijst", "Leden"))
					{
						var dummyAfdeling = new SelectListItem[] {new SelectListItem {Text = "op afdeling", Value = "0"}};
						var dummyFunctie = new SelectListItem[] {new SelectListItem {Text = "op functie", Value = "0"}};
						var dummySpeciaal = new SelectListItem[] {new SelectListItem {Text = "speciale lijst", Value = "0"}};%>
				
                	<div id="acties">
                    <h1>Acties</h1>
                    <ul>
                    <li><%= Html.ActionLink(
		      	                "Lijst downloaden", 
		      	                "Download", 
		      	                new { id = Model.IDGetoondGroepsWerkJaar, afdelingID = Model.AfdelingID, functieID = Model.FunctieID, sortering = Model.GekozenSortering, ledenLijst = Model.SpecialeLijst }, 
		      	                new { title = "Download de geselecteerde gegevens in een Excel-bestand" })%></li>
                    </ul>
                     
                    <%if (Model.KanLedenBewerken)
                    {%>   
                    <h1>Selectie</h1>
    	            <select id="GekozenActie" name="GekozenActie">
		                <option value="0">kies een actie</option>
						<%if(Model.JaartalGetoondGroepsWerkJaar!=Model.JaartalHuidigGroepsWerkJaar){ %>
							<option value="1">Inschrijven</option>
						<%}else{ %>
							<option value="2">Uitschrijven</option>
						<%} %>
		                <option value="3">Afdeling aanpassen</option>
                        <option value="4">Inschrijven voor uitstap/bivak</option>
	                </select>
	                <input id="kiesActie" type="submit" value="Uitvoeren" />

                    <%
                    }else{%>
					<h1>Selectie</h1>
    	            <select id="Select1" name="GekozenActie">
		                <option value="0">kies een actie</option>
						<%if(Model.JaartalGetoondGroepsWerkJaar!=Model.JaartalHuidigGroepsWerkJaar){ %>
							<option value="1">Inschrijven</option>
						<%}else{ %>
							<option value="2">Uitschrijven</option>
						<%} %>
	                </select>
	                <input id="Submit1" type="submit" value="Uitvoeren" />
					<%} %>

                    <h1>Filteren</h1>                    	

                        <%
                          if ((Model.GroepsNiveau & (Niveau.Gewest | Niveau.Verbond)) == 0)
                          {
                        %>

  						    <%=Html.DropDownListFor(
                          		mdl => mdl.AfdelingID,
                          		dummyAfdeling.Union(
                          			Model.AfdelingsInfoDictionary.Select(
                          				src =>
                          				new SelectListItem
                          					{Text = src.Value.AfdelingNaam, Value = src.Value.AfdelingID.ToString()})))%> <br />
                        <%
                          }
                        %>
                        
						<%=Html.DropDownListFor(
							mdl=>mdl.FunctieID, 
								dummyFunctie.Union(
							Model.FunctieInfoDictionary.Select(src => new SelectListItem {Text = src.Value.Naam, Value = src.Value.ID.ToString()}))) %>
                        <br />

						<%=Html.DropDownListFor(
							mdl=>mdl.SpecialeLijst, 
								dummySpeciaal.Union(
							Model.SpecialeLijsten.Select(src => new SelectListItem {Text = src.Value, Value = src.Key.ToString()}))) %>									
                    
						<input id="filter" type="submit" value="Filter toepassen" />
						
						<%=Html.HiddenFor(s => s.IDGetoondGroepsWerkJaar)%>
						<%=Html.HiddenFor(s => s.GekozenSortering)%>
	                    </div>
	
	                    <div class="pager">
	                    Pagina:
	                    <%= Html.WerkJaarLinks(
                                    ViewData.Model.IDGetoondGroepsWerkJaar, 
                                    ViewData.Model.WerkJaarInfos,
				                    wj => Url.Action("Lijst", new { Controller = "Leden", id = wj.ID, sortering = Model.GekozenSortering, afdelingID = Model.AfdelingID, functieID = Model.FunctieID, ledenLijst = Model.SpecialeLijst }))%>
	                    </div>
	
	                    <% Html.RenderPartial("LedenLijstControl"); %>

   		            <%
			            }
		            %>

</asp:Content>
