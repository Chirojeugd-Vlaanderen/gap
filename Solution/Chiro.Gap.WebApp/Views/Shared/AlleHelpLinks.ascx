<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="helpmenu">
<ul>
<li><%=Html.ActionLink("HelloWorld", "BestandTonen", new { Controller = "Handleiding", helpBestand = "HelloWorld" }) %></li>
<li><%=Html.ActionLink("HelloWorld", "BestandTonen", new { Controller = "Handleiding", helpBestand = "HelloWorld" }) %></li>
<li><%=Html.ActionLink("HelloWorld", "BestandTonen", new { Controller = "Handleiding", helpBestand = "HelloWorld" }) %></li>
<li><%=Html.ActionLink("HelloWorld", "BestandTonen", new { Controller = "Handleiding", helpBestand = "HelloWorld" }) %></li>
</ul>

</div>