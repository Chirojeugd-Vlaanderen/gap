<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersoonEnLidModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        Persoonlijke gegevens</h3>
    <p>
        <%= Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.VolledigeNaam) %>
        (<%=Html.Geslacht(Model.PersoonLidInfo.PersoonDetail.Geslacht) %>)
        <br />
        <%if (Model.PersoonLidInfo.PersoonDetail.AdNummer != null)
          {%>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer) %>&nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "AD-nummer", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een AD-nummer?" } ) %>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer) %><br />
        <%
            }%>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>
        <br />
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.SterfDatum)%>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.SterfDatum)%>
        <br />
        <%=Html.ActionLink("[persoonlijke gegevens aanpassen]", "EditGegevens", new {id=Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID}) %><br />
    </p>
    <h3>
        Laatst gekende adressen</h3>
    <ul>
        <% foreach (PersoonsAdresInfo pa in ViewData.Model.PersoonLidInfo.PersoonsAdresInfo)
           { %>
        <li>
            <%=Html.Encode(String.Format("{0} {1} {2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus))%>,
            <%=Html.Encode(String.Format("{0} {3} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType, pa.PostCode))%>
            <%=Html.ActionLink("[verwijderen]", "AdresVerwijderen", new { id = pa.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
        </li>
        <%} %>
    </ul>
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
