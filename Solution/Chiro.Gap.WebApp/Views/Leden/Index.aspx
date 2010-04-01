<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>"></script>
<script type="text/javascript">

    $(function() {
        $("#afd").change(function() {
        window.location = $("#afd option:selected").val();
        });

    });
 
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<form id="Afdeling" action="" method="post">
<ul id="acties">
<li><%= Html.ActionLink("Lijst downloaden", "Download", new { id = Model.GroepsWerkJaarIdZichtbaar, afdID = Model.HuidigeAfdeling })%></li>
<li>
    <select id="afd" name="afd">
    <option value="">Filter op afdeling...</option>
    <% 
        // WTF? Dat trekt hier op niks!  Zie ticket #412
        
      foreach(var s in  Model.AfdelingsInfoDictionary) 
      {
          String s2 = Html.ActionLink("AfdelingBekijken", "List", new { id = Model.GroepsWerkJaarIdZichtbaar, afdID = s.Value.AfdelingID}).ToHtmlString();
          int start = s2.IndexOf("href=\"")+6;
          int end = s2.IndexOf("\"", start);
          Response.Write("<option value=" + s2.Substring(start, end - start) + ">" + s.Value.Naam + "</option>\n");
      }
      String alles = Html.ActionLink("AfdelingBekijken", "List", new { id = Model.GroepsWerkJaarIdZichtbaar, afdID = 0 }).ToHtmlString();
      int ss = alles.IndexOf("href=\"") + 6;
      int e = alles.IndexOf("\"", ss);
      Response.Write("<option value=" + alles.Substring(ss, e - ss) + ">" + "Alle leden" + "</option>\n");
    %>
    </select>

</li>
</ul>
</form>
    
    <% Html.RenderPartial("LedenLijstControl"); %>

</asp:Content>
