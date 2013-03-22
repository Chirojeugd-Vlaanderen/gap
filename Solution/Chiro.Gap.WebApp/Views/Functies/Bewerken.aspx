<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.FunctieModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Bewerken</h2>
    
    <% using (Html.BeginForm())
       { %>

    <%=Html.LabelFor(xx => xx.HuidigeFunctie.Naam) %> <%=Html.EditorFor(mdl => mdl.HuidigeFunctie.Naam) %> <br />
    <%=Html.LabelFor(xx => xx.HuidigeFunctie.Code) %> <%=Html.EditorFor(mdl => mdl.HuidigeFunctie.Code) %> <br />
    <%=Html.HiddenFor(mdl => mdl.HuidigeFunctie.ID) %>
    <input type="submit" value="Bewaren" />

    <% } %>
    

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
