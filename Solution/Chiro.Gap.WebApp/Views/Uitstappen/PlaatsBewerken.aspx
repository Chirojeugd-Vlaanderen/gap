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
    <%=Html.LabelFor(mdl=>mdl.Uitstap.PlaatsNaam) %> 
    <%=Html.EditorFor(mdl=>mdl.Uitstap.PlaatsNaam) %>
    <%=Html.ValidationMessageFor(mdl=>mdl.Uitstap.PlaatsNaam) %>
    </p>

    <!-- En nu voor dat adres... -->



    </div>
                    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
