<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AdresModel>"  %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
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
<% Html.RenderPartial("AdresBewerkenScriptsControl", Model); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		Html.EnableClientValidation();
// ReSharper disable Asp.NotResolved
		using (Html.BeginForm())	// ReSharper geeft een foutmelding omdat er geen action bestaat die AdresBewerken heet,
									// maar de view wordt in andere actions opgeroepen
// ReSharper restore Asp.NotResolved
		{ %>
	<ul id="acties">
		<li>
			<input type="submit" name="action" value="Bewaren" id="bewaarAdres" />
        </li>
	</ul>
	<fieldset>
		<legend>Toepassen op:</legend>
		<%=Html.CheckBoxList("GelieerdePersoonIDs", Model.Bewoners)%>
	</fieldset>
    <% =Html.ValidationSummary()  %>
    <% var values = from AdresTypeEnum e in Enum.GetValues(typeof(AdresTypeEnum))
				  select new { value = e, text = e.ToString() }; 
		%>

	<fieldset>
		<legend>Adresgegevens</legend>
    <table>
        <tr>
			<td><%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.AdresType) %></td>
			<td><%=Html.DropDownListFor(mdl => mdl.PersoonsAdresInfo.AdresType, new SelectList(values, "value", "text"))%></td>
		</tr>
        <tr>
            <td><%=Html.LabelFor(mdl => mdl.Land) %></td>
            <td><%=Html.DropDownListFor(mdl => mdl.Land, new SelectList(Model.AlleLanden, "Naam", "Naam")) %></td>  
        </tr>
    </table>

    <p id="uitlegBinnenland" hidden>
			<strong>Opgelet:</strong> voor binnenlandse adressen wordt alleen de officiële spelling van de straatnaam geaccepteerd.<br />
			Ben je zeker van de straatnaam maar wordt ze geweigerd? Lees in
			<%=Html.ActionLink("de handleiding", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweStraatnaam"})%>
			hoe we daar een mouw aan kunnen passen.
    </p>
		
        <p id="uitlegBuitenland" hidden>
    Voor een buitenlands adres kun je behalve een postnummer ook een postcode invullen:
    dat is bijvoorbeeld de lettercode die in Nederlandse adressen na het postnummer
    komt (bv. 1216 RA Hilversum).</p>

        <table id="tabel" hidden>
        <%
			Html.RenderPartial("AdresBewerkenControl", Model);
         %>
        
        <!-- Rap hier iets tussen zetten, om te vermijden dat resharper vervelend doet -->

		<%
			if (Model.OudAdresID == 0)
			{
				// De mogelijkheid om aan te kruisen of het nieuwe adres het voorkeursadres wordt, krijg je alleen bij een nieuw
				// adres, en niet bij een verhuis.  I.e. als OudAdresID == 0.
		%>
		<tr>
			<td><%=Html.LabelFor(mdl=>mdl.Voorkeur) %></td>
			<td><%=Html.EditorFor(mdl => mdl.Voorkeur)%></td>
		</tr>
		<%
			}
		%>
		<%=Html.HiddenFor(mdl=>mdl.AanvragerID) %>
		<%=Html.HiddenFor(mdl=>mdl.OudAdresID) %>
        </table>
	</fieldset>
	<%} %>
</asp:Content>
