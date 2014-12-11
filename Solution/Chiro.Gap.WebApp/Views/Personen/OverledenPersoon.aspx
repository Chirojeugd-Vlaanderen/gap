<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersoonEnLidModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Persoonlijke gegevens</h3>
    <p>
        <%= Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.VolledigeNaam) %>
        (<%=Html.Geslacht(Model.PersoonLidInfo.PersoonDetail.Geslacht) %>)
        <br />
        <%if (Model.PersoonLidInfo.PersoonDetail.AdNummer != null)
          {%>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer) %>&nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "AD-nummer", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een Civi-ID?" } ) %>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer) %><br />
        <%
            }%>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>
        <br />
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.SterfDatum)%>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.SterfDatum)%>
        <br />
        <%=Html.ActionLink("[persoonlijke gegevens aanpassen]", "Nieuw", new {id=Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID}) %><br />
    </p>
    <h3>
        Laatst gekende adressen</h3>
    <ul>
        <% foreach (PersoonsAdresInfo pa in ViewData.Model.PersoonLidInfo.PersoonsAdresInfo)
           { %>
        <li>
            <%=Html.Encode(String.Format("{0} {1} {2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus))%>,
            <%=Html.Encode(String.Format("{0} {3} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType, pa.PostCode))%>
            <%=Html.ActionLink("[verwijderen]", "AdresVerwijderen", new { id = pa.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
        </li>
        <%} %>
    </ul>
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
