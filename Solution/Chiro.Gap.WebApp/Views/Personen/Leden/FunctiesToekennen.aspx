<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<LidFunctiesModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{%>
	<ul id="acties">
		<li>
			<input type="submit" value="Gegevens wijzigen" /></li>
	</ul>
    <div id="functies">
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
		<%= Html.HiddenFor(mdl=>mdl.HuidigLid.PersoonDetail.GelieerdePersoonID) %>
	</fieldset>
	<br />
	<%} %></div>
	<%= Html.ActionLink("Terug naar de persoonsfiche", "EditRest", new { Controller = "Personen", id = Model.HuidigLid.PersoonDetail.GelieerdePersoonID}) %>
    
</asp:Content>
