<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersoonsDetailsUserControl.ascx.cs" Inherits="MvcWebApp.Views.Shared.PersoonDetailsUserControl" %>
<%@ Import Namespace="Cg2.Orm" %>

<h2>Persoonsdetails: <%=Html.Encode(ViewData.Model.Persoon.VolledigeNaam) %></h2>

<h3>Algemeen</h3>

<table>
<tr><td>Familienaam</td><td><%=Html.Encode(ViewData.Model.Persoon.Naam) %></td></tr>
<tr><td>Voornaam</td><td><%=Html.Encode(ViewData.Model.Persoon.VoorNaam) %></td></tr>
<tr><td>Geboortedatum</td><td><%=ViewData.Model.Persoon.GeboorteDatum == null ? "?" : ((DateTime)ViewData.Model.Persoon.GeboorteDatum).ToString("d") %></td></tr>
<tr><td>Geslacht</td><td><%=ViewData.Model.Persoon.Geslacht.ToString() %></td></tr>
<tr><td>Chiroleeftijd</td><td><%=ViewData.Model.ChiroLeefTijd %></td></tr>
</table>

<h3>Adressen</h3>

<div class="adreslijst">
<% foreach (PersoonsAdres pa in ViewData.Model.PersoonsAdres)
   { %>
<div class="<%= pa.IsStandaard ? "adres standaardadres" : "adres" %>">
    <div class="adreslijn"><%=Html.Encode(String.Format("{0} {1}", pa.Adres.Straat.Naam, pa.Adres.HuisNr)) %></div>
    <div class="adreslijn"><%=Html.Encode(String.Format("{0} {1} {2}", pa.Adres.Straat.PostNr, pa.Adres.PostCode, pa.Adres.Subgemeente.Naam)) %></div>    
    <div class="adresopmerking"><%=Html.Encode(pa.Opmerking) %></div>    
</div> 
<%} %>
</div>


<h3>Communicatie</h3>

