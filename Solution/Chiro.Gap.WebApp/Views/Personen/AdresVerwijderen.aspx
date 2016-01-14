<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AdresVerwijderenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{ %>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" id="verwijderAdres"/></li>
	</ul>
	<fieldset>
		<legend>Adresgegevens</legend>
		<%=Html.Encode(String.Format("{0} {1} {2}", Model.Adres.StraatNaamNaam, Model.Adres.HuisNr, Model.Adres.Bus))%>
		<br />
		<%=Html.Encode(String.Format("{0} {1}", Model.Adres.PostNr, Model.Adres.WoonPlaatsNaam))%>
		<br />
		<%=Html.Hidden("Adres.ID") %>
		<%=Html.Hidden("AanvragerID") %>
	</fieldset>
	<fieldset>
		<legend>Wonen niet meer op bovenstaand adres:</legend>
		<%
			List<CheckBoxListInfo> info
			   = (from pa in Model.Adres.Bewoners
				  select new CheckBoxListInfo(
					 pa.PersoonID.ToString()
					 , pa.PersoonVolledigeNaam
					 , Model.PersoonIDs.Contains(pa.PersoonID))).ToList();
		%>
		<%=Html.CheckBoxList("PersoonIDs", info) %>
	</fieldset>
	<%
		} %>
</asp:Content>
