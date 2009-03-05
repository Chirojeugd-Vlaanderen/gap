<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GelieerdePersoon>" %>
<%@ Import Namespace="Cg2.Orm" %>

    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) {%>

<h3>Algemeen</h3>

    <table>
    <tr><td>Ad-nummer:</td><td><%=ViewData.Model.Persoon.AdNummer %></td></tr>    
    <tr><td>Familienaam</td><td><%=Html.TextBox("Persoon.Naam") %></td></tr>
    <tr><td>Voornaam</td><td><%=Html.TextBox("Persoon.VoorNaam") %></td></tr>
    <tr><td>Geboortedatum</td><td><%=Html.TextBox("Persoon.GeboorteDatum") %></td></tr>
    <tr><td>Geslacht</td><td><%=Html.TextBox("Persoon.Geslacht") %></td></tr>
    <tr><td>Chiroleeftijd</td><td><%=Html.TextBox("ChiroLeeftijd") %></td></tr>
    </table>

    <p>
        <%=Html.Hidden("ID") %>
        <%=Html.Hidden("VersieString") %>
        <%=Html.Hidden("BusinessKey") %>
        <%=Html.Hidden("Persoon.ID") %>
        <%=Html.Hidden("Persoon.VersieString") %>
        <%=Html.Hidden("Persoon.BusinessKey") %>
        <%=Html.Hidden("Persoon.AdNummer") %>
        <%=Html.Hidden("EntityKey") %>
    
        <input type="submit" value="Save" />
    </p>

    <% } %>
