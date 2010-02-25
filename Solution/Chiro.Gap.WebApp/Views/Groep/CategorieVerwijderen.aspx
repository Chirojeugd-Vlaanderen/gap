<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.PersonenLinksModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <p class="validation-summary-errors">
    Er zijn personen gekoppeld aan de categorie die je wilt verwijderen.  Als je de categorie verwijdert,
    dan blijven deze personen bestaan, maar verdwijnt uiteraard hun koppeling met de categorie.  Ben je
    er zeker van dat je de categorie wilt verwijderen?
    </p>

    <% using (Html.BeginForm())
       { %>

        <ul id="acties">
        <li><input type="submit" value="Ja"/></li>        
        <li><%=Html.ActionLink("Nee", "Index")%></li>
        </ul>

    
        <h3>Gekoppelde personen</h3>
    
        <% Html.RenderPartial("PersonenLinksControl", Model); %>    
        <%=Html.HiddenFor(mdl => mdl.CategorieID) %>  
        
    <%} %>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
