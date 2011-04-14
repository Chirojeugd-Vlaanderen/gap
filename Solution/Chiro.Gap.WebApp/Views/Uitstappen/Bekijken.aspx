<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="kaderke">
    <div class="kadertitel"><%=Model.Uitstap.IsBivak ? "Details bivak" : "Details uitstap" %></div>

    <p>
    <em>Periode:</em> <%=String.Format("{0:d}", Model.Uitstap.DatumVan) %> - <%=String.Format("{0:d}", Model.Uitstap.DatumTot)%>
    </p>

    <p>
    <%=Model.Uitstap.Opmerkingen %>
    </p>

    <p>
    [<%=Html.ActionLink("Bewerken", "Bewerken", new {id = Model.Uitstap.ID})%>]
    </p>

    <p>
    <%if (string.IsNullOrEmpty(Model.Uitstap.PlaatsNaam))
      {
    %>
        [<%=Html.ActionLink("Bivakplaats ingeven", "PlaatsBewerken", new {id = Model.Uitstap.ID}) %>]
    <%
      }
      else
      {
          %>
          
          <em>Plaats:</em> 
          <%=Model.Uitstap.PlaatsNaam %>, 
          <%=Model.Uitstap.Adres.StraatNaamNaam %> <%=Model.Uitstap.Adres.HuisNr %> <%=Model.Uitstap.Adres.Bus %>,
          <%=Model.Uitstap.Adres.PostNr %> <%=Model.Uitstap.Adres.PostCode %> <%=Model.Uitstap.Adres.WoonPlaatsNaam %>
          (<%=Model.Uitstap.Adres.LandNaam%>)
          <br />
          [<%=Html.ActionLink("Bivakplaats wijzigen", "PlaatsBewerken", new {id = Model.Uitstap.ID}) %>]
          <%
      }
    %>
    </p>

    </div>
                    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
