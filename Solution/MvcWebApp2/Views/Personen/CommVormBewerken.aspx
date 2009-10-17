<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CommVormModel>" %>
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
           </fieldset>
           
           <fieldset>
           <legend>Communicatiegegevens</legend>
           
           <% if(Model.NieuweCommVorm.CommunicatieType==1){ %>
           
           <%}else{%>
           <%}%>
           
           <label for="NaarAdres_Straat_Naam">Straat</label>
           <%=Html.TextBox("NaarAdres.Straat.Naam") %> <%= Html.ValidationMessage("NaarAdres.Straat.Naam") %> <br />
           
           <label for="NaarAdres_HuisNr">Nr.</label>
           <%=Html.TextBox("NaarAdres.HuisNr") %> <br />
           
           <label for="NaarAdres_Straat_PostNr">Postnr.</label>
           <%=Html.TextBox("NaarAdres.Straat.PostNr") %> <br />
           
           <label for="NaarAdres_Subgemeente_Naam">Gemeente</label>
           <%=Html.TextBox("NaarAdres.Subgemeente.Naam") %> <%= Html.ValidationMessage("NaarAdres.Subgemeente.Naam") %> <br />
           
           <%=Html.Hidden("VanAdresMetBewoners.ID") %>
           <%=Html.Hidden("AanvragerID") %>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
