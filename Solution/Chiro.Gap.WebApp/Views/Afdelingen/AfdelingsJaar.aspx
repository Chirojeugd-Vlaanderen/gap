<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AfdelingsJaarModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<!-- Opgelet! Voor scripts de expliciete closing tag laten staan; anders krijg je een lege pagina. (zie #694) -->
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<%= Html.ValidationSummary() %>
	<% 
		Html.EnableClientValidation();
		using (Html.BeginForm("Bewerken", "Afdelingen", new { groepID = Model.GroepID }))
		{%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
		<li>
			<input type="reset" value="  Reset  " /></li>
	</ul>
	<fieldset>
		<%=Html.LabelFor(mdl=> mdl.Afdeling.Naam)%>
		<%=Html.TextBoxFor(mdl => mdl.Afdeling.Naam, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" })%><br />
		<%=Html.LabelFor(mdl => mdl.Afdeling.Afkorting)%>
		<%=Html.TextBoxFor(mdl => mdl.Afdeling.Afkorting, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" })%><br />
		<%=Html.LabelFor(s => s.AfdelingsJaar.Geslacht)%>
		<%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Gemengd, Model.AfdelingsJaar.Geslacht == GeslachtsType.Gemengd)%>
		Gemengd
		<%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Man, Model.AfdelingsJaar.Geslacht == GeslachtsType.Man)%>
		Jongens
		<%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Vrouw, Model.AfdelingsJaar.Geslacht == GeslachtsType.Vrouw)%>
		Meisjes
		<%=Html.ValidationMessageFor(s => s.AfdelingsJaar.Geslacht)%><br />
		<%=Html.LabelFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%>
		<%=Html.EditorFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%>
		<%=Html.ValidationMessageFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%><br />
		<%=Html.LabelFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%>
		<%=Html.EditorFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%>
		<%=Html.ValidationMessageFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%><br />
		<% var values = from oa in Model.OfficieleAfdelingen
				  select new { value = oa.ID, text = oa.Naam }; 
		%>
		<%=Html.LabelFor(mdl=>mdl.AfdelingsJaar.OfficieleAfdelingID) %>
		<%=Html.DropDownListFor(mdl => mdl.AfdelingsJaar.OfficieleAfdelingID, new SelectList(values, "value", "text"))%><br />
		<%=Html.LabelFor(mdl=>mdl.AfdelingsJaar.GeenAutoVerdeling) %>
		<%=Html.CheckBoxFor(mdl=>mdl.AfdelingsJaar.GeenAutoVerdeling) %> <br />
		<%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingID) %>
		<%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingsJaarID) %>
		<%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.VersieString) %>
	</fieldset>
	<p>
		Ter informatie de &lsquo;standaardafdelingen&rsquo; voor dit werkjaar:
	</p>
	<table>
		<%  foreach (var oa in Model.OfficieleAfdelingen.OrderByDescending(ofaf => ofaf.StandaardGeboorteJaarTot))
	  {
		%>
		<tr>
			<td>
				<%=oa.Naam %>
			</td>
			<td>
				<%=oa.StandaardGeboorteJaarVan %>-<%=oa.StandaardGeboorteJaarTot%>
			</td>
		</tr>
		<%  }
		%>
	</table>
	<%
		} 
	%>
</asp:Content>
