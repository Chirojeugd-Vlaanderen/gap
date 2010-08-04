<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<h2>
	Geen verbinding</h2>
<p>
	De database waarin de gegevens van je groep opgeslagen zijn, is tijdelijk niet beschikbaar.</p>
<% 
	// Als de gebruikers opnieuw proberen verbinding te maken en ze krijgen opnieuw deze pagina te zien,
	// dan zorgt de volgende regel ervoor dat ze verschil zien met hun vorige poging.
%>
<p>
	Laatste poging ondernomen om
	<%=DateTime.Now.ToLongTimeString() %>.</p>
<p>
	Als het probleem blijft bestaan, neem dan contact op met het nationaal secretariaat:
	03-231 07 95.</p>
