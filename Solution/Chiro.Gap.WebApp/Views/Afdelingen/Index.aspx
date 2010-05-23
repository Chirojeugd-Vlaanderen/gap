<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AfdelingsOverzichtModel>" %>
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

<% foreach (var ai in Model.Actief)
   { %>
    <tr>
        <td><%=ai.AfdelingNaam %></td>
        <td><%=ai.AfdelingAfkorting %></td>
        <td><%=ai.OfficieleAfdelingNaam %></td>
        <td><%=ai.GeboorteJaarVan %></td>
        <td><%=ai.GeboorteJaarTot %></td>
        <td>
            <%=Html.ActionLink("Bewerken", "Bewerken", new { Controller = "Afdelingen", id = ai.AfdelingsJaarID } )%>
            <% if (ai.IsLeeg) { %>
            <%=Html.ActionLink("Verwijderen", "Verwijderen", new { Controller = "Afdelingen", id = ai.AfdelingsJaarID } )%>
            <% } %>
        </td>
    </tr>
<% } %>

</table>

<h3>Overige afdelingen (niet geactiveerd in dit werkjaar)</h3>
    
<table>
<tr>
<th>Afdeling</th><th>Afkorting</th><th>Actie</th>
</tr>

<% foreach (var ai in Model.NietActief)
   { %>
    <tr>
        <td><%=ai.Naam %></td>
        <td><%=ai.Afkorting %></td>
        <td><%=Html.ActionLink("Activeren in huidig werkjaar", "Activeren", new { Controller = "Afdelingen", id = ai.ID } )%></td>
    </tr>
<% } %>

</table>



</asp:Content>
