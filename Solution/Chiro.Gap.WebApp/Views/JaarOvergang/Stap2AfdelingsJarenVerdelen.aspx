﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<JaarOvergangAfdelingsJaarModel>" %>

<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%
/*
 * Copyright 2008-2013, 2015 the GAP developers. See the NOTICE file at the 
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
    <% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #697) %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.ValidationSummary() %>

    <br />
    <%using (Html.BeginForm("Stap2AfdelingsJarenVerdelen", "JaarOvergang"))
      { %>

    <table>
        <tr>
            <th>Afdeling</th>
            <th>Officiële afdeling</th>
            <th>Geboren vanaf </th>
            <th>tot en met </th>
            <th>Geslacht </th>
        </tr>
        <% for (var j = 0; j < Model.Afdelingen.Count(); ++j)
           { %>
        <tr>
            <td>

                <%: Html.HiddenFor(mdl => mdl.Afdelingen[j].AfdelingID) %>
                <%: Model.Afdelingen[j].AfdelingNaam %>
            </td>
            <td>
                <%
                    var officieleAfdelingLijstItems = (from oa in Model.OfficieleAfdelingen
                                                          select new SelectListItem
                                                             {
                                                                 Selected =
                                                                     (Model.Afdelingen[j].OfficieleAfdelingID == oa.ID),
                                                                 Text = oa.Naam,
                                                                 Value = oa.ID.ToString()
                                                             }).ToArray();
                %>
                <%: Html.DropDownListFor(mdl=>mdl.Afdelingen[j].OfficieleAfdelingID, officieleAfdelingLijstItems) %>
            </td>
            <td>
                <%: Html.EditorFor(mdl=>mdl.Afdelingen[j].GeboorteJaarVan) %>
            </td>
            <td>
                <%: Html.EditorFor(mdl=>mdl.Afdelingen[j].GeboorteJaarTot) %>
            </td>
            <td>
                <%
                    var geslachtsLijstItems =
                        Enum.GetValues(typeof (GeslachtsType))
                            .OfType<GeslachtsType>() // hackje om de array queryable te maken
                            .Where(e => e != GeslachtsType.X && e != GeslachtsType.Onbekend) // Voor het derde geslacht moet/mag er geen aparte afdeling zijn.
                            .Select(
                                e =>
                                    new SelectListItem
                                    {
                                        Selected = (Model.Afdelingen[j].Geslacht == e),
                                        Value = ((int) e).ToString(),
                                        Text = e.ToString()
                                    }).ToArray();%>
                <%: Html.DropDownListFor(mdl=>mdl.Afdelingen[j].Geslacht, geslachtsLijstItems) %>
            </td>
        </tr>
        <% } %>
    </table>
    <br />
    <%: Html.RadioButtonFor(mdl => mdl.LedenMeteenInschrijven, true)%>
    Verdergaan en een lijst weergeven van alle huidige leden om deze in te schrijven
    in het nieuwe jaar.  <strong>Dit kan een paar minuutjes duren!</strong>
    <br />
    <%: Html.RadioButtonFor(mdl => mdl.LedenMeteenInschrijven, false)%>
    Jaarovergang afmaken, ik zal de leden later zelf herinschrijven.
    <br />
    <input id="volgende" type="submit" value="Verdeling bewaren en verdergaan" />
    <%} %>
    <p>
        De leden en de leiding van vorig jaar wordt automatisch opnieuw ingeschreven.
        Ze krijgen daarbij opnieuw een instapperiode
       &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is die instapperiode?" } ) %>.
        Je hebt dus drie weken (of tot 15 oktober) de tijd om de nodige mensen uit te
        schrijven, zodat je geen factuur krijgt voor hun aansluiting. Achteraf kun je
        nog inschrijven wie je wilt, het hele jaar door.</p>
    <p>
        Ter informatie de &lsquo;standaardafdelingen&rsquo; voor dit werkjaar:
    </p>
    <table>
        <!--TODO exentsion method die gegeven een werkjaar, het standaardgeboortejaar berekent. Nu is het niet correct. -->
        <%  foreach (var oa in Model.OfficieleAfdelingen.Where(ofaf => ofaf.ID != (int)NationaleAfdeling.Speciaal).OrderBy(ofaf => ofaf.LeefTijdTot))
            {%>
        <tr>
            <td>
                <%=oa.Naam %>
            </td>
            <td>
                <%=oa.StandaardGeboorteJaarVan(Model.NieuwWerkjaar) %>-<%=oa.StandaardGeboorteJaarTot(Model.NieuwWerkjaar)%>
            </td>
        </tr>
        <%}%>
    </table>
    <p>
        <%=Html.ActionLink("Meer weten over afdelingen die een speciaal geval zijn?", "ViewTonen", new { Controller = "Handleiding", helpBestand = "SpecialeAfdelingen" })%></p>
</asp:Content>
