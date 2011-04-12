<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IAdresBewerkenModel>" %>
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
