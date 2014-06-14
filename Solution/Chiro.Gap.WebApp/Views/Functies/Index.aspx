<%@ Page Language="C#" Inherits="ViewPage<GroepsInstellingenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
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
<%// Opgelet! Scripts MOETEN een expliciete closing tag (</script>) hebben!  Ze oa #722 %>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())	{%>
    
	<ul id="functies_bestaand">
    <%foreach (var fie in Model.Detail.Functies.OrderBy(fie => fie.Code)){%>
		<li>
		    <%=Html.Encode(String.Format("{0} ({1})", fie.Naam, fie.Code))%>
            <% if ((Model.Detail.Niveau & Niveau.Groep) != 0) { %>							                
			    <%=Html.Encode(String.Format(
				    " - Kan toegekend worden aan ingeschreven {0}",
				    fie.Type == LidType.Kind ? "leden" : fie.Type == LidType.Leiding ? "leiding" : "leden en leiding"))%>                
            <%}%>      
            [<%=Html.ActionLink("Verwijderen", "Verwijderen", new {id = fie.ID}) %>]
            [<%=Html.ActionLink("Bewerken","Bewerken", new {id = fie.ID}) %>]
		</li>
		<%}%>
	</ul>

	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" id="functieToevoegen" />
        </li>
	</ul>
    <% //Default tonen, verdwijnt wanneer op 'bewerken' geklikt werd %>
   <fieldset >
		<legend>Functie toevoegen</legend>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.Naam) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.Naam) %><br />
			<%=Html.ValidationMessageFor(mdl => mdl.NieuweFunctie.Naam) %>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.Code) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.Code) %><br />
			<%=Html.ValidationMessageFor(mdl => mdl.NieuweFunctie.Code) %>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.MaxAantal) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.MaxAantal) %><br />
			(Mag leeg blijven als er geen maximumaantal is)
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.MinAantal) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.MinAantal) %><br />
			(Moet 0 zijn als er geen minimumaantal is)<%=Html.ValidationMessageFor(mdl => mdl.NieuweFunctie.MinAantal) %>
		</p>
        <% 	if ((Model.Detail.Niveau & Niveau.Groep) != 0)	{ %>							                
			<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.Type)%>
            <%	var values = from LidType lt in Enum.GetValues(typeof (LidType)) 
                 where lt!=LidType.Geen
			     select new {
	                value = lt,
	                text = String.Format(
	                    "Ingeschreven {0}",
	                    lt == LidType.Kind ? "leden" : lt == LidType.Leiding ? "leiding" : "leden en leiding")
                };%>
			<%=Html.DropDownListFor(mdl => mdl.NieuweFunctie.Type, new SelectList(values.Reverse(), "value", "text"))%>
			</p>
        <%}%>
	</fieldset>
	<%}	%>
</asp:Content>
