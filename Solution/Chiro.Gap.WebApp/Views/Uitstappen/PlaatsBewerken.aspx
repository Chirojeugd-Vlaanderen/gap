<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapModel>"
    MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <% Html.RenderPartial("AdresBewerkenScriptsControl", Model); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="kaderke">
        <div class="kadertitel">
            <%=Model.Uitstap.IsBivak ? "Details bivak" : "Details uitstap" %></div>
        <p>
            <em>Periode:</em>
            <%=String.Format("{0:d}", Model.Uitstap.DatumVan) %>
            -
            <%=String.Format("{0:d}", Model.Uitstap.DatumTot)%>
        </p>
        <p>
            <%=Model.Uitstap.Opmerkingen %>
        </p>
        <% 
            // Ik neem in het form hidden de niet-wijzigbare informatie uit het model op.  Op die manier is die
            // gemakkelijk beschikbaar als er zich validatiefouten voordoen.

            Html.EnableClientValidation();
            using (Html.BeginForm())
            {%>
        <ul id="acties">
            <li>
                <input type="submit" name="action" value="Bewaren" /></li>
        </ul>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.Naam) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.IsBivak) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.DatumVan) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.DatumTot) %>
        <%=Html.HiddenFor(mdl=>mdl.Uitstap.Opmerkingen) %>
        <p>
            <%=Html.LabelFor(mdl => mdl.Uitstap.PlaatsNaam)%>
            <%=Html.EditorFor(mdl => mdl.Uitstap.PlaatsNaam)%>
            <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.PlaatsNaam)%>
        </p>
        <% Html.RenderPartial("AdresBewerkenControl", Model); %>
        <%
}%>
    </div>
</asp:Content>
