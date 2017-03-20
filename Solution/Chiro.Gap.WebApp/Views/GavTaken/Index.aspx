<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GavTakenModel>" %>

<%@ Import Namespace="System.Linq" %>
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
	<p>
		Hier kun je zien welke administratieve taken er nog op jou liggen te wachten.
		Gaat het over functies die niet toegekend zijn, dan vind je meer info in 
		<%=Html.ActionLink("de handleiding", "ViewTonen", new { controller = "Handleiding", helpBestand = "IemandsFunctiesAanpassen" })%>.
	</p>
	<% if (Model.Mededelingen != null && Model.Mededelingen.Any())
	{
	%>
	<ul>
		<%
			foreach (var m in Model.Mededelingen)
			{
		%>
		<li><strong>
			<%=m.Type %>:</strong>
			<%=m.Info %></li>
		<%
			}
		%>
	</ul>
	<%
		}
	%>
</asp:Content>
