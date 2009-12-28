<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="/Scripts/jquery-1.3.2.min.js"></script>
<script type="text/javascript">

    $(function() {
        $("#Afdeling").change(function() {
        window.location = $("#Afdeling select option:selected").val();
        });

    });
 
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<ul id="acties">
<li>
<form id="Afdeling" action="" method="post">
    <select name="d">
    <option value="">Kies een afdeling...</option>
    <% 
      foreach(var s in  Model.AfdelingsInfoDictionary) 
      {
          String s2 = Html.ActionLink("AfdelingBekijken", "List", new { groepsWerkJaarId = Model.GroepsWerkJaarIdZichtbaar, afdID = s.Value.ID}).ToHtmlString();
          int start = s2.IndexOf("href=\"")+6;
          int end = s2.IndexOf("\"", start);
          Response.Write("<option value=" + s2.Substring(start, end - start) + ">" + s.Value.Naam + "</option>\n");
      }
      String alles = Html.ActionLink("AfdelingBekijken", "List", new { groepsWerkJaarId = Model.GroepsWerkJaarIdZichtbaar, afdID = 0 }).ToHtmlString();
      int ss = alles.IndexOf("href=\"") + 6;
      int e = alles.IndexOf("\"", ss);
      Response.Write("<option value=" + alles.Substring(ss, e - ss) + ">" + "Alle leden" + "</option>\n");
    %>
    </select>
</form>
</li>
</ul>
    
    <% Html.RenderPartial("LedenLijstControl"); %>

</asp:Content>
