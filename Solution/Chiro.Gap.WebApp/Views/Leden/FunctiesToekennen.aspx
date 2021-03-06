<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidFunctiesModel>" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{%>
  
	<ul id="acties">
		<li>
			<input id="bewaarFuncties" type="submit" value="Gegevens wijzigen" /></li>
	</ul>
    
	<fieldset>
		<legend>Functies</legend>
		<%
			if (Model.AlleFuncties != null && Model.AlleFuncties.FirstOrDefault() != null)
			{
		%>
		<%
			List<CheckBoxListInfo> info = (from f in Model.AlleFuncties
										   select new CheckBoxListInfo(
									f.ID.ToString(),
									String.Format("{0} ({1})", f.Naam, f.Code),
									Model.FunctieIDs.Contains(f.ID))).ToList();
		%>
		<p>
			<%= Html.CheckBoxList("FunctieIDs", info)%></p>
		<%
			}
		%>
		<%= Html.HiddenFor(mdl=>mdl.Persoon.GelieerdePersoonID) %>
	</fieldset>
	<br />
	<%} %>
	<%= Html.ActionLink("Terug naar de persoonsfiche", "Bewerken", new { Controller = "Personen", id = Model.Persoon.GelieerdePersoonID}) %>
    
</asp:Content>
