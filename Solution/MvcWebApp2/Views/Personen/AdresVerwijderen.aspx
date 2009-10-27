<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.AdresVerwijderenModel>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Adres verwijderen</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Html.BeginForm())
   { %>
   
   <ul id="acties">
   <li><input type="submit" value="Bewaren" /></li>
   </ul>
   
           <fieldset>
           <legend>Adresgegevens</legend>
           
           <%=Html.Encode(String.Format("{0} {1} {2}", Model.AdresMetBewoners.Straat.Naam, Model.AdresMetBewoners.HuisNr, Model.AdresMetBewoners.Bus)) %> <br />
           <%=Html.Encode(String.Format("{0} {1}", Model.AdresMetBewoners.Subgemeente.PostNr, Model.AdresMetBewoners.Subgemeente.Naam)) %> <br />
           
           <%=Html.Hidden("AdresMetBewoners.ID") %>
           <%=Html.Hidden("AanvragerGelieerdePersoonID") %>
           
           </fieldset>

   
           <fieldset>
           <legend>Wonen niet meer op bovenstaand adres:</legend>
           
           <%
               // bestaande bewoners in een lijst van
               // CheckBoxListInfo kwakken
           
               List<CheckBoxListInfo> info
                   = (from PersoonsAdres pa in Model.AdresMetBewoners.PersoonsAdres
                      select new CheckBoxListInfo(
                         pa.Persoon.ID.ToString()
                         , pa.Persoon.VolledigeNaam
                         , Model.PersoonIDs.Contains(pa.Persoon.ID) )).ToList<CheckBoxListInfo>();
           
               // Zodat we ze kunnen gebruiken in onze custom
               // HtmlHelper 'CheckBoxList'
           %>
           
           <%=Html.CheckBoxList("PersoonIDs", info) %>
                 
           </fieldset>   
      
   <%
    } %>

</asp:Content>
