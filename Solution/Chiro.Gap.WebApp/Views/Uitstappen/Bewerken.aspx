﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<%
/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
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
    <script src="<%=ResolveUrl("~/Scripts/jquery-uitstap.js")%>" type="text/javascript"></script>
    
    <%	// OPGELET: de opening en closing tag voor 'script' niet vervangen door 1 enkele tag, want dan
		// toont de browser blijkbaar niets meer van dit form.  (Zie #664) %>

	<% 
        // Deze view kan zowel gebruikt worden voor het aanmaken van nieuwe uitstappen, als voor het bewerken
        // van bestaande
        
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

                        <%
                            // Voor DatumVan en DatumTot gebruiken we niet EditorFor,
                            // maar zetten we hier de letterlijke html-code voor de input
                            // box.
                            // Dit is een workaround voor issue #2700. %>
                    <%=Html.LabelFor(mdl=>mdl.Uitstap.DatumVan) %>
                    <input class="text-box single-line" id="Uitstap_DatumVan" name="Uitstap.DatumVan" type="text" value="<%=Html.DisplayFor(mdl => mdl.Uitstap.DatumVan) %>" />
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.DatumVan) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.DatumTot) %>
                    <input class="text-box single-line" id="Uitstap_DatumTot" name="Uitstap.DatumTot" type="text" value="<%=Html.DisplayFor(mdl => mdl.Uitstap.DatumTot) %>" />
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.DatumTot) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.Opmerkingen) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.Opmerkingen) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.Opmerkingen) %><br />

                    </fieldset>
                    
                <%}%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
