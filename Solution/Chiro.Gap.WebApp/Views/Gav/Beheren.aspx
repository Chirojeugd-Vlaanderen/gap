<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GavOverzichtModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<table>
    <tr>
        <th>login</th>
        <th>naam</th>
        <th>vervaldatum</th>
        <th>acties</th>
    </tr>
    <%
        foreach (var gebr in Model.GebruikersDetails.Where(det => det.VervalDatum >= DateTime.Now))
        {
            %>
    <tr>
        <td><%:gebr.GavLogin %></td>
        <td><%=Html.ActionLink(String.Format("{0} {1}", gebr.VoorNaam, gebr.FamilieNaam), "EditRest", new { Controller = "Personen", id = gebr.GelieerdePersoonID}) %></td>
        <td><%:gebr.VervalDatum == null ? "nooit" : ((DateTime)gebr.VervalDatum).ToString("d") %></td>
        <td>
            <%
                if (gebr.Verlengbaar)
                {
                    // gebruikersrecht toekennen/verlengen is onderliggend dezelfde controller action
            %>
              <%=Html.ActionLink("verlengen", "Toekennen", new { Controller = "GebruikersRecht", id = gebr.GelieerdePersoonID }) %>
            <%                
                    
                }
            %>
              <%=Html.ActionLink("afnemen", "Afnemen", new { Controller = "GebruikersRecht", id = gebr.GelieerdePersoonID }) %></>
        </td>
    </tr>
            <%
        }
    %>
</table>

</asp:Content>
