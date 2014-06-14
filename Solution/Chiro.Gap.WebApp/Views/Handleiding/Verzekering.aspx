<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
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
	Handleiding: Verzekering
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>Verzekering: ongevalsaangifte</h2>
    <p>
        Je kunt online een ongeval aangeven. Dat moet op de website van de verzekeraar gebeuren, maar je moet wel via het GAP gaan: de GAP-website geeft namelijk (op een veilige manier) door wie je bent en bij welke groep je hoort.
    </p>
    <h3>De aangifte invullen</h3>
    <p>
        Stappen in de procedure:
    </p>
    <ul>
        <li>Zoek eerst het mailadres van het slachtoffer op, want dat heb je nodig. Het is ook handig als je het rekeningnummer van die persoon bij de hand hebt.</li>
        <li>Klik op het tabblad 'Verzekering'.</li>
        <li>Als alles goed gaat, kom je op de website van IC-Verzekeringen terecht. (En anders krijg je een foutmelding met verdere instructies.) </li>
        <li>Klik daar op de knop 'Schadeaangifte'.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/IC-startpagina.png") %>"
        alt="Startpagina ongevalsaangifte bij IC-Verzekeringen" />
    <p>Je krijgt een volgend scherm, waar je informatie over het tijdstip van het ongeval (datum en uur) en over het slachtoffer invult.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/IC-schadeaangifte-stap1.png") %>"
        alt="Stap 1 van de schadeaangifte: info over de schadelijder" />
    <p>Het mailadres van het slachtoffer is nodig om alle documenten door te mailen. Als je het rekeningnummer invult, vermijd je extra briefwisseling in de toekomst.</p>
    <p>
        Let wel: het gaat over de schadelijder, dus niet noodzakelijk over een verzekerde. Mogelijk heeft een derde lichamelijk letsel of materiële schade opgelopen door 
        het handelen of een nalatigheid van een verzekerde (lid, leiding, kampkok, vrijwilliger, enz.) Ook een bezoeker die over een loszittende tegel valt, kan dus een schadelijder zijn.
    </p>
    <p>
        In de volgende stap moet je vragen beantwoorden over het ongeval.: plaats en oorzaak, het letsel, schade van betrokken partijen en vaststelling door de politie. Telkens je een antwoord 
        geeft, zal er een specifieke volgende vraag verschijnen. Wees zo gedetailleerd en volledig mogelijk. Als de verzekeraar contact moet opnemen omdat er gegevens ontbreken, 
        betekent dat altijd tijdverlies. Geef je per ongeluk een fout antwoord, dan kun je met de knop 'Terug' altijd terugkeren en je antwoord nog aanpassen.
    </p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/IC-schadeaangifte-stap2.png") %>"
        alt="Stap 2 van de schadeaangifte: info over het ongeval" />
    <p>Wanneer alle vragen beantwoord zijn, kun je nog documenten (bv. medisch attest) en/of foto's uploaden. Dat helpt om een duidelijk inzicht te krijgen in de omstandigheden van het ongeval en de omvang van de schade.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/IC-schadeaangifte-stap3.png") %>"
        alt="Stap 3 van de schadeaangifte: documenten en foto's uploaden" />
    <p>Klik op de knop 'Verzenden aangifte' om te procedure af te ronden. Op dat moment wordt je schadedossier aangemaakt bij IC-Verzekeringen. Op dat moment wordt er ook twee bevestigingen gemaild: een naar de contactpersoon van je groep, en een naar het adres dat je invulde bij de schadelijder.</p>
    <p>De contactpersoon van je groep krijgt bij die bevestigingsmail een schadefiche, en afhankelijk van de aard en de gevolgen van het ongeval ook enkele gepersonaliseerde documenten (medisch getuigschrift, attest tandletsel, onkostenstaat) en een brief voor de schadelijder. <b>Lees de schadefiche altijd goed!</b> Op dat document staan alle gegevens die je doorgaf en een boodschap over de verdere afhandeling (rode tekst).</p>
    <p>Voor de schadelijder zit er bij de bevestigingsmail een brief met nuttige informatie over de verdere afhandeling van het schadedossier. Naargelang de aard en de gevolgen van het schadegeval zitten ook hier de gepersonaliseerde documenten bij. De schadefiche zit daar niet bij, alleen de contactpersoon van de groep krijgt die. Vulde je geen mailadres in bij de schadelijder, dan moet je hem of haar zelf nog de brief en de gepersonaliseerde documenten bezorgen.</p>
    <p>Komt de mail niet aan of raak je de documenten kwijt, dan vind je ze ook nog altijd op de website van IC-Verzekeringen: zie verder.</p>
    <h3>De aangifte opvolgen</h3>
    <p>Na je aangifte kun je de afhandeling ervan verder volgen. Ook daarvoor klik je op het tabblad Verzekering. Op de website van IC-Verzekeringen klik je dan niet op de knop 'Schadeaangifte' maar op 'Schade-overzicht'. Op het nieuwe scherm selecteer je het kalenderjaar waarin het ongeval gebeurde.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/IC-schadeoverzicht-stap1.png") %>"
        alt="Opvolging van de aangifte: kalenderjaar selecteren" />
    <p>Wanneer je op Volgende klikt, krijg je een overzicht van alle schadegevallen voor jouw groep in het gekozen kalenderjaar.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/IC-schadeoverzicht-stap2.jpg") %>"
        alt="Opvolging van de aangifte: overzicht van aangiften" />
    <p>Klik op 'Bekijk' om de documenten te bekijken. Via de knop 'Opladen' kun je nog extra documenten en/of toevoegen. De kolommen 'Reeds betaald' en 'Ontbrekende documenten' geven je een zicht op de stand van zaken van het dossier.</p>
</asp:Content>
