<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<Chiro.Gap.WebApp.Models.MasterViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1>EVEN WACHTEN!</h1>
    
    <h2>Je riskeert gegevens te verliezen!</h2>
    <p>
        <% string vorigWerkjaar = String.Format("{0}-{1}", Model.HuidigWerkJaar - 1, Model.HuidigWerkJaar); %>
        Het actieve werkjaar is <strong><%:Model.WerkJaarWeergave %></strong>. Als je
        je jaarovergang al deed terwijl dat nog niet de bedoeling was, dan
        (en alleen dan) kun je nog terug naar het vorige werkjaar
        (<%:vorigWerkjaar%>).
    </p>
    
    <h2>Je leden voor <%:Model.WerkJaarWeergave %> worden uitgeschreven!</h2>
    <p>
        Als je hiermee verder gaat, dan gaat je afdelingsverdeling
        voor <%:Model.WerkJaarWeergave %> verloren. Je leden voor
        <%:Model.WerkJaarWeergave %> worden uitgeschreven.
    </p>
    
    <h2>Er is geen weg terug</h2>

    <p>
    Ben je er zeker van dat je de jaarovergang naar <%:Model.HuidigWerkJaar %> ongedaan wilt maken,
    en dat je terug wilt naar <%: vorigWerkjaar %>?        
    </p>

<form method="post">
    <input type="submit" value="100% zeker"/>
</form>

</asp:Content>
