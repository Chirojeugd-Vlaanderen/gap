<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.MasterViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%
    /*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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

    <h2>We zijn niet zeker wie je bent</h2>

    <p class="error">
        Om ongevalsaangiften in te dienen of op te volgen, moet je login gekoppeld zijn aan een persoon uit je leidingsploeg.
    </p>
    <p>
        Bij nieuwe logins wordt die koppeling automatisch gemaakt, maar jij hebt de jouwe blijkbaar al lang (of je bent iemand van de GAP-werkgroep).
        <a href="http://chiro.be/eloket/feedback-gap">Neem contact op met de helpdesk om die koppeling in orde te brengen</a>. Vermeld je loginnaam (je wachtwoord niet!), je groep en je AD-nummer (dat vind je op je persoonlijke fiche).
    </p>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
