<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GroepsInstellingenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div class="kaderke">
		<div class="kadertitel">
			Algemene groepsinfo</div>
		<table border="0">
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.Detail.Naam)%>
				</td>
				<td>
					<%=Html.DisplayFor(mdl => mdl.Detail.Naam)%>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.Detail.Plaats)%>
				</td>
				<td>
					<%=Html.DisplayFor(mdl => mdl.Detail.Plaats)%>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.Detail.StamNummer)%>
				</td>
				<td>
					<%=Html.DisplayFor(mdl => mdl.Detail.StamNummer)%>
				</td>
			</tr>
		</table>
	</div>
	<div class="kaderke">
		<div class="kadertitel">
			Actieve afdelingen dit werkjaar</div>
		<ul>
			<%
				foreach (var afd in Model.Detail.Afdelingen.OrderByDescending(afd => afd.GeboorteJaarVan))
				{
			%>
			<li>
				<%=Html.Encode(String.Format("{0} ({1}) -- officiële variant: {2}", afd.AfdelingNaam, afd.AfdelingAfkorting, afd.OfficieleAfdelingNaam.ToLower())) %></li>
			<%
				}
			%>
		</ul>
		[<%=Html.ActionLink("afdelingsverdeling aanpassen", "Index", "Afdelingen") %>]
	</div>
	<div class="kaderke">
		<div class="kadertitel">
			Categorieën voor ingeschreven en niet-ingeschreven personen</div>
		<ul>
			<%
				foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code))
				{
			%>
			<li>
				<%=Html.Encode(String.Format("{0} ({1})", cat.Naam, cat.Code))%>
			</li>
			<%
				}
			%>
		</ul>
		[<%=Html.ActionLink("categorieën toevoegen/verwijderen", "Index", "Categorieen") %>]
	</div>
	<div class="kaderke">
		<div class="kadertitel">
			Eigen functies voor ingeschreven leden en leiding</div>
		<ul>
			<%
				foreach (var fie in Model.Detail.Functies.OrderBy(fie => fie.Type))
				{
			%>
			<li>
				<%=Html.Encode(String.Format(
			    "{0} ({1}) -- kan toegekend worden aan ingeschreven {2}", 
			    fie.Naam, 
                fie.Code, 
                fie.Type == LidType.Kind ? "leden" : fie.Type == LidType.Leiding ? "leiding" : "leden en leiding"))%>
			</li>
			<%
				}%>
		</ul>
		[<%=Html.ActionLink("functies toevoegen/verwijderen", "Index", "Functies") %>]
	</div>
</asp:Content>
