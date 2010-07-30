<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.BevestigingsModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Content" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">

    <% using (Html.BeginForm()){%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bevestigen" /></li>
		<li>
			<%=Html.ActionLink("Annuleren", "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %></li>
	</ul>
	
	Je staat op het punt om <%=Html.ActionLink(Model.VolledigeNaam, "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %>
	te <a href="http://info.chiro.be/loonverlies">verzekeren tegen loonverlies</a>.  Hiervoor zal &euro; <%=Model.Prijs %> aangerekend worden. 
	
	Klik op &lsquo;bevestigen&rsquo; om de verzekering af te sluiten.
	
    <%} %>
    

</asp:Content>
