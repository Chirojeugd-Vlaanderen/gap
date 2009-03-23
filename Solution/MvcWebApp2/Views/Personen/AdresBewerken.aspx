<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cg2.Orm.Adres>" %>
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
                   = (from PersoonsAdres pa in ViewData.Model.PersoonsAdres
                      select new CheckBoxListInfo(
                         pa.GelieerdePersoonID.ToString()
                         , Html.ActionLink(Html.Encode(pa.GelieerdePersoon.Persoon.VolledigeNaam), "Edit", new { Controller = "Personen", id = pa.GelieerdePersoonID })
                         , true)).ToList<CheckBoxListInfo>();
           
               // Zodat we ze kunnen gebruiken in onze custom
               // HtmlHelper 'CheckBoxList'
           %>
           
           <%=Html.CheckBoxList("Bewoners", info) %>
                 
           </fieldset>
           
           <fieldset>
           <legend>Adresgegevens</legend>
           
           <label for="Straat_Naam">Straat</label>
           <%=Html.TextBox("Straat.Naam") %> <br />
           
           <label for="HuisNr">Nr.</label>
           <%=Html.TextBox("HuisNr") %> <br />
           
           <label for="Straat_PostNr">Postnr.</label>
           <%=Html.TextBox("Straat.PostNr") %> <br />
           
           <label for="Subgemeente_Naam">Gemeente</label>
           <%=Html.TextBox("Subgemeente.Naam") %> <br />
           
           <%=Html.Hidden("ID") %>
           <%=Html.Hidden("VersieString") %>
           
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
