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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div class="kaderke">
		<div class="kadertitel">
			Algemene groepsinfo</div>
		<table>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.Detail.Naam)%>
				</td>
				<td>
					<%=Html.DisplayFor(mdl => mdl.Detail.Naam)%> [<%=Html.ActionLink("Wijzigen", "NaamWijzigen", "Groep")%>]
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.Detail.Plaats)%>
				</td>
				<td>
					<%=Html.DisplayFor(mdl => mdl.Detail.Plaats)%>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.Detail.StamNummer)%>
				</td>
				<td>
					<%=Html.DisplayFor(mdl => mdl.Detail.StamNummer)%>
				</td>
			</tr>
		</table>
	</div>
<% 
		if ((Model.Detail.Niveau & Niveau.Groep) != 0)
		{
  			// Afdelingen enkel tonen voor Chirogroepen
  			// (niet voor kadergroepen of satellieten)  	
%>
			<div class="kaderke">
				<div class="kadertitel">
					Actieve afdelingen dit werkjaar</div>
				<ul>
<%
  				foreach (var afd in Model.Detail.Afdelingen.OrderByDescending(afd => afd.GeboorteJaarVan))
  				{
%>
						<li>
							<%=
  						Html.Encode(String.Format("{0} ({1}) -- officiële variant: {2}",
  									  afd.AfdelingNaam,
  									  afd.AfdelingAfkorting,
  									  afd.OfficieleAfdelingNaam.ToLower()))%></li>
<%
 				}
%>
				</ul>
				[<%=Html.ActionLink("Afdelingsverdeling aanpassen", "Index", "Afdelingen")%>]
			</div>
<%
		}
%>
	<div class="kaderke">
		<div class="kadertitel">
			Categorieën voor ingeschreven en niet-ingeschreven personen</div>
		<ul>
			<%
				foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code))
				{
			%>
			<li>
				<%=Html.Encode(String.Format("{0} ({1})", cat.Naam, cat.Code))%>
			</li>
			<%
				}
			%>
		</ul>
		[<%=Html.ActionLink("Categorieën toevoegen/verwijderen", "Index", "Categorieen") %>]
	</div>
	<div class="kaderke">
		<div class="kadertitel">
			Eigen functies voor ingeschreven leden en leiding</div>
		<ul>
			<%
				foreach (var fie in Model.Detail.Functies.OrderBy(fie => fie.Type))
				{
			%>
			<li>
				<%=Html.Encode(String.Format(
			    "{0} ({1}) -- kan toegekend worden aan ingeschreven {2}", 
			    fie.Naam, 
                fie.Code, 
                fie.Type == LidType.Kind ? "leden" : fie.Type == LidType.Leiding ? "leiding" : "leden en leiding"))%>
			</li>
			<%
				}%>
		</ul>
		[<%=Html.ActionLink("Functies toevoegen/verwijderen", "Index", "Functies") %>]
	</div>
</asp:Content>
