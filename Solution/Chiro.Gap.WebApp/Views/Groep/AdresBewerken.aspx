<%@ Page Title="Adres lokalen bewerken" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GroepsAdresModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
<script src="<%= ResolveUrl("~/Scripts/AdresBewerken.js") %>" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {

        // Ehh...
        
        $('#tabel').show();
        $('#postCode').hide();
        $('#woonplaatsBuitenland').hide();

       AdresBewerken();
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="kaderke">
        <div class="kadertitel">Adres van de lokalen</div>
        <% 
            using (Html.BeginForm())
            {%>
        <ul id="acties">
            <li>
                <input type="submit" name="action" value="Bewaren" /></li>
        </ul>
        <p>
			<strong>Opgelet:</strong> voor binnenlandse adressen wordt alleen de officiële spelling van de straatnaam geaccepteerd.<br />
			Ben je zeker van de straatnaam maar wordt ze geweigerd? Lees in
			<%=Html.ActionLink("de handleiding", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweStraatnaam"})%>
			hoe we daar een mouw aan kunnen passen.</p>
            <% =Html.ValidationSummary() %>
            
            <table>
            <% Html.RenderPartial("AdresBewerkenControl", Model); %>
            </table>
        <%
}%>
    </div>
</asp:Content>


