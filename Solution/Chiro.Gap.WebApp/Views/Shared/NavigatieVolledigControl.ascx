<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IMasterViewModel>" %>
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
	<ul>
		<li>
			<%= Html.ActionLink("Handleiding", "ViewTonen", "Handleiding")%></li>
        <li>
            <%= Html.ActionLink("Verzekering", "Index", "Verzekering") %></li>
		<li>
			<%= Html.ActionLink("Uitstappen/bivak", "Index", "Uitstappen") %></li>
		<li>
			<%= Html.ActionLink("Persoon toevoegen", "Nieuw", "Personen") %></li>
		<li>
			<%= Html.ActionLink("Groep", "Index", "Groep")%></li>
		<li>
			<%= Html.ActionLink("Iedereen", "Index", "Personen")%></li>
		<li>
			<%= Html.ActionLink("Ingeschreven", "Index", "Leden")%></li>
<% 
   if (Model.IsInOvergangsPeriode)
   {
%>
		<li><%=Html.ActionLink("Jaarovergang", "Index", "JaarOvergang")%></li>
<%
   }
%>
			
	</ul>