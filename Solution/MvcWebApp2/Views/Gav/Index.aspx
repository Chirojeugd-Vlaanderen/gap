<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GavModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<% using (Html.BeginForm())
   {        
%>

    <ul id="acties">
        <li><input type="submit" value="OK" /></li>        
    </ul>

<%
       foreach (var item in ViewData.Model.GroepenLijst)
       {
%>
           <%=Html.RadioButton("GeselecteerdeGroepID", item.ID, (item.ID == Sessie.GroepID))%> 
           <%=String.Format("{0} - {1} ({2})", item.StamNummer, item.Groepsnaam, item.Plaats)%>
           <br />
<%
           
       }
   }
%>

</asp:Content>