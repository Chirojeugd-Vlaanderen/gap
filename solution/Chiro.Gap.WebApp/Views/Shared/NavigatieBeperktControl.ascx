<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IMasterViewModel>" %>
<ul>
    <li>
        <%= Html.ActionLink("Handleiding", "ViewTonen", "Handleiding")%></li>
    <li>
        <%= Html.ActionLink("Probeer aan te melden bij een groep", "Index", "Gav")%></li>
</ul>
