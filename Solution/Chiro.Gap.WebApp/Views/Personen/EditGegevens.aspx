<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" EnableViewState="False">

	<script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>

	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="False">
	<%=Html.ValidationSummary("Er zijn enkele opmerkingen:") %>
	<% Html.EnableClientValidation(); // Deze instructie moet (enkel) voor de BeginForm komen %>
	<% using (Html.BeginForm())
	{ %>
	<%
		if (Model.GelijkaardigePersonen != null && Model.GelijkaardigePersonen.Count() > 0)
		{
			// Toon gelijkaardige personen
	%>
	<p class="validation-summary-errors">
		Let op! Uw nieuwe persoon lijkt verdacht veel op (een) reeds bestaande perso(o)n(en).
		Als u zeker bent dat u niemand dubbel toevoegt, klik dan opnieuw op &lsquo;Bewaren&rsquo;.
	</p>
	<ul>
		<% 
			foreach (PersoonDetail pi in Model.GelijkaardigePersonen)
			{
		%>
		<li>
			<%Html.RenderPartial("PersoonsLinkControl", pi);%>
			-
			<%=String.Format("{0:d}", pi.GeboorteDatum) %></li>
		<%
			}
		%>
	</ul>
	<%      
		}
	%>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
		<li>
			<input type="reset" value=" Reset " /></li>
	</ul>
	<fieldset>
		<legend>Persoonlijke gegevens</legend>
		<%=Html.HiddenFor(s => s.HuidigePersoon.AdNummer) %>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.VoorNaam) %>
			<%=Html.EditorFor(s => s.HuidigePersoon.VoorNaam) %>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.VoorNaam) %>
		</p>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.Naam) %>
			<%=Html.EditorFor(s => s.HuidigePersoon.Naam) %>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.Naam) %>
		</p>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.GeboorteDatum) %>
			<%=Html.EditorFor(s => s.HuidigePersoon.GeboorteDatum)%>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.GeboorteDatum) %>
		</p>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.Geslacht)%>
			<%= Html.RadioButton("HuidigePersoon.Geslacht", GeslachtsType.Man,   Model.HuidigePersoon.Geslacht == GeslachtsType.Man)%>
			Man
			<%= Html.RadioButton("HuidigePersoon.Geslacht", GeslachtsType.Vrouw, Model.HuidigePersoon.Geslacht == GeslachtsType.Vrouw)%>
			Vrouw
		</p>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.ChiroLeefTijd)%>
			<%=Html.EditorFor(s => s.HuidigePersoon.ChiroLeefTijd)%>
			<%=Html.ValidationMessageFor(s => s.HuidigePersoon.ChiroLeefTijd) %>
		</p>
		<%=Html.HiddenFor(s => s.HuidigePersoon.GelieerdePersoonID)%>
		<%=Html.HiddenFor(s => s.BroerzusID)%>
		<%=Html.HiddenFor(s => s.HuidigePersoon.VersieString)%>
		<%
			if (Model.Forceer)
			{
				// Ik krijg onderstaande niet geregeld met een html helper... :(
		%>
		<input type="hidden" name="Forceer" id="Forceer" value="True" />
		<%
			}
		%>
	</fieldset>
	<%} %>
	<% Html.RenderPartial("TerugNaarFicheLinkControl"); %>
</asp:Content>
