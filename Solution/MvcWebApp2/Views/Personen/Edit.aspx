<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% using (Html.BeginForm())
       {%>

    <ul id="acties">
        <li><input type="submit" value="Bewaren" /></li>        
    </ul>
    
    <h2><%=Model.HuidigePersoon.Persoon.VolledigeNaam%></h2>

    <fieldset>
    <legend>Persoonlijke gegevens</legend>
    
    <label for="Persoon_AdNummer">Ad-nummer</label> 
    <%=Model.HuidigePersoon.Persoon.AdNummer %><br />
    
    <label for="Persoon_VoorNaam">Voornaam</label> 
    <%=Html.TextBox("HuidigePersoon.Persoon.VoorNaam")%><br />
    
    <label for="Persoon_Naam">Familienaam</label> 
    <%=Html.TextBox("HuidigePersoon.Persoon.Naam")%><br />
    
    <label for="Persoon_GeboorteDatum">Geboortedatum</label> 
    <%=Html.TextBox("HuidigePersoon.Persoon.GeboorteDatum", ((DateTime) Model.HuidigePersoon.Persoon.GeboorteDatum).ToString("d"))%> <br />
    
    <label for="Persoon_Geslacht">Geslacht</label> 
    <%= Html.RadioButton("HuidigePersoon.Persoon.Geslacht", GeslachtsType.Man, Model.HuidigePersoon.Persoon.Geslacht == GeslachtsType.Man)%> Man
    <%= Html.RadioButton("HuidigePersoon.Persoon.Geslacht", GeslachtsType.Vrouw, Model.HuidigePersoon.Persoon.Geslacht == GeslachtsType.Vrouw)%> Vrouw <br />
    
    <label for="ChiroLeefTijd">Chiroleeftijd</label> 
    <%=Html.TextBox("HuidigePersoon.ChiroLeefTijd", (Model.HuidigePersoon.ChiroLeefTijd > 0 ? "+" : "") + Model.HuidigePersoon.ChiroLeefTijd.ToString())%>

    <%=Html.Hidden("HuidigePersoon.ID")%>
    <%=Html.Hidden("HuidigePersoon.VersieString")%>
    <%=Html.Hidden("HuidigePersoon.Persoon.ID")%>
    <%=Html.Hidden("HuidigePersoon.Persoon.VersieString")%>

    </fieldset>
        
    <h3>Adressen</h3>

    <ul>
    <% foreach (PersoonsAdres pa in ViewData.Model.HuidigePersoon.PersoonsAdres)
       { %>
       <li>
            <%=Html.Encode(String.Format("{0} {1}", pa.Adres.Straat.Naam, pa.Adres.HuisNr))%>,
            <%=Html.Encode(String.Format("{0} {1} {2}", pa.Adres.Straat.PostNr, pa.Adres.PostCode, pa.Adres.Subgemeente.Naam))%>
            <%= pa.IsStandaard ? "(standaardadres)" : ""%>
            <%=Html.ActionLink( "[verhuizen]", "Verhuizen", new {Controller="Personen", id = pa.Adres.ID} ) %>
        </li>
    <%} %>
        <li><%=Html.ActionLink( "[nieuw adres]", "NieuwAdres", new {Controller="Personen", id = ViewData.Model.HuidigePersoon.ID} ) %></li>
    </ul>   
    

    <h3>Communicatie</h3>

    <ul>
    <% foreach (CommunicatieVorm cv in Model.HuidigePersoon.Communicatie)
    { %>
    <li>
            <%=cv.Type.ToString() %>:
            <%=Html.Encode(cv.Nummer) %>
            <%=cv.Voorkeur ? "(voorkeur)" : "" %>
        </li>
    <%} %>
    </ul>     
 
    <%} %>
    
    <div>
        <%=Html.ActionLink("Terug naar de lijst", "Index")%>
    </div>
</asp:Content>
