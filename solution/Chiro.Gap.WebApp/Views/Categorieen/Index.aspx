<%@ Page Language="C#" Inherits="ViewPage<GroepsInstellingenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
	<%// Opgelet! Scripts MOETEN een expliciete closing tag (</script>) hebben!  Ze oa #722 %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="MainContent">
	<fieldset>
		<legend>Persoonscategorie&euml;n</legend>
		<ul>
			<%
				foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code))
				{
			%>
			<li>
				<%=Html.Encode(String.Format("{0} - {1}", cat.Code, cat.Naam)) %>
				[<%=Html.ActionLink("verwijderen", "CategorieVerwijderen", new {id = cat.ID }) %>]
			</li>
			<%
				}
			%>
		</ul>
	</fieldset>
	<% 
		Html.EnableClientValidation();
		using (Html.BeginForm())
		{ %>
	<fieldset>
		<legend>Categorie toevoegen</legend>
		<p>
			<%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Naam) %>
			<%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Naam) %><br />
			<%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Naam) %>
		</p>
		<p>
			<%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Code) %>
			<%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Code) %><br />
			<%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Code) %>
		</p>
		<p>
			<input type="submit" value="Bewaren" />
		</p>
	</fieldset>
	<%} %>
</asp:Content>
