<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PersoonInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="/Scripts/jquery-1.2.6.min.js"></script>
<script type="text/javascript">

    $(function() {
        $("#Categorie").change(function() {
            //var link = '<%= Html.ActionLink("CategorieBekijken", "Categorie") %>';
            //var data = $("#Categorie").val();
            //url = window.location.href.replace("List", "Categorie");
            //window.location = url.substring(0, url.lastIndexOf("/")).concat("/" + data + "/" + 1);
        window.location = $("#Categorie select option:selected").val();
        });

    });
 
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="acties">
<li><%= Html.ActionLink("Nieuwe persoon", "Nieuw") %></li>
<li>
<form id="Categorie" action="" method="post">
    <select name="d">
    <option value="">Kies een categorie...</option>
    <% 
      foreach(var s in  Model.GroepsCategorieen) 
      {
          String s2 = Html.ActionLink("CategorieBekijken", "List", new { page = 1, id = s.Value});
          int start = s2.IndexOf("href=\"")+6;
          int end = s2.IndexOf("\"", start);
          Response.Write("<option value=" + s2.Substring(start, end - start) + ">" + s.Text + "</option>\n");
      }
    %>
    </select>
</form>
</li>
</ul>
<ul id="info">
<li>Totaal aantal personen: <%= Model.Totaal %></li>
</ul>
<% Html.RenderPartial("PersonenLijstControl"); %>

</asp:Content>
