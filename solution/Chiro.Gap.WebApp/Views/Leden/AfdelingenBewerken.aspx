<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.AfdelingenBewerkenModel>" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
	<% using (Html.BeginForm("AfdelingenBewerken", "Leden"))
	{
	%>
	    <ul id="acties">
		<li><input type="submit" value="Bewaren" /></li>
	    </ul>

	    <fieldset>
		<legend>Afdelingen</legend>

        <p>Selecteer afdeling(en) voor </p>
        <ul>
        <%
            foreach (var p in Model.Personen)
            {
        %>
            <li><%=Html.Hidden("LidIDs", p.LidID)%><%=p.VolledigeNaam %></li>
        <%
            }
        %>
        </ul>


		<%
            // Enkel als er alleen leiding geselecteerd is, kunnen er meerdere afdelingen geselecteerd worden.

		    bool enkelLeiding = (from p in Model.Personen
		                       where !p.IsLeiding
		                       select p).FirstOrDefault() == null;
                                   
			if (enkelLeiding)
			{
                  
			    // Het zou misschien leuk zijn moesten de afdelingen van bijv. de 1ste persoon als standaard
                // gekozen worden.  Maar voorlopig zijn alle checkboxes leeg.
                            
			    List<CheckBoxListInfo> info =
				    (from pa in Model.BeschikbareAfdelingen
				     select new CheckBoxListInfo(
								     pa.AfdelingsJaarID.ToString()
								     , pa.Naam
								     , false)).ToList();
        %>
			    <%=Html.CheckBoxList("AfdelingsJaarIDs", info)%>
		<%	
            }
			else
			{
		%>
		<%
			    foreach (var ai in Model.BeschikbareAfdelingen)
			    {
         
        %>           
                    <%=Html.RadioButtonFor(mdl=>mdl.AfdelingsJaarIDs, ai.AfdelingsJaarID)%> <%=ai.Naam %> (<%=ai.Afkorting %>) <br />
        <%
			    }
			}
		%>
	</fieldset>
	<%
		} %>
</asp:Content>
