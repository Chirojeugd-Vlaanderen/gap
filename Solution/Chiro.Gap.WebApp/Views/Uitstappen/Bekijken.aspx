<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="kaderke">
    <div class="kadertitel"><%=Model.Uitstap.Naam %> <%=Model.Uitstap.IsBivak ? "(bivak)" : "(uitstap)" %></div>

    <p>
    <em>Periode:</em> <%=String.Format("{0:d}", Model.Uitstap.DatumVan) %> - <%=String.Format("{0:d}", Model.Uitstap.DatumTot)%>
    </p>

    <p>
    <%=Model.Uitstap.Opmerkingen %>
    </p>

    <p>
    [<%=Html.ActionLink("Bewerken", "Bewerken", new {id = Model.Uitstap.ID})%>]
    </p>

    </div>
                    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
