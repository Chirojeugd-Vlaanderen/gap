<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcWebApp2.Models.NieuwAdresInfo>" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="MvcWebApp2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Nieuw adres</title>
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
           <legend>Adres toevoegen voor</legend>
           
           <%
               // bestaande bewoners in een lijst van
               // CheckBoxListInfo kwakken
           
               List<CheckBoxListInfo> info
                   = (from GelieerdePersoon gp in Model.MogelijkeBewoners
                      select new CheckBoxListInfo(
                         gp.ID.ToString()
                         , Html.ActionLink(Html.Encode(gp.Persoon.VolledigeNaam), "Edit", new { Controller = "Personen", id = gp.ID })
                         , Model.GelieerdePersoonIDs.Contains(gp.ID) )).ToList<CheckBoxListInfo>();
           
               // Zodat we ze kunnen gebruiken in onze custom
               // HtmlHelper 'CheckBoxList'
           %>
           
           <%=Html.CheckBoxList("GelieerdePersoonIDs", info) %>
                 
           </fieldset>
           
           <fieldset>
           <legend>Adresgegevens</legend>
           
           <label for="NieuwAdres_Straat_Naam">Straat</label>
           <%=Html.TextBox("NieuwAdres.Straat.Naam") %> <%= Html.ValidationMessage("NieuwAdres.Straat.Naam") %> <br />
           
           <label for="NieuwAdres_HuisNr">Nr.</label>
           <%=Html.TextBox("NieuwAdres.HuisNr") %> <br />
           
           <label for="NieuwAdres_Straat_PostNr">Postnr.</label>
           <%=Html.TextBox("NieuwAdres.Straat.PostNr") %> <br />
           
           <label for="NieuwAdres_Subgemeente_Naam">Gemeente</label>
           <%=Html.TextBox("NieuwAdres.Subgemeente.Naam") %> <%= Html.ValidationMessage("NieuwAdres.Subgemeente.Naam") %> <br />
           
           <%=Html.Hidden("AanvragerID") %>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
