<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AdresModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="/Scripts/jquery-1.2.6.min.js"></script>
<script type="text/javascript" src="/Scripts/jquery.autocomplete.min.js"></script>
<link rel="stylesheet" type="text/css" href="/Content/jquery.autocomplete.css" />
<script type="text/javascript">

$(function(){
	$("#Adres_Subgemeente_Naam").keyup(function(){
		$("#notfound").html("");
		document.getElementById("Adres.Straat.Naam").disabled = true;
		$("#Adres_Straat_Naam").val("");
		$("#Adres_Straat_PostNr").val("");
	});
});

$(document).ready(function() {	
	document.getElementById("Adres.Straat.PostNr").readOnly = true;
	
	if($("#Adres_Subgemeente_Naam").val().length==0)
	{
		document.getElementById("Adres.Straat.Naam").disabled = true;		
	}

	$("#Adres_Subgemeente_Naam").autocomplete('<%=Url.Action("GetGemeentes", "Personen") %>',
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
		document.getElementById("Adres.Straat.Naam").disabled = false;
		$.getJSON('<%=Url.Action("GetPostCode", "Personen") %>', { gemeente: $("#Adres_Subgemeente_Naam").val() }, function(data) {
			$("#Adres_Straat_PostNr").val(data + "");
		});
	});

	/*$("#Adres_Straat_Naam").autocomplete('<%=Url.Action("GetStraten", "Personen") %>',
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
	extraParams: { "gemeente": function() { return $("#Adres_Subgemeente_Naam").val(); },
					"straat": function() { return $("#Adres_Straat_Naam").val(); }
		}
	});*/
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

	<label>Type</label>
	<%=Html.DropDownList("AdresType", new SelectList(values, "value", "text"))%>
	<br />
   
	<label>Gemeente</label>
	<%= Html.TextBox("Adres.Subgemeente.Naam")%> <%= Html.ValidationMessage("Adres.Subgemeente.Naam")%>
	<div id="notfound"></div>
	<br />
	
	<label>PostCode</label>
	<%=Html.TextBox("Adres.Straat.PostNr")%>
	<br />
	
	<label>Straat</label>
	<%= Html.TextBox("Adres.Straat.Naam")%> <%= Html.ValidationMessage("Adres.Straat.Naam")%>
	<br />
	
	<label>Nr.</label>
    <%=Html.TextBox("Adres.HuisNr")%> <br />
   
   <%=Html.Hidden("Bewoners.ID")%>
   <%=Html.Hidden("AanvragerID")%>
   <%=Html.Hidden("OudAdresID")%>
   
   </fieldset>
   
<%} %>
</asp:Content>