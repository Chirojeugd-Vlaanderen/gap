<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.PersoonInfoModel>" %>
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

    <script src="<%=ResolveUrl("~/Scripts/jquery-1.9.1.js")%>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("~/Scripts/jquery-ui-1.10.2.custom.js")%>" type="text/javascript"></script>
    <link href="<%=ResolveUrl("~/Content/jquery-ui-1.10.2.custom.css") %>"rel="stylesheet" type="text/css" />
    <link href="<%=ResolveUrl("~/Content/Site.css")%>" rel="stylesheet" type="text/css" /> 

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
	                
	                lastXhr = $.getJSON(
	                    '<%= Url.Action("PersoonZoeken", "Personen") %>', 
	                    { naamOngeveer: term, groepID: <%=Model.GroepID %> }, 
	                    function (data, status, xhr)
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
