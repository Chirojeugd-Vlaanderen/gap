<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PersoonInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.min.js")%>"></script>

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
    <ul id="acties">
        <li>
            <%= Html.ActionLink("Nieuwe persoon", "Nieuw") %></li>
        <li>
        <%using (Html.BeginForm("List"))
          { %>
            <div>
                <%= Html.DropDownListFor(mdl => mdl.GekozenCategorieID, new SelectList(Model.GroepsCategorieen, "ID", "Naam"))%>
                <input id="kiesCategorie" type="submit" />
            </div>
            <%
          } %>
        </li>
    </ul>
    <ul id="info">
        <li>Totaal aantal personen:
            <%= Model.Totaal %></li>
    </ul>
    <% Html.RenderPartial("PersonenLijstControl", Model); %>
</asp:Content>
