<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AdresModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="/Scripts/jquery-1.3.2.min.js"></script>
<script type="text/javascript" src="/Scripts/jquery.autocomplete.min.js"></script>
<link rel="stylesheet" type="text/css" href="/Content/jquery.autocomplete.css" />
<script type="text/javascript">

$(function(){
	$("#Adres_Gemeente").keyup(function(){
	$("#notfound").html("");
	// Straat nergens disabelen, als workaround voor probleem #201
	// https://develop.chiro.be/trac/cg2/ticket/201
		//document.getElementById("Adres_Straat").disabled = true;
		$("#Adres_Straat").val("");
		$("#Adres_PostNr").val("");
		//Clear de straat cache als de gemeente verandert.
		$("#Adres_Straat").flushCache();
	});
});

$(document).ready(function() {
// Onderstaande lijnen wegcommentarieren werkt rond probleem #201.
// https://develop.chiro.be/trac/cg2/ticket/201

	// document.getElementById("Adres_PostNr").readOnly = true;
	
	// if($("#Adres_Gemeente").val().length==0)
	// {
	//  	document.getElementById("Adres_Straat").disabled = true;		
	// }

	$("#Adres_Gemeente").autocomplete('<%=Url.Action("GemeentesVoorstellen", "Personen") %>',
	{
	dataType: 'json',
	parse: function(data) {
		if(data.length==0){
			$("#notfound").html("Er bestaat geen gemeente met die naam.");
			return new Array();
		}else{
			var rows = new Array();
			for (var i = 0; i < data.length; i++) {
				rows[i] = { data: data[i], value: data[i].Tag, result: data[i].Tag };
			}
			return rows;
		}
	},
	formatItem: function(row, i, max) {
		return row.Tag;
	},
	width: 300,
	minChars: 2,
	max: 10,
	highlight: false
	}).result(function(event, data, formatted) {
		/*staat op verkeerde plaats if(data.length==0){
			$("#notfound").html("Er bestaat geen gemeente met die naam.");
		}*/
		document.getElementById("Adres_Straat").disabled = false;
		$.post('<%=Url.Action("GetPostCode", "Personen") %>', { gemeente: $("#Adres_Gemeente").val() }, function(data) {
			$("#Adres_PostNr").val(data + "");
		}, "json");
	});
	
	$("#Adres_Straat").autocomplete('<%=Url.Action("StratenVoorstellen", "Personen") %>',
	{
	dataType: 'json',
	parse: function(data) {
		var rows = new Array();
		for (var i = 0; i < data.length; i++) {
			rows[i] = { data: data[i], value: data[i].Tag, result: data[i].Tag };
		}
		return rows;
	},
	formatItem: function(row, i, max) {
		return row.Tag;
	},
	width: 300,
	minChars: 2,
	highlight: false,
	multiple: false,
	extraParams: { "gemeenteNaam": function() { return $("#Adres_Gemeente").val(); },
					"gedeeltelijkeStraatNaam": function() { return $("#Adres_Straat").val(); }
		}
	});
});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% using (Html.BeginForm()){%>

	<!--werkt niet (om _ te vervangen door . in de namen, want de . wordt sowieso verkeerd geinterpreteerd) HtmlHelper.IdAttributeDotReplacement = ".";-->
   
   <ul id="acties">
   <li><input type="submit" value="Bewaren" /></li>
   </ul>
   
   <fieldset>
   <legend>Adres </legend>
   
   <%
	List<CheckBoxListInfo> info
		= (from p in Model.Bewoners
		   select new CheckBoxListInfo(p.PersoonID.ToString(), p.VolledigeNaam, Model.PersoonIDs.Contains(p.PersoonID)))
			.ToList<CheckBoxListInfo>();
   %>
   
   <%=Html.CheckBoxList("PersoonIDs", info)%>
   
   </fieldset>

   <fieldset>
   <legend>Adresgegevens</legend>
   
   	<% var values = from AdresTypeEnum e in Enum.GetValues(typeof(AdresTypeEnum))
				 select new { value = e, text = e.ToString() }; 
	%>

	<%=Html.LabelFor(mdl => mdl.AdresType) %>
	<%=Html.DropDownListFor(mdl => mdl.AdresType, new SelectList(values, "value", "text"))%>
	<br />
   
	<%=Html.LabelFor(mdl => mdl.Adres.Gemeente) %>
	<%=Html.EditorFor(mdl => mdl.Adres.Gemeente) %>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.Gemeente)%>
	<div id="notfound"></div>
	<br />

	<%=Html.LabelFor(mdl => mdl.Adres.PostNr) %>
	<%=Html.EditorFor(mdl => mdl.Adres.PostNr) %>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.PostNr) %>	
	<br />
	
	<%=Html.LabelFor(mdl => mdl.Adres.Straat) %>
	<%=Html.EditorFor(mdl => mdl.Adres.Straat) %>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.Straat) %>	
	<br />
	
	<%=Html.LabelFor(mdl => mdl.Adres.HuisNr) %>
	<%=Html.EditorFor(mdl => mdl.Adres.HuisNr) %>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.HuisNr) %>	
    <br />
    
	<%=Html.LabelFor(mdl => mdl.Adres.Bus) %>
	<%=Html.EditorFor(mdl => mdl.Adres.Bus)%>
    <%=Html.ValidationMessageFor(mdl => mdl.Adres.Bus)%>	
    <br />
   
   <%=Html.HiddenFor(mdl=>mdl.AanvragerID) %>
   <%=Html.HiddenFor(mdl=>mdl.OudAdresID) %>
   
   </fieldset>
   
<%} %>
</asp:Content>