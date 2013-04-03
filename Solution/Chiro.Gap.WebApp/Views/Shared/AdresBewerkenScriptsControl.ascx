<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IAdresBewerkenModel>" %>
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #669) %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery-ui-1.8.18.custom.min.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
	<!--<script src="<%= ResolveUrl("~/Scripts/jquery.autocomplete.js")%>" type="text/javascript"></script>-->
	<!--<link href="<%= ResolveUrl("~/Content/jquery.autocomplete.css")%>" rel="stylesheet" type="text/css" />-->
    <link href="<%= ResolveUrl("~/Content/jquery-ui-1.8.18.custom.css")%>" rel="stylesheet" type="text/css" />
	<script type="text/javascript">
	    $(function() {
	        // Automatisch invullen gemeentes na keuze postnummer
	        $("input#PostNr").change(function () {
	            
                if(!$(this).val()) {
                    $("input#Straat").attr('value', '').attr('disabled', 'disabled');
                    $("input#HuisNr").attr('value', '').attr('disabled', 'disabled');
                    $("input#Bus").attr('value', '').attr('disabled', 'disabled');
                } else {
                    $("input#Straat").removeAttr('disabled');
                    $("input#HuisNr").removeAttr('disabled');
                    $("input#Bus").removeAttr('disabled');
                }

	            $.getJSON('<%=Url.Action("WoonPlaatsenOphalen", "Adressen")%>', { postNummer: $(this).val() }, function(j) {
	                var options = '';
	                for (var i = 0; i < j.length; i++) {
	                    options += '<option value="' + j[i].Naam + '">' + j[i].Naam + '</option>';
	                }
	                $("select#WoonPlaats").html(options);
	            });
	        });


	        var stratenCache = {};
	        var lastXhr;
            
            $("input#Straat").autocomplete({
                minLength: 2,
			    source: function( request, response )
			    {
				    var term = request.term;

			        if ( term in stratenCache )
				    {
				        response( stratenCache[ term ] );
					    return;
				    }

			        lastXhr = $.getJSON("<%= Url.Action("StratenVoorstellen", "Adressen") %>", { q: term, postNummer: $("input#PostNr").val() }, function(data, status, xhr)
				    {
			            stratenCache[ term ] = data;

			            if ( xhr === lastXhr )
					    {
					        response( data );
					    }
				    });
			    }
		    });

	        // Autocomplete straten
	        /*$("input#Straat").autocomplete({
                source: '<%= Url.Action("StratenVoorstellen", "Adressen") %>',
                { extraParams: { "postNummer": function () { return $("input#PostNr").val(); } } });*/



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
