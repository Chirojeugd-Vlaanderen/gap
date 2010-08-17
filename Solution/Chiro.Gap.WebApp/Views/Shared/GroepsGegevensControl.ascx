<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IMasterViewModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<h1>
	<%=Html.Encode(ViewData.Model.GroepsNaam) %>
</h1>
<%  if (ViewData.Model.Plaats != "geen" && ViewData.Model.Plaats != "nvt")
	{ %>
<p>
	<%=Html.Encode(ViewData.Model.Plaats)%>
	<br />
	Stamnummer:
	<%=Html.Encode(ViewData.Model.StamNummer)%>
</p>
<%  }
	else
	{ %>
<p>
	Stamnummer:
	<%=Html.Encode(ViewData.Model.StamNummer)%></p>
<%  } %>