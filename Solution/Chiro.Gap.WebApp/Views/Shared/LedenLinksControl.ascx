<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LedenLinksModel>" %>

<ul>
<% foreach (var info in Model.Leden)
   { %>
    <li>        
        <%=Html.PersoonsLink(info.PersoonDetail.GelieerdePersoonID, info.PersoonDetail.VoorNaam, info.PersoonDetail.Naam)%>
    </li>
     <%
   }
   if (Model.Leden.Count() < Model.TotaalAantal)
   {
     %>
   <li><a href="<%=Model.VolledigeLijstUrl%>">(alle <%=Model.TotaalAantal %> personen)</a></li>
     <%
    }
%>
</ul>