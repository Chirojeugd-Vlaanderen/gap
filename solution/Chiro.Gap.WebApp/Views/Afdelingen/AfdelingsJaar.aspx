<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AfdelingsJaarModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<% // Opgelet! Voor scripts de expliciete closing tag laten staan; anders krijg je een lege pagina. (zie #694) %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>" type="text/javascript"></script>
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
		using (Html.BeginForm("AfdJaarBewerken", "Afdelingen", new { groepID = Model.GroepID }))
		{%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
		<li>
			<input type="reset" value="  Reset  " /></li>
	</ul>
	<fieldset>
		<%=Html.LabelFor(mdl=> mdl.Afdeling.Naam)%>
		<%=Html.EditorFor(mdl => mdl.Afdeling.Naam)%>
        <%=Html.ValidationMessageFor(mdl => mdl.Afdeling.Naam)%>
        <br />
		<%=Html.LabelFor(mdl => mdl.Afdeling.Afkorting)%>
		<%=Html.EditorFor(mdl => mdl.Afdeling.Afkorting)%>
        <%=Html.ValidationMessageFor(mdl => mdl.Afdeling.Afkorting)%>
        <br />
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
		<%=Html.DropDownListFor(mdl => mdl.AfdelingsJaar.OfficieleAfdelingID, new SelectList(values, "value", "text"))%> 
        <%=Html.HiddenFor(mdl=>mdl.Afdeling.ID) %>
		<%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingID) %>
		<%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingsJaarID) %>
		<%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.VersieString) %>
	</fieldset>

	<%
		} 
	%>
	
	<p>
		Ter informatie de &lsquo;standaardafdelingen&rsquo; voor dit werkjaar:
	</p>
	<table>
		<%  foreach (var oa in Model.OfficieleAfdelingen.Where(ofaf=>ofaf.ID != (int)NationaleAfdeling.Speciaal).OrderBy(ofaf => ofaf.LeefTijdTot)){%>
		<tr>
			<td>
				<%=oa.Naam %>
			</td>
			<td>
				<%=oa.StandaardGeboorteJaarVan(Model.HuidigWerkJaar) %>-<%=oa.StandaardGeboorteJaarTot(Model.HuidigWerkJaar)%>
			</td>
		</tr>
		<%}%>
	</table>
	<p>
		<%=Html.ActionLink("Meer weten over afdelingen die een speciaal geval zijn?", "ViewTonen", new { controller = "Handleiding", helpBestand = "SpecialeAfdelingen" })%></p>
</asp:Content>