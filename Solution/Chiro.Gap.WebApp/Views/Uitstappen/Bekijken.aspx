<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapDeelnemersModel>"
	MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
			<%=Model.Uitstap.IsBivak ? "Details bivak" : "Details uitstap"%></div>
		<p>
			<em>
				<%=String.Format("{0:d}", Model.Uitstap.DatumVan)%>
				-
				<%=String.Format("{0:d}", Model.Uitstap.DatumTot)%></em>.
			<%=Model.Uitstap.Opmerkingen%>
			[<%=Html.ActionLink("Bewerken", "Bewerken", new { id = Model.Uitstap.ID })%> - <%=Html.ActionLink("Verwijderen", "Verwijderen", new { uitstapID = Model.Uitstap.ID })%>]
		</p>
		<p>
			<% if (string.IsNullOrEmpty(Model.Uitstap.PlaatsNaam)) { %>
			    [<%=Html.ActionLink("Bivakplaats ingeven", "PlaatsBewerken", new {id = Model.Uitstap.ID})%>]
			<% } else {%>
                <% var pa = Model.Uitstap.Adres; %>
                <%= Html.Encode(String.Format("{0} {1}{2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus.IsEmpty()? "" : " bus " + pa.Bus))%>,
                <%if (!pa.IsBelgisch) { // \note Duplicate code met Personen/Bewerken.aspx%>
                    <%= Html.Encode(String.Format("{0} {1} ({2})", pa.PostCode, pa.WoonPlaatsNaam, pa.LandNaam)) %>
                <% } else { %> 
                    <%= Html.Encode(String.Format("{0} {1}", pa.PostNr, pa.WoonPlaatsNaam)) %>
                <% }%>
			    [<%=Html.ActionLink("Bivakplaats wijzigen", "PlaatsBewerken", new {id = Model.Uitstap.ID})%>]
			<%}%>
		</p>
	</div>
	<p>
		Informatie over uitstappen wordt niet doorgegeven aan Chirojeugd Vlaanderen.
		Van een kamp wordt alleen de informatie voor de bivakaangifte
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Bivakaangifte", new { helpBestand = "Trefwoorden" }, new { title = "Bivakaangifte" } ) %>
		doorgestuurd. De deelnemerslijst is alleen voor je groep toegankelijk.</p>
    <p>
        <strong>TER INFO!</strong> Je kookploeg moet je niet meer apart verzekeren. Die mensen vallen automatisch onder de algemene Chiroverzekering. Meer info daarover vind je 
        <a href='https://chiro.be/administratie/verzekeringen/extra-verzekering/beperkte-periode'>op de Chirosite</a>.
    </p>
	<%
		if (Model.Deelnemers == null || Model.Deelnemers.FirstOrDefault() == null)
		{
	%>
	<p>
		Je hebt nog geen deelnemers opgegeven voor deze uitstap. Je moet minstens één
		iemand inschrijven, want je moet een contactpersoon aanduiden. Dat is een onderdeel
		van je bivakaangifte
		<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Bivakaangifte", new { helpBestand = "Trefwoorden" }, new { title = "Bivakaangifte" } ) %>,
		die voor 1 juni in orde moet zijn. De rest van de deelnemers mag je gelijk wanneer
		inschrijven - die gegevens zijn alleen voor je groep.
	</p>
	<%
		}
		else
		{
	%>
	<h3>
		Deelnemerslijst</h3>
		<p><%= Html.ActionLink(
		      	"Lijst downloaden", 
		      	"Download", 
		      	new { id = Model.Uitstap.ID }, 
		      	new { title = "Download de deelnemerslijst in een Excel-bestand" })%>
		</p>
	<table>
		<tr>
			<th />
			<!-- volgnr -->
			<th>Type </th>
			<th>Afd </th>
			<th>Naam </th>
			<th>Med. Fiche </th>
			<th>Betaald </th>
			<th>Opmerkingen </th>
			<th>Acties </th>
		</tr>
		<%
			var volgnr = 0;
			var lijst =
				Model.Deelnemers.OrderByDescending(d => d.Type).ThenByDescending(d => d.Afdelingen.FirstOrDefault() == null ? String.Empty : d.Afdelingen.First().Afkorting).ThenBy(d => d.VoorNaam).ThenBy(d => d.FamilieNaam);
			foreach (var d in lijst)
			{
				string klasse = (++volgnr & 1) == 0 ? "even" : "oneven";
				if (d.IsContact)
				{
					klasse = klasse + " highlight";
				}
		%>
		<tr class="<%=klasse%>">
			<td>
				<%=volgnr %>
			</td>
			<td>
				<%=d.Type == DeelnemerType.Deelnemer ? "lid" : d.Type == DeelnemerType.Begeleiding ? "leiding" : d.Type == DeelnemerType.Logistiek ? "logistiek" : "???" %>
			</td>
			<td>
				<%=Html.AfdelingsLinks(d.Afdelingen, Model.Uitstap.GroepsWerkJaarID, Model.GroepID) %>
			</td>
			<td>
				<%:Html.ActionLink(String.Concat(d.FamilieNaam, " ", d.VoorNaam), "DeelnemerBewerken", new { id = d.DeelnemerID }, new { title = "Bivakinschrijving bewerken" })%>
			</td>
			<td>
				<%=d.MedischeFicheOk ? "ja": "nee" %>
			</td>
			<td>
				<%=d.HeeftBetaald ? "ja": "nee" %>
			</td>
			<td>
				<%=d.Opmerkingen %>
			</td>
			<td>
				<%:Html.ActionLink("uitschrijven", "Uitschrijven", new {id=d.DeelnemerID}) %>
				<%:d.IsContact ? MvcHtmlString.Empty : Html.ActionLink("instellen als contact", "ContactInstellen", new {id=d.DeelnemerID}) %>
			</td>
		</tr>
		<%
}
		%>
	</table>
	<%
		}
	%>
	<p>
		Je kunt in het
		<%=Html.ActionLink("personen-", "Index", new {Controller="Personen", groepID = Model.GroepID} ) %>
		of
		<%=Html.ActionLink("ledenoverzicht", "Index", new {Controller="Leden", groepID = Model.GroepID} ) %>
		deelnemers aanvinken en inschrijven voor deze uitstap.
	</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
