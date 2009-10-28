<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<NieuwAdresModel>" %>
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
           <legend>Adres toevoegen voor</legend>
           
           <%
               // bestaande bewoners in een lijst van
               // CheckBoxListInfo kwakken
           
               List<CheckBoxListInfo> info
                   = (from Persoon p in Model.MogelijkeBewoners
                      select new CheckBoxListInfo(
                         p.ID.ToString()
                         , p.VolledigeNaam
                         , Model.PersoonIDs.Contains(p.ID) )).ToList<CheckBoxListInfo>();
           
               // Zodat we ze kunnen gebruiken in onze custom
               // HtmlHelper 'CheckBoxList'
           %>
           
           <%=Html.CheckBoxList("PersoonIDs", info) %>
                 
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
           
<%
	// TODO: DropDownList toevoegen om adrestype te selecteren
%>

           <%=Html.Hidden("AanvragerID") %>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
