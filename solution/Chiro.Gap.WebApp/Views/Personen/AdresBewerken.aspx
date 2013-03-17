<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AdresModel>" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<% Html.RenderPartial("AdresBewerkenScriptsControl", Model); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% 
		Html.EnableClientValidation();
// ReSharper disable Asp.NotResolved
		using (Html.BeginForm())	// ReSharper geeft een foutmelding omdat er geen action bestaat die AdresBewerken heet,
									// maar de view wordt in andere actions opgeroepen
// ReSharper restore Asp.NotResolved
		{ %>
	<ul id="acties">
		<li>
			<input type="submit" name="action" value="Bewaren" /></li>
	</ul>
	<fieldset>
		<legend>Toepassen op:</legend>
		<%=Html.CheckBoxList("GelieerdePersoonIDs", Model.Bewoners)%>
	</fieldset>
    <%
		   =Html.ValidationSummary()
         %>
	<fieldset>
		<legend>Adresgegevens</legend>
		<p>
			<strong>Opgelet:</strong> voor binnenlandse adressen wordt alleen de officiële spelling van de straatnaam geaccepteerd.<br />
			Ben je zeker van de straatnaam maar wordt ze geweigerd? Lees in
			<%=Html.ActionLink("de handleiding", "ViewTonen", new { controller = "Handleiding", helpBestand = "NieuweStraatnaam"})%>
			hoe we daar een mouw aan kunnen passen.</p>
		<% var values = from AdresTypeEnum e in Enum.GetValues(typeof(AdresTypeEnum))
				  select new { value = e, text = e.ToString() }; 
		%>
		<p>
			<%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.AdresType) %>
			<%=Html.DropDownListFor(mdl => mdl.PersoonsAdresInfo.AdresType, new SelectList(values, "value", "text"))%>
		</p>

        <%
			Html.RenderPartial("AdresBewerkenControl", Model);
         %>
        
        <!-- Rap hier iets tussen zetten, om te vermijden dat resharper vervelend doet -->

		<%
			if (Model.OudAdresID == 0)
			{
				// De mogelijkheid om aan te kruisen of het nieuwe adres het voorkeursadres wordt, krijg je alleen bij een nieuw
				// adres, en niet bij een verhuis.  I.e. als OudAdresID == 0.
		%>
		<p>
			<%=Html.LabelFor(mdl=>mdl.Voorkeur) %>
			<%=Html.EditorFor(mdl => mdl.Voorkeur)%>
		</p>
		<%
			}
		%>
		<%=Html.HiddenFor(mdl=>mdl.AanvragerID) %>
		<%=Html.HiddenFor(mdl=>mdl.OudAdresID) %>
	</fieldset>
	<%} %>
</asp:Content>
