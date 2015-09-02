<%@ Page Title="Title" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.VoorkeursMailModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
    
<!-- Hier kunnen we nog scripts toevoegen, maar voorlopig is dat niet nodig. -->

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        using (Html.BeginForm())
        {
    %>
    <ul id="acties">
        <li>
            <input type="submit" value="Bewaren" />
        </li>
    </ul>
    
    <%= Html.CheckBoxFor(mdl => mdl.PersoonLidInfo.NieuwsBrief) %> Stuur <%:Model.PersoonLidInfo.PersoonDetail.VolledigeNaam %> de nieuwsbrief op <%=Html.EditorFor(mdl => mdl.EmailAdres) %>.
        <br/>
    <%= Html.ValidationMessageFor(mdl => mdl.EmailAdres) %>

    <%
        } 
    %>
</asp:Content>