<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidFunctiesModel>" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{%>
	<ul id="acties">
		<li>
			<input type="submit" value="Gegevens wijzigen" /></li>
	</ul>
	<fieldset>
		<legend>Functies</legend>
		<%
			if (Model.AlleFuncties != null && Model.AlleFuncties.FirstOrDefault() != null)
			{
		%>
		<%
			List<CheckBoxListInfo> info = (from f in Model.AlleFuncties
										   select new CheckBoxListInfo(
									f.ID.ToString(),
									String.Format("{0} ({1})", f.Naam, f.Code),
									Model.FunctieIDs.Contains(f.ID))).ToList();
		%>
		<p>
			<%= Html.CheckBoxList("FunctieIDs", info)%></p>
		<%
			}
		%>
		<%= Html.HiddenFor(mdl=>mdl.HuidigLid.PersoonDetail.GelieerdePersoonID) %>
	</fieldset>
	<br />
	<%} %>
	<%= Html.ActionLink("Terug naar de persoonsfiche", "EditRest", new { Controller = "Personen", id = Model.HuidigLid.PersoonDetail.GelieerdePersoonID}) %>
</asp:Content>
