<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersoonInfo>" %>
<%@ Import Namespace="MvcWebApp2.Models" %>
<%@ Import Namespace="Cg2.Orm" %>
<%@ Import Namespace="Cg2.ServiceContracts" %>

<%=Html.ActionLink(Html.Encode(ViewData.Model.VolledigeNaam), "EditRest", new { Controller = "Personen", id = ViewData.Model.GelieerdePersoonID })%>