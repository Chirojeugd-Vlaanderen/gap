<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.FunctieModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div id="bewerken">
    <h2>Bewerken</h2>
    
    <% using (Html.BeginForm())
       { %>
        <table>
            <tr>
                <td><%=Html.LabelFor(xx => xx.HuidigeFunctie.Naam) %> </td>
                <td><%=Html.EditorFor(mdl => mdl.HuidigeFunctie.Naam) %></td>
            </tr>
            <tr>
                <td><%=Html.LabelFor(xx => xx.HuidigeFunctie.Code) %></td> 
                <td><%=Html.EditorFor(mdl => mdl.HuidigeFunctie.Code) %></td>
                <%=Html.HiddenFor(mdl => mdl.HuidigeFunctie.ID) %>  
            </tr>
        </table>
    <input type="submit" value="Bewaren" id="bewaarFunctie"/>

    <% } %>
    
</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
