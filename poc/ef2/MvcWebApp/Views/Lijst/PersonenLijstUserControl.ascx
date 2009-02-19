<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonenLijstUserControl.ascx.cs" Inherits="MvcWebApp.Views.Lijst.PersonenLijstUserControl" %>
<%@ Import Namespace="Cg2.Orm" %>

<p>
<% foreach (GelieerdePersoon p in ViewData.Model) {  %>

<%=p.Persoon.Naam %> <%=p.Persoon.VoorNaam %> <br />

<% } %>
</p>