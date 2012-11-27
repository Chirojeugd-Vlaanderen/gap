<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GavOverzichtModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<table>
    <tr>
        <th>Login</th>
        <th>Naam</th>
        <th>Vervaldatum</th>
        <th>Actie</th>
    </tr>
    <%
        foreach (var gebr in Model.GebruikersDetails.Where(det => det.VervalDatum >= DateTime.Now))
        {
            %>
    <tr>
        <td><%:gebr.GavLogin %></td>
        <td><%=gebr.PersoonID > 0 ? Html.ActionLink(String.Format("{0} {1}", gebr.VoorNaam, gebr.FamilieNaam), "EditRest", new { Controller = "Personen", id = gebr.GelieerdePersoonID}).ToHtmlString() : "(onbekend)" %></td>
        <td><%:gebr.VervalDatum == null ? "nooit" : ((DateTime)gebr.VervalDatum).ToString("d") %></td>
        <td>
            <% if (gebr.IsVerlengbaar)
                { // gebruikersrecht toekennen/verlengen is onderliggend dezelfde controller action
            %>
              <%=Html.ActionLink("Verlengen", "AanmakenOfVerlengen", new { gebruikersNaam = gebr.GavLogin }) %>
            <%              
                }

                if (Model.GebruikersDetails.Count() > 1)
                {
%>
              <%= Html.ActionLink("Afnemen", "Intrekken", new { gebruikersNaam = gebr.GavLogin })%>
            <% } %>
        </td>
    </tr>

            <%
        }
    %>
</table>

<p>TIP: Je kunt een gebruiker bijmaken door op een personenfiche op &lsquo;gebruikersrecht toekennen&rsquo; te klikken. Het e-mailadres van die persoon moet wel ingevuld zijn, anders kan het wachtwoord natuurlijk niet doorgemaild worden.</p>

</asp:Content>

