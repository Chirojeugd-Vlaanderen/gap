<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IMasterViewModel>" %>
<%
	if (Model.IsLive)
	{
%>
<div id="livetest" class="liveomgeving">
	LIVE
</div>
<%
	}
	else
	{
%>
<div id="livetest" class="testomgeving">
	TEST
</div>
<%
	}
%>