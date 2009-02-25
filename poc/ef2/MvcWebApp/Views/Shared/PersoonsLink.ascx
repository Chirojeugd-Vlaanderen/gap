<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersoonsLink.ascx.cs" Inherits="MvcWebApp.Views.Personen.PersoonsLink" %>
<%@ Import Namespace="Cg2.Orm" %>

<%=Html.ActionLink(Html.Encode(ViewData.Model.Persoon.VolledigeNaam), "Details", new { Controller = "Personen", id = ViewData.Model.ID })%>
