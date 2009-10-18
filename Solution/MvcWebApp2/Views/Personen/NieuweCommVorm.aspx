<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<NieuweCommVormModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

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
           <legend>Communicatievorm toevoegen voor <%=Model.Aanvrager.Persoon.VolledigeNaam %></legend>     
           
           <table>
           <tr>
                <td><%=Html.DropDownList("Model.geselecteerdeCommVorm", new SelectList(Model.Types.Select(x => new { value = x.ID, text = x.Omschrijving }), "value", "text"))%>
                :</td>
                <td>
                    <%=Html.TextBox("Model.NieuweCommVorm.Nummer")%>
                    <%= Html.ValidationMessage("Model.NieuweCommVorm.Nummer")%> 
                </td>
           </tr>
           <tr>
                <td>Is dit de voorkeurscommunicatie voor dit communicatietype?</td>
                <td><%=Html.CheckBox("Model.NieuweCommVorm.Voorkeur", false)%>
                    <%= Html.ValidationMessage("Model.NieuweCommVorm.Voorkeur")%> 
                </td>
           </tr>
           <tr>
                <td>Is het gezinsgebonden?</td>
                <td>
                <%=Html.CheckBox("Model.NieuweCommVorm.IsGezinsgebonden", false)%>
                <%= Html.ValidationMessage("Model.NieuweCommVorm.IsGezinsgebonden")%> 
                </td>
           </tr>
           <tr>
                <td>Extra informatie</td>
                <td>
                <%=Html.TextBox("Model.NieuweCommVorm.Nota")%> 
                <%= Html.ValidationMessage("Model.NieuweCommVorm.Nota")%> 
                </td>
           </tr>
           </table>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
