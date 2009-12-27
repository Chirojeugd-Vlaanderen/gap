<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PersoonInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="/Scripts/jquery-1.2.6.min.js"></script>
<script type="text/javascript">
//HEEL BELANGRIJK: voor een dropdownlist, moet het select statement zowel een id als een name hebben, die dezelfde zijn, en die moet ook in het event gebruikt worden
$(function() {
	$("#cat").change(function() {
		window.location = $("#cat option:selected").val();
	});
});
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<ul id="acties">
<li><%= Html.ActionLink("Nieuwe persoon", "Nieuw") %></li>
<li>
<form id="Categorie" action="" method="post">
    <select id="cat" name="cat">
    <option value="">Kies een categorie...</option>
    <% 
        String s2;
        int start, end;
      foreach(var s in  Model.GroepsCategorieen) 
      {
          s2 = Html.ActionLink("CategorieBekijken", "List", new { page = 1, id = s.Value});
          start = s2.IndexOf("href=\"")+6;
          end = s2.IndexOf("\"", start);
          Response.Write("<option value=" + s2.Substring(start, end - start) + ">" + s.Text + "</option>\n");
      }
      s2 = Html.ActionLink("CategorieBekijken", "List", new { page = 1, id = 0 });
      start = s2.IndexOf("href=\"") + 6;
      end = s2.IndexOf("\"", start);
      Response.Write("<option value=" + s2.Substring(start, end - start) + ">Alle Personen</option>\n");
    %>
    </select>
</form>
</li>
</ul>
<ul id="info">
<li>Totaal aantal personen: <%= Model.Totaal %></li>
</ul>
<% Html.RenderPartial("PersonenLijstControl", Model); %>
</asp:Content>
