<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AfdelingInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {%>
       
    <ul id="acties">
        <li><input type="submit" value="Bewaren"/></li>
        <li><input type="reset" value="  Reset  " /></li>
    </ul>
    
    <fieldset>
    
    <label for="Afdeling_Naam">Naam</label> 
    <%=Html.TextBox("HuidigeAfdeling.Naam")%><br />
    
    <label for="Afdeling_Afkorting">Afkorting</label> 
    <%=Html.TextBox("HuidigeAfdeling.Afkorting")%><br />
    
    </fieldset>
    
    <%} %>
</asp:Content>
