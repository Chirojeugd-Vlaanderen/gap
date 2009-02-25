<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonenLijstUserControl.ascx.cs" Inherits="MvcWebApp.Views.Lijst.PersonenLijstUserControl" %>
<%@ Import Namespace="Cg2.Orm" %>


<table>
<tr>
<th><%=Html.CheckBox("CheckAll") %></th><th>Ad-nr.</th><th>Naam</th><th>Geboortedatum</th><th>Geslacht</th>
</tr>

<% foreach (GelieerdePersoon p in ViewData.Model) {  %>
<tr>
    <td><%=Html.CheckBox(String.Format("Check{0}", p.ID)) %></td>
    <td><%=p.Persoon.AdNummer %></td>
    <td><% Html.RenderPartial("PersoonsLink", p); %></td>
    <td><%=p.Persoon.GeboorteDatum == null ? "?" : ((DateTime)p.Persoon.GeboorteDatum).ToString("d") %></td>
    <td><%=p.Persoon.Geslacht.ToString() %></td>
</tr>
<% } %>

</table>
