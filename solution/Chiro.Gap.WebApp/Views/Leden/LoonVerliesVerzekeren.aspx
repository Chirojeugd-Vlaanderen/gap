<%@ Page Language="C#" Inherits="ViewPage<BevestigingsModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content runat="server" ID="Content" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
<div id="ver">
	    <% using (Html.BeginForm()) {%>
	    
    
	    Je staat op het punt om
	    <%=Html.ActionLink(Model.VolledigeNaam, "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %>
	    te <a href="http://www.chiro.be/administratie/verzekeringen/extras-en-opties/loonverlies">verzekeren tegen loonverlies</a>.
        <%if (Model.GroepsNiveau.HasFlag(Niveau.KaderGroep))
          { %> (dit is gratis voor kaderleden)
        <% }else{ %>
        (Kostprijs: &euro;
        <%= Model.Prijs.ToString() %>) 
        <% } %>
        Klik op &lsquo;bevestigen&rsquo; om de verzekering af te sluiten.
	    <%} %>
        </div>
        <ul id="acties">
		    <li>
			    <input type="submit" value="Bevestigen" /></li>
		    <li>
			    <%=Html.ActionLink("Annuleren", "EditRest", new { Controller="Personen", id = Model.GelieerdePersoonID }) %></li>
	    </ul>

</asp:Content>
