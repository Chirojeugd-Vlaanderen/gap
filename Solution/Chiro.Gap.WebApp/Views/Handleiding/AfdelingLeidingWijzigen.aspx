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
        Handleiding: Afdeling van leiding wijzigen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>De afdeling van leiding aanpassen
    </h2>
    <p>Leiding ingeschreven zonder de afdeling te kiezen? Dat kan vervelend zijn. Omdat de leiding 
        van de jongste afdelingen in het voorjaar het spel voor hun leden moet krijgen, omdat we aspileiding info willen geven over Aspitrant, 
        en omdat dat anders allemaal bij de contactpersoon van je groep aankomt en hij of zij voor de verdere verdeling moet zorgen. Geen nood: 
        dat is makkelijk aan te passen! Ga naar het tabblad Ingeschreven en klik daar op de naam van de leiding. Op de persoonlijke fiche klik 
        je bij Chirogegevens op het potloodicoontje naast Afdelingen. In het pop-up-venstertje vink je de juiste afdeling(en) aan of af en klik je op Bewaren.</p>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Afdeling_van_leiding_wijzigen.png") %>"
        alt="Inschrijvingsgegevens aanpassen" />
</asp:Content>
