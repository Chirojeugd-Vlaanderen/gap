<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.PersoonInfoModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>

    <%
        //HEEL BELANGRIJK: voor een dropdownlist moet het select statement zowel een id als een name hebben, die dezelfde zijn, en die moet ook in het event gebruikt worden
        // (Broes, ik snap niet wat je met bovenstaande bedoelt.)
    %>

    <script type="text/javascript">
        $(document).ready(function() {
        $('#kiesCategorie').hide();
        $("#GekozenCategorieID").change(function() {
                $('#kiesCategorie').click();
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%using (Html.BeginForm("List"))
  { %>

    <ul id="acties">
        <li><%= Html.ActionLink("Nieuwe persoon", "Nieuw") %></li>
        <li><%= Html.ActionLink("Lijst downloaden", "Download", new { id = Model.GekozenCategorieID })%></li>
        <li>
                <%= Html.DropDownListFor(mdl => mdl.GekozenCategorieID, new SelectList(Model.GroepsCategorieen, "ID", "Naam"))%>
                <input id="kiesCategorie" type="submit" />
        </li>
    </ul>
    <%
  } %>

    <% Html.RenderPartial("PersonenLijstControl", Model); %>

    <ul id="info">
        <li>Totaal aantal personen:
            <%= Model.Totaal %></li>
    </ul>
</asp:Content>
