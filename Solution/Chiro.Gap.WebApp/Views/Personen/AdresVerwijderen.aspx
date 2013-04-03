<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<AdresVerwijderenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{ %>
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" /></li>
	</ul>
	<fieldset>
		<legend>Adresgegevens</legend>
		<%=Html.Encode(String.Format("{0} {1} {2}", Model.Adres.StraatNaamNaam, Model.Adres.HuisNr, Model.Adres.Bus))%>
		<br />
		<%=Html.Encode(String.Format("{0} {1}", Model.Adres.PostNr, Model.Adres.WoonPlaatsNaam))%>
		<br />
		<%=Html.Hidden("Adres.ID") %>
		<%=Html.Hidden("AanvragerID") %>
	</fieldset>
	<fieldset>
		<legend>Wonen niet meer op bovenstaand adres:</legend>
		<%
			List<CheckBoxListInfo> info
			   = (from pa in Model.Adres.Bewoners
				  select new CheckBoxListInfo(
					 pa.PersoonID.ToString()
					 , pa.PersoonVolledigeNaam
					 , Model.PersoonIDs.Contains(pa.PersoonID))).ToList();
		%>
		<%=Html.CheckBoxList("PersoonIDs", info) %>
	</fieldset>
	<%
		} %>
</asp:Content>
