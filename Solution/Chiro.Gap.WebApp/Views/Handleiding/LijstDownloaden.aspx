<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
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
    Handleiding: Lijst downloaden
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>Een lijst downloaden</h2>
    <p>De gegevens van je groep worden natuurlijk pas echt bruikbaar als je er meer
        mee kunt doen dan ze alleen online bekijken. Daarom kun je allerlei lijstjes
        downloaden als Excel-bestand. Je kunt ze dan openen in Excel van Microsoft,
        Calc van LibreOffice, en misschien ook nog in andere spreadsheetprogramma's.
        Heb je geen programma waarmee je het bestand kunt openen? Calc van <a href="http://www.libreoffice.org/download/" target="new">
        LibreOffice</a> is gratis.</p>
    <p>
        Microsoft Office voor Mac kan blijkbaar problemen hebben met de geboortedatums,
        daar moet je dus mee opletten.</p>
    <p>In het overzicht van ingeschreven personen kan je de lijst als een Excel- of PDF-bestand downloaden via de knoppen linksboven
    in de tabel. </p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/LijstDownloaden_knoppen.png") %>" alt="Dowloaden via knoppen" />
    <p>OPGELET: de lijst die je hier downloadt bevat alleen de personen die op dat moment in de tabel staan! Als je al je leden wil exporteren, 
    zorg er dan voor dat je zoekbalk leeg is en er op de tabel geen filters toegepast zijn. Kijk onderaan de tabel om te kijken
    hoeveel personen er op dat moment weergegeven worden.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/LijstDownloaden_AantalPersonen.png") %>" alt="Dowloaden via knoppen" />
    <p>In het overzicht 'Iedereen' kan je een lijst downloaden van iedereen die ooit aan het GAP voor jou groep toegevoegd
    werd via de link 'Lijst downloaden'.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/LijstDownloaden_Link.png") %>" alt="Dowloaden via link" />
    
    <p>
        De lijsten die je kunt downloaden, zijn opgemaakt in het formaat van Office
        2007. Als je Office 2003 hebt en je krijgt het bestand niet open, dan moet je
        eerst een <a href="http://www.microsoft.com/downloads/details.aspx?familyid=941b3470-3ae9-4aee-8f43-c6bb74cd1466&displaylang=nl" target="new" 
            title="Hulpprogramma om Office 2007-bestanden te kunnen openen in Office 2003">
            extra programma</a> installeren. Zorg ook dat je <a href="http://update.microsoft.com" target="new">
                de recentste updates van Office</a> geïnstalleerd hebt. Van LibreOffice
        heb je minstens versie 3.2 nodig. Gebruik de links in de eerste alinea om die
        te downloaden.
    </p>
    <p>
        Als je in de kolom met geboortedata alleen '#####' ziet staan, dan betekent dat
        gewoon dat de kolom te smal is. Er is dus niets foutgelopen. Maak de kolom wat
        breder om de data te zien.</p>
    <p>
        Lijsten die je kunt downloaden:</p>
    <ul>
        <li>Alle personen
            <ul>
                <li>Klik op het tabblad 'Iedereen'</li>
                <li>Klik op de link 'Lijst downloaden'</li>
            </ul>
        </li>
        <li>Alle ingeschreven leden
            <ul>
                <li>Klik op het tabblad 'Ingeschreven'</li>
                <li>Klik op
                    <ul>
                        <li>'Opslaan als PDF' voor de PDF-versie</li>
                        <li>'Opslaan als Excel' voor een Excel bestand. (dit bestand heeft de extensie '.csv')</li>
                    </ul>
                </li>
            </ul>
        </li>
        <li>De leden en leiding van één bepaalde afdeling
            <ul>
                <li>Klik op het tabblad 'Ingeschreven'</li>
                <li>Selecteer rechts van de tabel de afdeling die je nodig hebt (en klik eventueel
                    nog op de knop 'Afdeling bekijken' - als die er niet staat, wordt de lijst automatisch
                    gefilterd)</li>
                <li>Even wachten tot de lijst gefilterd is...</li>
                <li>Klik op de link 'Lijst downloaden'</li>
            </ul>
        </li>
        <li>De personen in een bepaalde categorie (bv. de kookploeg)
            <ul>
                <li>Klik op het tabblad 'Iedereen'</li>
                <li>Selecteer rechts van de tabel de categorie die je nodig hebt (en klik eventueel
                    nog op de knop 'Categorie bekijken' - als die er niet staat, wordt de lijst
                    automatisch gefilterd)</li>
                <li>Even wachten tot de lijst gefilterd is...</li>
                <li>Klik op de link 'Lijst downloaden'</li>
            </ul>
        </li>
    </ul>
</asp:Content>
