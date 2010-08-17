<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
	Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	Handleiding: Verhuizen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
	<h2>
		Verhuizen</h2>
	<p>
		Zit je met een adreswijziging, dan kun je in principe het oude adres verwijderen
		en een nieuw toevoegen. Het programma kijkt na of er nog andere mensen op dat
		adres wonen, en je krijgt de mogelijkheid om ook voor hen dat oude adres te
		verwijderen. Dat betekent dan wel dat je voor verschillende mensen een nieuw
		adres moet toevoegen.</p>
	<p>
		Gelukkig is er een eenvoudiger procedure: verhuizen. Ook dan kijkt het programma
		na of er nog andere mensen op het oude adres wonen, én je kunt ze allemaal ineens
		verhuizen.</p>
	<p>
		Stappen in de procedure:</p>
	<ul>
		<li>Klik op het tabblad 'Ingeschreven' of 'Iedereen'.</li>
		<li>Klik daar op de naam van iemand die je wilt verhuizen. Je komt dan op de persoonsfiche.</li>
		<li>Klik achter het oude adres op de link 'verhuizen'. Verhuist iemand van kot,
			dan klik je op de link achter het kotadres, enz. Je komt dan op het verhuisformulier.</li>
		<li>Bovenaan zie je iedereen die op dat adres woont. Vink aan wie er allemaal mee
			verhuist. Vul onderaan het nieuwe adres in en klik op de knop 'Bewaren'.
			<ul>
				<li class="nietgoed">Als er iets foutliep, krijg je daar een foutmelding voor zodat
					je de nodige aanpassingen nog kunt doen.</li>
				<li class="goed">Als er geen problemen meer zijn, krijg je een melding dat de gegevens
					opgeslagen zijn.</li>
			</ul>
		</li>
	</ul>
</asp:Content>
