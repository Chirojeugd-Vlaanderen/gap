﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AfdelingInfoModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<!-- Opgelet! Voor scripts de expliciete closing tag laten staan; anders krijg je een lege pagina. (zie #694) -->
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		Html.EnableClientValidation(); // Deze instructie moet (enkel) voor de BeginForm komen
        using (Html.BeginForm("AfdBewerken", "Afdelingen", new { groepID = Model.GroepID }))
        {
    %>
	<ul id="acties">
		<li>
		    <input type="submit" value="Bewaren" />
		</li>
	</ul>
	<fieldset>
		<legend>Afdeling bewerken</legend>
		<%=Html.LabelFor(mdl => mdl.Info.Naam) %>
		<%=Html.EditorFor(mdl => mdl.Info.Naam)%>
		<br />
		<%=Html.ValidationMessageFor(mdl => mdl.Info.Naam)%>
		<br />
		<%=Html.LabelFor(mdl => mdl.Info.Afkorting)%>
		<%=Html.EditorFor(mdl => mdl.Info.Afkorting)%>
		<br />
		<%=Html.ValidationMessageFor(mdl => mdl.Info.Afkorting)%>
        <%=Html.HiddenFor(mdl => mdl.Info.ID) %>
	</fieldset>
	<% } %>
</asp:Content>