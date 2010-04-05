<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AdresModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.min.js")%>"></script>
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.autocomplete.min.js")%>"></script>
<link rel="stylesheet" type="text/css" href="<%= ResolveUrl("~/Content/jquery.autocomplete.css")%>" />

<script type="text/javascript">
    // Automatisch invullen gemeentes na keuze postnummer
    $(function() {
        $("input#Adres_PostNr").change(function() {
            $.getJSON('<%=Url.Action("WoonPlaatsenOphalen", "Adressen")%>', { postNummer: $(this).val() }, function(j) {
                var options = '';
                for (var i = 0; i < j.length; i++) {
                    options += '<option value="' + j[i].ID + '">' + j[i].Naam + '</option>';
                }
                $("select#Adres_WoonPlaatsID").html(options);
            })
        })
    });

    // Autocomplete straten
    $(document).ready(function() {
    $("input#Adres_StraatNaamNaam").autocomplete(
        '<%= Url.Action("StratenVoorstellen", "Adressen") %>', 
        { extraParams: { "postNummer": function() { return $("input#Adres_PostNr").val(); } } });
    }); 

</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% using (Html.BeginForm()){%>
 
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

	<%=Html.LabelFor(mdl => mdl.AdresType) %>
	<%=Html.DropDownListFor(mdl => mdl.AdresType, new SelectList(values, "value", "text"))%>
	<br />

	<%=Html.LabelFor(mdl => mdl.Adres.PostNr) %>
	<%=Html.EditorFor(mdl => mdl.Adres.PostNr) %>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.PostNr) %>	
    <noscript>
        <input type="submit" name="action" value="Woonplaatsen ophalen" />
    </noscript>
	<br />

	<%=Html.LabelFor(mdl => mdl.Adres.StraatNaamNaam) %>
	<%=Html.EditorFor(mdl => mdl.Adres.StraatNaamNaam)%>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.StraatNaamNaam)%>	
	<br />
	
	<%=Html.LabelFor(mdl => mdl.Adres.HuisNr) %>
	<%=Html.EditorFor(mdl => mdl.Adres.HuisNr) %>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.HuisNr) %>	
    <br />
    
	<%=Html.LabelFor(mdl => mdl.Adres.Bus) %>
	<%=Html.EditorFor(mdl => mdl.Adres.Bus)%>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.Bus)%>	
    <br />

	<%=Html.LabelFor(mdl => mdl.Adres.WoonPlaatsNaam) %> 
    <%=Html.DropDownListFor(mdl => mdl.Adres.WoonPlaatsID, new SelectList(Model.WoonPlaatsen, "ID", "Naam")) %>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.WoonPlaatsNaam)%>
	<br />
	
   <%=Html.HiddenFor(mdl=>mdl.AanvragerID) %>
   <%=Html.HiddenFor(mdl=>mdl.OudAdresID) %>
   
   </fieldset>
   
<%} %>
</asp:Content>