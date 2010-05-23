<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GroepsInstellingenModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

	<script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<fieldset>
		<legend>Algemene groepsinfo</legend>
		<p>
			<%=Html.LabelFor(mdl => mdl.Detail.ID) %>
			<%=Html.DisplayFor(mdl => mdl.Detail.ID)%>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.Detail.Naam)%>
			<%=Html.DisplayFor(mdl => mdl.Detail.Naam)%>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.Detail.Plaats)%>
			<%=Html.DisplayFor(mdl => mdl.Detail.Plaats)%>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.Detail.StamNummer)%>
			<%=Html.DisplayFor(mdl => mdl.Detail.StamNummer)%>
		</p>
	</fieldset>
	<fieldset>
		<legend>Actieve afdelingen dit werkjaar</legend>
		<ul>
			<%
				foreach (var afd in Model.Detail.Afdelingen.OrderByDescending(afd => afd.GeboorteJaarVan))
				{
			%>
			<li>
				<%=Html.Encode(String.Format("{0} - {1} ({2})", afd.AfdelingAfkorting, afd.AfdelingNaam, afd.OfficieleAfdelingNaam)) %></li>
			<%
				}
			%>
		</ul>
		[<%=Html.ActionLink("afdelingsverdeling aanpassen", "Index", "Afdelingen") %>]
	</fieldset>
	<fieldset>
		<legend>Categorieën voor personen en leden</legend>
		<ul>
			<%
				foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code))
				{
			%>
			<li>
				<%=Html.Encode(String.Format("{0} - {1}", cat.Code, cat.Naam)) %>
			</li>
			<%
				}
			%>
		</ul>
		[<%=Html.ActionLink("categorieënlijst bewerken", "Index", "Categorieen") %>]
	</fieldset>
	<fieldset>
		<legend>Groepsgebonden functies</legend>
		<ul>
			<%
				foreach (var fie in Model.Detail.Functies.OrderBy(fie => fie.Type))
				{
			%>
			<li>
				<%=
    		Html.Encode(String.Format("{0} ({1}) - Kan toegekend worden aan: {2}", fie.Naam, fie.Code, fie.Type))%>
			</li>
			<%
				}%>
		</ul>
		[<%=Html.ActionLink("groepsgebonden functies bewerken", "Index", "Functies") %>]
	</fieldset>
</asp:Content>
