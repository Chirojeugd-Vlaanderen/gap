<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersoonsLink.ascx.cs" Inherits="MvcWebApp.Views.Personen.PersoonsLink" %>
<%@ Import Namespace="Cg2.Orm" %>

<%=Html.ActionLink(new StringBuilder(ViewData.Model.Persoon.VoorNaam).Append(" ").Append(ViewData.Model.Persoon.Naam).ToString(), "Details", new { Controller = "Personen", id = ViewData.Model.ID })%>
