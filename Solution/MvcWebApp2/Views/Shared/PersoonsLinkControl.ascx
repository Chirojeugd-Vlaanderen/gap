<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GelieerdePersoon>" %>
<%@ Import Namespace="Cg2.Orm" %>

<%=Html.ActionLink(Html.Encode(ViewData.Model.Persoon.VolledigeNaam), "Edit", new { Controller = "Personen", id = ViewData.Model.ID })%>