<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" EnableViewState="False">
	<%	// OPGELET: de opening en closing tag voor 'script' niet vervangen door 1 enkele tag, want dan
		// toont de browser blijkbaar niets meer van dit form.  (Zie #664) %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server"
	EnableViewState="False">
	<%=Html.ValidationSummary("Er zijn enkele opmerkingen:") %>
	<% Html.EnableClientValidation(); // Deze instructie moet (enkel) voor de BeginForm komen %>
	<% using (Html.BeginForm())
	{ %>
	<%
		if (Model.GelijkaardigePersonen != null && Model.GelijkaardigePersonen.Count() > 0)
		{
			if (Model.GelijkaardigePersonen.Count() == 1)
			{
	%>
	<p class="validation-summary-errors">
		Pas op! Je nieuwe persoon lijkt verdacht veel op iemand die al gekend is in
		de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt, klik
		dan opnieuw op &lsquo;Bewaren&rsquo;.
	</p>
	<%
		}
			else
			{
	%>
	<p class="validation-summary-errors">
		Pas op! Je nieuwe persoon lijkt verdacht veel op personen die al gekend zijn
		in de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt,
		klik dan opnieuw op &lsquo;Bewaren&rsquo;.
	</p>
	<%
		}
	%>
	<ul>
		<% 
			// Toon gelijkaardige personen
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
	<% if(Model.BroerzusID != 0)
{
	%>
	<p>Het adres en de gezinsgebonden communicatievormen <%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "GezinsgebondenCommunicatievorm", new { helpBestand = "Trefwoorden" }, new { title = "Wat zijn 'gezinsgebonden communicatievormen'?" })%> worden gekopieerd van de broer of zus. <%=Html.ActionLink("Meer uitleg nodig over 'zus/broer maken'?", "ViewTonen", new { Controller = "Handleiding", helpBestand = "ZusBroer" }) %></p>	
	<%
} %>
	<fieldset>
		<legend>Persoonlijke gegevens</legend>
		<p>
			<%=Html.LabelFor(s => s.HuidigePersoon.AdNummer) %>
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "AD-nummer", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een AD-nummer?" } ) %>
			<%=Html.DisplayFor(s => s.HuidigePersoon.AdNummer) %>
			<%=Html.HiddenFor(s => s.HuidigePersoon.AdNummer)  %>
			<% // Het AD-nummer moet mee terug gepost worden, zowel als het null is als wanneer het niet null is:
               // De service zal wel protesteren als het ad-nummer niet overeenkomt met het oorspronkelijke. %>
		</p>
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
			<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?" } ) %>
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
