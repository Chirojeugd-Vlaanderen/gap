<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GeselecteerdePersonenEnLedenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{
	%>
	<ul id="acties">
		<li><input type="submit" value="Bewaren" /></li>
	</ul>

	<%
	var j = 0;
	var info =
		(from pa in Model.PersoonEnLidInfos
			select new CheckBoxListInfo(
				pa.GelieerdePersoonID.ToString()
				, ""
				, true)).ToList();
	var leidingmaken =
		(from pa in Model.PersoonEnLidInfos
		 select new CheckBoxListInfo(
			 pa.GelieerdePersoonID.ToString()
			 , ""
			 , pa.LeidingMaken)).ToList();
	%>

	<fieldset>
		<legend>Afdelingen</legend>

		<p>
			Selecteer voor elke persoon welke afdeling je hem wil geven en of hij/zij lid of leiding is. Als je een specifieke persoon toch niet lid wil maken, kan je hem afvinken.
		</p>

		<table class="overzicht">
		<tr class="oneven">
			<th><%=Html.CheckBox("checkall") %></th>
			<th>Persoon</th>
			<th>Leiding maken</th>
			<th>Afdeling</th>
		</tr>
		
		<%
		var counter = 0;
		foreach(var pl in Model.PersoonEnLidInfos)
		{%>
			<tr class="even">
			<%
			//TODO de checkboxlist gaat alleen die bevatten die true zijn, terwijl we ook ergens de volledige lijst nog moeten hebben %>
				<%= Html.Hidden("gelieerdePersoonIDs", pl.GelieerdePersoonID) %>
				<th><%=Html.CheckBoxList("InTeSchrijvenGelieerdePersoonIDs", info[j])%></th>
				<th><%=pl.VolledigeNaam %></th>
				<td><%=Html.CheckBoxList("LeidingMakenGelieerdePersoonIDs", leidingmaken[j])%></td>
				<th>
				<%//TODO juist voorspelde afdelingsjaar hier invullen! (property toevoegen aan persoondetail ofzo) %>
				<% IEnumerable<DropDownListHelper.DropDownListItem<int>> afdelingIDItems = Model.BeschikbareAfdelingen.Select(e => new DropDownListHelper.DropDownListItem<int>() { Waarde = e.AfdelingsJaarID, DisplayNaam = e.Naam });%>
				<%=Html.DropDownList("ToegekendeAfdelingsJaarIDs", afdelingIDItems, pl.AfdelingsJaarID.HasValue?pl.AfdelingsJaarID.Value:Model.BeschikbareAfdelingen.First().AfdelingsJaarID)%>
				</th>

				<%/*PersoonDetail pl1 = pl;
				var lidTypeLijst = new List<LidType>();
				lidTypeLijst.Add(LidType.Kind);
				lidTypeLijst.Add(LidType.Leiding);
				IEnumerable<RadioButtonListInfo> lidtypeitems = lidTypeLijst.Select(e => new RadioButtonListInfo() { Value = e.ToString(), DisplayText = e.ToString(), IsChecked = (pl1.IsLid && e == LidType.Kind) });*/%>
				<%//foreach(var lidtypeitem in lidtypeitems){%>
				<%//=Html.RadioButtonList("LidMaken", lidtypeitem, counter)%>
			</tr>
			<% j++; %>
		<%}%>
		</table>
	</fieldset>
	<%}%>
</asp:Content>
