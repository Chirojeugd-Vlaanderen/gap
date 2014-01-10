<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.DeelnemerBewerkenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head">
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
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<%
    using (Html.BeginForm())
    {
        %>
        <ul id="acties">
        <li><input type="submit" value="Bewaren" /></li>
        </ul>

        <fieldset class="invulformulier">
        <legend>Inschrijvingsgegevens</legend>

        <%:Html.PersoonsLink(Model.Deelnemer.GelieerdePersoonID, Model.Deelnemer.VoorNaam, Model.Deelnemer.FamilieNaam) %><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.IsLogistieker) %>
        <%:Html.EditorFor(mdl => mdl.Deelnemer.IsLogistieker)%>
        <%:Html.ValidationMessageFor(mdl => mdl.Deelnemer.IsLogistieker)%><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.HeeftBetaald) %>
        <%:Html.EditorFor(mdl=>mdl.Deelnemer.HeeftBetaald) %>
        <%:Html.ValidationMessageFor(mdl=>mdl.Deelnemer.HeeftBetaald) %><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.MedischeFicheOk) %>
        <%:Html.EditorFor(mdl=>mdl.Deelnemer.MedischeFicheOk) %>
        <%:Html.ValidationMessageFor(mdl=>mdl.Deelnemer.MedischeFicheOk) %><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.Opmerkingen) %>
        <%:Html.EditorFor(mdl=>mdl.Deelnemer.Opmerkingen) %>
        <%:Html.ValidationMessageFor(mdl=>mdl.Deelnemer.Opmerkingen) %><br />

        <%:Html.HiddenFor(mdl=>mdl.Deelnemer.UitstapID) %>

        </fieldset>

        <%
    } %>

</asp:Content>
