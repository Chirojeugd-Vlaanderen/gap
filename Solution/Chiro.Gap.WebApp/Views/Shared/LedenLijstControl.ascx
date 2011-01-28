<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>

<table class="overzicht">
	<tr>
		<th />
		<th>Ad-nr. </th>
		<th>Type </th>
		<th>
			<%= Html.ActionLink(
				"Naam", 
				"Lijst", 
				new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Naam, afdelingID = Model.AfdelingID, ledenLijst = Model.SpecialeLijst, functieID = Model.FunctieID }, 
				new { title = "Sorteren op naam" })%>
		</th>
		<th>
			<%= Html.ActionLink(
				"Geboortedatum", 
				"Lijst", 
				new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Leeftijd, afdelingID = Model.AfdelingID, ledenLijst = Model.SpecialeLijst, functieID = Model.FunctieID }, 
				new { title = "Sorteren op geboortedatum" })%>
		</th>
		<th>
			<%=Html.Geslacht(GeslachtsType.Man) %>
			<%=Html.Geslacht(GeslachtsType.Vrouw) %>
		</th>
		<th>Betaald </th>
	<% 
		if ((Model.GroepsNiveau & Niveau.Groep) != 0)
		{
 			// Afdelingen enkel relevant voor plaatselijke groepen

	%>		
			<th>
				<%=Html.ActionLink("Afd.",
 						  "Lijst",
 						  new
 	                                  		{
 	                                  			Controller = "Leden",
 	                                  			id = Model.IDGetoondGroepsWerkJaar,
								groepID = Model.GroepID,
 	                                  			afdelingID = Model.AfdelingID,
								ledenLijst = Model.SpecialeLijst,
 	                                  			functieID = Model.FunctieID,
								sortering = LidEigenschap.Afdeling
 	                                  		},
 						  new {title = "Sorteren op afdeling"})%>
			</th>
	<%
		}
	%>
		<th>Func. </th>
		<th>
			<%= Html.ActionLink(
				"Instap tot",
				"Lijst",
				new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.InstapPeriode, afdelingID = Model.AfdelingID, ledenLijst = Model.SpecialeLijst, functieID = Model.FunctieID },
				new { title = "Sorteren op einde instapperiode" })%>
		</th>
		<th>Telefoon</th>
		<th>E-mail</th>
		<%=Model.KanLedenBewerken ? "<th>Acties</th>" : String.Empty %>
	</tr>
	<%
		var volgnr = 0;
     foreach (LidOverzicht lidOverzicht in ViewData.Model.LidInfoLijst)
	{
		volgnr++;
		 
		 if(volgnr%2==0)
		 {%>
<tr class="even">
		 <%}
		 else
		 {%>
<tr class="oneven">			 
		 <%}

%>
		<td><%=volgnr.ToString() %></td>
		<td>
			<%= lidOverzicht.Type == LidType.Kind ? "Lid" : "Leiding" %>
		</td>
		<td>
			<%= lidOverzicht.AdNummer %>
		</td>
		<td>
			<% Html.RenderPartial("LedenLinkControl", lidOverzicht); %>
		</td>
		<td class="right">
			<%=lidOverzicht.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)lidOverzicht.GeboorteDatum).ToString("d")%>
		</td>
		<td class="center">
			<%= Html.Geslacht(lidOverzicht.Geslacht)%>
		</td>
		<td>
			<%= lidOverzicht.LidgeldBetaald?"Ja":"Nee"%>
		</td>
<% 
		if ((Model.GroepsNiveau & Niveau.Groep) != 0)
		{
			// Afdelingen enkel relevant voor plaatselijke groepen
	%>		
			<td>
	<%
			foreach (var a in lidOverzicht.Afdelingen)
			{
	%>	
				<%=Html.ActionLink(Html.Encode(a.Afkorting),
							  "Afdeling",
							  new
		                                  		{
		                                  			Controller = "Leden",
		                                  			groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar,
									groepID = Model.GroepID,
		                                  			id = a.ID,
		                                  		},
							  new {title = "Toon alleen afdeling " + a.Naam})%>
	<%
			}
	%>
			</td>
	<%
		}
%>
		<td>
			<% foreach (var functieInfo in lidOverzicht.Functies)
	  { %>
			<%=Html.ActionLink(
				Html.Encode(functieInfo.Code), 
				"Functie", 
				new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, groepID = Model.GroepID, id = functieInfo.ID }, 
				new { title = "Toon alleen mensen met functie " + functieInfo.Naam })%>
			<% } %>
		</td>
		<td><%=lidOverzicht.EindeInstapPeriode == null ? String.Empty : ((DateTime)lidOverzicht.EindeInstapPeriode).ToString("d") %></td>
		<td>
			<%=Html.Telefoon(lidOverzicht.TelefoonNummer) %> </td><td>
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
