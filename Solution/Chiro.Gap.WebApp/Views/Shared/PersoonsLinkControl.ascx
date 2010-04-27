<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonDetail>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<%=Html.ActionLink(Model.VolledigeNaam, "EditRest", new { Controller = "Personen", id = ViewData.Model.GelieerdePersoonID })%>