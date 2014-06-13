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
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="MainContent">
	<% 
		using (Html.BeginForm())
		{ %>
    <ul id="acties">
		<li>
			<input type="submit" value="Bewaren" id="categorieToevoegen" />
        </li>
	</ul>
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
	</fieldset>
	<%} %>
</asp:Content>
