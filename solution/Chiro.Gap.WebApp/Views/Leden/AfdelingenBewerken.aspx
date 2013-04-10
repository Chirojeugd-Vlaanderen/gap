<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.AfdelingenBewerkenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
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
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
	<% using (Html.BeginForm("AfdelingenBewerken", "Leden"))
	{
	%>
	    <ul id="acties">
		<li><input type="submit" value="Bewaren" /></li>
	    </ul>

	    <fieldset>
		<legend>Afdelingen</legend>

        <p>Selecteer afdeling(en) voor </p>
        <ul>
        <%
            foreach (var p in Model.Personen)
            {
        %>
            <li><%=Html.Hidden("LidIDs", p.LidID)%><%=p.VolledigeNaam %></li>
        <%
            }
        %>
        </ul>


		<%
            // Enkel als er alleen leiding geselecteerd is, kunnen er meerdere afdelingen geselecteerd worden.

		    bool enkelLeiding = (from p in Model.Personen
		                       where !p.IsLeiding
		                       select p).FirstOrDefault() == null;
                                   
			if (enkelLeiding)
			{
                  
			    // Het zou misschien leuk zijn moesten de afdelingen van bijv. de 1ste persoon als standaard
                // gekozen worden.  Maar voorlopig zijn alle checkboxes leeg.
                            
			    List<CheckBoxListInfo> info =
				    (from pa in Model.BeschikbareAfdelingen
				     select new CheckBoxListInfo(
								     pa.AfdelingsJaarID.ToString()
								     , pa.Naam
								     , false)).ToList();
        %>
			    <%=Html.CheckBoxList("AfdelingsJaarIDs", info)%>
		<%	
            }
			else
			{
		%>
		<%
			    foreach (var ai in Model.BeschikbareAfdelingen)
			    {
         
        %>           
                    <%=Html.RadioButtonFor(mdl=>mdl.AfdelingsJaarIDs, ai.AfdelingsJaarID)%> <%=ai.Naam %> (<%=ai.Afkorting %>) <br />
        <%
			    }
			}
		%>
	</fieldset>
	<%
		} %>
</asp:Content>
