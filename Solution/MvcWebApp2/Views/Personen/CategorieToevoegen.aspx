<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CategorieModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
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
           <legend>Categorie toevoegen</legend>     
           
           <%=Model.Aanvrager.Persoon.VolledigeNaam %> toevoegen aan de categorie 
           <%=Html.DropDownList("Model.selectie", new SelectList(Model.Categorieen.Select(x => new { value = x.ID, text = x.Naam }), "value", "text"))%>
 
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
