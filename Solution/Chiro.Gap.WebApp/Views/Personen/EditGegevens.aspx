<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<NieuwePersoonModel>" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" EnableViewState="False">
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

// Dit form wordt normaal gezien enkel gebruikt om een nieuwe persoon aan te maken.
// (al dan niet een kloon van een bestaande)
%>
    <script src="<%=ResolveUrl("~/Scripts/AdresBewerken.js")%>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("~/Scripts/jquery-nieuwe-persoon.js")%>" type="text/javascript"></script>

<script type="text/javascript">

    // validatie reguliere expressies e-mail en telefoonnummer

    $.validator.addMethod(
            "regex",
            function (value, element, regexp) {
                var re = new RegExp(regexp, 'i');   // case insensitive
                return this.optional(element) || re.test(value);
            },
            "Ongeldig formaat."
        );
    
    $(function() {
        $("#EMail_Nummer").rules("add", { regex: "<%=Model.EMailType.Validatie %>" });
        $("#TelefoonNummer_Nummer").rules("add", { regex: "<%=Model.TelefoonNummerType.Validatie %>" });
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
		Pas op! Je nieuwe persoon lijkt erg veel op iemand die al gekend is in
		de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt, klik
		dan opnieuw op &lsquo;Bewaren&rsquo;.
	</p>
	<%
		}
			else
			{
	%>
	<p class="validation-summary-errors">
		Pas op! Je nieuwe persoon lijkt erg veel op personen die al gekend zijn
		in de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt,
		klik dan opnieuw op &lsquo;Bewaren&rsquo;.
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
     
	<%=Html.ValidationSummary("Er zijn enkele opmerkingen:") %>
	<% Html.EnableClientValidation(); // Deze instructie moet (enkel) voor de BeginForm komen %>
	<% using (Html.BeginForm())
	{ %>

	<ul id="acties">
		<li><button name="button" type="submit" class="ui-button-text-only" id="knopBewaren" value="bewaren">Bewaren</button></li>
		<li><button name="button" type="submit" class="ui-button-text-only" value="bewarenEnNieuw">Bewaren en nog iemand toevoegen</button></li>
	</ul>

	<% if (Model.BroerzusID != 0)
	   {
	%>
	    <p>
	    Het adres en de gezinsgebonden communicatievormen&nbsp;
        <%= Html.InfoLink("gezinsCom") %>
        worden gekopieerd van de broer of zus. 
        <%= Html.InfoLink("zusBroer") %>
        </p>	
	<%
	   } %>
       
    <%Html.EnableClientValidation(); %>
    
	<fieldset>
	  <legend>Persoonlijke gegevens</legend>
        
      <table>
        
        <% if (Model.NieuwePersoon.AdNummer != null) {  %> 
            
		    <tr>
                <td><%=Html.LabelFor(s => s.NieuwePersoon.AdNummer)%></td>        
                <td>
                    <%=Html.DisplayFor(s => s.NieuwePersoon.AdNummer)%>
                    <%=Html.InfoLink("Ad-info")%>
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
			    <%=Html.EditorFor(s => s.NieuwePersoon.GeboorteDatum)%>
			    <%=Html.ValidationMessageFor(s => s.NieuwePersoon.GeboorteDatum)%>
			</td>
		</tr>
        <tr>
            <td><%=Html.LabelFor(s => s.NieuwePersoon.Geslacht)%></td>
		    <td>
		        <%=Html.RadioButtonFor(mdl => mdl.NieuwePersoon.Geslacht, GeslachtsType.Man) %> <%=Html.Geslacht(GeslachtsType.Man) %>
                <%=Html.RadioButtonFor(mdl => mdl.NieuwePersoon.Geslacht, GeslachtsType.Vrouw) %> <%=Html.Geslacht(GeslachtsType.Vrouw) %>
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
            <tr>
                <td><%=Html.LabelFor(s => s.EMail) %></td>
                <td>
                    E-mailadres:<br />
                    <%=Html.TextBoxFor(mdl => mdl.EMail.Nummer, new { placeholder = Model.EMailType.Voorbeeld }) %> 
                    <%=Html.ValidationMessageFor(mdl => mdl.EMail.Nummer) %> <br />
                    <%=Html.CheckBoxFor(mdl => mdl.EMail.IsVoorOptIn) %>
                    <%=Html.LabelFor(mdl => mdl.EMail.IsVoorOptIn) %>
                    <%=Html.InfoLink("snelBerichtInfo")%><br />
                    <%=Html.CheckBoxFor(mdl => mdl.EMail.IsGezinsGebonden) %>
                    <%=Html.LabelFor(mdl => mdl.EMail.IsGezinsGebonden) %><br />
                    <%=Html.HiddenFor(mdl => mdl.EMail.Voorkeur) %>
                    <%=Html.HiddenFor(mdl => mdl.EMail.CommunicatieTypeID) %>
                </td>
            </tr>
            <tr>
                <td><%=Html.LabelFor(s => s.TelefoonNummer) %></td>
                <td>
                    Telefoonnummer:<br />
                    <%=Html.TextBoxFor(mdl => mdl.TelefoonNummer.Nummer, new { placeholder = Model.TelefoonNummerType.Voorbeeld}) %> 
                    <%=Html.ValidationMessageFor(mdl => mdl.TelefoonNummer.Nummer)%><br />
                    <%=Html.CheckBoxFor(mdl => mdl.TelefoonNummer.IsGezinsGebonden)%>
                    <%=Html.LabelFor(mdl => mdl.TelefoonNummer.IsGezinsGebonden)%><br />
                    <%=Html.HiddenFor(mdl => mdl.TelefoonNummer.Voorkeur) %>
                    <%=Html.HiddenFor(mdl => mdl.TelefoonNummer.CommunicatieTypeID) %>
                </td>
            </tr>
        </table>
    </fieldset>
    
	<%=Html.HiddenFor(s => s.BroerzusID)%>
    <%=Html.HiddenFor(mdl => mdl.GroepsWerkJaarID) %>
    <%--<%=Html.HiddenFor(mdl => mdl.Forceer) %>--%>
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
                        
                        // In principe is InTeSchrijvenLid.AfdelingsJaarIDs een array, maar in praktijk kunnen we voor dit
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
