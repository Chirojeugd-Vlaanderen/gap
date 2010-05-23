<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GroepsInstellingenModel>" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<h2>
		Functies bewerken</h2>
	<ul>
		<%
			foreach (var fie in Model.Detail.Functies.OrderBy(fie => fie.Code))
			{
		%>
		<li>
			[<%=Html.ActionLink("verwijderen", "FunctieVerwijderen", new {id = fie.ID }) %>] 
			<%=Html.Encode(String.Format("{0} ({1}) - Kan toegekend worden aan: {2}", fie.Naam, fie.Code, fie.Type))%>
		</li>
		<%
			}
		%>
	</ul>
	<% using (Html.BeginForm())
	{%>
	<fieldset>
		<legend>Functie toevoegen</legend>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.Naam) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.Naam) %><br />
			<%=Html.ValidationMessageFor(mdl => mdl.NieuweFunctie.Naam) %>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.Code) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.Code) %><br />
			<%=Html.ValidationMessageFor(mdl => mdl.NieuweFunctie.Code) %>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.MaxAantal) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.MaxAantal) %><br />
			(Mag leeg blijven als er geen maximumaantal is)
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.MinAantal) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.MinAantal) %><br />
			(Moet 0 zijn als er geen minimumaantal is)<%=Html.ValidationMessageFor(mdl => mdl.NieuweFunctie.MinAantal) %>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.WerkJaarVan) %>
			<%=Html.EditorFor(mdl => mdl.NieuweFunctie.WerkJaarVan) %><br />
			(Mag leeg blijven als het er niet toe doet)
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.Type ) %>
			<%
				var values = from LidType lt in Enum.GetValues(typeof(LidType))
							 select new { value = lt, text = lt.ToString() };
			%>
			<%=Html.DropDownListFor( mdl => mdl.NieuweFunctie.Type, new SelectList(values, "value", "text")) %>
		</p>
		<p>
			<input type="submit" value="Bewaren" />
		</p>
		<%}
		%>
	</fieldset>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
