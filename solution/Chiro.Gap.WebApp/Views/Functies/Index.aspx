﻿<%@ Page Language="C#" Inherits="ViewPage<GroepsInstellingenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%// Opgelet! Scripts MOETEN een expliciete closing tag (</script>) hebben!  Ze oa #722 %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>" type="text/javascript"></script>
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
		{%>
	<ul>
<%
			foreach (var fie in Model.Detail.Functies.OrderBy(fie => fie.Code))
			{
%>
				<li>[<%=Html.ActionLink("verwijderen", "Verwijderen", new {id = fie.ID }) %>]
		
				<%=Html.Encode(String.Format(
					"{0} ({1})", 
					fie.Naam, 
					fie.Code))%>
<% 
				if ((Model.Detail.Niveau & Niveau.Groep) != 0)
				{
%>							                
					<%=Html.Encode(String.Format(
					" - Kan toegekend worden aan ingeschreven {0}",
					fie.Type == LidType.Kind ? "leden" : fie.Type == LidType.Leiding ? "leiding" : "leden en leiding"))%>                
<%
				}
%>
		</li>
		<%
			}
		%>
	</ul>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
	</ul>
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
<% 
		if ((Model.Detail.Niveau & Niveau.Groep) != 0)
		{
%>							                
			
			<p>
			<%=Html.LabelFor(mdl => mdl.NieuweFunctie.Type)%>
<%
			var values = from LidType lt in Enum.GetValues(typeof (LidType))
			     select new
	                    		{
	                    			value = lt,
	                    			text = String.Format(
	                    				"Ingeschreven {0}",
	                    				lt == LidType.Kind ? "leden" : lt == LidType.Leiding ? "leiding" : "leden en leiding")
	                    		};
%>
			<%=Html.DropDownListFor(mdl => mdl.NieuweFunctie.Type,
						       new SelectList(values.Reverse(), "value", "text"))%>
			</p>
<%
		}
%>
	</fieldset>
	<%}
	%>
</asp:Content>