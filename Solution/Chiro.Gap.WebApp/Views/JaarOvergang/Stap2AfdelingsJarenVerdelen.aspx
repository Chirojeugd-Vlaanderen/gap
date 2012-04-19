<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<JaarOvergangAfdelingsJaarModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #697) %>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%=Html.ActionLink("Terug naar stap 1", "Stap1AfdelingenSelecteren", new { Controller = "JaarOvergang" })%>
    <br />
    <br />
    <%using (Html.BeginForm("Stap2AfdelingsJarenVerdelen", "JaarOvergang"))
      { %>
    <table>
        <tr>
            <th>Afdeling</th>
            <th>Officiële afdeling</th>
            <th>Vanaf geboortejaar </th>
            <th>Tot geboortejaar </th>
            <th>Geslacht </th>
        </tr>
        <% for (int j = 0; j < Model.Afdelingen.Count(); ++j)
           { %>
        <tr>
            <td>
                <%: Html.HiddenFor(mdl => mdl.Afdelingen[j].AfdelingID) %>
                <%: Html.LabelFor(mdl => mdl.Afdelingen[j].AfdelingNaam) %>
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
                        Enum.GetValues(typeof (GeslachtsType)).OfType<GeslachtsType>().ToList().Select(
                            e =>
                            new SelectListItem
                            {
                               Selected = (Model.Afdelingen[j].Geslacht == e),
                               Value = ((int) e).ToString(),
                               Text = e.ToString()
                            });%>
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
        <%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Instapperiode", new { helpBestand = "Trefwoorden" }, new { title = "Wat is die instapperiode?" } ) %>.
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
