<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GavModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<ul>
		<%
			// Ik weet niet precies hoe het komt, maar soms krijg ik null-items
			// in mijn groepenlijst.  Via de where hieronder, omzeil ik dat probleem.
	    
			foreach (var item in ViewData.Model.GroepenLijst.Where(it=>it != null))
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
        if (Model.GroepenLijst.FirstOrDefault() == null)
        {
            %>
            <p>Je groep werd niet gevonden.  Misschien is je gebruikersrecht vervallen? Vraag aan een collega-GAV om je 
            gebruikersrecht te verlengen, of <a href='http://www.chiro.be/eloket/aansluitingen-chirogroepen'>contacteer het secretariaat.</a></p>
            <%
        }
	%>
</asp:Content>
