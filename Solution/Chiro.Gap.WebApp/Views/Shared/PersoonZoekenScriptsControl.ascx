<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.PersoonInfoModel>" %>
	<% // OPGELET! script-tags *moeten* een excpliciete closing tag hebben! (zie oa #669) %>
	<script src="<%= ResolveUrl("~/Scripts/jquery-1.7.1.min.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery-ui-1.8.18.custom.min.js")%>" type="text/javascript"></script>

	<!--<script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
	<script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>-->

    <link href="<%= ResolveUrl("~/Content/jquery-ui-1.8.18.custom.css")%>" rel="stylesheet" type="text/css" />
	<script type="text/javascript">
	    $(function () {

	        var personenCache = {};
	        var lastXhr;

	        $("input#Persoon_Zoeken").autocomplete({
	            minLength: 2,
	            source: function (request, response) {
	                var term = request.term;

	                if (term in personenCache) {
	                    response(personenCache[term]);
	                    return;
	                }
	                
	                lastXhr = $.getJSON('<%= Url.Action("PersoonZoeken", "Personen") %>', { naamOngeveer: term, groepID: <%=Model.GroepID %> }, function (data, status, xhr)
	                {
	                    personenCache[term] = data;

	                    if (xhr === lastXhr)
	                    {
	                        response($.map(data, function(item) {
	                            return {
	                                label: item.Naam + ' ' + item.VoorNaam,
	                                value: item.GelieerdePersoonID
	                            };
	                        }));
	                    }
	                });
	            },
	            select: function(event, ui)
                {
	                $(this).val(ui.item.label);

                    $(location).attr('href', '../EditRest/' + ui.item.value);
	                
	                return false;
                }
	        });
	    }); 
	</script>
