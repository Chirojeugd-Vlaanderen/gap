﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
<script type="text/javascript">
    $(function () {
        $('#helpmenu').menu();
    });
</script>

<h2>Handleiding</h2>
<ul id="helpmenu">
    <li><a href="#">Info per tabblad</a>
        <ul>
            <li><%=Html.ActionLink("Jaarovergang", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Jaarovergang" }) %></li>
            <li><%=Html.ActionLink("Ingeschreven", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Ingeschreven" }) %></li>
            <li><%=Html.ActionLink("Iedereen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Iedereen" }) %></li>
            <li><%=Html.ActionLink("Groep", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Groep" }) %></li>
            <li><%=Html.ActionLink("Persoon toevoegen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwePersoon" }) %></li>
            <li><%=Html.ActionLink("Uitstappen/bivak", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Uitstappen" }) %></li>
            <li><%=Html.ActionLink("Verzekering", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Verzekering" }) %></li>
        </ul>
    </li>
    <li><a href="#">Gegevens opzoeken en gebruiken</a>
        <ul>
            <li><%=Html.ActionLink("Overzichten filteren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Filteren" })%></li>
            <li><%=Html.ActionLink("Zoeken in de tabellen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Zoeken" })%></li>
            <li><%=Html.ActionLink("Lijsten downloaden", "ViewTonen", new { Controller = "Handleiding", helpBestand = "LijstDownloaden" })%></li>
            <li><%=Html.ActionLink("Etiketten maken", "ViewTonen", new { Controller = "Handleiding", helpBestand = "EtikettenMaken" })%></li>
            <li><%=Html.ActionLink("Een verjaardagslijst maken", "ViewTonen", new { Controller = "Handleiding", helpBestand = "ExcelVerjaardagslijst" })%></li>
        </ul>
    </li>

    <li><a href="#">Nieuwe gegevens toevoegen</a>
        <ul>
            <li><%=Html.ActionLink("Afdeling", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweAfdeling" }, new { title = "Een nieuwe afdeling aanmaken" }) %></li>
            <li><%=Html.ActionLink("Categorie", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweCategorie" }, new { title = "Een nieuwe categorie aanmaken" })%></li>
            <li><%=Html.ActionLink("Functie", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweFunctie" }, new { title = "Een nieuwe functie aanmaken" })%></li>
            <li><%=Html.ActionLink("Lid/leiding", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwLid" }, new { title = "Gegevens van een nieuw lid toevoegen (= iemand inschrijven)" })%></li>
            <li><%=Html.ActionLink("Persoon", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwePersoon" }, new { title = "Gegevens van een nieuwe persoon toevoegen" })%></li>
            <li><%=Html.ActionLink("Tel./mail/enz.", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweCommunicatievorm"}, new { title = "Tel./mail/enz. toevoegen"}) %></li>
            <li><%=Html.ActionLink("Zus/broer", "ViewTonen", new { Controller = "Handleiding", helpBestand = "ZusBroer" }, new { title = "Gegevens toevoegen voor de zus of broer van iemand die al in je gegevensbestand zit" })%></li>
            <li><%=Html.ActionLink("Adres", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwAdres" }, new { title = "Een nieuw adres toevoegen voor iemand die al in je gegevensbestand zit" })%></li>
            <li><%=Html.ActionLink("Straatnaam", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuweStraatnaam" }, new { title = "Een aanvraag indienen om een straatnaam toe te voegen aan het vaste lijstje" })%></li>
            <li><%=Html.ActionLink("Uitstap/bivak", "ViewTonen", new { Controller = "Handleiding", helpBestand = "UitstapToevoegen" }, new { title = "Een uitstap of bivak registreren" })%></li>
        </ul>
    </li>

    <li><a href="#">Gegevens aanpassen</a>
        <ul>
            <li><%=Html.ActionLink("Persoonlijke- en Chirogegevens", "ViewTonen", new { Controller = "Handleiding", helpBestand = "PersoonlijkeGegevensfiche" })%></li>
            <li><%=Html.ActionLink("Adressen aanpassen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "AdresWijzigen" })%></li>
            <li><%=Html.ActionLink("Mensen in categorieën stoppen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Categoriseren" })%></li>
            <li><%=Html.ActionLink("Leiding veranderen van afdeling", "ViewTonen", new { Controller = "Handleiding", helpBestand = "AfdelingLeidingWijzigen" })%></li>
            <li><%=Html.ActionLink("Iemand uitschrijven", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Uitschrijven" })%></li>
            <li><%=Html.ActionLink("Logins beheren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "GavsBeheren" })%></li>
            <li><%=Html.ActionLink("Groepen fusioneren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Fusioneren" })%></li>
            <li><%=Html.ActionLink("'Speciale' afdelingen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "SpecialeAfdelingen" })%></li>
        </ul>
    </li>

    <li><a href="#">Scenario's</a> 
        <ul>
            <li><%=Html.ActionLink("Gebruikersrechten regelen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "GAVsBeheren" })%></li>
            <li><%=Html.ActionLink("Jaarovergang uitvoeren", "ViewTonen", new { Controller = "Handleiding", helpBestand = "JaarovergangUitvoeren" })%></li>
            <li><%=Html.ActionLink("Ledenadministratie regelen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "NieuwLid" }, new { title = "Gegevens van een nieuw lid toevoegen (= iemand inschrijven)" })%></li>
            <li><%=Html.ActionLink("Bivakaangifte regelen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "UitstapToevoegen" })%></li>
            <li><%=Html.ActionLink("Ongevalsaangifte indienen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Verzekering" })%></li>
        </ul>
    </li>
   
    <li><a href="#">Aanvragen</a>
        <ul>
            <li><%=Html.ActionLink("Verzekering loonverlies", "ViewTonen", new { Controller = "Handleiding", helpBestand = "VerzekeringLoonverlies" }, new { title = "Werkende leiding extra verzekeren" })%></li>
        </ul>
    </li>

    <li><a href="#">Algemeen</a>
        <ul>
            <li><a href="http://www.chiro.be/eloket/feedback-gap">Vragen of feedback?</a></li>
            <li><%=Html.ActionLink("Deadlines", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Deadlines" }) %></li>
            <li><%=Html.ActionLink("Trefwoorden", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Trefwoorden" }) %></li>
            <li><%=Html.ActionLink("Veelgestelde vragen", "ViewTonen", new { Controller = "Handleiding", helpBestand = "VeelgesteldeVragen" }) %></li>
        </ul>
    </li>
    <li><a href="#">Extra info</a>
        <ul>
            <li><%=Html.ActionLink("Privacystatement", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Privacy" })%></li>
            <li><%=Html.ActionLink("Colofon", "ViewTonen", new { Controller = "Handleiding", helpBestand = "Colofon" })%></li>
        </ul>
    </li>
    <li><a href="<%=ResolveUrl("~/Content/media/gap-handleiding.pdf")%>" target="new">Download Pdf-versie</a></li>
</ul>
