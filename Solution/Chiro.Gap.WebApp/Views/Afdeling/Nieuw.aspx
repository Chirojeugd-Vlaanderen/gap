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
    <legend>Nieuwe afdeling</legend>
    
    <%=Html.LabelFor(mdl => mdl.HuidigeAfdeling.Naam) %>
    <%=Html.EditorFor(mdl => mdl.HuidigeAfdeling.Naam) %>
    <br />
    <%=Html.ValidationMessageFor(mdl => mdl.HuidigeAfdeling.Naam) %>
    <br />

    <%=Html.LabelFor(mdl => mdl.HuidigeAfdeling.Afkorting) %>
    <%=Html.EditorFor(mdl => mdl.HuidigeAfdeling.Afkorting)%>
    <br />
    <%=Html.ValidationMessageFor(mdl => mdl.HuidigeAfdeling.Afkorting)%>
    
    
    </fieldset>
    
    <%} %>
</asp:Content>
