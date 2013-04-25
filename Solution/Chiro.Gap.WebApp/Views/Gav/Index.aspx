<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GavModel>" %>

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
	<ul>
		<%
			// Ik weet niet precies hoe het komt, maar soms krijg ik null-items
			// in mijn groepenlijst.  Via de where hieronder, omzeil ik dat probleem.
	    
			foreach (var item in ViewData.Model.GroepenLijst.Where(it=>it != null))
			{
           
		%>
		<li>
			<%=Html.ActionLink(
               String.Format("{0} - {1} ({2})", item.StamNummer, item.Naam, item.Plaats),
                "Index", 
                new {Controller = "Handleiding", groepID = item.ID})%></li>
		<%
           
			}
		%>
	</ul>
	<%
        if (Model.GroepenLijst.FirstOrDefault() == null)
        {
            %>
            <p>Je groep werd niet gevonden.  Misschien is je gebruikersrecht vervallen? Vraag aan een collega-GAV om je 
            gebruikersrecht te verlengen, of <a href='http://www.chiro.be/eloket/aansluitingen-chirogroepen'>contacteer het secretariaat.</a></p>
            <%
        }
	%>
</asp:Content>
