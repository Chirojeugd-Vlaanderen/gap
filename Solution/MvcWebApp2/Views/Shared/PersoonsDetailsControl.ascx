<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GelieerdePersonenModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2.Models" %>

<h3>Algemeen</h3>

<table>
<tr><td>Ad-nummer:</td><td><%=ViewData.Model.HuidigePersoon.Persoon.AdNummer%></td></tr>
<tr><td>Familienaam</td><td><%=Html.Encode(ViewData.Model.HuidigePersoon.Persoon.Naam)%></td></tr>
<tr><td>Voornaam</td><td><%=Html.Encode(ViewData.Model.HuidigePersoon.Persoon.VoorNaam)%></td></tr>
<tr><td>Geboortedatum</td><td><%=ViewData.Model.HuidigePersoon.Persoon.GeboorteDatum == null ? "?" : ((DateTime)ViewData.Model.HuidigePersoon.Persoon.GeboorteDatum).ToString("d")%></td></tr>
<tr><td>Geslacht</td><td><%=ViewData.Model.HuidigePersoon.Persoon.Geslacht.ToString()%></td></tr>
<tr><td>Chiroleeftijd</td><td><%=ViewData.Model.HuidigePersoon.ChiroLeefTijd%></td></tr>
</table>

<h3>Adressen</h3>

<ul>
<% foreach (PersoonsAdres pa in ViewData.Model.HuidigePersoon.PersoonsAdres)
   { %>
   <li>
        <%=Html.Encode(String.Format("{0} {1}", pa.Adres.Straat.Naam, pa.Adres.HuisNr)) %>,
        <%=Html.Encode(String.Format("{0} {1} {2}", pa.Adres.Straat.PostNr, pa.Adres.PostCode, pa.Adres.Subgemeente.Naam)) %>
        <%= pa.IsStandaard ? "(standaardadres)" : "" %>
    </li>
<%} %>
</ul>


<h3>Communicatie</h3>

<ul>
<% foreach (CommunicatieVorm cv in ViewData.Model.HuidigePersoon.Communicatie)
   { %>
   <li>
        <%=cv.Type.ToString() %>:
        <%=Html.Encode(cv.Nummer) %>
        <%=cv.Voorkeur ? "(voorkeur)" : "" %>
    </li>
<%} %>
</ul>

