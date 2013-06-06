<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AfdelingsJaarModel>" %>

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
	<% // Opgelet! Voor scripts de expliciete closing tag laten staan; anders krijg je een lege pagina. (zie #694) %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		Html.EnableClientValidation();
		using (Html.BeginForm("AfdJaarBewerken", "Afdelingen", new { groepID = Model.GroepID }))
		{%>
            
	<ul id="acties">
		<li><input id="afdelingBewerken_bewaar" type="submit" value="Bewaren" /></li>
		<li><input id="afdelingBewerken_reset" type="reset" value="Reset" /></li>
	</ul>

	<fieldset>
	    <table>
	        <tr>
		        <td><%=Html.LabelFor(mdl=> mdl.Afdeling.Naam)%></td>
		        <td><%=Html.EditorFor(mdl => mdl.Afdeling.Naam)%></td>
                <%=Html.ValidationMessageFor(mdl => mdl.Afdeling.Naam)%>
            </tr>
            <tr>
		        <td><%=Html.LabelFor(mdl => mdl.Afdeling.Afkorting)%></td>
		        <td><%=Html.EditorFor(mdl => mdl.Afdeling.Afkorting)%></td>
                <%=Html.ValidationMessageFor(mdl => mdl.Afdeling.Afkorting)%>
            </tr>
            <tr>
		        <td><%=Html.LabelFor(s => s.AfdelingsJaar.Geslacht)%></td>
		        <td>
                    <%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Gemengd, Model.AfdelingsJaar.Geslacht == GeslachtsType.Gemengd)%>
		            Gemengd
		            <%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Man, Model.AfdelingsJaar.Geslacht == GeslachtsType.Man)%>
		            Jongens
		            <%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Vrouw, Model.AfdelingsJaar.Geslacht == GeslachtsType.Vrouw)%>
		            Meisjes
                </td>
		        <%=Html.ValidationMessageFor(s => s.AfdelingsJaar.Geslacht)%>
            </tr>
            <tr>
		        <td><%=Html.LabelFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%></td>
		        <td><%=Html.EditorFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%></td>
		        <%=Html.ValidationMessageFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%>
            </tr>
            <tr>
		        <td><%=Html.LabelFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%></td>
		        <td><%=Html.EditorFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%></td>
		        <%=Html.ValidationMessageFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%>
            </tr>
            <tr>
		       <td><% var values = from oa in Model.OfficieleAfdelingen
				          select new { value = oa.ID, text = oa.Naam }; 
		        %>
		        <%=Html.LabelFor(mdl=>mdl.AfdelingsJaar.OfficieleAfdelingID) %></td>
		        <td><%=Html.DropDownListFor(mdl => mdl.AfdelingsJaar.OfficieleAfdelingID, new SelectList(values, "value", "text"))%> </td>
            </tr>
            </table>
                <%=Html.HiddenFor(mdl=>mdl.Afdeling.ID) %>
		        <%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingID) %>
		        <%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingsJaarID) %>
		        <%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.VersieString) %>
	</fieldset>

	<%
		} 
	%>
	
	<p>
		Ter informatie de &lsquo;standaardafdelingen&rsquo; voor dit werkjaar:
	</p>
	<table>
		<%  foreach (var oa in Model.OfficieleAfdelingen.Where(ofaf=>ofaf.ID != (int)NationaleAfdeling.Speciaal).OrderBy(ofaf => ofaf.LeefTijdTot)){%>
		<tr>
			<td>
				<%=oa.Naam %>
			</td>
			<td>
				<%=oa.StandaardGeboorteJaarVan(Model.HuidigWerkJaar) %>-<%=oa.StandaardGeboorteJaarTot(Model.HuidigWerkJaar)%>
			</td>
		</tr>
		<%}%>
	</table>
	<p>
	    
		<%=Html.ActionLink("Meer weten over afdelingen die een speciaal geval zijn?", "ViewTonen", new { controller = "Handleiding", helpBestand = "SpecialeAfdelingen" })%>
	</p>
</asp:Content>
