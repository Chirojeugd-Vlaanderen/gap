<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LidInfo>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<%=Html.ActionLink(ViewData.Model.PersoonInfo.VolledigeNaam, "EditRest", new { Controller = "Leden", lidID = ViewData.Model.LidID })%>