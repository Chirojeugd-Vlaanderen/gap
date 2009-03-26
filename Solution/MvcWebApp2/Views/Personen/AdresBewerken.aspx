<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcWebApp2.Models.VerhuisInfo>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Adresgegevens</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <% using (Html.BeginForm())
       {
           %>
           
           <ul id="acties">
           <li><input type="submit" value="Bewaren" /></li>
           </ul>
           
           <h2>Adresgegevens</h2>
           
           <fieldset>
           <legend>Adres aanpassen voor</legend>
           
           <%
               // bestaande bewoners in een lijst van
               // CheckBoxListInfo kwakken
           
               List<CheckBoxListInfo> info
                   = (from PersoonsAdres pa in Model.Adres.PersoonsAdres
                      select new CheckBoxListInfo(
                         pa.GelieerdePersoonID.ToString()
                         , Html.ActionLink(Html.Encode(pa.GelieerdePersoon.Persoon.VolledigeNaam), "Edit", new { Controller = "Personen", id = pa.GelieerdePersoonID })
                         , Model.GelieerdePersoonIDs.Contains(pa.GelieerdePersoonID) )).ToList<CheckBoxListInfo>();
           
               // Zodat we ze kunnen gebruiken in onze custom
               // HtmlHelper 'CheckBoxList'
           %>
           
           <%=Html.CheckBoxList("GelieerdePersoonIDs", info) %>
                 
           </fieldset>
           
           <fieldset>
           <legend>Adresgegevens</legend>
           
           <label for="Adres_Straat_Naam">Straat</label>
           <%=Html.TextBox("Adres.Straat.Naam") %> <br />
           
           <label for="Adres_HuisNr">Nr.</label>
           <%=Html.TextBox("Adres.HuisNr") %> <br />
           
           <label for="Adres_Straat_PostNr">Postnr.</label>
           <%=Html.TextBox("Adres.Straat.PostNr") %> <br />
           
           <label for="Adres_Subgemeente_Naam">Gemeente</label>
           <%=Html.TextBox("Adres.Subgemeente.Naam") %> <br />
           
           <%=Html.Hidden("Adres.ID") %>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
