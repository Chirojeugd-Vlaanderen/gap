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
        <script src="<%=ResolveUrl("~/Scripts/algemeneFuncties.js")%>" type="text/javascript"></script>
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

	<ul id="acties">
		<li>
			<button type="submit" class="ui-button-text-only" id="knopBewaren" hidden>Bewaren</button></li>
		<li>
			<button type="button" class="ui-button-text-only" id="knopReset"/>Reset</li>
	</ul>
	<% if(Model.BroerzusID != 0)
{
	%>
	<p>Het adres en de gezinsgebonden communicatievormen&nbsp;
    <%=Html.InfoLink("gezinsCom") %>
    <%//= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "GezinsgebondenCommunicatievorm", new { helpBestand = "Trefwoorden" }, new { title = "Wat zijn 'gezinsgebonden communicatievormen'?" })%> 
    worden gekopieerd van de broer of zus. 
    <%=Html.InfoLink("zusBroer") %>
    <%//=Html.ActionLink("Meer uitleg nodig over 'zus/broer maken'?", "ViewTonen", new { Controller = "Handleiding", helpBestand = "ZusBroer" }) %></p>	
	<%
} %>
    <div id="errorfield" class="ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" hidden>
        <img src="/Content/images/Exclamation.png" /></div>
	<fieldset>
		<legend>Persoonlijke gegevens</legend>
        <div id="confirmDialog" title="Reset" hidden>
            <p>Ben je zeker dat je alle velden wil resetten?</p>
        </div>

        <div id="extraInfoDialog" hidden><img src="/Content/images/loading.gif"/></div> 
        
        <% /* Dit stuk is enkel relevant bij personen die reeds ingeschreven zijn en hoeft dus niet 
             getoond te worden bij nieuwe personen */ %>
        <% if (Model.HuidigePersoon.AdNummer != null) {  %> 
            
		    <p>
                <%=Html.LabelFor(s => s.HuidigePersoon.AdNummer)%>   
                <%=Html.DisplayFor(s => s.HuidigePersoon.AdNummer) %>
                <%=Html.InfoLink("Ad-info")%>
			    <%=Html.HiddenFor(s => s.HuidigePersoon.AdNummer)  %>
			    <% // Het AD-nummer moet mee terug gepost worden, zowel als het null is als wanneer het niet null is:
                   // De service zal wel protesteren als het ad-nummer niet overeenkomt met het oorspronkelijke. %>
		    </p>
        <% }%>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.VoorNaam) %>
			<%=Html.EditorFor(s => s.HuidigePersoon.VoorNaam)%>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.VoorNaam) %>
		</p>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.Naam) %>
			<%=Html.EditorFor(s => s.HuidigePersoon.Naam) %>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.Naam) %>
		</p>
        
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.GeboorteDatum) %>
			<%=Html.EditorFor(s => s.HuidigePersoon.GeboorteDatum)%>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.GeboorteDatum) %>
		</p>
        <p><%=Html.LabelFor(s => s.HuidigePersoon.Geslacht)%>
            
		     <span id="geslacht">
			
		        <input type="radio" id="man" name="HuidigePersoon.Geslacht" value="Man" 
                <%= Model.HuidigePersoon.Geslacht==GeslachtsType.Man ? "checked=\"checked\"":"" %> />
                <label for="man">Man</label>

		        <input type="radio" id="vrouw" name="HuidigePersoon.Geslacht" value="Vrouw" 
                <%= Model.HuidigePersoon.Geslacht==GeslachtsType.Vrouw ? "checked=\"checked\"":"" %>/>
                <label for="vrouw">Vrouw</label>
 
            </span> 
        </p>
        <button id="btn_verder">Ga Verder</button>
        <div id="andereGegevens">
	
   
    <p>
        <label>Leiding/Lid</label>
        <span id="type">
			
		        <input type="radio" id="leiding" name="type" value="Leiding" />
                <label for="leiding">Leiding</label>

		        <input type="radio" id="lid" name="type" value="Lid" />
                <label for="lid">Lid</label>
 
        </span> 
    </p>
    <%
		if ((Model.GroepsNiveau & Niveau.Groep) != 0)
		{
			// Chiroleeftijd is enkel relevant voor plaatselijke groepen
	%>		
		
			<p id="wrap_chiroleeftijd" hidden>
			    <label>Chiroleeftijd</label>
				<select id="select_chiroleeftijd">
				    <option>0</option>
                    <option>1</option>
                    <option>2</option>
                    <option>3</option>
				</select>
                <%=Html.InfoLink("clInfo") %>
			</p>

	<%
		}
	%>
     <p>
        <label>Adres</label>
        <input type="text" id="np_adres"/>
    </p>
    </div>

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
	</fieldset>
	<% } %>
    <% if (Model.HuidigePersoon.GelieerdePersoonID > 0) { %>
	<%= Html.ActionLink("Terug naar de persoonsfiche", "EditRest", new { Controller = "Personen", id = Model.HuidigePersoon.GelieerdePersoonID }) %>
    <% } %>
</asp:Content>
