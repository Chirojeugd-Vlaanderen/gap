<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% using (Html.BeginForm())
       {%>

    <ul id="acties">
        <li><input type="submit" value="Gegevens wijzigen" /></li>        
    </ul>

    <fieldset>
    <legend>Persoonlijke gegevens</legend>    
    
    <label for="Persoon_AdNummer">Ad-nummer</label>
    <%=Model.HuidigePersoon.Persoon.AdNummer %>
    <%=Html.Hidden("HuidigePersoon.Persoon.AdNummer")%>
    <br />
    
    <label for="Persoon_VoorNaam">Voornaam</label> 
    <%=Model.HuidigePersoon.Persoon.VoorNaam%>
    <%=Html.Hidden("HuidigePersoon.Persoon.VoorNaam")%>
    <br />
    
    <label for="Persoon_Naam">Familienaam</label> 
    <%=Model.HuidigePersoon.Persoon.Naam%>
    <%=Html.Hidden("HuidigePersoon.Persoon.Naam")%>
    <br />
    
    <label for="Persoon_GeboorteDatum">Geboortedatum</label> 
    <%=Html.Hidden("HuidigePersoon.Persoon.GeboorteDatum")%>
    <% if (Model.HuidigePersoon.Persoon.GeboorteDatum != null)
       { %>
           <%=((DateTime)Model.HuidigePersoon.Persoon.GeboorteDatum).ToString("d") %><br />
    <% } %>
    <br />
    
    <label for="Persoon_Geslacht">Geslacht</label> 
    <%=Html.Hidden("HuidigePersoon.Persoon.Geslacht")%>
    <%if (Model.HuidigePersoon.Persoon.Geslacht == GeslachtsType.Man)
      {%>
        Man<br />
    <% }
      else
      {%>
      Vrouw<br />
    <%} %>
    
    <label for="ChiroLeefTijd">Chiroleeftijd</label> 
    <%=Model.HuidigePersoon.ChiroLeefTijd%><br />
    <%=Html.Hidden("HuidigePersoon.ChiroLeefTijd")%>

    <%=Html.Hidden("HuidigePersoon.ID")%>
    <%=Html.Hidden("HuidigePersoon.VersieString")%>
    <%=Html.Hidden("HuidigePersoon.Persoon.ID")%>
    <%=Html.Hidden("HuidigePersoon.Persoon.VersieString")%>

    </fieldset>
        
    <h3>Adressen</h3>

    <ul>
    <% foreach (PersoonsAdres pa in ViewData.Model.HuidigePersoon.Persoon.PersoonsAdres)
       { %>
       <li>
            <%=Html.Encode(String.Format("{0} {1}", pa.Adres.Straat.Naam, pa.Adres.HuisNr))%>,
            <%=Html.Encode(String.Format("{0} {1} {2} ({3}) ", pa.Adres.Straat.PostNr, pa.Adres.PostCode, pa.Adres.Subgemeente.Naam, pa.Adres.AdresType))%>
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
            <%=Html.ActionLink("[bewerken]", "BewerkenCommVorm", new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%></li>
        </li>
    <%} %>
    <li><%=Html.ActionLink("[communicatievorm toevoegen]", "NieuweCommVorm", new { gelieerdePersoonID = ViewData.Model.HuidigePersoon.ID })%></li>
    </ul>     
 
    <h3>Categorieen</h3>

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
