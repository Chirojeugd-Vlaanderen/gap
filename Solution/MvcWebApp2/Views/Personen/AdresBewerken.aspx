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
                   = (from PersoonsAdres pa in Model.VanAdresMetBewoners.PersoonsAdres
                      select new CheckBoxListInfo(
                         pa.GelieerdePersoon.ID.ToString()
                         , Html.ActionLink(Html.Encode(pa.GelieerdePersoon.Persoon.VolledigeNaam), "Edit", new { Controller = "Personen", id = pa.GelieerdePersoon.ID })
                         , Model.GelieerdePersoonIDs.Contains(pa.GelieerdePersoon.ID) )).ToList<CheckBoxListInfo>();
           
               // Zodat we ze kunnen gebruiken in onze custom
               // HtmlHelper 'CheckBoxList'
           %>
           
           <%=Html.CheckBoxList("GelieerdePersoonIDs", info) %>
                 
           </fieldset>
           
           <fieldset>
           <legend>Adresgegevens</legend>
           
           <label for="NaarAdres_Straat_Naam">Straat</label>
           <%=Html.TextBox("NaarAdres.Straat.Naam") %> <%= Html.ValidationMessage("NaarAdres.Straat.Naam") %> <br />
           
           <label for="NaarAdres_HuisNr">Nr.</label>
           <%=Html.TextBox("NaarAdres.HuisNr") %> <br />
           
           <label for="NaarAdres_Straat_PostNr">Postnr.</label>
           <%=Html.TextBox("NaarAdres.Straat.PostNr") %> <br />
           
           <label for="NaarAdres_Subgemeente_Naam">Gemeente</label>
           <%=Html.TextBox("NaarAdres.Subgemeente.Naam") %> <%= Html.ValidationMessage("NaarAdres.Subgemeente.Naam") %> <br />
           
           <%=Html.Hidden("VanAdresMetBewoners.ID") %>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
