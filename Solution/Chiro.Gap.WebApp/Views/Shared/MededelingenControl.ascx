<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IMasterViewModel>" %>
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
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<%	if (Model.Mededelingen != null && Model.Mededelingen.Any())
	{
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
		if (Model.Mededelingen.Count() == 1)
		{
%>

<div class="mededelingen ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary">
    <img src="<%=ResolveUrl("~/Content/images/Exclamation.png")%>" alt="Opgelet"/>
	<strong>Opgelet:</strong> Er is nog
	<%=Html.ActionLink("1 zaak", "Index", "GavTaken")%>
	die je in orde moet brengen.
</div>
<%
	}
		else
		{
%>
<div class="mededelingen ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary">
    <img src= "<%=ResolveUrl("~/Content/images/Exclamation.png")%>" alt="Opgelet"/>
	<strong>Opgelet:</strong> Er zijn nog
	<%=Html.ActionLink(Model.Mededelingen.Count + " zaken", "Index", "GavTaken")%>
	die je in orde moet brengen.
</div>
<%
	}
	}
	// ReSharper restore ConvertIfStatementToConditionalTernaryExpression
%>
