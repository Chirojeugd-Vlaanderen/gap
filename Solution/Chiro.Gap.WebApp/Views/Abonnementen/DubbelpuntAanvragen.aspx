<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.BevestigingsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <% using (Html.BeginForm()){%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bevestigen" /></li>
		<li>
			<%=Html.ActionLink("Annuleren", "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %></li>
	</ul>
	
	Je staat op het punt om een <a href='http://www.chiro.be/dubbelpunt'>Dubbelpuntabonnement</a> 
	te bestellen voor <%=Html.ActionLink(Model.VolledigeNaam, "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %>.<br />
	<% 
		   
	%>
	
	Hiervoor zal <strong>&euro; <%=Model.Prijs %></strong> aangerekend worden. 
	
	Klik op &lsquo;bevestigen&rsquo; om Dubbelpunt te bestellen.
	
    <%} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
