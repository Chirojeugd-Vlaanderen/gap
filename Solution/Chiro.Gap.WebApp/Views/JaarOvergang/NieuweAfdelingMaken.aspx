﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AfdelingInfoModel>" %>

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
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #697) %> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<%=Html.ValidationSummary() %>
	<% using (Html.BeginForm())
	{%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
		<li>
			<input type="reset" value="  Reset  " /></li>
	</ul>
	<fieldset>
		<legend>Nieuwe afdeling</legend>
		<%=Html.LabelFor(mdl => mdl.Info.Naam) %>
		<%=Html.EditorFor(mdl => mdl.Info.Naam)%>
		<br />
		<%=Html.ValidationMessageFor(mdl => mdl.Info.Naam)%>
		<br />
		<%=Html.LabelFor(mdl => mdl.Info.Afkorting)%>
		<%=Html.EditorFor(mdl => mdl.Info.Afkorting)%>
		<br />
		<%=Html.ValidationMessageFor(mdl => mdl.Info.Afkorting)%>
		
		<%=Html.HiddenFor(mdl => mdl.Info.ID) %>
	</fieldset>
	<%} %>
</asp:Content>
