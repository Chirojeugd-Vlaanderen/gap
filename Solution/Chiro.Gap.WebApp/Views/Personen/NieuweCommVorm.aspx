<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<NieuweCommVormModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		Html.EnableClientValidation();
		using (Html.BeginForm())
		{
	%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
	</ul>
	<fieldset>
		<legend>Communicatievorm toevoegen voor
			<%=Model.Aanvrager.VolledigeNaam %></legend>
		<p>
			Bij het type communicatievorm in het selectievakje zie je telkens een voorbeeldje,
			zodat je ziet aan welke vormvereisten je gegevens moeten voldoen. Vooral bij
			telefoonnummers is dat van belang!</p>
		<%=Html.ValidationSummary() %>
		<table>
			<tr>
				<td>
					<%=Html.DropDownListFor(
						mdl=>mdl.NieuweCommVorm.CommunicatieTypeID, 
						new SelectList(Model.Types.Select(x => new { value = x.ID, text = string.Format("{0}  ({1})", x.Omschrijving, x.Voorbeeld)}), "value", "text"))%>:
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nummer) %>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nummer) %>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsVoorOptIn)%><br />
					<em>(alleen bij mailadressen van toepassing)</em>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsVoorOptIn) %>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
				</td>
			</tr>
			<tr>
				<td>
					Extra informatie
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nota) %><br />
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nota) %>
				</td>
			</tr>
		</table>
	</fieldset>
	<%
		} %>
</asp:Content>
