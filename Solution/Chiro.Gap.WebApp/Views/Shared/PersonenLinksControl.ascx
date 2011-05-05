<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.PersonenLinksModel>" %>

<ul>
<% foreach (var info in Model.Personen)
   { %>
    <li><%=Html.PersoonsLink(info.GelieerdePersoonID, info.VoorNaam, info.Naam)%></li>
     <%
   }
   if (Model.Personen.Count() < Model.TotaalAantal)
   {
     %>
   <li><a href="<%=Model.VolledigeLijstUrl%>">(alle <%=Model.TotaalAantal %> personen)</a></li>
     <%
    }
%>
</ul>