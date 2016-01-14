<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<NieuweCommVormModel>" %>

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
    <script src="<%=ResolveUrl("~/Scripts/jquery/jquery-ui-1.10.2.custom.min.js")%>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("~/Scripts/jquery/jqueryui-editable.js")%>" type="text/javascript"></script>

    <% // CSS files %>
	<link href="<%=ResolveUrl("~/Content/Site.css")%>" rel="stylesheet" type="text/css" />
    <link href="<%=ResolveUrl("~/Content/jquery-ui-1.10.2.custom.css")%>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		using (Html.BeginForm())
		{
	%>
    

	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" id="bewaarComm" /></li>
	</ul>
	<fieldset>
		<legend>Tel./mail/enz. toevoegen voor
			<%=Model.Aanvrager.VolledigeNaam %></legend>
		<%=Html.ValidationSummary() %>
		<table>
			<tr>
				<td>
					<%=Html.DropDownListFor(
						mdl=>mdl.NieuweCommVorm.CommunicatieTypeID, 
						new SelectList(Model.Types.Select(x => new { value = x.ID, text = string.Format("{0}", x.Omschrijving)}), "value", "text"))%>
				</td>
                
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nummer) %>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nummer) %>
				</td>
			</tr>
            <tr id="verbRij" hidden >
                    <td colspan="2"  id="fouten"></td>
                </tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %><br />
                    <em>(wordt gekoppeld aan iedereen op hetzelfde adres)</em>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					Extra informatie
				</td>
            </tr>
            <tr>
				<td colspan="2">
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nota) %><br />
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nota) %>
				</td>
			</tr>
		</table>
	</fieldset>
	<%
		} %>
</asp:Content>
