<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cg2.Orm.GelieerdePersoon>" %>
<%@ Import Namespace="Cg2.Orm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Gegevens bewerken: <%=Model.Persoon.VolledigeNaam %></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% using (Html.BeginForm())
       {%>

    <ul id="acties">
        <li><input type="submit" value="Bewaren" /></li>        
    </ul>
    
    <h2><%=Model.Persoon.VolledigeNaam%></h2>

    <fieldset>
    <legend>Persoonlijke gegevens</legend>
    
    <label for="Persoon_AdNummer">Ad-nummer</label> 
    <%=Html.TextBox("Persoon.AdNummer", Model.Persoon.AdNummer, new {@readonly = "readonly"})%> <br />
    
    <label for="Persoon_Naam">Familienaam</label> 
    <%=Html.TextBox("Persoon.Naam")%> <br />
    
    <label for="Persoon_VoorNaam">Voornaam</label> 
    <%=Html.TextBox("Persoon.VoorNaam")%> <br />
    
    <label for="Persoon_GeboorteDatum">Geboortedatum</label> 
    <%=Html.TextBox("Persoon.GeboorteDatum") %> <br />
    
    <label for="Persoon_Geslacht">Geslacht</label> 
    <%=Html.TextBox("Persoon.Geslacht") %> <br />
    
    <label for="ChiroLeefTijd">Chiroleeftijd</label> 
    <%=Html.TextBox("ChiroLeefTijd") %>

    <%=Html.Hidden("ID")%>
    <%=Html.Hidden("VersieString")%>
    <%=Html.Hidden("Persoon.ID")%>
    <%=Html.Hidden("Persoon.VersieString")%>

    </fieldset>
        
    <h3>Adressen</h3>

    <ul>
    <% foreach (PersoonsAdres pa in ViewData.Model.PersoonsAdres)
       { %>
       <li>
            <%=Html.Encode(String.Format("{0} {1}", pa.Adres.Straat.Naam, pa.Adres.HuisNr))%>,
            <%=Html.Encode(String.Format("{0} {1} {2}", pa.Adres.Straat.PostNr, pa.Adres.PostCode, pa.Adres.Subgemeente.Naam))%>
            <%= pa.IsStandaard ? "(standaardadres)" : ""%>
            <%=Html.ActionLink( "[verhuizen]", "Verhuizen", new {Controller="Personen", id = pa.Adres.ID} ) %>
        </li>
    <%} %>
        <li><%=Html.ActionLink( "[nieuw adres]", "NieuwAdres", new {Controller="Personen", id = ViewData.Model.ID} ) %></li>
    </ul>   
    

    <h3>Communicatie</h3>

    <ul>
    <% foreach (CommunicatieVorm cv in Model.Communicatie)
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
        <%=Html.ActionLink("Back to List", "Index")%>
    </div>


</asp:Content>

