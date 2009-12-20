<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AfdelingInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="acties">
<li><%= Html.ActionLink("Nieuwe afdeling", "Nieuw") %></li>
</ul>

<h3>Afdelingen beschikbaar in het huidige werkjaar</h3>
    
<table>
<tr>
<th>Afdeling</th><th>Afkorting</th><th>Offici&euml;le afdeling</th><th>Van</th><th>Tot</th><th>Actie</th>
</tr>

<% foreach (AfdelingInfo ai in Model.GebruikteAfdelingLijst)
   { %>
    <tr>
        <td><%=ai.Naam %></td>
        <td><%=ai.Afkorting %></td>
        <td><%=ai.OfficieleAfdelingNaam %></td>
        <td><%=ai.GeboorteJaarVan %></td>
        <td><%=ai.GeboorteJaarTot %></td>
        <td>
            <%=Html.ActionLink("Bewerken", "Bewerken", new { Controller = "Afdeling", id = ai.AfdelingsJaarID } )%>
            <% if (ai.AfdelingsJaarMagVerwijderdWorden) { %>
            <%=Html.ActionLink("Verwijderen", "Verwijderen", new { Controller = "Afdeling", id = ai.AfdelingsJaarID } )%>
            <% } %>
        </td>
    </tr>
<% } %>

</table>

<h3>Overige afdelingen (nog niet actief in huidige werkjaar (i.e. geen AfdelingsJaar))</h3>
    
<table>
<tr>
<th>Afdeling</th><th>Afkorting</th><th>Actie</th>
</tr>

<% foreach (AfdelingInfo ai in Model.OngebruikteAfdelingLijst)
   { %>
    <tr>
        <td><%=ai.Naam %></td>
        <td><%=ai.Afkorting %></td>
        <td><%=Html.ActionLink("Activeren in huidig werkjaar", "Activeren", new { Controller = "Afdeling", id = ai.ID } )%></td>
    </tr>
<% } %>

</table>



</asp:Content>
