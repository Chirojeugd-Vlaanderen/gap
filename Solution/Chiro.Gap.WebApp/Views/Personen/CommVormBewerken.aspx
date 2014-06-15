<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<CommVormModel>" %>

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
    <%-- ReSharper disable SelfClosedScript --%>
    <%-- ReSharper disable Asp.DeadCode --%>
    <!-- OPGELET! Voor scripts is een expliciete closing tag nodig, anders krijg je problemen (zie #677)
    	     Dus niet <script blablabla />!.  -->
    <%-- ReSharper restore Asp.DeadCode --%>
    <%-- ReSharper restore SelfClosedScript --%>
    <script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% 
        using (Html.BeginForm())
        {
    %>
    <ul id="acties">
        <li>
            <input type="submit" value="Bewaren" /></li>
    </ul>
    <fieldset>
        <legend>Communicatievorm bewerken voor
            <%=Model.Aanvrager.VolledigeNaam %></legend>
        <%=Html.ValidationSummary() %>
        <table>
            <tr>
                <td>
                    <%=Model.NieuweCommVorm.CommunicatieTypeOmschrijving%>:
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nummer) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nummer)%>
                </td>
            </tr>
            <tr>
                <td>
                    <%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsVoorOptIn)%><%= Html.InfoLink("snelBerichtInfo")%>
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsVoorOptIn) %>
                </td>
                <td />
            </tr>
            <tr>
                <td>
                    <%=Html.LabelFor(mdl => mdl.NieuweCommVorm.Voorkeur)%>
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Voorkeur)%>
                </td>
            </tr>
            <tr>
                <td>
                    <%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden)%>
                </td>
            </tr>
            <tr>
                <td>
                    Extra informatie
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nota) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nota)%>
                </td>
            </tr>
        </table>
        <%=Html.HiddenFor(mdl=>mdl.NieuweCommVorm.ID) %>
        <%=Html.HiddenFor(mdl=>mdl.NieuweCommVorm.VersieString) %>
    </fieldset>
    <%
        } %>
</asp:Content>
