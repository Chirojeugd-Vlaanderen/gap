<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<CategorieModel>" %>

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
<%// Opgelet! Scripts MOETEN een expliciete closing tag (</script>) hebben!  Ze oa #722 %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.9.1.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<%
		Html.EnableClientValidation();
		using (Html.BeginForm())
		{
	%>
    <div class="test">
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
	</ul>
    
	<fieldset>
		<legend>Aan welke categorieën wil je
			<%= Model.GelieerdePersoonIDs.Count == 1 ? "hem/haar" : "hen" %>
			toevoegen?</legend>
		
		Personen om aan de categorie toe te voegen:<br />
		<ul>
		<%foreach (var p in Model.GelieerdePersoonNamen){ %>	
			<li><%=p %><br/></li>
		<%} %>
		</ul>
		
		<%
			List<CheckBoxListInfo> info
			   = (from pa in Model.Categorieen
				  select new CheckBoxListInfo(
					 pa.ID.ToString()
					 , pa.Naam
					 , false)).ToList();
		%>
		<%= Html.CheckBoxList("GeselecteerdeCategorieIDs", info) %>
		<% 
			foreach (int id in Model.GelieerdePersoonIDs)
			{
		%>
		<input type="hidden" name="GelieerdePersoonIDs" value="<%=id %>" />
		<%
			}
		%>
	</fieldset>
    </div>
	<%
		} %>
</asp:Content>
