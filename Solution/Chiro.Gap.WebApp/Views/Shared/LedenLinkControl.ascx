<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.ServiceContracts.DataContracts.LidOverzicht>" %>
<%	// Het zou logisch zijn als Ledenlink verwijst naar een pagina waar je de lidgegevens kunt bewerken,
	// maar op EditRest staan die ook vermeld. Zo verwijst elke link op een naam ook naar dezelfde pagina.
	// Het model is hier wel anders dan in PersoonsLinkControl, dus deze control blijft nuttig. %>
<%=Html.ActionLink(String.Format("{0} {1}", Model.VoorNaam, Model.Naam), "EditRest", new { Controller = "Personen", id = ViewData.Model.GelieerdePersoonID  })%>