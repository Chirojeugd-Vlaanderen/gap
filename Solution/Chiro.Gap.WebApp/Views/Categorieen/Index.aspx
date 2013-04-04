<%@ Page Language="C#" Inherits="ViewPage<GroepsInstellingenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
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
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="MainContent">
	<fieldset>
		<legend>Persoonscategorie&euml;n</legend>
		<ul>
			<%
				foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code))
				{
			%>
			<li>
				<%=Html.Encode(String.Format("{0} - {1}", cat.Code, cat.Naam)) %>
				[<%=Html.ActionLink("verwijderen", "CategorieVerwijderen", new {id = cat.ID }) %>]
			</li>
			<%
				}
			%>
		</ul>
	</fieldset>
	<% 
		Html.EnableClientValidation();
		using (Html.BeginForm())
		{ %>
	<fieldset>
		<legend>Categorie toevoegen</legend>
		<p>
			<%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Naam) %>
			<%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Naam) %><br />
			<%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Naam) %>
		</p>
		<p>
			<%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Code) %>
			<%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Code) %><br />
			<%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Code) %>
		</p>
		<p>
			<input type="submit" value="Bewaren" />
		</p>
	</fieldset>
	<%} %>
</asp:Content>
