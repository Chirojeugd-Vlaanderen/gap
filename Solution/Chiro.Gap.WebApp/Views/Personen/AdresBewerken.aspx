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
		document.getElementById("Adres.Straat").disabled = true;
		$("#Adres_Straat").val("");
		$("#Adres_PostNr").val("");
		//Clear de straat cache als de gemeente verandert.
		$("#Adres_Straat").flushCache();
	});
});

$(document).ready(function() {	
	document.getElementById("Adres.PostNr").readOnly = true;
	
	if($("#Adres_Gemeente").val().length==0)
	{
		document.getElementById("Adres.Straat").disabled = true;		
	}

	$("#Adres_Gemeente").autocomplete('<%=Url.Action("GetGemeentes", "Personen") %>',
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
		document.getElementById("Adres.Straat").disabled = false;
		$.post('<%=Url.Action("GetPostCode", "Personen") %>', { gemeente: $("#Adres_Gemeente").val() }, function(data) {
			$("#Adres_PostNr").val(data + "");
		}, "json");
	});
	
	$("#Adres_Straat").autocomplete('<%=Url.Action("GetStraten", "Personen") %>',
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
	extraParams: { "gemeente": function() { return $("#Adres_Gemeente").val(); },
					"straat": function() { return $("#Adres_Straat").val(); }
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

	<label>Type</label>
	<%=Html.DropDownList("AdresType", new SelectList(values, "value", "text"))%>
	<br />
   
	<label>Gemeente</label>
	<%= Html.TextBox("Adres.Gemeente")%> <%= Html.ValidationMessage("Adres.Gemeente")%>
	<div id="notfound"></div>
	<br />
	
	<label>PostCode</label>
	<%=Html.TextBox("Adres.PostNr")%>
	<br />
	
	<label>Straat</label>
	<%= Html.TextBox("Adres.Straat")%> <%= Html.ValidationMessage("Adres.Straat")%>
	<br />
	
	<label>Nr.</label>
    <%=Html.TextBox("Adres.HuisNr")%> 
    <br />
    
    <label>Bus</label>
    <%=Html.TextBox("Adres.Bus")%> 
    <br />
   
   <%=Html.Hidden("Bewoners.ID")%>
   <%=Html.Hidden("AanvragerID")%>
   <%=Html.Hidden("OudAdresID")%>
   
   </fieldset>
   
<%} %>
</asp:Content>