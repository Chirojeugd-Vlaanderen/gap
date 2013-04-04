<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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

	<% 
        // Deze view kan zowel gebruikt worden voor het aanmaken van nieuwe uitstappen, als voor het bewerken
        // van bestaande
        
		Html.EnableClientValidation(); // Deze instructie moet (enkel) voor de BeginForm komen
        using (Html.BeginForm())
                {%>

                	<ul id="acties">
                    <li><input type="submit" value="Bewaren" /></li>
                    </ul>

                    <fieldset class="invulformulier">
                    <legend>Info over uitstap of bivak</legend>
                    
                    <%=Html.HiddenFor(mdl=>mdl.Uitstap.ID) %>
                    <%=Html.HiddenFor(mdl=>mdl.Uitstap.VersieString) %>

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.Naam) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.Naam) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.Naam) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.IsBivak) %>
                    <%=Html.CheckBoxFor(mdl => mdl.Uitstap.IsBivak) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.DatumVan) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.DatumVan) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.DatumVan) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.DatumTot) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.DatumTot) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.DatumTot) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.Opmerkingen) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.Opmerkingen) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.Opmerkingen) %><br />

                    </fieldset>
                    
                <%}%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
