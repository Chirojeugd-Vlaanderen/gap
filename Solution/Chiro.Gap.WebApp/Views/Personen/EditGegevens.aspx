<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
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
%>
	<%	// OPGELET: de opening en closing tag voor 'script' niet vervangen door 1 enkele tag, want dan
		// toont de browser blijkbaar niets meer van dit form.  (Zie #664) %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server"
	EnableViewState="False">
     
	<%=Html.ValidationSummary("Er zijn enkele opmerkingen:") %>
	<% Html.EnableClientValidation(); // Deze instructie moet (enkel) voor de BeginForm komen %>
	<% using (Html.BeginForm())
	{ %>
        <script src="<%=ResolveUrl("~/Scripts/jquery-nieuwe-persoon.js")%>" type="text/javascript"></script>
	<%
		if (Model.GelijkaardigePersonen != null && Model.GelijkaardigePersonen.Any())
		{
			if (Model.GelijkaardigePersonen.Count() == 1)
			{
	%>
	<p class="validation-summary-errors">
		Pas op! Je nieuwe persoon lijkt verdacht veel op iemand die al gekend is in
		de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt, klik
		dan opnieuw op &lsquo;Bewaren&rsquo;.
	</p>
	<%
		}
			else
			{
	%>
	<p class="validation-summary-errors">
		Pas op! Je nieuwe persoon lijkt verdacht veel op personen die al gekend zijn
		in de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt,
		klik dan opnieuw op &lsquo;Bewaren&rsquo;.
	</p>
	<%
		}
	%>
	<ul>
		<% 
			// Toon gelijkaardige personen
			foreach (PersoonDetail pi in Model.GelijkaardigePersonen)
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
    <input id="np_groepID" value="<%=Model.GroepID %>" hidden/>
    <input id="np_werkJaarID" value="<%=Model.GroepsWerkJaarID %>" hidden/>

	<ul id="acties">
		<li>
			<button type="button" class="ui-button-text-only" id="knopBewaren">Bewaren</button></li>
		<li>
			<button type="button" class="ui-button-text-only" id="knopReset"/>Reset</li>
	</ul>
	<% if(Model.BroerzusID != 0)
{
	%>
	<p>
	Het adres en de gezinsgebonden communicatievormen&nbsp;
    <%=Html.InfoLink("gezinsCom") %>
    worden gekopieerd van de broer of zus. 
    <%=Html.InfoLink("zusBroer") %>
    </p>	
	<%
} %>
    <div id="errorfield" class="ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" hidden>
        <h3>Fouten</h3>
        <img src="<%=ResolveUrl("~/Content/images/Exclamation.png")%>" />
        <div id="errorMessages">
            errors
        </div>
        <div style="clear: both"></div>
    </div>
    
    <div id="progress" hidden>
        <div id="balk"><div class="progress-label">Verwerken...</div></div>
    </div>

	<fieldset>
		<legend>Persoonlijke gegevens</legend>
        <div id="confirmDialog" title="Reset" hidden>
            <p>Ben je zeker dat je alle velden wil resetten?</p>
        </div>

        <div id="extraInfoDialog" hidden><img src="/Content/images/loading.gif"/></div> 
        
        <% /* Dit stuk is enkel relevant bij personen die reeds ingeschreven zijn en hoeft dus niet 
             getoond te worden bij nieuwe personen */ %>
    <table>
        
        <% if (Model.HuidigePersoon.AdNummer != null) {  %> 
            
		    <tr>
                <td><%=Html.LabelFor(s => s.HuidigePersoon.AdNummer)%></td>        
                <td>
                    <%=Html.DisplayFor(s => s.HuidigePersoon.AdNummer) %>
                    <%=Html.InfoLink("Ad-info")%>
                </td>
			    <%=Html.HiddenFor(s => s.HuidigePersoon.AdNummer) %>
			    <% // Het AD-nummer moet mee terug gepost worden, zowel als het null is als wanneer het niet null is:
                   // De service zal wel protesteren als het ad-nummer niet overeenkomt met het oorspronkelijke. %>
		    </tr>
        <% }%>
		<tr>
			<td><%=Html.LabelFor(s => s.HuidigePersoon.VoorNaam) %></td>
			<td><%=Html.EditorFor(s => s.HuidigePersoon.VoorNaam)%></td>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.VoorNaam) %>
		</tr>
		<tr>
			<td><%=Html.LabelFor(s => s.HuidigePersoon.Naam) %></td>
			<td><%=Html.EditorFor(s => s.HuidigePersoon.Naam) %></td>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.Naam) %>
		</tr>
        
		<tr>
			<td><%=Html.LabelFor(s => s.HuidigePersoon.GeboorteDatum) %></td>
			<td><%=Html.EditorFor(s => s.HuidigePersoon.GeboorteDatum)%></td>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.GeboorteDatum) %>
		</tr>
        <tr>
            <td><%=Html.LabelFor(s => s.HuidigePersoon.Geslacht)%></td>
		    <td>
                <span id="geslacht">
		            <input type="radio" id="man" name="HuidigePersoon.Geslacht" value="Man" 
                    <%= Model.HuidigePersoon.Geslacht==GeslachtsType.Man ? "checked=\"checked\"":"" %> />
                    <label for="man">Man</label>
		            <input type="radio" id="vrouw" name="HuidigePersoon.Geslacht" value="Vrouw" 
                    <%= Model.HuidigePersoon.Geslacht==GeslachtsType.Vrouw ? "checked=\"checked\"":"" %>/>
                    <label for="vrouw">Vrouw</label> 
                </span>
             </td> 
        </tr>

        <tr class="np_adres" id="eersteAdres">
                <td>Adres:</td>
                <td>
                    Land: <select id="landSelect"></select>
                    Postnummer: <input type="text" id="np_postNr" size="6"/>
                    <span id="postCode" hidden>Postcode: <input type="text" size="6"/></span>
                    Gemeente: <select id="np_gemeente"></select><input type="text" id="buitenlandseGemeente" hidden/>
                    <br/>
                    <br/>
                    Straat: <input type="text" id="np_straat"/> 
                    Nr: <input type="text" size="6" id="np_nummer"/> Bus: <input type="text" id="np_bus" size="5"/>
                    Type: <select id="adresType">
                              <option>Thuis</option>
                              <option>Kot</option>
                              <option>Werk</option>
                              <option>Overig</option>
                          </select>
                </td>
        </tr>
        <tr hidden>
            <td><b>Adres toevoegen</b></td>
            <td><div class="ui-icon ui-icon-circle-plus" id="np_adresToevoegen" title="Toevoegen" style="cursor: pointer"></div></td> 
        </tr>

    </table>
    
    

    <fieldset>
        <legend>Telefoonnummer</legend>
            <table>
                <tr class="np_telefoonnummer" id="eersteTel" hidden>
                    <td>Telefoonnnummer</td>
                    <td><input type="text" id="np_telefoonnummer"/></td>
                </tr>
                <tr>
                    <td><b>Telefoonnummer <br/> toevoegen</b></td>
                    <td><div class="ui-icon ui-icon-circle-plus" id="np_telToevoegen" title="Toevoegen" style="cursor: pointer"></div></td>
                </tr>
            </table>
    </fieldset>
    
   
    <fieldset>
        <legend>E-mail adres</legend>
        <table>
            <tr class="np_emailadres" id="eersteEmail" hidden>
                <td>E-mail adres</td>
                <td><input type="email" id="np_emailadres" size="30"/></td>
            </tr>
            <tr>
                <td><b>E-mailadres toevoegen</b></td>
                <td><div class="ui-icon ui-icon-circle-plus" id="np_emailToevoegen" title="Toevoegen" style="cursor: pointer"></div></td>
            </tr>
        </table>
    </fieldset>
		<%=Html.HiddenFor(s => s.HuidigePersoon.GelieerdePersoonID)%>
		<%=Html.HiddenFor(s => s.BroerzusID)%>
		<%=Html.HiddenFor(s => s.HuidigePersoon.VersieString)%>
		<%
			if (Model.Forceer)
			{
				// Ik krijg onderstaande niet geregeld met een html helper... :(
		%>
		<input type="hidden" name="Forceer" id="Forceer" value="True" />
		<%
			}
		%>
       <button type="button" id="tochInschrijven" hidden>Ik weet niet wat ik wil en ga deze persoon TOCH inschrijven</button>
       <fieldset id='chiroGegevens'>
          <legend>Chirogegevens</legend>
          <p>Inschrijven in huidige werkjaar?
                <button id='ja' type="button">Ja</button>
                <button id='nee' type="button">Nee</button>
          </p>

          <table hidden>
              <tr>
            <td>Leiding/Lid</td>
            <td>
                <span id="type">
		                <input type="radio" id="leiding" name="type" value="Leiding" />
                        <label for="leiding">Leiding</label>

		                <input type="radio" id="lid" name="type" value="Lid" />
                        <label for="lid">Lid</label>
                </span> 
            </td>
        </tr>
        <tr>
            <td>Afdeling</td>
            <td>
                <span id="afdelingSelectie">

                </span>
            </td>
        </tr>
         <%
		if ((Model.GroepsNiveau & Niveau.Groep) != 0)
		{
			// Chiroleeftijd is enkel relevant voor plaatselijke groepen
	%>		
		
			<tr id="wrap_chiroleeftijd" hidden>
			    <td>Chiroleeftijd</td>
				<td>
				    <select id="select_chiroleeftijd">
				        <option>0</option>
                        <option>1</option>
                        <option>2</option>
                        <option>3</option>
				    </select>
                    <%=Html.InfoLink("clInfo") %>
                </td>
			</tr>
	<%
		}
	%>
        <tr>
           <td colspan="2"><button id="tochNiet" type="button" hidden>Oeps, toch niet inschrijven!</button></td> 
        </tr>
        <%//<button id="btn_verder">Ga Verder</button> %>

   
          </table>
      </fieldset>
	</fieldset>
   
	<% } %>
    <% if (Model.HuidigePersoon.GelieerdePersoonID > 0) { %>
	<%= Html.ActionLink("Terug naar de persoonsfiche", "EditRest", new { Controller = "Personen", id = Model.HuidigePersoon.GelieerdePersoonID }) %>
    <% } %>
</asp:Content>
