<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.ServiceContracts.DataContracts.PersoonDetail>" %>

<%=Html.ActionLink(Model.VolledigeNaam, "EditRest", new { Controller = "Personen", id = ViewData.Model.GelieerdePersoonID })%>