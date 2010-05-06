<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.PersonenLinksModel>" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<ul>
<% foreach (var info in Model.Personen)
   { %>
    <li><% Html.RenderPartial("PersoonsLinkControl", info); %></li>
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