<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
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
	Handleiding: kosten aansluiting
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
<h2>Hoeveel kost een aansluiting?</h2>
	<p>
		Het bedrag op de factuur bestaat uit:</p>
	<ul>
		<li>8 euro per lid, leid(st)er, VB of proost</li>
		<li>2,11 euro per persoon die extra verzekerd is voor loonverlies</li>
	</ul>
	<p>
		Het bedrag van 8 euro voor elk lid is opgebouwd uit:</p>
	<ul>
		<li>3,90 euro nationaal lidgeld</li>
		<li>0,82 euro verzekering burgerlijke aansprakelijkheid</li>
		<li>1,49 euro ongevallenverzekering</li>
		<li>1,29 euro verzekering overlijden en invaliditeit</li>
		<li>0,50 euro ledenuitgaven</li>
	</ul>
	<p>&nbsp;</p>

</asp:Content>
