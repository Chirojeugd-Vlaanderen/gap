<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapPlaatsBewerkenModel>"
    MasterPageFile="~/Views/Shared/Site.Master" %>

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
        <div class="kadertitel">
            <%=Model.Uitstap.IsBivak ? "Details bivak" : "Details uitstap" %></div>
        <p>
            <em>Periode:</em>
            <%=String.Format("{0:d}", Model.Uitstap.DatumVan) %>
            -
            <%=String.Format("{0:d}", Model.Uitstap.DatumTot)%>
        </p>
        <p>
            <%=Model.Uitstap.Opmerkingen %>
        </p>
        <% 
            // Ik neem in het form hidden de niet-wijzigbare informatie uit het model op.  Op die manier is die
            // gemakkelijk beschikbaar als er zich validatiefouten voordoen.

            using (Html.BeginForm())
            {%>
        <ul id="acties">
            <li>
                <input type="submit" name="action" value="Bewaren" /></li>
        </ul>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.Naam) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.IsBivak) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.DatumVan) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.DatumTot) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.Opmerkingen) %>
        <p>
            <%=Html.LabelFor(mdl => mdl.Uitstap.PlaatsNaam)%>
            <%=Html.EditorFor(mdl => mdl.Uitstap.PlaatsNaam)%>
            <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.PlaatsNaam)%>
        </p>
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
