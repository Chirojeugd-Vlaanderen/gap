<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master" Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
Handleiding: Iemands functies aanpassen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
<h2>
	Iemands functies aanpassen</h2>
<p>
	Je kunt alleen functies toekennen aan ingeschreven leden en leiding. Op het
	tabblad 'Ingeschreven' zie je in de kolom 'Func.' de afkortingen van de functies
	die iemand heeft.</p>
<p>
	Je kunt iemand een functie toekennen of ze weer afnemen. Als je iemand uitschrijft,
	worden de toegekende functies automatisch afgenomen.</p>
<p>
	Stappen in het proces:</p>
<ul>
	<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
	<li>Klik daar op de naam van degene voor wie je een adres wilt toevoegen. Je komt
		dan op de persoonsfiche.</li>
	<li>Klik links onderaan, bij de lidgegevens op de link 'functies aanpassen'.</li>
</ul>
</asp:Content>