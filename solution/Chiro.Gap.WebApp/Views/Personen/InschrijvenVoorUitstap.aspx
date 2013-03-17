<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapInschrijfModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<%using (Html.BeginForm())
  {%>

  <ul id="acties">
    <li><input type="submit" value="Bewaren" /></li>
  </ul>

  <%=Html.LabelFor(mdl => mdl.GeselecteerdeUitstapID) %>:
  <%=Html.DropDownListFor(mdl => mdl.GeselecteerdeUitstapID, new SelectList(Model.Uitstappen, "ID", "Naam")) %> <br />

  <%=Html.CheckBoxFor(mdl => mdl.LogistiekDeelnemer) %> 
  <%=Html.LabelFor(mdl => mdl.LogistiekDeelnemer) %>

  <ul>
  <%foreach (var p in Model.GelieerdePersonen)
    { %>
    <li>
        <%=Html.Hidden("GelieerdePersoonIDs", p.GelieerdePersoonID) %>
        <%=Html.PersoonsLink(p.GelieerdePersoonID, p.VoorNaam, p.Naam)%>
    </li>
  <%
    }%>
  </ul>
<%
  }%>

</asp:Content>
