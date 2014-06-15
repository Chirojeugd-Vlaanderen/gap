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
    Handleiding: Nieuwe communicatievorm
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Een nieuwe communicatievorm toevoegen</h2>
    <p>
        Met 'communicatievorm' bedoelen we elke manier om op een directe manier met iemand
        in contact te komen. De meest typische voorbeelden zijn een telefoonnummer en een
        mailadres.</p>
    <p>
        Elk van die communicatievormen moet aan bepaalde voorwaarden voldoen. Voor een mailadres
        is dat een technische kwestie: als het geen geldig adres is, kun je het niet gebruiken.
        Voor telefoonnummers hebben we de afspraak gemaakt dat we de notatie van de Chirohuisstijl
        volgen: het zonenummer, een koppelteken, en dan de rest van het nummer in groepjes
        van twee of drie cijfers. In Chiropublicaties kun je bijvoorbeeld zien dat we het
        nummer van het nationaal secretariaat altijd als volgt schrijven: 03-231 07 95.
        Geen haakjes, geen puntjes. Dat is overzichtelijker als je een lijst met telefoonnummers
        nodig hebt.</p>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
        <li>Klik daar op de naam van degene voor wie je een adres wilt toevoegen. Je komt dan
            op de persoonsfiche.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/EditRest_communicatievormenlink.png") %>"
        alt="Een nieuwe communicatoevorm toevoegen vanop de persoonsfiche" />
    <ul>
        <li>Klik daar op de link 'communicatievorm toevoegen'. Je komt dan op een formuliertje.</li></ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Formulier_Nieuwe_communicatievorm.png") %>"
        alt="Een nieuwe communicatievorm toevoegen" />
    <ul>
        <li>Selecteer het type: mailadres, telefoonnummer, enz.</li>
        <li>Vul het nummer of het mailadres in.</li>
        <li>Je kunt aanvinken of de communicatievorm voor het hele gezin
           &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "GezinsgebondenCommunicatievorm", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een gezinsgebonden communicatievorm?" })%>
            is. Een typisch voorbeeld is het nummer van een vaste telefoonlijn: daarmee kun
            je het hele gezin bereiken. Elke nieuwe persoon die je toevoegt voor hetzelfde thuisadres
            krijgt automatisch de gezinsgebonden communicatievormen die je al toevoegde. Nieuwe
            gegevens waar je aanvinkt dat ze voor heel het gezin is, wordt automatisch gekoppeld
            aan iedereen die op hetzelfde adres woont.</li>
        <li>Vul je een mailadres in, dan kun je aanvinken of het ingeschreven mag worden op
            een Snelleberichtenlijst
           &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Nieuwsbrief", new { helpBestand = "Trefwoorden" }, null)%>.
            Dat is dus alleen voor intern Chirogebruik. Chirojeugd Vlaanderen geeft dat soort
            gegevens nooit door aan derden - lees er de
            <%=Html.ActionLink("privacyverklaring", "ViewTonen", new { controller = "Handleiding", helpBestand = "Privacy" })%>
            maar op na.</li>
        <li>Klik op de knop 'Bewaren'.
            <ul>
                <li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
                    je nog zaken kunt aanpassen. Het kan bv. zijn dat het mailadres dat je invulde ongeldig
                    is of dat het telefoonnummer niet volgens de Chirohuisstijl opgesteld is.</li>
                <li class="goed">Als er geen problemen meer zijn, worden de gegevens opgeslagen en keer
                    je terug naar de persoonsfiche. Daar zie je de nieuwe gegevens al staan.</li>
            </ul>
        </li>
    </ul>
</asp:Content>
