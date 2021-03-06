﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<NieuwePersoonModel>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.Poco.Model" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" EnableViewState="False">
<%
/*
 * Copyright 2008-2013, 2015 the GAP developers. See the NOTICE file at the 
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

// Dit form wordt normaal gezien enkel gebruikt om een nieuwe persoon aan te maken.
// (al dan niet een kloon van een bestaande)
%>
	<script src="<%=ResolveUrl("~/Scripts/Modules/AdresModule.js")%>" type="text/javascript"></script>
	<script src="<%=ResolveUrl("~/Scripts/Modules/PersoonModule.js")%>" type="text/javascript"></script>

<script type="text/javascript">
	$(document).ready(function () {
		PersoonModule.InitVoorNieuw();
		// validatie reguliere expressies e-mail en telefoonnummer
		$.validator.addMethod(
				"regex",
				function (value, element, regexp) {
					var re = new RegExp(regexp, 'i');   // case insensitive
					return this.optional(element) || re.test(value);
				},
				"Ongeldig formaat."
			);
		<%
			foreach (var comType in Model.CommunicatieTypes.Values)
			{
		%>
				jQuery.validator.addClassRules('<%=typeof(CommunicatieTypeEnum).Name+"_"+comType.ID%>', 
					{ regex: "<%=comType.Validatie %>" });
		<%
			}
		 %>
		var voornaamElement = document.getElementById('NieuwePersoon_VoorNaam');
		$(voornaamElement.form).validate({
			rules: {
				"NieuwePersoon.VoorNaam": "required",
				"NieuwePersoon.Naam": "required"

			},
			messages: {
				"NieuwePersoon.VoorNaam": "Voornaam is verplicht",
				"NieuwePersoon.Naam": "Familienaam is verplicht"
			}
		});
	});
</script>    
	
	<%	// OPGELET: de opening en closing tag voor 'script' niet vervangen door 1 enkele tag, want dan
		// toont de browser blijkbaar niets meer van dit form.  (Zie #664) %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server"
	EnableViewState="False">
	
	<!-- Div hieronder wordt gebruikt voor hulplinks. -->
	<div id="extraInfoDialog" hidden><img src="/Content/images/loading.gif"/></div>
	
		<%
		if (Model.GelijkaardigePersonen != null && Model.GelijkaardigePersonen.Any())
		{
			if (Model.GelijkaardigePersonen.Count() == 1)
			{
	%>
	<p class="validation-summary-errors">
		Pas op! Je nieuwe persoon lijkt erg veel op <%=(Model.GelijkaardigePersonen.Count() == 1 ?"iemand":"personen") %> die al gekend is in
		de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt, klik
		dan opnieuw op &lsquo;Bewaren&rsquo;.
	</p>
	<%
			}
	%>
	<ul>
		<% 
			// Toon gelijkaardige personen
			foreach (var pi in Model.GelijkaardigePersonen)
			{
		%>
		<li>
			<%=Html.PersoonsLink(pi.GelieerdePersoonID, pi.VoorNaam, pi.Naam)%>
			-
			<%=String.Format("{0:d}", pi.GeboorteDatum) %></li>
		<%
			}
		%>
	</ul>
	<%      
		}
	%>
	 
	<%=Html.ValidationSummary() %>
	<% using (Html.BeginForm())
	{ %>

	<ul id="acties">
		<li><button name="button" type="submit" class="ui-button-text-only" id="knopBewaren" value="bewaren">Bewaren</button></li>
		<li><button name="button" type="submit" class="ui-button-text-only" value="bewarenEnNieuw">Bewaren en nog iemand toevoegen</button></li>
	</ul>
	  
	<fieldset>
	  <legend>Persoonlijke gegevens</legend>
		
	  <table>
		
		<% if (Model.NieuwePersoon.AdNummer != null) {  %> 
			
			<tr>
				<td><%=Html.LabelFor(s => s.NieuwePersoon.AdNummer)%></td>        
				<td>
					<%=Html.DisplayFor(s => s.NieuwePersoon.AdNummer)%>
					<%=Html.InfoLink("AD-nummer")%>
				</td>
			</tr>
		<% }%>
		<tr>
			<td><%=Html.LabelFor(s => s.NieuwePersoon.VoorNaam)%></td>
			<td>
				<%=Html.EditorFor(s => s.NieuwePersoon.VoorNaam)%>
				<%=Html.ValidationMessageFor(s => s.NieuwePersoon.VoorNaam)%>
			</td>
		</tr>
		<tr>
			<td><%=Html.LabelFor(s => s.NieuwePersoon.Naam)%></td>
			<td>
				<%=Html.EditorFor(s => s.NieuwePersoon.Naam)%>
				<%=Html.ValidationMessageFor(s => s.NieuwePersoon.Naam)%>
			</td>
		</tr>
		
		<tr >
			<td><%=Html.LabelFor(s => s.NieuwePersoon.GeboorteDatum)%></td>
			<td>
				<input class="text-box single-line" id="NieuwePersoon_GeboorteDatum" name="NieuwePersoon.GeboorteDatum" type="text" value="<%=Html.DisplayFor(s => s.NieuwePersoon.GeboorteDatum)%>" />
<%
					// Bovenstaande is een workaround voor #2700. Normaal gezien zouden we dit gebruiken:
					// 
					// Html.EditorFor(s => s.NieuwePersoon.GeboorteDatum)
					//
					// Maar dat rendert bovenstaande input met type="date". Hierdoor gebruikt chrome zijn built-in datepicker,
					// en dat geeft vermoedelijk conflicten met de datepicker van jquery.
					
					// De gebooredatum moet expliciet ingevuld worden (DisplayFor), omdat dezelfde view wordt
					// getoond als er een fout optrad. We willen dan niet dat de reeds ingevulde geboortedatum dan
					// verloren gaat.
%>
				<%=Html.ValidationMessageFor(s => s.NieuwePersoon.GeboorteDatum)%>
			</td>
		</tr>
		<tr>
			<td><%=Html.LabelFor(s => s.NieuwePersoon.Geslacht)%></td>
			<td>
				<%=Html.RadioButtonFor(mdl => mdl.NieuwePersoon.Geslacht, GeslachtsType.Man) %> <%=Html.Geslacht(GeslachtsType.Man) %>
				<%=Html.RadioButtonFor(mdl => mdl.NieuwePersoon.Geslacht, GeslachtsType.Vrouw) %> <%=Html.Geslacht(GeslachtsType.Vrouw) %>
				<%=Html.RadioButtonFor(mdl => mdl.NieuwePersoon.Geslacht, GeslachtsType.X) %> <%=Html.Geslacht(GeslachtsType.X) %>
				<%=Html.ValidationMessageFor(s => s.NieuwePersoon.Geslacht)%>
			</td>
		</tr>
	  </table>
	</fieldset>
		
	<fieldset>
	  <legend>Adres</legend>
		<% var adresTypes = from AdresTypeEnum e in Enum.GetValues(typeof(AdresTypeEnum))
				  select new { value = e, text = e.ToString() }; 
		%>        
	  <table>
		<tr>
			<td><%=Html.LabelFor(mdl => mdl.AdresType) %></td>
			<td><%=Html.DropDownListFor(mdl => mdl.AdresType, new SelectList(adresTypes, "value", "text"))%></td>
		</tr>
		  
			<% Html.RenderPartial("AdresBewerkenControl", Model); %>
	  </table>
	</fieldset>
	
	<fieldset>
		<legend>Communicatie</legend>
		<table>
			<%
				for (var i = 0; i < Model.CommunicatieInfos.Count; i++)
				{
	%>      <tr>
				<td>&nbsp;</td>
				<td class="communicatievormcount">
					<%=Model.CommunicatieTypes[Model.CommunicatieInfos[i].CommunicatieTypeID].Omschrijving
					+(Model.CommunicatieInfos[i].Voorkeur?": (Voorkeur)":":")%> <br />
					<%=Html.TextBoxFor(mdl => mdl.CommunicatieInfos[i].Nummer, new { placeholder = Model.CommunicatieTypes[Model.CommunicatieInfos[i].CommunicatieTypeID].Voorbeeld, @class = typeof(CommunicatieTypeEnum).Name+"_"+Model.CommunicatieInfos[i].CommunicatieTypeID }) %> 
					<%=Html.ValidationMessageFor(mdl => mdl.CommunicatieInfos[i].Nummer) %> <br />
					<%=Html.CheckBoxFor(mdl => mdl.CommunicatieInfos[i].IsGezinsGebonden) %>
					<%=Html.LabelFor(mdl => mdl.CommunicatieInfos[i].IsGezinsGebonden) %><br />
					<%=Html.HiddenFor(mdl => mdl.CommunicatieInfos[i].Voorkeur) %>
					<%=Html.HiddenFor(mdl => mdl.CommunicatieInfos[i].CommunicatieTypeID) %>
				</td>
			</tr>
	<%	        }
				 %>
			<tr id="CommunicatieVormenKnoppen">
				<td>Meer:</td>
				<td>
					<select id="communicatietype">
						<%
						foreach (var type in Model.CommunicatieTypes)
						{ %>
							<option value="<%=type.Key%>" data-placeholder="<%=type.Value.Voorbeeld %>"><%=type.Value.Omschrijving%></option>       
					 <% } %>
					</select>
					<div style="cursor: pointer" title="Toevoegen" class="commTypeToevoegen ui-icon ui-icon-circle-plus" onclick="PersoonModule.nieuweCommunicatieVorm();"></div>
				</td>
			</tr>
			<tr>
				<td>&nbsp;</td>
				<td>
					<%=Html.CheckBoxFor(mdl => mdl.NieuwePersoon.NieuwsBrief) %>
					<%=Html.LabelFor(mdl => mdl.NieuwePersoon.NieuwsBrief) %>
					<%=Html.InfoLink("snelBerichtInfo")%>                    
				</td>
			</tr>
		</table>
	</fieldset>
	
	<%=Html.HiddenFor(mdl => mdl.GroepsWerkJaarID) %>
	<%
		if (Model.Forceer)
		{
			// Ik krijg onderstaande niet geregeld met een html helper... :(
	%>
	<input type="hidden" name="Forceer" id="Forceer" value="True" />
	<%
		}
	%>

	<fieldset id='chiroGegevens'>
		<legend>Chirogegevens</legend>

		<table>
			<tr>
				<td>Meteen inschrijven?</td>
				<td>
					<%=Html.RadioButtonFor(mdl => mdl.InschrijvenAls, LidType.Geen) %> Nee
					<% if (Model.GroepsNiveau == Niveau.Groep)
					   { %>
					<%= Html.RadioButtonFor(mdl => mdl.InschrijvenAls, LidType.Kind) %> Lid
					<% } %>
					<%=Html.RadioButtonFor(mdl => mdl.InschrijvenAls, LidType.Leiding) %> Leiding
					<%=Html.ValidationMessageFor(mdl => mdl.InschrijvenAls) %>
				</td>
			</tr>
			<tr id="rij_afdeling">
				<td>Afdeling</td>
				<td>
<%
						// TODO: Erg gelijkaardige code komt voor in Views/Shared/LedenMaken.aspx.
						// Dat wordt best nog eens ontdubbeld.
						
						// In principe is InschrijvingsVoorstel.AfdelingsJaarIDs een array, maar in praktijk kunnen we voor dit
						// formulier maar 1 keuze aan. Vandaar dat enkel naar het eerste item in die array wordt gekeken.

						int geselecteerdAfdelingsJaarID = Model.AfdelingsJaarIDs == null
												  ? 0
												  : Model.AfdelingsJaarIDs.FirstOrDefault();
											  
						var afdelingsLijstItems = (from ba in Model.BeschikbareAfdelingen
							   select
								   new SelectListItem
								   {
									   // voorlopig maar 1 afdeling tegelijk (first or default)
									   Selected = (geselecteerdAfdelingsJaarID == ba.AfdelingsJaarID),
									   Text = ba.AfdelingNaam,
									   Value = ba.AfdelingsJaarID.ToString(CultureInfo.InvariantCulture)
								   }).ToList();
							   
						// Afdeling 'geen' is mogelijk (en default) als het om leiding gaat.
						// In principe moet die er enkel staan als 'leiding maken' aangevinkt is.  Maar
						// omdat het tonen en verbergen via javascript loopt, voeg ik het item
						// sowieso eerst altijd toe, om problemen te vermijden als javascript 
						// gedisabled is.
					
						afdelingsLijstItems.Add(new SelectListItem { Selected = geselecteerdAfdelingsJaarID == 0, Text = @"geen", Value = "0" });
					
					
					%>
															   
					<%=Html.DropDownListFor(mdl => mdl.AfdelingsJaarIDs, afdelingsLijstItems)%>

				</td>
			</tr>
		<%
	if ((Model.GroepsNiveau & Niveau.Groep) != 0)
	{
		// Chiroleeftijd is enkel relevant voor plaatselijke groepen
%>		
		
			<tr id="rij_chiroleeftijd">
				<td><%=Html.LabelFor(mdl => mdl.NieuwePersoon.ChiroLeefTijd) %></td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuwePersoon.ChiroLeefTijd) %>
					<%=Html.InfoLink("clInfo") %>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuwePersoon.ChiroLeefTijd) %>
				</td>
			</tr>
<%
	}
%>
  
		</table>
	</fieldset>
   
	<% } %>
	
</asp:Content>
