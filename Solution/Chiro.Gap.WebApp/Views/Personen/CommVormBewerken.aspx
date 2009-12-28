<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CommVormModel>" %>
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
           <li><input type="submit" value="Bewaren" /></li>
           </ul>
           
           <fieldset>
           <legend>Communicatievorm bewerken voor <%=Model.Aanvrager.Persoon.VolledigeNaam %></legend>     
           <%=Html.ValidationSummary() %>
           <table>
           <tr>
                <td><%=Model.NieuweCommVorm.CommunicatieType.Omschrijving%>
                :</td>
                <td>
                    <%=Html.TextBox("Model.NieuweCommVorm.Nummer", Model.NieuweCommVorm.Nummer)%>
                    <%= Html.ValidationMessage("Model.NieuweCommVorm.Nummer")%> 
                </td>
           </tr>
           <tr>
                <td>Is dit de voorkeurscommunicatie voor dit communicatietype?</td>
                <td><%=Html.CheckBox("Model.NieuweCommVorm.Voorkeur", Model.NieuweCommVorm.Voorkeur)%>
                    <%= Html.ValidationMessage("Model.NieuweCommVorm.Voorkeur")%> 
                </td>
           </tr>
           <tr>
                <td>Is het gezinsgebonden?</td>
                <td>
                <%=Html.CheckBox("Model.NieuweCommVorm.IsGezinsgebonden", Model.NieuweCommVorm.IsGezinsgebonden)%>
                <%= Html.ValidationMessage("Model.NieuweCommVorm.IsGezinsgebonden")%> 
                </td>
           </tr>
           <tr>
                <td>Extra informatie</td>
                <td>
                <%=Html.TextBox("Model.NieuweCommVorm.Nota", Model.NieuweCommVorm.Nota)%> 
                <%= Html.ValidationMessage("Model.NieuweCommVorm.Nota")%> 
                </td>
           </tr>
           </table>
           
            <%=Html.Hidden("Model.NieuweCommVorm.ID", Model.NieuweCommVorm.ID)%>
            <%=Html.Hidden("Model.NieuweCommVorm.VersieString", Model.NieuweCommVorm.VersieString)%>

           
           </fieldset>
           
           <%
        } %>
</asp:Content>
