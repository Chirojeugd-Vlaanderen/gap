<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IAdresBewerkenModel>" %>
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
%>
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #669) %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.9.1.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery-ui-1.10.2.custom.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>

    <link href="<%= ResolveUrl("~/Content/jquery-ui-1.10.2.custom.css")%>" rel="stylesheet" type="text/css" />
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
