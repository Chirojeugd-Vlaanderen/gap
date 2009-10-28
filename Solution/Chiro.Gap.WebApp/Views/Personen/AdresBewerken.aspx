<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<VerhuisModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <% using (Html.BeginForm())
       {
           %>
           
           <ul id="acties">
           <li><input type="submit" value="Bewaren" /></li>
           </ul>
           
           <fieldset>
           <legend>Adres aanpassen voor</legend>
           
           <%
               // bestaande bewoners in een lijst van
               // CheckBoxListInfo kwakken
           
               List<CheckBoxListInfo> info
                   = (from PersoonsAdres pa in Model.VanAdresMetBewoners.PersoonsAdres
                      select new CheckBoxListInfo(
                         pa.Persoon.ID.ToString()
                         , pa.Persoon.VolledigeNaam
                         , Model.PersoonIDs.Contains(pa.Persoon.ID) )).ToList<CheckBoxListInfo>();
           
               // Zodat we ze kunnen gebruiken in onze custom
               // HtmlHelper 'CheckBoxList'
           %>
           
           <%=Html.CheckBoxList("PersoonIDs", info) %>
                 
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
           <%=Html.Hidden("AanvragerID") %>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
