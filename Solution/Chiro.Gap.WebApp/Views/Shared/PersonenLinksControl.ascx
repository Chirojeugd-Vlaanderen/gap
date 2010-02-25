<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.PersonenLinksModel>" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<ul>
<% foreach (PersoonInfo info in Model.Personen)
   { %>
    <li><% Html.RenderPartial("PersoonsLinkControl", info); %></li>
<%
   } %>
   <li><a href="<%=Model.VolledigeLijstUrl%>">...</a></li>
</ul>