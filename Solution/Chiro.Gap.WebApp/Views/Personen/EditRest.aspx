<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()) {%>
    
    <ul id="acties">
        <li><input type="submit" value="Gegevens wijzigen" /></li>        
    </ul>
    <br />

    <fieldset>
        <legend>Persoonlijke gegevens</legend>    
        
            <p>
            <%=Html.LabelFor(s => s.HuidigePersoon.Persoon.AdNummer)%>
            <%=Html.TextBox("AdNummer", Model.HuidigePersoon.Persoon.AdNummer, 
                    new Dictionary<string, object> { 
                        {"readonly", "readonly"}, 
                        {"title", "Stamnummer kan niet ingegeven of gewijzigd worden." } })%>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.VoorNaam) %>
            <%=Html.DisplayFor(s=>s.HuidigePersoon.Persoon.VoorNaam)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.VoorNaam)%>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.Naam) %>
            <%=Html.DisplayFor(s=>s.HuidigePersoon.Persoon.Naam)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.Naam)%>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.GeboorteDatum) %>
            <%=Html.DisplayFor(s=>s.HuidigePersoon.Persoon.GeboorteDatum)%>
<%--            <% if (Model.HuidigePersoon.Persoon.GeboorteDatum.HasValue)
               { %>
                   <%=Model.HuidigePersoon.Persoon.GeboorteDatum.Value.ToString("d") %>
            <% } %>--%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.GeboorteDatum)%>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.Geslacht) %>
            <%=Html.DisplayFor(s=>s.HuidigePersoon.Persoon.Geslacht)%>
            <%--<%if (Model.HuidigePersoon.Persoon.Geslacht == GeslachtsType.Man)
              {%>
                Man
            <% }
              else
              {%>
              Vrouw
            <%} %>--%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.Geslacht)%>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.ChiroLeefTijd) %>
            <%=Html.DisplayFor(s=>s.HuidigePersoon.ChiroLeefTijd)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.ChiroLeefTijd)%>
            </p>
            
            <%=Html.HiddenFor(s=>s.HuidigePersoon.ID)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.VersieString)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.ID)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.VersieString)%>

    </fieldset>
        
    <h3>Adressen</h3>

    <ul>
    <% foreach (PersoonsAdres pa in ViewData.Model.HuidigePersoon.Persoon.PersoonsAdres)
       { %>
       <li>
            <%=Html.Encode(String.Format("{0} {1}", pa.Adres.StraatNaam.Naam, pa.Adres.HuisNr))%>,
            <%=Html.Encode(String.Format("{0}-{1}-{2} {3} ({4}) ", pa.Adres.StraatNaam.PostNummer, pa.Adres.PostCode, pa.Adres.WoonPlaats.PostNummer, pa.Adres.WoonPlaats.Naam, pa.AdresType))%>
            <%=Html.ActionLink( "[verhuizen]", "Verhuizen", new {id = pa.Adres.ID, aanvragerID = ViewData.Model.HuidigePersoon.ID} ) %>
            <%=Html.ActionLink("[verwijderen]", "AdresVerwijderen", new { id = pa.Adres.ID, gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%>
        </li>
    <%} %>
        <li><%=Html.ActionLink( "[nieuw adres]", "NieuwAdres", new {id = ViewData.Model.HuidigePersoon.ID} ) %></li>
    </ul>   
    

    <h3>Communicatie</h3>

    <ul>
    <% foreach (CommunicatieVorm cv in Model.HuidigePersoon.Communicatie)
    { %>
        <li>
            <%=cv.CommunicatieType.Omschrijving %>:
            <%=Html.Encode(cv.Nummer) %>
            <%=cv.Voorkeur ? "(voorkeur)" : "" %>
            <%=Html.ActionLink("[verwijderen]", "VerwijderenCommVorm", new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%>
            <%=Html.ActionLink("[bewerken]", "BewerkenCommVorm", new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%>
        </li>
    <%} %>
    <li><%=Html.ActionLink("[communicatievorm toevoegen]", "NieuweCommVorm", new { gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%></li>
    </ul>     
 
    <h3>categorieën</h3>

    <ul>
    <% foreach (Categorie cv in Model.HuidigePersoon.Categorie)
    { %>
    <li>
            <%=cv.Naam %>
            <%=Html.ActionLink("[verwijderen]", "VerwijderenCategorie", new { categorieID = cv.ID, gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%>
        </li>
    <%} %>
    
    <li><%=Html.ActionLink("[toevoegen aan categorie]", "ToevoegenAanCategorie", new { gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%></li>
    </ul>  
 
    <%} %>
    
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
