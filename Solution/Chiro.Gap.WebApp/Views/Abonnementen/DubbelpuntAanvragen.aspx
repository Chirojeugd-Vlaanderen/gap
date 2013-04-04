<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<BevestigingsModel>" %>

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
	<% 
	
            if (Model.HuidigWerkJaar >= 2010)
            {
            	using (Html.BeginForm())
            	{%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bevestigen" /></li>
		<li>
			<%=Html.ActionLink("Annuleren",
            		                                  "EditRest",
            		                                  new {Controller = "Personen", id = Model.GelieerdePersoonID})%></li>
	</ul>
    
    <%
        if (!String.IsNullOrEmpty(Model.ExtraWaarschuwing))
        {
            %>
            <div class="Foutmelding">
                <%=Model.ExtraWaarschuwing %>
            </div>
            <%
        }
    %>


	Je staat op het punt om een <a href='http://www.chiro.be/dubbelpunt'>Dubbelpuntabonnement</a>
	te bestellen voor
	<%=Html.ActionLink(Model.VolledigeNaam,
            		                                  "EditRest",
            		                                  new {Controller = "Personen", id = Model.GelieerdePersoonID})%>.<br />
	<%

%>
	Hiervoor zal <strong>&euro;
		<%=Model.Prijs.ToString() %></strong> aangerekend worden. Klik op &lsquo;bevestigen&rsquo;
	om Dubbelpunt te bestellen. <br />
	<%
            	}
            }
            else
                // Lelijk stukje businesslogica in UI, als snelle fix om te vermijden dat groepen
                // Dubbelpunt bestellen vooraleer ze in het nieuwe werkjaar zitten.
                
            {%>
            
            Je kan pas Dubbelpunt bestellen via GAP nadat je de jaarovergang naar 2010-2011 hebt uitgevoerd.
            
            <%
            }%>
</asp:Content>
