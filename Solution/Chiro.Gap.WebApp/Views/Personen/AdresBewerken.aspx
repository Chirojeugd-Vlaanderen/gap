<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.AdresModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
    
    <script src="<%= ResolveUrl("~/Scripts/jquery.autocomplete.js")%>" type="text/javascript"></script>
    <link  href="<%= ResolveUrl("~/Content/jquery.autocomplete.css")%>" rel="stylesheet" type="text/css"/>

    <script type="text/javascript">
    // Automatisch invullen gemeentes na keuze postnummer
    $(function() {
        $("input#PersoonsAdresInfo_PostNr").change(function() {
            $.getJSON('<%=Url.Action("WoonPlaatsenOphalen", "Adressen")%>', { postNummer: $(this).val() }, function(j) {
                var options = '';
                for (var i = 0; i < j.length; i++) {
                    options += '<option value="' + j[i].Naam + '">' + j[i].Naam + '</option>';
                }
                $("select#PersoonsAdresInfo_WoonPlaatsNaam").html(options);
            })
        })
    });

    // Autocomplete straten
    $(document).ready(function() {
    $("input#PersoonsAdresInfo_StraatNaamNaam").autocomplete(
        '<%= Url.Action("StratenVoorstellen", "Adressen") %>', 
        { extraParams: { "postNummer": function() { return $("input#PersoonsAdresInfo_PostNr").val(); } } });
    }); 

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% 
    Html.EnableClientValidation();
    using (Html.BeginForm()) {%>
 
   <ul id="acties">
   <li><input type="submit" name="action" value="Bewaren" /></li>
   </ul>
   
   <fieldset>
   <legend>Adres </legend>
   
   <%=Html.CheckBoxList("PersoonIDs", Model.Bewoners)%>
   
   </fieldset>

   <fieldset>
   <legend>Adresgegevens</legend>
   
  	<% var values = from AdresTypeEnum e in Enum.GetValues(typeof(AdresTypeEnum))
				 select new { value = e, text = e.ToString() }; 
	%>

	<%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.AdresType) %>
	<%=Html.DropDownListFor(mdl => mdl.PersoonsAdresInfo.AdresType, new SelectList(values, "value", "text"))%>
	<br />

	<%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.PostNr) %>
	<%=Html.EditorFor(mdl => mdl.PersoonsAdresInfo.PostNr)%>
    <%=Html.ValidationMessageFor(mdl => mdl.PersoonsAdresInfo.PostNr)%>	
    <noscript>
        <input type="submit" name="action" value="Woonplaatsen ophalen" />
    </noscript>
	<br />

	<%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.StraatNaamNaam)%>
	<%=Html.EditorFor(mdl => mdl.PersoonsAdresInfo.StraatNaamNaam)%>
    <%=Html.ValidationMessageFor(mdl => mdl.PersoonsAdresInfo.StraatNaamNaam)%>	
	<br />
	
	<%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.HuisNr)%>
	<%=Html.EditorFor(mdl => mdl.PersoonsAdresInfo.HuisNr)%>
    <%=Html.ValidationMessageFor(mdl => mdl.PersoonsAdresInfo.HuisNr)%>	
    <br />
    
	<%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.Bus)%>
	<%=Html.EditorFor(mdl => mdl.PersoonsAdresInfo.Bus)%>
    <%=Html.ValidationMessageFor(mdl => mdl.PersoonsAdresInfo.Bus)%>	
    <br />

	<%=Html.LabelFor(mdl => mdl.PersoonsAdresInfo.WoonPlaatsNaam)%> 
    <%=Html.DropDownListFor(mdl => mdl.PersoonsAdresInfo.WoonPlaatsNaam, new SelectList(Model.WoonPlaatsen, "Naam", "Naam"))%>
    <%=Html.ValidationMessageFor(mdl => mdl.PersoonsAdresInfo.WoonPlaatsNaam)%>
	<br />
	
   <%=Html.HiddenFor(mdl=>mdl.AanvragerID) %>
   <%=Html.HiddenFor(mdl=>mdl.OudAdresID) %>
   
   </fieldset>
   
<%} %>
</asp:Content>