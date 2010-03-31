<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<NieuweCommVormModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {
    %>
    <ul id="acties">
        <li>
            <input type="submit" value="Bewaren" /></li>
    </ul>
    <fieldset>
        <legend>Communicatievorm toevoegen voor
            <%=Model.Aanvrager.Persoon.VolledigeNaam %></legend>
        <%=Html.ValidationSummary() %>
        <table>
            <tr>
                <td>
                    <%=Html.DropDownListFor(mdl=>mdl.geselecteerdeCommVorm, new SelectList(Model.Types.Select(x => new { value = x.ID, text = x.Omschrijving }), "value", "text"))%>:
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nummer) %>
                </td>
            </tr>
            <tr>
                <td>
                    <%=Html.LabelFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
                </td>
            </tr>
            <tr>
                <td>
                    <%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsGezinsgebonden) %>
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsGezinsgebonden) %>
                </td>
            </tr>
            <tr>
                <td>
                    Extra informatie
                </td>
                <td>
                    <%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nota) %><br />
                </td>
            </tr>
        </table>
    </fieldset>
    <%
        } %>
</asp:Content>
