<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.ServiceContracts.DataContracts.PersoonInfo>" %>

<%=Html.ActionLink(String.Format("{0} {1}", Model.VoorNaam, Model.Naam), "EditRest", new { Controller = "Personen", id = ViewData.Model.GelieerdePersoonID })%>