<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.CommVormModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% 
        Html.EnableClientValidation();
        using (Html.BeginForm())
        {
    %>
    <ul id="acties">
        <li>
            <input type="submit" value="Bewaren" /></li>
    </ul>
    <fieldset>
        <legend>Communicatievorm bewerken voor
            <%=Model.Aanvrager.VolledigeNaam %></legend>
        <%=Html.ValidationSummary() %>
        <table>
            <tr>
                <td>
                    <%=Model.NieuweCommVorm.CommunicatieTypeOmschrijving%>:
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nummer) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nummer)%>
                </td>
            </tr>
            <tr>
                <td>
                    <%=Html.LabelFor(mdl => mdl.NieuweCommVorm.Voorkeur)%>
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Voorkeur)%>
                </td>
            </tr>
            <tr>
                <td>
                    <%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden)%>
                </td>
            </tr>
            <tr>
                <td>
                    Extra informatie
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nota) %>
                </td>
                <td>
                    <%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nota)%>
                </td>
            </tr>
        </table>
        <%=Html.HiddenFor(mdl=>mdl.NieuweCommVorm.ID) %>
        <%=Html.HiddenFor(mdl=>mdl.NieuweCommVorm.VersieString) %>
    </fieldset>
    <%
        } %>
</asp:Content>
