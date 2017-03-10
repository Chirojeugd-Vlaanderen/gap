<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersoonEnLidModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%
/*
 * Copyright 2008-2017 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Verfijnen gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
 * Cleanup en refactoring met module pattern: Copyright 2015 Sam Segers
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
    <link href="<%= ResolveUrl("~/Content/print.css") %>" media="print" rel="stylesheet" type="text/css" />

    <script src="<%= ResolveUrl("~/Scripts/jquery-persoons-fiche.js") %>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/Modules/AdresModule.js") %>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/moment.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="printLogo"></div>
    <div id="PersoonsInformatie">
        <% // dialog voor het weergeven van het info-kadertje %>
        <div id="extraInfoDialog" hidden>
            <img src="<%= ResolveUrl("~/Content/images/loading.gif") %>" /></div>
        <input id="groepID" value="<%= Model.GroepID %>" hidden readonly />
        <% if (Model.PersoonLidGebruikersInfo.LidInfo != null)
            { %>
        <input id="lidIdH" value="<%= Model.PersoonLidGebruikersInfo.LidInfo.LidID %>" hidden readonly />
        <input id="gwJaar" value="<%= Model.PersoonLidGebruikersInfo.LidInfo.GroepsWerkJaarID %>" hidden readonly />

        <input id="lidType" value="<%=Model.PersoonLidGebruikersInfo.LidInfo.Type %>" hidden readonly />
        <% }
            else
            { %>
        <input id="lidIdH" value="geenLidID" hidden readonly />
        <input id="gwJaar" value="geenGwJaar" hidden readonly />
        <input id="lidType" value="geenType" hidden readonly />
        <% }%>
        <input id="werkjaar" value="<%= Model.HuidigWerkJaar %>" hidden readonly />
        <input id="GPid" value="<%=Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID  %>" hidden readonly />
        <input id="versieString" value="<%=Model.PersoonLidGebruikersInfo.PersoonDetail.VersieString %>" hidden readonly />

        <div id="afdelingenDialog" hidden>
            <p>Selecteer de afdelingen:</p>
            <fieldset>
            </fieldset>
        </div>

        <div id="commDialog" hidden>
            <form>
                <table>
                    <tr>
                        <td>
                            <select id="selectType">
                            </select>
                        </td>
                        <td>
                            <input id="nummer" required /></td>
                    </tr>
                    <tr id="error" hidden style="background-color: red">
                        <td id="errorTekst" colspan="2"></td>
                    </tr>
                    <tr>
                        <td>Voorkeur:</td>
                        <td>
                            <input id="voorkeurCheck" type="checkbox" /></td>
                    </tr>
                    <tr id="gezin">
                        <td>Voor heel het gezin:</td>
                        <td>
                            <input id="gezinCheck" type="checkbox" /></td>
                    </tr>
                    <tr>
                        <td>Extra info:</td>
                        <td>
                            <textarea id="adresNota" placeholder="Vul hier extra informatie over de communicatievorm in."></textarea></td>
                    </tr>
                </table>
            </form>
        </div>
        <% // PERSOONLIJKE GEGEVENS (LINKERKANT) %>
        <h3>Persoonlijke gegevens</h3>
        <hr />

        <table class="persoonlijkeGegevens">
            <tr>
                <td>Voornaam</td>
                <td>
                    <b id="voornaamInfo"><%=Html.DisplayFor(s => s.PersoonLidGebruikersInfo.PersoonDetail.VoorNaam) %></b>
                </td>
                <td>
                    <div class="ui-icon ui-icon-pencil" title="Bewerken" id="bewerkVoorNaam" style="cursor: pointer"></div>
                </td>
                <td id="voornaamIconen"></td>
            </tr>
            <tr>
                <td>Naam</td>
                <td>
                    <b id="achternaamInfo"><%=Html.DisplayFor(s => s.PersoonLidGebruikersInfo.PersoonDetail.Naam) %></b>
                </td>
                <td>
                    <div class="ui-icon ui-icon-pencil" title="Bewerken" id="bewerkAchterNaam" style="cursor: pointer"></div>
                </td>
                <td id="achternaamIconen"></td>
            </tr>
            <tr>
                <td>Geslacht</td>
                <td id="geslachtsInfo"><%=Html.DisplayFor(s => s.PersoonLidGebruikersInfo.PersoonDetail.Geslacht) %></td>
                <td>
                    <div class="ui-icon ui-icon-pencil" title="Bewerken" id="bewerkGeslacht" style="cursor: pointer"></div>
                </td>
                <td id="geslachtIconen"></td>
            </tr>
            <tr>
                <td>GeboorteDatum</td>
                <td id="gdTekst"></td>
                <td id="gdInput" style="text-align: left" hidden>
                    <input id="gdInfo" value="<%=Html.DisplayFor(s => s.PersoonLidGebruikersInfo.PersoonDetail.GeboorteDatum)%>" />
                </td>
                <td style="text-align: center">
                    <div class="ui-icon ui-icon-pencil" title="Bewerken" id="bewerkGd" style="cursor: pointer"></div>
                </td>
                <td id="gdIconen"></td>
            </tr>

            <% int counter = 0; %>
            <% if (!Model.PersoonLidGebruikersInfo.PersoonsAdresInfo.Any())
                { %>
            <tr id="adressenLeeg">
                <td>Adres</td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <% }
                else
                { %>
            <% foreach (PersoonsAdresInfo pa in ViewData.Model.PersoonLidGebruikersInfo.PersoonsAdresInfo)
                { %>
            <tr id="adressen">
                <% //Hidden fields om gegevens in jQuery uit te kunnen lezen %>

                <% counter += 1; %>
                <td>Adres <%= counter %>
                    <input id="persoonsAdresID" value="<%= pa.ID %>" hidden readonly />
                    <input id="strH" value="<%= pa.StraatNaamNaam %>" hidden readonly />
                    <input id="hsnrH" value="<%= pa.HuisNr %>" hidden readonly />
                    <input id="pstcdH" value="<%= pa.PostNr %>" hidden readonly />
                    <input id="busH" value="<%= pa.Bus %>" hidden readonly />
                    <input id="voorkeursadresID" value="<%=pa.PersoonsAdresID %>" hidden readonly />
                </td>
                <td id="adres">
                    <%= Html.Encode(String.Format("{0} {1}{2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus.IsEmpty()? "" : " bus " + pa.Bus))%>,
                         <%if (!pa.IsBelgisch)
                             { // \note Duplicate code met Uitstappen/Bekijken.aspx%>
                    <%= Html.Encode(String.Format("{0} {1}, {2} ({3}) ", pa.PostCode, pa.WoonPlaatsNaam, pa.LandNaam, pa.AdresType)) %>
                    <% }
                    else
                    { %>
                    <%= Html.Encode(String.Format("{0} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType)) %>
                    <% }%>
                    <% if (Model.PersoonLidGebruikersInfo.PersoonDetail.VoorkeursAdresID == pa.PersoonsAdresID)
                        { %>
                    <div class="ui-icon ui-icon-mail-closed" id="vkAdres" title="Voorkeursadres. Klik voor meer info." style="cursor: pointer"></div>
                    <% } %>
                </td>
                <td>
                    <div class="bewerkAdres ui-icon ui-icon-pencil" title="Bewerken" style="cursor: pointer"></div>
                </td>
                <td>
                    <% if (Model.PersoonLidGebruikersInfo.PersoonDetail.VoorkeursAdresID != pa.PersoonsAdresID)
                        { %>
                    <div class="voorkeursAdresMaken ui-icon ui-icon-home" title="Voorkeursadres maken" style="cursor: pointer"></div>
                    <% } %>
                </td>
                <% } %>
            </tr>
            <% } %>

            <% //E-MAIL EN TELEFOONNUMMER 
                var gegroepeerdeComm = Model.PersoonLidGebruikersInfo.CommunicatieInfo.GroupBy(
                        cv => new
                        {
                            Omschrijving = cv.CommunicatieTypeOmschrijving,
                            Validatie = cv.CommunicatieTypeValidatie,
                            Voorbeeld = cv.CommunicatieTypeVoorbeeld
                        });
            %>
            <tr id="commLeeg">
                <td>Tel./mail/enz. toevoegen</td>
                <td></td>
                <td></td>
                <td id="laadNieuweCom"></td>
            </tr>

            <%
                foreach (var commType in gegroepeerdeComm)
                {
                    int teller = 0;
                    foreach (var cv in commType)
                    {
                        string ctTekst = String.Format(
                            cv.CommunicatieTypeID == (int)CommunicatieTypeEnum.Email ? "<a href='mailto:{0}'>{0}</a>" : "{0}",
                            Html.Encode(cv.Nummer));
                        teller++;

                        // FIXME: dit is gepruts

                        string cvID = (cv.CommunicatieTypeID == (int)CommunicatieTypeEnum.Email ? "email" : "tel") + cv.ID;
                        string tag = cv.Voorkeur ? "strong" : "span";
            %>
            <tr>

                <td><%= commType.Key.Omschrijving + " " + teller %> </td>
                <td title="<%= Html.Encode(cv.Nota) %>">
                    <<%:tag %> id="<%:cvID %>" class="contact"><%=ctTekst %></<%:tag %>>
                    <% if (cv.Nota != null && cv.Nota != string.Empty)
                        { %>
                    <br />
                    (<%= Html.Encode(cv.Nota) %>)
                    <% } %>
                </td>
                
                <td>
                    <% if (cv.IsVerdacht)
                        { %>
                            <div id="controlestatus" class="uitlegIsVerdacht ui-icon ui-icon-alert" title="Mailadres ziet er verdacht uit" style="cursor: pointer"></div>
                            &nbsp;
                        <% } %>
                    <div class="contactBewerken ui-icon ui-icon-pencil" title="Bewerken" style="cursor: pointer"></div>
                </td>
                <td>
                    <div class="contactVerwijderen ui-icon ui-icon-circle-minus" title="Verwijderen" style="cursor: pointer" id="cverw<%: cvID %>"></div>
                </td>
            </tr>
            <% } %>

            <% } %>
        </table>
        <br />

        <%//CHIROGEGEVENS %>
        <h3>Chirogegevens</h3>
        <hr />

        <table class="chiroGegevens">
            <% if (Model.PersoonLidGebruikersInfo.PersoonDetail.AdNummer != null)
                { %>
            <tr>
                <td><%= Html.LabelFor(s => s.PersoonLidGebruikersInfo.PersoonDetail.AdNummer) %><%= Html.InfoLink("AD-Info") %></td>
                <td id="adNrInfo"><%= Html.DisplayFor(s => s.PersoonLidGebruikersInfo.PersoonDetail.AdNummer) %></td>
                <td id="ad"></td>
            </tr>
            <% } %>
            <% if ((Model.GroepsNiveau & Niveau.Groep) != 0)
                { // Chiroleeftijd is enkel relevant voor plaatselijke groepen 
            %>
            <tr>
                <td>Chiroleeftijd<%= Html.InfoLink("clInfo") %></td>
                <td><a id="chiroleeftijdInfo" data-type="select"><%= Html.DisplayFor(s => s.PersoonLidGebruikersInfo.PersoonDetail.ChiroLeefTijd) %></a></td>
                <td>
                    <div class="ui-icon ui-icon-pencil" id="bewerkCl" title="Bewerken" style="cursor: pointer"></div>
                </td>
            </tr>
            <% } %>

            <% 
                // Omdat ik die JQuery-toestanden in zijn huidige vorm zodanig
                // moeilijk te onderhouden vind, maak ik gewoon saaie actionlinks
                // om abonnementen te bewerken. Dat werkt ook.
                // Van zodra we een framework gebruiken voor JQuery, klappen we
                // nog eens :) %>

            <tr>
                <td>Dubbelpunt</td>
                <td>
                    <%= Html.DisplayFor(mdl => mdl.PersoonLidGebruikersInfo.DubbelpuntAbonnement) %>
                </td>
                <td>
                    <%:Html.ActionLink("[Wijzig]", "Dubbelpunt", new { id = Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %>
                </td>
            </tr>
            <tr>
                <td><%= Html.LabelFor(mdl => mdl.PersoonLidGebruikersInfo.PersoonDetail.NieuwsBrief) %> <%= Html.InfoLink("snelBerichtInfo")%></td>
                <td>
                    <%: Model.PersoonLidGebruikersInfo.PersoonDetail.NieuwsBrief ? "ja" : "nee" %>
                </td>
                <td>
                    <%:Html.ActionLink("[Wijzig]", "NieuwsBrief", new { id = Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %>
                </td>
            </tr>


            <% // controleert of de persoon ingeschreven is %>
            <%if ((Model.PersoonLidGebruikersInfo.PersoonDetail.IsLid || Model.PersoonLidGebruikersInfo.PersoonDetail.IsLeiding) &&
                                                        !Model.PersoonLidGebruikersInfo.LidInfo.NonActief)
                { %>
            <% // Plaatselijke groep: toon lid/leiding en afdeling(en)
                if ((Model.GroepsNiveau & Niveau.Groep) != 0)
                {
                    // Ingeschreven als lid of leiding:
            %>
            <tr>
                <td>Ingeschreven als</td>
                <td><a id="lidTypeOmschrijving" data-type="select" style="font-weight: bold;"><%= Model.PersoonLidGebruikersInfo.LidInfo.Type == LidType.Kind ? "Lid" : "Leiding" %></a>
                    <%: Html.ActionLink("(uitschrijven)", "Uitschrijven", new { gelieerdePersoonID = Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %></td>
                <td>
                    <div class="ui-icon ui-icon-pencil" id="typeToggle" title="Bewerken" style="cursor: pointer"></div>
                </td>
            </tr>

            <tr>
                <% 

                    // In afdeling:

                    if (Model.PersoonLidGebruikersInfo.LidInfo.Type == LidType.Leiding)
                    {
                        // Leiding heeft meerdere afdelingen
                %>
                <td>Afdeling(en)</td>
                <td id="afdelingInfo"><%=Html.PrintLijst(Model.PersoonLidGebruikersInfo.LidInfo.AfdelingIdLijst, Model.AlleAfdelingen) %></td>
                <td>
                    <div class="ui-icon ui-icon-pencil" id="bewerkAfdeling" title="Bewerken" style="cursor: pointer"></div>
                </td>
                <% 
                    }
                    else
                    {
                        // TODO: Opkuis. Een lid heeft maar 1 afdeling; hoe moeilijk kan het zijn.

                        if (Model.PersoonLidGebruikersInfo.LidInfo.AfdelingIdLijst.Count > 0 &&
                            Model.AlleAfdelingen.FirstOrDefault(
                                s => s.AfdelingID == Model.PersoonLidGebruikersInfo.LidInfo.AfdelingIdLijst.ElementAt(0)) != null)
                        { %>
                <td>Afdeling</td>
                <td>
                    <a id="afdelingInfo" data-type="select">
                        <%=Model.AlleAfdelingen.First(s => s.AfdelingID == Model.PersoonLidGebruikersInfo.LidInfo.AfdelingIdLijst.ElementAt(0)).AfdelingNaam %>
                    </a>
                </td>
                <td>
                    <div class="ui-icon ui-icon-pencil" id="bewerkAfdeling" title="Bewerken" style="cursor: pointer"></div>
                </td>
                <%  
                        }
                    }
                %>
            </tr>

            <% }
                else
                {
                    // Maar we moeten kaderleden wel kunnen uitschrijven%>
            <tr>
                <td>Ingeschreven</td>
                <td />
                <td><%: Html.ActionLink("[Uitschr.]", "Uitschrijven", new { gelieerdePersoonID = Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %></td>
            </tr>
            <% } %>
            <% // Geeft alle functies van een persoon weer %>
            <tr>
                <td>Functies</td>
                <% if ((Model.PersoonLidGebruikersInfo.LidInfo.Functies).Count == 0)
                    { %>
                <td><b>Geen</b></td>
                <% }
                    else
                    { %>
                <td>
                    <% foreach (var f in Model.PersoonLidGebruikersInfo.LidInfo.Functies)
                        { %>

                    <%= Html.ActionLink(f.Code, "Functie", "Leden",
                                                            new
                                                            {
                                                                groepsWerkJaarID = Model.PersoonLidGebruikersInfo.LidInfo.GroepsWerkJaarID,
                                                                id = f.ID,
                                                                groepID = Model.GroepID,
                                                            },
                                                            new { title = f.Naam }) %>

                    <% } %>
                </td>

                <% } %>
                <td>
                    <div class="ui-icon ui-icon-pencil" id="bewerkFuncties" title="Bewerken" style="cursor: pointer"></div>
                </td>
            </tr>
            <% // Controleert of het lidgeld betaald werd
                if ((Model.GroepsNiveau & (Niveau.Gewest | Niveau.Verbond | Niveau.Nationaal)) == 0)
                { %>
            <tr>
                <td>Lidgeld <%=Html.InfoLink("lidgeldInfo") %></td>
                <td id="lidgeldInfo">
                    <% if (Model.PersoonLidGebruikersInfo.LidInfo.LidgeldBetaald == true)
                        { %>
                    <b>Betaald</b>
                    <% }
                        else
                        { %>
                    <b>Nog niet betaald</b>
                    <% } %>
                </td>
                <td>
                    <div class="ui-icon ui-icon-pencil" id="bewerkLidgeld" title="Bewerken" style="cursor: pointer"></div>
                </td>
            </tr>
            <% } %>

            <% if ((Model.GroepsNiveau & (Niveau.Gewest | Niveau.Verbond | Niveau.Nationaal)) == 0)
                {
                    // Lidgeld en instapperiode zijn niet van toepassing op kadergroepen. %>
            <tr>
                <td>Instapperiode<%=Html.InfoLink("instapperiodeInfo") %></td>
                <td><%= String.Format(
                              Model.PersoonLidGebruikersInfo.LidInfo.EindeInstapperiode < DateTime.Today ? "Verliep op {0:d}" : "tot {0:d}",
                                     Model.PersoonLidGebruikersInfo.LidInfo.EindeInstapperiode)  %>
                </td>
                <td id="instap"></td>
            </tr>
            <% } %>

            <% if (Model.PersoonLidGebruikersInfo.LidInfo.Type == LidType.Leiding)
                { %>

            <div id="bewerkVerzekeringDialog"></div>
            <tr>
                <td>Verz. tegen loonverlies <%=Html.InfoLink("loonVerlies") %></td>
                <% //controleert verzekering tegen loonverlies 
                    if (Model.PersoonLidGebruikersInfo.LidInfo.VerzekeringLoonVerlies)
                    {%>
                <td>
                    <b>Ja</b>
                </td>
                <td></td>
                <% }
                    else if (Model.PersoonLidGebruikersInfo.PersoonDetail.GeboorteDatum != null && Model.KanVerzekerenLoonVerlies)
                    { %>
                <td>
                    <b>Nee</b>
                </td>
                <td>
                    <%=Html.ActionLink("[Verzeker]", "LoonVerliesVerzekeren", new { Controller = "Leden", id = Model.PersoonLidGebruikersInfo.LidInfo.LidID, groepID = Model.GroepID }) %>
                    <% if (Model.GroepsNiveau.HasFlag(Niveau.KaderGroep))
                        { %>
              (dit is gratis voor kaderleden)
            <% } %>
                </td>
                <% }
                    else
                    {%>
                <td>
                    <b>Kan niet verzekeren tegen loonverlies</b>
                </td>
                <td></td>
                <% } %>
                <% } %>
            </tr>

            <% }
                else
                {
                    // Dit wordt weergegeven wanneer de persoon niet ingeschreven is in het huidige werkjaar%>
            <p>
                <span style="color: red">Niet ingeschreven in het huidige werkjaar.</span>
                <br />
                <button id="btn_inschrijven">Inschrijven</button>
            </p>

            <% } %>
        </table>
        <br />
        <br />
        <br />
        <button id="terug"><% Html.RenderPartial("TerugNaarLijstLinkControl"); %></button>
    </div>

    <%//OVERIGE INFORMATIE (RECHTERKANT) %>
    <div class="opzij">
        <h3>Overige gegevens</h3>
        <hr />
        <h3>GAP-account</h3>
        <%
            if (Model.PersoonLidGebruikersInfo.GebruikersInfo == null)
            {
                // Geen account
        %>
                    Geen Chirologin.
                    <p><%: Html.ActionLink("Chirologin maken", "LoginMaken", new { Controller = "GebruikersRecht", id = Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %></p>
        <%
            }
            else if (Model.PersoonLidGebruikersInfo.GebruikersInfo.VervalDatum < DateTime.Now
            || Model.PersoonLidGebruikersInfo.GebruikersInfo.GebruikersRecht.GroepsPermissies == Permissies.Geen
            || Model.PersoonLidGebruikersInfo.GebruikersInfo.GebruikersRecht.IedereenPermissies == Permissies.Geen)
            {
                // Account zonder (gav-)gebruikersrecht
        %>
        <p><b>Chirologin: </b><%: Model.PersoonLidGebruikersInfo.GebruikersInfo.Login %></p>
        <p>
            Geen toegang tot de gegevens van jouw groep.
            <br />
            <%: Html.ActionLink("Gebruikersrecht toekennen", "AanGpToekennen", new { Controller = "GebruikersRecht", id = Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %>
        </p>
        <%
            }
            else
            {
                // Account met gebruikersrecht
                // DisplayFor formatteert datums correct.
        %>
        <ul>
            <li>Chirologin: <%: Model.PersoonLidGebruikersInfo.GebruikersInfo.Login %></li>
            <li>Vervaldatum gebruikersrecht: <%: Html.DisplayFor(src => src.PersoonLidGebruikersInfo.GebruikersInfo.VervalDatum) %></li>
            <%
                if (Model.PersoonLidGebruikersInfo.GebruikersInfo.IsVerlengbaar)
                {
                    // gebruikersrecht toekennen/verlengen is onderliggend dezelfde controller action
            %>
            <!--<button id="Button1">Gebruikersrecht toekennen</button>-->
            <li><%: Html.ActionLink("Gebruikersrecht verlengen", "AanGpToekennen", new { Controller = "GebruikersRecht", id = ViewData.Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %></li>
            <%
                }
            %>
            <li><%: Html.ActionLink("Gebruikersrecht afnemen", "VanGpAfnemen", new { Controller = "GebruikersRecht", id = ViewData.Model.PersoonLidGebruikersInfo.PersoonDetail.GelieerdePersoonID }) %></li>
        </ul>
        <%
            }
        %>

        <h3>Toegevoegd aan de volgende categorie&euml;n:</h3>
        <table>
            <% if (!Model.PersoonLidGebruikersInfo.PersoonDetail.CategorieLijst.Any())
                { %>
            <p>Geen</p>
            <% }
                foreach (var info in Model.PersoonLidGebruikersInfo.PersoonDetail.CategorieLijst)
                { %>
            <tr>
                <td>
                    <%=Html.ActionLink(String.Format("{0} ({1})", info.Naam, info.Code),
                    "List",
                    "Personen",
                    new { id = info.ID, groepID = Model.GroepID },
                    new { title = info.Naam })%>
                    <input id="catID" value="<%=info.ID %>" hidden />
                </td>
                <td>

                    <div class="catVerw ui-icon ui-icon-circle-minus" title="Verwijderen" style="cursor: pointer"></div>
                </td>
            </tr>
            <% } %>
        </table>
        <br />
        <button id="toevoegenAanCat">Toevoegen aan categorieën</button>
        <br />

        <h3>Opties</h3>
        <hr />
        <button id="print">Print deze pagina</button>
    </div>
</asp:Content>
