<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<GeselecteerdePersonenEnLedenModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	    <script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
		<script type="text/javascript">
		    $(document).ready(function () {
		        $(':checkbox').click(function () {
		            if ($(this).attr('checked')) {
		                $(this).parent().parent().find('select').append('<option value="0">geen</option>');
		            }
		            else {
		                $(this).parent().parent().find('select').find('option[value="0"]').remove();
		            }
		        });
		    });
		</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{
	%>
	<ul id="acties">
		<li><input type="submit" value="Bewaren" /></li>
	</ul>

	<fieldset>
		<legend>Afdelingen</legend>

		<p>
            Selecteer voor iedereen de juiste afdeling. Voor leiding zet je een vinkje in de kolom 'Leiding maken'.
            Staat er iemand tussen die je toch niet wilt inschrijven, verwijder dan het vinkje in de eerste kolom.
		</p>

		<table class="overzicht">
		<tr>
			<th><%: Html.CheckBox("checkall") %></th>
			<th>Persoon</th>
			<th>Leiding maken</th>
			<th>Afdeling</th>
		</tr>
		
		<%
	    for (int j = 0; j < Model.PersoonEnLidInfos.Count(); ++j)
        {%>
			<tr class="<%: ((j&1)==0)?"even":"oneven" %>">
			<%
            //TODO de checkboxlist gaat alleen die bevatten die true zijn, terwijl we ook ergens de volledige lijst nog moeten hebben %>
				<td>
                    <%:Html.HiddenFor(mdl => mdl.PersoonEnLidInfos[j].GelieerdePersoonID)%>
                    <%:Html.CheckBoxFor(mdl => mdl.PersoonEnLidInfos[j].InTeSchrijven) %>
                </td>
				<td><%:Html.DisplayFor(mdl => mdl.PersoonEnLidInfos[j].VolledigeNaam)%></td>
				<td class="leidingmaken"><%:Html.CheckBoxFor(mdl => mdl.PersoonEnLidInfos[j].LeidingMaken)%></td>
				<td>

				<%
                    var afdelingsLijstItems = (from ba in Model.BeschikbareAfdelingen
                           select
                               new SelectListItem
                               {
                                   // voorlopig maar 1 afdeling tegelijk (first or default)
                                   Selected = (Model.PersoonEnLidInfos[j].AfdelingsJaarIDs.FirstOrDefault() == ba.AfdelingsJaarID),
                                   Text = ba.Naam,
                                   Value = ba.AfdelingsJaarID.ToString()
                               }).ToArray();
                %>
                                                               
                <%=Html.DropDownListFor(mdl => mdl.PersoonEnLidInfos[j].AfdelingsJaarIDs, afdelingsLijstItems)%>
				</td>

			</tr>
		<%}%>
		</table>
	</fieldset>
	<%}%>
</asp:Content>
