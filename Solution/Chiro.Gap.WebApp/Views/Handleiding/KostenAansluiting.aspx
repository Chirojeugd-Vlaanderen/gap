<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: kosten aansluiting
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
<h2>Hoeveel kost een aansluiting?</h2>
	<p>
		Het bedrag op de factuur bestaat uit:</p>
	<ul>
		<li>8 euro per lid, leid(st)er, VB of proost</li>
		<li>2,11 euro per persoon die extra verzekerd is voor loonverlies</li>
	</ul>
	<p>
		Het bedrag van 8 euro voor elk lid is opgebouwd uit:</p>
	<ul>
		<li>3,90 euro nationaal lidgeld</li>
		<li>0,82 euro verzekering burgerlijke aansprakelijkheid</li>
		<li>1,49 euro ongevallenverzekering</li>
		<li>1,29 euro verzekering overlijden en invaliditeit</li>
		<li>0,50 euro ledenuitgaven</li>
	</ul>
	<p>&nbsp;</p>

</asp:Content>
