<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AdresModel>" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #669) %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.autocomplete.js")%>" type="text/javascript"></script>
	<link href="<%= ResolveUrl("~/Content/jquery.autocomplete.css")%>" rel="stylesheet"
		type="text/css" />
	<script type="text/javascript">
	    $(function () {
	        // Automatisch invullen gemeentes na keuze postnummer
	        $("input#PostNr").change(function () {
	            $.getJSON('<%=Url.Action("WoonPlaatsenOphalen", "Adressen")%>', { postNummer: $(this).val() }, function (j) {
	                var options = '';
	                for (var i = 0; i < j.length; i++) {
	                    options += '<option value="' + j[i].Naam + '">' + j[i].Naam + '</option>';
	                }
	                $("select#WoonPlaats").html(options);
	            })
	        });

	        // Autocomplete straten
	        $("input#Straat").autocomplete(
                '<%= Url.Action("StratenVoorstellen", "Adressen") %>',
                { extraParams: { "postNummer": function () { return $("input#PostNr").val(); } } });

	        // Tonen en verbergen van internationale velden

	        // We doen dat 1 keer initieel, en daarna iedere keer een ander land
	        // wordt gekozen.

	        if ($("#Land").val() != "België") {
	            $(".binnenland").hide();
	            $(".buitenland").show();
	        }
	        else {
	            $(".binnenland").show();
	            $(".buitenland").hide();
	        };

	        // valt het op dat mijn jquery skills niet zo geweldig zijn? :)

	        $("#Land").change(function () {
	            if ($("#Land").val() != "België") {
	                $(".binnenland").hide();
	                $(".buitenland").show();
	            }
	            else {
	                $(".binnenland").show();
	                $(".buitenland").hide();
	            }
	        });
	    }); 
	</script>
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
	<fieldset>
		<legend>Adresgegevens</legend>
		<p>
			<strong>Opgelet:</strong> alleen de officiële spelling van de straatnaam wordt geaccepteerd.<br />
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
