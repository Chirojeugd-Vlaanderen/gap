﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapOverzichtModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="kaderke">
        <div class="kadertitel">
            Overzicht uitstappen</div>
        <p>
            Informatie over uitstappen wordt niet doorgegeven aan Chirojeugd Vlaanderen. Van
            een kamp wordt alleen de informatie voor de bivakaangifte
           &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Bivakaangifte", new { helpBestand = "Trefwoorden" }, new { title = "Bivakaangifte" } ) %>
            doorgestuurd. De deelnemerslijst is alleen voor je groep toegankelijk.</p>
        <p>
            [<%=Html.ActionLink("Uitstap/bivak toevoegen", "Nieuw", "Uitstappen") %>]
        </p>
        <table>
            <tr>
                <th>
                    Periode
                </th>
                <th>
                    Omschrijving
                </th>
                <th>
                    Bivak
                </th>
                <th>
                    Opmerking
                </th>
            </tr>
            <% foreach (var uitstap in Model.Uitstappen)
               {%>
            <tr>
                <td>
                    <%=String.Format("{0:d}", uitstap.DatumVan) %>
                    -
                    <%=String.Format("{0:d}", uitstap.DatumTot)%>
                </td>
                <td>
                    <%=Html.ActionLink(uitstap.Naam, "Bekijken", new {id = uitstap.ID})%>
                </td>
                <td>
                    <%=uitstap.IsBivak ? "&#10003;" : String.Empty %>
                </td>
                <td>
                    <%=uitstap.Opmerkingen %>
                </td>
            </tr>
            <%} %>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>