<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GroepsInstellingenModel>"
	MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="head">
    <script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
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
