<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IMasterViewModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%	if (Model.Mededelingen != null && Model.Mededelingen.Count() > 0)
	{
		// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
		if (Model.Mededelingen.Count() == 1)
		{
%>
<div class="mededelingen">
	<strong>Opgelet:</strong> er is nog
	<%=Html.ActionLink("1 zaak", "Index", "GavTaken")%>
	die je in orde moet brengen.
</div>
<%
	}
		else
		{
%>
<div class="mededelingen">
	<strong>Opgelet:</strong> er zijn nog
	<%=Html.ActionLink(Model.Mededelingen.Count + " zaken", "Index", "GavTaken")%>
	die je in orde moet brengen.
</div>
<%
	}
	}
	// ReSharper restore ConvertIfStatementToConditionalTernaryExpression
%>
