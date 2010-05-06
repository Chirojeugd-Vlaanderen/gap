<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.ServiceContracts.LidInfo>" %>

<%=Html.ActionLink(ViewData.Model.PersoonDetail.VolledigeNaam, "EditRest", new { Controller = "Personen", id = ViewData.Model.PersoonDetail.GelieerdePersoonID })%>