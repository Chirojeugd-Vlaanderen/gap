<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GavModel>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
	<!-- De stylesheet wordt hier 2 keer gereferencet.  De eerste is in principe niet nodig, maar als die
	ontbreekt, protesteert de aspx-editor dat hij klasses niet vindt.  De tweede is nodig als de app niet in
	de 'root' van de webserver staat.  (zoals het geval in de live-omgeving) -->
	
	<link href="/Content/GeenGroepGekozen.css" rel="stylesheet" type="text/css" />
	<link href="<%=ResolveUrl("~/Content/GeenGroepGekozen.css")%>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{        
	%>
	<ul>
		<%
			foreach (var item in ViewData.Model.GroepenLijst)
			{
           
		%>
		<li>
			<%=Html.ActionLink(
               String.Format("{0} - {1} ({2})", item.StamNummer, item.Naam, item.Plaats),
                "Index", 
                new {Controller = "Handleiding", groepID = item.ID})%></li>
		<%
           
			}
		%>
	</ul>
	<%
		}
	%>
</asp:Content>
