<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.ProbeerLedenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<p>
Het overzicht hieronder toont je &lsquo;probeerleden en -leiding&rsquo;: de leden en leiding waarvan de 
instapperiode <a href="#Instapperiode">[?]</a> nog 
niet voorbij is.  Zodra de instapperiode van een lid/leid(st)er voorbij is, wordt hij/zij automatisch
aangesloten, en zal je een factuur krijgen.
</p>

<%
	int aantal = Model.LidInfoLijst.Count();
	
	if (aantal > 0)
  {%>
<p>
Op dit moment is van <%=aantal %> personen de probeerperiode nog niet voorbij.  Als al deze personen
in de Chiro blijven, zal je een factuur krijgen van <strong>&euro; <%=aantal*Model.AansluitingsPrijs %></strong>.
Als onderstaande lijst personen bevat die niet aangesloten moeten worden, schrijf deze dan uit v&oacute;&oacute;r
het verstrijken van hun instapperiode.
</p>
<%
  }
%>


<% 
// Ik had graag hiervoor de ledenlijstcontrol gebruiken, maar het is niet voor de hand liggend om de
// sortering via de kolomkoppen goed te krijgen.  Dus voorlopig dubbele code :-(		
%>

<table class="overzicht">
	<tr>
		<th>Ad-nr. </th>
		<th>Type </th>
		<th>
			<%= Html.ActionLink("Naam", "ProbeerLeden", new { Controller = "Leden", sortering = LidEigenschap.Naam, groepID = Model.GroepID }, new { title = "Sorteren op naam" })%>
		</th>
		<th>
			<%= Html.ActionLink("Adres", "ProbeerLeden", new { Controller = "Leden", sortering = LidEigenschap.Naam, groepID = Model.GroepID }, new { title = "Sorteren op adres" })%>
		</th>	

		<th>
			<%= Html.ActionLink("Geboortedatum", "ProbeerLeden", new { Controller = "Leden", sortering = LidEigenschap.Leeftijd, groepID = Model.GroepID }, new { title = "Sorteren op geboortedatum" })%>
		</th>
		<th>
			<%=Html.Geslacht(GeslachtsType.Man) %>
			<%=Html.Geslacht(GeslachtsType.Vrouw) %>
		</th>
		<th>Betaald </th>
		<th>
			<%= Html.ActionLink("Afd.", "ProbeerLeden", new { Controller = "Leden", sortering = LidEigenschap.Afdeling, groepID = Model.GroepID }, new { title = "Sorteren op afdeling" })%>
		</th>
		<th>Func. </th>
		<th>
			<%= Html.ActionLink("Instap tot", "ProbeerLeden", new { Controller = "Leden", sortering = LidEigenschap.InstapPeriode, groepID = Model.GroepID }, new { title = "Sorteren op afdeling" })%>
		</th>
		<th>Contact</th>
		<%=Model.KanLedenBewerken ? "<th>Acties</th>" : String.Empty %>
	</tr>
	<%
     foreach (LidOverzicht lidOverzicht in ViewData.Model.LidInfoLijst)
	{  %>
	<tr>
		<td>
			<%= lidOverzicht.AdNummer %>
		</td>
		<td>
			<%= lidOverzicht.Type == LidType.Kind ? "Lid" : "Leiding" %>
		</td>
		<td>
			<% Html.RenderPartial("LedenLinkControl", lidOverzicht); %>
		</td>
		<td><%=Html.Adres(lidOverzicht) %></td>
		<td class="right">
			<%=lidOverzicht.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)lidOverzicht.GeboorteDatum).ToString("d")%>
		</td>
		<td class="center">
			<%= Html.Geslacht(lidOverzicht.Geslacht)%>
		</td>
		<td>
			<%= lidOverzicht.LidgeldBetaald?"Ja":"Nee"%>
		</td>
		<td>
			<% foreach (var a in lidOverzicht.Afdelingen)
	  { %>
			<%=Html.ActionLink(Html.Encode(a.Afkorting), "Lijst", new { Controller = "Leden", id = 0, sortering = Model.GekozenSortering, afdelingID = a.ID, functieID = 0 }, new { title = a.Naam })%>
			<% } %>
		</td>
		<td>
			<% foreach (var functieInfo in lidOverzicht.Functies)
	  { %>
			<%=Html.ActionLink(Html.Encode(functieInfo.Code), "Lijst", new { Controller = "Leden", id = 0, sortering = Model.GekozenSortering, afdelingID = 0, functieID = functieInfo.ID }, new { title = functieInfo.Naam })%>
			<% } %>
		</td>
		<td><%=lidOverzicht.EindeInstapPeriode == null ? String.Empty : ((DateTime)lidOverzicht.EindeInstapPeriode).ToString("d") %></td>
		<td>
			<%=lidOverzicht.TelefoonNummer %> <br />
			<a href='mailto:<%=lidOverzicht.Email %>'><%=lidOverzicht.Email %></a>
		</td>
		
		<%if (Model.KanLedenBewerken)
	{%>
		<td>
			<%=Html.ActionLink("uitschrijven", "DeActiveren", new { Controller = "Leden", id = lidOverzicht.GelieerdePersoonID }, new { title = "Deze persoon verwijderen uit de lijst met ingeschreven leden en leiding" })%>
			<%=Html.ActionLink("afd.", "AfdelingBewerken", new { Controller = "Leden", lidID = lidOverzicht.LidID }, new { title = "Bij een (andere) afdeling zetten" })%>
		</td>
		<%
			}%>
	</tr>
	<% } %>
</table>



</asp:Content>
