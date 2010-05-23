<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.ServiceContracts.DataContracts.PersoonLidInfo>" %>

<%=Html.ActionLink(ViewData.Model.PersoonDetail.VolledigeNaam, "EditLidGegevens", new { Controller = "Leden", id = ViewData.Model.PersoonDetail.GelieerdePersoonID })%>