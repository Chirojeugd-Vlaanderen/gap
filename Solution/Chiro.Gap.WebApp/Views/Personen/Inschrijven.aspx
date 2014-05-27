<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<InschrijvingsModel>" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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
		<script type="text/javascript">
		    $(document).ready(function () {

		        // Gedrag van vakje 'alles selecteren'

		        $("#checkall").click(function () {
		            $(this).closest('table').find("input[name*='InTeSchrijven']").prop('checked', this.checked);
		        });
		        
		        // onderstaande zorgt ervoor dat een klik op een checkbox 'leiding maken' de optie
		        // 'geen' toevoegt of verwijdert aan de lijst met mogelijke afdelingen

		        $('input[id*="LeidingMaken"]').click(function () {
		            if ($(this).is(':checked')) {
		                $(this).parent().parent().find('select').append('<option value="0">geen</option>');
		            }
		            else {
		                $(this).parent().parent().find('select').find('option[value="0"]').remove();
		            }
		        });

		        // initieel moet de optie 'geen' verwijderd worden voor alle rijen waar 'leiding maken'
		        // niet is geselecteerd.  We doen dit met javascript, zodat het inobtrusive blijft.

		        $('input[id*="LeidingMaken"]').each(function () {
		            if (!$(this).attr('checked')) {
		                $(this).parent().parent().find('select').find('option[value="0"]').remove();
		            }
		        });
		    });
		</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
    
    // Dit formulier wordt gebruikt door verschillende actions. De boeling is dat het
    // bij submit altijd doorschakelt naar Personen/Inschrijven.
    using (Html.BeginForm("Inschrijven", "Personen"))
	{
	%>
	<ul id="acties">
		<li><input id="bewaar" type="submit" value="Bewaren" /></li>
	</ul>

	<fieldset>
		<legend>Afdelingen</legend>

		<p>
            Selecteer voor iedereen de juiste afdeling. 
            <% if (!Model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep)){ %>
                Voor leiding zet je een vinkje in de kolom 'Leiding maken'.
            <% } %>
            Staat er iemand tussen die je toch niet wilt inschrijven, verwijder dan het vinkje in de eerste kolom.
            <% if (!Model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep)){ %>
		        Iedereen die je inschrijft, krijgt een instapperiode
			    <%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is die instapperiode?" } ) %> van drie weken (of tot 
                15 oktober). Tot die tijd kun je hen nog uitschrijven, daarna krijg je een factuur voor hun aansluiting.
            <% } %>
        </p>
		<table class="overzicht">
		<tr>
			<th><%: Html.CheckBox("checkall") %></th>
			<th>Persoon</th>
            <% if (!Model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep)) {%>
			    <th>Leiding maken</th>
			    <th>Afdeling</th>
            <%}%>
		</tr>
		
		<%
        for (var j = 0; j < Model.Inschrijvingen.Count(); ++j)
        {%>
			<tr class="<%: ((j&1)==0)?"even":"oneven" %>">
			<%
            //TODO de checkboxlist gaat alleen die bevatten die true zijn, terwijl we ook ergens de volledige lijst nog moeten hebben %>
				<td>
                    <%:Html.HiddenFor(mdl => mdl.Inschrijvingen[j].GelieerdePersoonID)%>
                    <%:Html.CheckBoxFor(mdl => mdl.Inschrijvingen[j].InTeSchrijven) %>
                </td>
				<td><%:Html.DisplayFor(mdl => mdl.Inschrijvingen[j].VolledigeNaam)%></td>
                
                <% if (!Model.GroepsNiveau.HeeftNiveau(Niveau.KaderGroep)) { %>
				    <td><%:Html.CheckBoxFor(mdl => mdl.Inschrijvingen[j].LeidingMaken)%></td>
				    <td>

				    <%
                        // Dit formulier selecteert by default het voorgestelde afdelingsjaar van ieder in te schrijven lid.
                        // In principe is InschrijvingsVoorstel.AfdelingsJaarIDs een array, maar in praktijk kunnen we voor dit
                        // formulier maar 1 keuze aan. Vandaar dat enkel naar het eerste item in die array wordt gekeken.

                       int voorgesteldAjID = Model.Inschrijvingen[j].AfdelingsJaarIDs == null
				                                  ? 0
                                                  : Model.Inschrijvingen[j].AfdelingsJaarIDs.FirstOrDefault();
                                              
                        var afdelingsLijstItems = (from ba in Model.BeschikbareAfdelingen
                               select
                                   new SelectListItem
                                   {
                                       // voorlopig maar 1 afdeling tegelijk (first or default)
                                       Selected = (voorgesteldAjID == ba.AfdelingsJaarID),
                                       Text = ba.Naam,
                                       Value = ba.AfdelingsJaarID.ToString(CultureInfo.InvariantCulture)
                                   }).ToList();
                               
                        // Afdeling 'geen' is mogelijk (en default) als het om leiding gaat.
                        // In principe moet die er enkel staan als 'leiding maken' aangevinkt is.  Maar
                        // omdat het tonen en verbergen via javascript loopt, voeg ik het item
                        // sowieso eerst altijd toe, om problemen te vermijden als javascript 
                        // gedisabled is.

                        afdelingsLijstItems.Add(new SelectListItem { Selected = Model.Inschrijvingen[j].LeidingMaken, Text = @"geen", Value = "0" });
                    
                    
                    %>
                                                               
                    <%=Html.DropDownListFor(mdl => mdl.Inschrijvingen[j].AfdelingsJaarIDs, afdelingsLijstItems)%>

				    </td>
            <% }
                   else
                   {
                       // Bij kaderploegen heb je de mogelijkheid niet om lid/leiding
                       // te kiezen.
                       %>
                       <%:Html.HiddenFor(mdl=>mdl.Inschrijvingen[j].LeidingMaken) %>
                       <%                       
                   } %>
			</tr>
		<%}%>
		</table>

	</fieldset>
	<%}%>
</asp:Content>
