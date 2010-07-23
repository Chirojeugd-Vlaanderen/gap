<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GroepsInstellingenModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div class="kaderke">
		<div class="kadertitel">Algemene groepsinfo</div>
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
	</div>
	<div class="kaderke">
		<div class="kadertitel">Actieve afdelingen dit werkjaar</div>
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
	</div>
	<div class="kaderke">
		<div class="kadertitel">Categorieën voor ingeschreven en niet-ingeschreven personen</div>
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
		[<%=Html.ActionLink("categorieën toevoegen/verwijderen", "Index", "Categorieen") %>]
	</div>
	<div class="kaderke">
		<div class="kadertitel">Eigen functies voor ingeschreven leden en leiding</div>
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
		[<%=Html.ActionLink("functies toevoegen/verwijderen", "Index", "Functies") %>]
	</div>
</asp:Content>
