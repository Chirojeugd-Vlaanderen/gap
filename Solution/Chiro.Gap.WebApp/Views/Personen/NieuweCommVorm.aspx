<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<NieuweCommVormModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript" />
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript" />
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript" />
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript" />
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript" />
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript" />
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
		<%=Html.ValidationSummary() %>
		<table>
			<tr>
				<td>
					<%=Html.DropDownListFor(mdl=>mdl.NieuweCommVorm.CommunicatieTypeID, new SelectList(Model.Types.Select(x => new { value = x.ID, text = x.Omschrijving }), "value", "text"))%>:
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nummer) %>
				</td>
				<td>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nummer) %>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsVoorOptIn)%>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsVoorOptIn) %>
				</td>
				<td>
					<em>(alleen bij mailadressen van toepassing)</em>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
				</td>
				<td>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Voorkeur) %>
				</td>
			</tr>
			<tr>
				<td>
					<%=Html.LabelFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
				</td>
				<td>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.IsGezinsGebonden) %>
				</td>
			</tr>
			<tr>
				<td>
					Extra informatie
				</td>
				<td>
					<%=Html.EditorFor(mdl => mdl.NieuweCommVorm.Nota) %><br />
				</td>
				<td>
					<%=Html.ValidationMessageFor(mdl => mdl.NieuweCommVorm.Nota) %>
				</td>
			</tr>
		</table>
	</fieldset>
	<%
		} %>
</asp:Content>
