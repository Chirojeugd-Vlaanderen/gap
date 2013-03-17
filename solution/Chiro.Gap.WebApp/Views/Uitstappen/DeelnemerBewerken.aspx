<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.DeelnemerBewerkenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<%
    Html.EnableClientValidation();
    using (Html.BeginForm())
    {
        %>
        <ul id="acties">
        <li><input type="submit" value="Bewaren" /></li>
        </ul>

        <fieldset class="invulformulier">
        <legend>Inschrijvingsgegevens</legend>

        <%:Html.PersoonsLink(Model.Deelnemer.GelieerdePersoonID, Model.Deelnemer.VoorNaam, Model.Deelnemer.FamilieNaam) %><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.IsLogistieker) %>
        <%:Html.EditorFor(mdl => mdl.Deelnemer.IsLogistieker)%>
        <%:Html.ValidationMessageFor(mdl => mdl.Deelnemer.IsLogistieker)%><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.HeeftBetaald) %>
        <%:Html.EditorFor(mdl=>mdl.Deelnemer.HeeftBetaald) %>
        <%:Html.ValidationMessageFor(mdl=>mdl.Deelnemer.HeeftBetaald) %><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.MedischeFicheOk) %>
        <%:Html.EditorFor(mdl=>mdl.Deelnemer.MedischeFicheOk) %>
        <%:Html.ValidationMessageFor(mdl=>mdl.Deelnemer.MedischeFicheOk) %><br />

        <%:Html.LabelFor(mdl=>mdl.Deelnemer.Opmerkingen) %>
        <%:Html.EditorFor(mdl=>mdl.Deelnemer.Opmerkingen) %>
        <%:Html.ValidationMessageFor(mdl=>mdl.Deelnemer.Opmerkingen) %><br />

        <%:Html.HiddenFor(mdl=>mdl.Deelnemer.UitstapID) %>

        </fieldset>

        <%
    } %>

</asp:Content>
